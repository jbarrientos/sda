using MvcFlashMessages;
using SDA.WebApp.Models;
using SDA.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDA.WebApp.Controllers
{
    public class SolicitudController : Controller
    {

        ApplicationDbContext _context;

        public SolicitudController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult NewSubasta(int id)
        {
            var contingente = _context.DetallesContingenteVario
                .Include(c => c.ContingenteVario.TipoContingente.UnidadMedida)
                .Include(c => c.Contribuyente)
                .SingleOrDefault(c => c.Id == id);
            if (contingente == null)
                return HttpNotFound();


            //var contribuyentes = _context.Contribuyentes
            //    .ToList().OrderBy(c => c.nombre);

            var vm = new SubastaFormViewModel
            {
                //Importadores = contribuyentes,
                Detalle = contingente, 
                Solicitud = new Solicitud
                {
                    ContingenteVarioId = id,
                    solicitudId = 0
                }
            };

            return View("FormSolicitudSubasta",vm);
        }

        public ActionResult Create(int detalleContingenteId)
        {
            var model = new Solicitud();
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            var importador =
                _context.Contribuyentes
                .SingleOrDefault(c => c.nit == User.Identity.Name);
            //Test - En PRD el id del importador se tomara del usuario
            //
            ViewBag.contribuyente = importador;
            var detalle = _context.DetallesContingente
                .Single(d => d.detalleContingenteId == detalleContingenteId);
            var contingente = _context.Contingentes
                .Single(c => c.contingenteId == detalle.contingenteId);

            ViewBag.contingente = contingente;
            ViewBag.detalle = detalle;
            var tipo = _context.TiposContingente
                .Single(t => t.tipoContingenteId == contingente.tipoContingenteId);
            int tipoContingenteId = tipo.tipoContingenteId;
            int unidadMedidaId = tipo.unidadMedidaId;
            ViewBag.tipo = tipo;
            ViewBag.tratado = _context.Tratados.Single(t => t.tratadoId == contingente.tratadoId);
            var fracciones = (from f in _context.FraccionesTipoContingente
                             where f.tipoContingenteId == tipoContingenteId
                             select f).ToList();

            var dbUnidades = (from u in _context.UnidadesMedida
                            where u.unidadMedidaId == unidadMedidaId || u.unidadMedidaBaseId == unidadMedidaId
                            select u).ToList();

            List<Fraccion> lista = new List<Fraccion>();
            List<UnidadMedida> listaUnidades = new List<UnidadMedida>();
            foreach (FraccionTipoContingente fr in fracciones)
            {
                lista.Add(_context.Fracciones.Single(f => f.fraccionId == fr.fraccionId));
            }
            foreach (UnidadMedida uni in dbUnidades)
            {
                listaUnidades.Add(_context.UnidadesMedida.Single(u => u.unidadMedidaId == uni.unidadMedidaId));
            }
            ViewBag.fracciones = lista;
            ViewBag.unidades = listaUnidades;

            model.detalleContingenteId = detalleContingenteId;
            model.contribuyenteId = importador.contribuyenteId;

            return View(model);
        }
        [HttpPost]
        public ActionResult Create(Solicitud model)
        {
            var existeSolicitud = _context.Solicitudes
                .Where(s => s.contribuyenteId == model.contribuyenteId && s.detalleContingenteId == model.detalleContingenteId)
                .SingleOrDefault();

            if (existeSolicitud != null)
            {
                this.Flash("error", "Ya existe solicitud para el contingente seleccionado.");
                return RedirectToAction("Create", new { detalleContingenteId = model.detalleContingenteId });
            }
            model.fechaRegistro = DateTime.Now;
            UnidadMedida uni = _context.UnidadesMedida.Single(u => u.unidadMedidaId == model.unidadMedidaId);
            var detalle = _context.DetallesContingente
                .Include(d => d.Contingente)
                .Include(d => d.Contingente.TipoContingente)
                .Single(d => d.detalleContingenteId == model.detalleContingenteId);

            if (!detalle.Contingente.TipoContingente.EspecificarFraccion)
            {
                if (uni.unidadMedidaId != detalle.Contingente.TipoContingente.unidadMedidaId)
                {
                    model.volumenSolicitado =
                        model.volumenSolicitado / Decimal.Parse(uni.factor.ToString());
                    model.unidadMedidaId = detalle.Contingente.TipoContingente.unidadMedidaId;
                }
            }else
            {
                model.volumenSolicitado = Decimal.Parse("0.00");
            }
            
            model.estado = "S";
            model.esImportadorHistorico = "N";
            model.volumenImportado = Decimal.Parse("0.00");
            model.reasignacion = false;
            model.volumenReasignacion = Decimal.Parse("0.00");
            model.volumenSolicitadoReasignacion = Decimal.Parse("0.00");
            model.saldoReportadoImportador = Decimal.Parse("0.00");
            model.valorRedistribucion = Decimal.Parse("0.00");
            model.volumenImportadoReasignacion = Decimal.Parse("0.00");

            _context.Solicitudes.Add(model);
            //
            if (detalle.Contingente.TipoContingente.EspecificarFraccion)
            {
                // Caso de contingentes para diferentes variedades de quesos
                var fracciones = (from f in _context.FraccionesTipoContingente
                                  where f.tipoContingenteId == detalle.Contingente.tipoContingenteId
                                  select f).ToList();
                //
                var sumaSolicitado = 0.00;
                foreach (var frac in fracciones)
                {
                    var paramFraccion = "fraccion_" + frac.fraccionId;
                    //
                    //if (this.Request.QueryString[paramFraccion] != null)
                    //{
                    var valorSolicitado = HttpContext.Request.Params.Get(paramFraccion);

                    if (valorSolicitado == "")
                    {
                        valorSolicitado = "0.00";
                    }
                    if (valorSolicitado != "0.00")
                    {
                        var detalleSolicitud = new DetalleSolicitud();
                        detalleSolicitud.Asignado = 0.00;
                        detalleSolicitud.FraccionId = frac.fraccionId;
                        detalleSolicitud.Solicitado = double.Parse(valorSolicitado);
                        detalleSolicitud.SolicitudId = model.solicitudId;
                        _context.DetallesSolicitud.Add(detalleSolicitud);
                        sumaSolicitado += detalleSolicitud.Solicitado;
                    }

                    //}
                }
                model.volumenSolicitado = (decimal)sumaSolicitado;
            }
            //
            _context.SaveChanges();
            this.Flash("success", "Solicitud No. <strong>" + model.solicitudId.ToString() + "</strong> ha sido registrada.");
            return RedirectToAction("Dashboard", "Contribuyente", new { detalleContingenteId = model.detalleContingenteId });
        }

        public ActionResult CreateAdmin(int detalleContingenteId)
        {
            var model = new Solicitud();
            //Test - En PRD el id del importador se tomara del usuario
            //
            var detalle = _context.DetallesContingente
                .Include(d => d.Contingente)
                .Include(d => d.Contingente.TipoContingente)
                .Single(d => d.detalleContingenteId == detalleContingenteId);
            ViewBag.contingente = detalle.Contingente;
            ViewBag.detalle = detalle;
            // var tipo = this.tipos.GetById(ViewBag.contingente.tipoContingenteId);
            //int tipoContingenteId = tipo.tipoContingenteId;
            //int unidadMedidaId = tipo.unidadMedidaId;
            ViewBag.tipo = detalle.Contingente.TipoContingente;
            ViewBag.tratado = detalle.Contingente.Tratado;
            var importadores = (from i in _context.Contribuyentes
                                orderby(i.nombre)
                              select i).ToList();
            var fracciones = (from f in _context.FraccionesTipoContingente
                              where f.tipoContingenteId == detalle.Contingente.tipoContingenteId
                              select f).ToList();

            var dbUnidades = (from u in _context.UnidadesMedida
                              where u.unidadMedidaId == detalle.Contingente.TipoContingente.unidadMedidaId 
                              || u.unidadMedidaBaseId == detalle.Contingente.TipoContingente.unidadMedidaId
                              select u).ToList();

            List<Fraccion> lista = new List<Fraccion>();
            List<UnidadMedida> listaUnidades = new List<UnidadMedida>();
            List<Contribuyente> listaImportadores = new List<Contribuyente>();
            foreach (FraccionTipoContingente fr in fracciones)
            {
                lista.Add(_context.Fracciones.Single(f => f.fraccionId == fr.fraccionId));
            }
            foreach (UnidadMedida uni in dbUnidades)
            {
                listaUnidades.Add(_context.UnidadesMedida.Single(u => u.unidadMedidaId == uni.unidadMedidaId));
            }
            foreach (Contribuyente con in importadores)
            {
                listaImportadores.Add(_context.Contribuyentes.Single(c => c.contribuyenteId == con.contribuyenteId));
            }
            ViewBag.fracciones = lista;
            ViewBag.unidades = listaUnidades;
            ViewBag.importadores = listaImportadores;

            model.detalleContingenteId = detalleContingenteId;


            return View(model);
        }
        [HttpPost]
        public ActionResult CreateAdmin(Solicitud model)
        {
            model.fechaRegistro = DateTime.Now;
            model.reasignacion = false;
            model.estado = "R";
            model.esImportadorHistorico = "N";
            model.volumenImportado = Decimal.Parse("0.00");
            model.volumenReasignacion = Decimal.Parse("0.00");
            model.volumenSolicitadoReasignacion = Decimal.Parse("0.00");
            model.volumenImportadoReasignacion = Decimal.Parse("0.00");

            _context.Solicitudes.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Index", new { detalleContingenteId = model.detalleContingenteId });
        }


        // GET: Solicitud
        public ActionResult Index(int detalleContingenteId)
        {
            var detalle = _context.DetallesContingente
                .Include(_ => _.Contingente)
                .Include(_ => _.Contingente.TipoContingente)
                .Single(d => d.detalleContingenteId == detalleContingenteId);
            
            ICollection<IndexSolicitudes> lista = new List<IndexSolicitudes>();

            var solicitudes = _context.Solicitudes
                .Include(s => s.contribuyente)
                .Include(s => s.unidadMedida)
                .Where(s => s.detalleContingenteId == detalleContingenteId && 
                s.reasignacion == false).ToList();
                
            //
            double? totalSolicitado = _context.Solicitudes
                .Where(s => s.detalleContingenteId == detalleContingenteId &&
                s.reasignacion == false).Sum(s => (double?)s.volumenSolicitado) ?? 0.00;

            double? totalAsignado = _context.Solicitudes
                .Where(s => s.detalleContingenteId == detalleContingenteId &&
                s.reasignacion == false).Sum(s => (double?)s.volumenAsignado) ?? 0.00;
            //
            ViewBag.TotalSolicitado = totalSolicitado;
            ViewBag.TotalAsignado = totalAsignado;
            ViewBag.TotalDisponible = detalle.monto - (double)totalAsignado;
            //
            ViewBag.tipoContingente = detalle.Contingente.TipoContingente;
            ViewBag.cerrada = _context.Asignaciones
                .Where(a => a.DetalleContingenteId == detalleContingenteId
                && a.Cerrada).Count()>0;
            ViewBag.detalle = detalle; //  this.contingentes.GetById(detalle.contingenteId);
            //
            //ViewBag.tratado = tratados.GetById(ViewBag.contingente.tratadoId);
            //var model = fracciones.ToList<FraccionTratado>();
            ApplicationDbContext ctx = new ApplicationDbContext();
            foreach (Solicitud sol in solicitudes)
            {
                var item = new IndexSolicitudes();
                item.Contribuyente = sol.contribuyente;
                item.FechaRegistro = sol.fechaRegistro;
                item.Fraccion = ctx.Fracciones.SingleOrDefault(f => f.fraccionId == sol.fraccionId);
                item.Id = sol.solicitudId.ToString();
                item.Historico = sol.esImportadorHistorico == "Y" ? "SI" : "NO";
                item.VolumenAsignado = (Double)sol.volumenAsignado;
                item.VolumenSolicitado = (Double)sol.volumenSolicitado;
                item.VolumenImportado = (Double)sol.volumenImportado;
                item.UnidadMedida = sol.unidadMedida;
                item.VolumenARedistribuir = item.VolumenAsignado - item.VolumenImportado;
                item.VolumenSolicitadoReasignacion = (Double)sol.volumenSolicitadoReasignacion;
                item.Estado = sol.estado == "S" ? "Solicitada" : (sol.estado == "R" ? "Registrada" : (sol.estado == "A" ? "Aprobada" : "Anulada"));
                lista.Add(item);
            }

            var model = lista; //  tratado.fracciones;
            return View(model);
        }

        public ActionResult IndexVarios(int id)
        {
            var contingente = _context.ContingentesVarios
                .Include(_ => _.TipoContingente)
                .Single(d => d.Id == id);

            ICollection<IndexSolicitudes> lista = new List<IndexSolicitudes>();

            var solicitudes = _context.Solicitudes
                .Include(s => s.contribuyente)
                .Include(s => s.unidadMedida)
                .Where(s => s.ContingenteVarioId == id).ToList();

            //
            double? totalSolicitado = _context.Solicitudes
                .Where(s => s.ContingenteVarioId == id).Sum(s => (double?)s.volumenSolicitado) ?? 0.00;

            double? totalAsignado = _context.Solicitudes
                .Where(s => s.ContingenteVarioId == id).Sum(s => (double?)s.volumenAsignado) ?? 0.00;
            //
            ViewBag.TotalSolicitado = totalSolicitado;
            ViewBag.TotalAsignado = totalAsignado;
            ViewBag.TotalDisponible = contingente.TotalRecepcion - (double)totalAsignado;
            //
            ViewBag.tipoContingente = contingente.TipoContingente;
            
            ViewBag.detalle = contingente; 
            ApplicationDbContext ctx = new ApplicationDbContext();
            foreach (Solicitud sol in solicitudes)
            {
                var item = new IndexSolicitudes();
                item.Contribuyente = sol.contribuyente;
                item.FechaRegistro = sol.fechaRegistro;
                item.Id = sol.solicitudId.ToString();
                item.VolumenAsignado = (Double)sol.volumenAsignado;
                item.VolumenSolicitado = (Double)sol.volumenSolicitado;
                item.VolumenImportado = (Double)sol.volumenImportado;
                item.UnidadMedida = sol.unidadMedida;
                item.VolumenARedistribuir = item.VolumenAsignado - item.VolumenImportado;
                item.Estado = sol.estado == "S" ? "Solicitada" : "Registrada";
                lista.Add(item);
            }

            var model = lista; 
            return View(model);
        }

        public ActionResult IndexCertificados(int id)
        {
            var detalle = _context.DetallesContingenteVario
                .Include(_ => _.ContingenteVario.TipoContingente)
                .Include(_ => _.Contribuyente)
                .Single(d => d.Id == id);
            
            if(detalle == null)
                return HttpNotFound();

            ICollection<IndexSolicitudes> lista = new List<IndexSolicitudes>();

            var solicitudes = _context.Solicitudes
                .Include(s => s.contribuyente)
                .Include(s => s.unidadMedida)
                .Where(s => s.ContingenteVarioId == detalle.ContingenteVarioId && 
                s.contribuyenteId == detalle.ContribuyenteId).ToList();

            //
            double? totalSolicitado = _context.Solicitudes
                .Where(s => s.ContingenteVarioId == id).Sum(s => (double?)s.volumenSolicitado) ?? 0.00;

            double? totalAsignado = _context.Solicitudes
                .Where(s => s.ContingenteVarioId == id).Sum(s => (double?)s.volumenAsignado) ?? 0.00;
            //
            //ViewBag.TotalSolicitado = totalSolicitado;
            //ViewBag.TotalAsignado = totalAsignado;
            //ViewBag.TotalDisponible = contingente.TotalRecepcion - (double)totalAsignado;
            //
            ViewBag.tipoContingente = detalle.ContingenteVario.TipoContingente;

            ViewBag.detalle = detalle;
            ApplicationDbContext ctx = new ApplicationDbContext();
            foreach (Solicitud sol in solicitudes)
            {
                var item = new IndexSolicitudes();
                item.Contribuyente = sol.contribuyente;
                item.FechaRegistro = sol.fechaRegistro;
                item.Id = sol.solicitudId.ToString();
                item.VolumenAsignado = (Double)sol.volumenAsignado;
                item.VolumenSolicitado = (Double)sol.volumenSolicitado;
                item.VolumenImportado = (Double)sol.volumenImportado;
                item.UnidadMedida = sol.unidadMedida;
                item.VolumenARedistribuir = item.VolumenAsignado - item.VolumenImportado;
                item.Estado = sol.estado == "S" ? "Solicitada" : "Registrada";
                item.CertificadoExportacion = item.CertificadoExportacion;
                item.FechaCertificado = item.FechaCertificado;
                lista.Add(item);
            }

            var model = lista;
            return View(model);
        }

        public ActionResult Consultar(int id)
        {

            var solicitud = _context.Solicitudes
                .Include(_ => _.DetalleContingente)
                .Include(_ => _.contribuyente)
                .Include(_ => _.DetalleContingente.Contingente)
                .Include(_ => _.DetalleContingente.Contingente.TipoContingente)
                .Include(_ => _.DetalleContingente.Contingente.TipoContingente.UnidadMedida)
                .Single(s => s.solicitudId == id);

            if (solicitud == null)
                return HttpNotFound();


            var requisitos = _context.RequisitosSolicitud.Include(r => r.Requisito)
                .Where(r => r.SolicitudId == id).ToList();

            var reqs = _context.Requisitos
                .Where(r => r.TipoContingenteId == solicitud.DetalleContingente
                .Contingente.tipoContingenteId).ToList();
            List<DetalleSolicitud> detalleSolicitud = new List<DetalleSolicitud>();
            if(solicitud.detalleContingenteId != null)
            {
                if (solicitud.DetalleContingente.Contingente.TipoContingente.EspecificarFraccion)
                {
                    detalleSolicitud = _context.DetallesSolicitud
                        .Include(d => d.Fraccion)
                        .Where(d => d.SolicitudId == id)
                        .ToList();
                }
            }

            

            List<Requisito> pendientes = new List<Requisito>();

            ApplicationDbContext ctx = new ApplicationDbContext();

            foreach (var req in reqs)
            {
                var existe = ctx.RequisitosSolicitud
                    .SingleOrDefault(r => r.RequisitoId == req.Id && r.SolicitudId == solicitud.solicitudId);
                if (existe == null)
                    pendientes.Add(req);
            }

            var vm = new SolicitudConsultaViewModel
            {
                Solicitud = solicitud,
                Requisitos = _context.RequisitosSolicitud
                .Include(r => r.Requisito)
                .Where(s => s.SolicitudId == solicitud.solicitudId).ToList(),
                Pendientes = pendientes,
                DetallesFraccion = detalleSolicitud
            };

            return View(vm);
        }

        public ActionResult ConsultaImportador(int id)
        {

            var solicitud = _context.Solicitudes
                .Include(_ => _.DetalleContingente)
                .Include(_ => _.contribuyente)
                .Include(_ => _.DetalleContingente.Contingente)
                .Include(_ => _.DetalleContingente.Contingente.TipoContingente)
                .Include(_ => _.DetalleContingente.Contingente.TipoContingente.UnidadMedida)
                .Include(_ => _.ContingenteVario.TipoContingente.UnidadMedida)
                .Single(s => s.solicitudId == id);

            if (solicitud == null)
                return HttpNotFound();

            var tipoContingente = solicitud.DetalleContingente == null ? solicitud.ContingenteVario.TipoContingente :
                solicitud.DetalleContingente.Contingente.TipoContingente;

            var requisitos = _context.RequisitosSolicitud.Include(r => r.Requisito)
                .Where(r => r.SolicitudId == id).ToList();

            var reqs = _context.Requisitos
                .Where(r => r.TipoContingenteId == solicitud.DetalleContingente
                .Contingente.tipoContingenteId).ToList();
            List<DetalleSolicitud> detalleSolicitud = new List<DetalleSolicitud>();
            if (solicitud.detalleContingenteId != null)
            {
                if (solicitud.DetalleContingente.Contingente.TipoContingente.EspecificarFraccion)
                {
                    detalleSolicitud = _context.DetallesSolicitud
                        .Include(d => d.Fraccion)
                        .Where(d => d.SolicitudId == id)
                        .ToList();
                }
            }


            List<Requisito> pendientes = new List<Requisito>();

            ApplicationDbContext ctx = new ApplicationDbContext();

            foreach (var req in reqs)
            {
                var existe = ctx.RequisitosSolicitud
                    .SingleOrDefault(r => r.RequisitoId == req.Id && r.SolicitudId == solicitud.solicitudId);
                if (existe == null)
                    pendientes.Add(req);
            }

            var vm = new SolicitudConsultaViewModel
            {
                Solicitud = solicitud,
                Requisitos = _context.RequisitosSolicitud
                .Include(r => r.Requisito)
                .Where(s => s.SolicitudId == solicitud.solicitudId).ToList(),
                Pendientes = pendientes,
                DetallesFraccion = detalleSolicitud,
                TipoContingente = tipoContingente,
                Licencias = _context.Licencias.Where(l => l.solicitudId == solicitud.solicitudId).ToList()
            };

            return View(vm);
        }

        public FileContentResult GetRequisito(int id)
        {
            var requisito = _context.RequisitosSolicitud.FirstOrDefault(p => p.Id == id);
            if (requisito != null)
            {
                if (requisito.MimeType != null)
                    return File(requisito.Documento, requisito.MimeType);
                else return null;
            }
            else
            {
                return null;
            }
        }

        public ActionResult RetirarReasignacion(int solicitudId)
        {
            var model = _context.Solicitudes
                .Include(s => s.contribuyente)
                .Single(s => s.solicitudId == solicitudId);
            ViewBag.importador = model.contribuyente;
            return View(model);
        }
        [HttpPost]
        public ActionResult RetirarReasignacion(Solicitud model)
        {
            //model.retirarReasignacion = true;
            //model.fechaRetiroReasignacion = DateTime.Now;
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return RedirectToAction("index", new { detalleContingenteId = model.detalleContingenteId});
        }

        public ActionResult Update(int id)
        {
            var solicitud = _context.Solicitudes
                .Include(_ => _.DetalleContingente)
                .Include(_ => _.DetalleContingente.Contingente)
                .Include(_ => _.DetalleContingente.Contingente.TipoContingente)
                .Include(_ => _.DetalleContingente.Contingente.TipoContingente.UnidadMedida)
                .Include(_ => _.DetalleContingente.Contingente.Tratado)
                .Include(_ => _.contribuyente)
                .Single(s => s.solicitudId == id);

            if (solicitud == null)
                return HttpNotFound();


            List<Fraccion> lista = new List<Fraccion>();
            var fracciones = _context.FraccionesTipoContingente
                .Include(f => f.Fraccion)
                .Where(f => f.tipoContingenteId == solicitud.DetalleContingente.Contingente
                .tipoContingenteId).ToList();

            foreach (var fr in fracciones)
            {
                lista.Add(_context.Fracciones.Single(f => f.fraccionId == fr.fraccionId));
            }
            var viewModel = new SolicitudFormViewModel
            {
                Solicitud = solicitud,
                UnidadesMedida = _context.UnidadesMedida
                // u.unidadMedidaId == unidadMedidaId || u.unidadMedidaBaseId == unidadMedidaId
                .Where(u => 
                u.unidadMedidaId == solicitud
                .DetalleContingente.Contingente
                .TipoContingente.unidadMedidaId ||
                u.unidadMedidaBaseId == solicitud
                .DetalleContingente.Contingente
                .TipoContingente.unidadMedidaId).ToList(),
                Fracciones = lista
            };

            return View("SolicitudForm", viewModel);
        }

        [HttpPost]
        public ActionResult Save(SolicitudFormViewModel vm)
        {

            var solicitud = _context.Solicitudes
                .Include(_ => _.DetalleContingente)
                .Include(_ => _.DetalleContingente.Contingente)
                .Include(_ => _.DetalleContingente.Contingente.TipoContingente)
                .Include(_ => _.DetalleContingente.Contingente.TipoContingente.UnidadMedida)
                .Single(s => s.solicitudId == vm.Solicitud.solicitudId);

            if (solicitud == null)
                return HttpNotFound();

            if (!ModelState.IsValid)
            {

                List<Fraccion> lista = new List<Fraccion>();
                var fracciones = _context.FraccionesTipoContingente
                    .Include(f => f.Fraccion)
                    .Where(f => f.tipoContingenteId == solicitud.DetalleContingente.Contingente
                    .tipoContingenteId).ToList();

                foreach (var fr in fracciones)
                {
                    lista.Add(_context.Fracciones.Single(f => f.fraccionId == fr.fraccionId));
                }

                var viewModel = new SolicitudFormViewModel
                {
                    Solicitud = solicitud,
                    UnidadesMedida = _context.UnidadesMedida
                .Where(u =>
                u.unidadMedidaId == solicitud
                .DetalleContingente.Contingente
                .TipoContingente.unidadMedidaId ||
                u.unidadMedidaBaseId == solicitud
                .DetalleContingente.Contingente
                .TipoContingente.unidadMedidaId).ToList(),
                    Fracciones = lista
                };

                return View("SolicitudForm", viewModel);
            }
            //

            if (solicitud.unidadMedidaId != 
                solicitud.DetalleContingente.Contingente.TipoContingente.unidadMedidaId)
            {
                solicitud.volumenSolicitado =
                    solicitud.volumenSolicitado / Decimal.Parse(
                        solicitud.DetalleContingente.Contingente.TipoContingente.UnidadMedida.factor.ToString());
                solicitud.unidadMedidaId =
                    solicitud.DetalleContingente.Contingente.TipoContingente.unidadMedidaId;
            }

            solicitud.comentarios = vm.Solicitud.comentarios;
            solicitud.fraccionId = vm.Solicitud.fraccionId;
            solicitud.unidadMedidaId = vm.Solicitud.unidadMedidaId;
            solicitud.volumenSolicitado = vm.Solicitud.volumenSolicitado;

            _context.SaveChanges();

            this.Flash("success", "La solicitud No. " + solicitud.solicitudId.ToString() +
                " ha sido actualizada exitosamente.");

            return RedirectToAction("Dashboard", "Contribuyente");
        }

        [HttpPost]
        public ActionResult SaveSubasta(SubastaFormViewModel model, string detalleId)
        {

            if (!ModelState.IsValid)
            {

                var contribuyentes = _context.Contribuyentes
                .ToList().OrderBy(c => c.nombre);

                var vm = new SubastaFormViewModel
                {
                    
                    Detalle = _context.DetallesContingenteVario
                    .Single(v => v.Id == model.Detalle.Id),
                    Solicitud = new Solicitud
                    {
                        ContingenteVarioId = model.Solicitud.solicitudId,
                        solicitudId = 0
                    }
                };

                return View("FormSolicitudSubasta", vm);
            }
            //

            var detId = Int32.Parse(detalleId);

            var contingente = _context.DetallesContingenteVario
                .Include(c => c.ContingenteVario.TipoContingente.UnidadMedida)
                .Single(c => c.Id == detId);
            //
            if (model.Solicitud.solicitudId == 0)
            {
                // Crear
                var solicitud = new Solicitud
                {
                    CertificadoExportacion = model.Solicitud.CertificadoExportacion,
                    FechaCertificado = model.Solicitud.FechaCertificado,
                    comentarios = model.Solicitud.comentarios,
                    ContingenteVarioId = contingente.ContingenteVarioId,
                    contribuyenteId = contingente.ContribuyenteId,
                    esImportadorHistorico = "I",
                    estado = "R",
                    fechaRegistro = DateTime.Now,
                    unidadMedidaId = contingente.ContingenteVario.TipoContingente.unidadMedidaId,
                    valorRedistribucion = (decimal)0.00,
                    volumenAsignado = model.Solicitud.volumenAsignado,
                    volumenImportado = (decimal)0.00,
                    volumenSolicitado = model.Solicitud.volumenAsignado,
                    volumenImportadoReasignacion = (decimal)0.00
                };
                _context.Solicitudes.Add(solicitud);
            }else
            {
                var solic = _context.Solicitudes
                    .Single(s => s.solicitudId == model.Solicitud.solicitudId);
                solic.CertificadoExportacion = model.Solicitud.CertificadoExportacion;
                solic.FechaCertificado = model.Solicitud.FechaCertificado;
                solic.comentarios = model.Solicitud.comentarios;
                solic.volumenAsignado = model.Solicitud.volumenAsignado;
                solic.volumenSolicitado = model.Solicitud.volumenAsignado;
            }

            contingente.ContingenteVario.SolicitudesGeneradas = true;

            _context.SaveChanges();

            this.Flash("success", "La solicitud No. " + 
                model.Solicitud.solicitudId.ToString() +
                " ha sido actualizada exitosamente.");

            return RedirectToAction("IndexCertificados", "Solicitud",
                new { id = contingente.Id });
        }

        public ActionResult Edit(int solicitudId)
        {
            var model = _context.Solicitudes
                .Include(s => s.contribuyente)
                .Include(d => d.DetalleContingente)
                .Include(t => t.DetalleContingente.Contingente)
                .Include(t => t.DetalleContingente.Contingente.TipoContingente)
                .Single(s => s.solicitudId == solicitudId);
            ViewBag.importador = model.contribuyente;
            ViewBag.detalle = model.DetalleContingente;
            ViewBag.contingente = model.DetalleContingente.Contingente;
            ViewBag.tipo = model.DetalleContingente.Contingente.TipoContingente;
            //
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(Solicitud model)
        {
            
            model.fechaRetiroReasignacion = null;
            if (model.retirarReasignacion)
            {
                model.fechaRetiroReasignacion = DateTime.Now;
            }
            _context.SaveChanges();
            Contribuyente importador = _context.Contribuyentes
                .Single(c => c.contribuyenteId == model.contribuyenteId);
            this.Flash("success", "La solicitud No. " + model.solicitudId.ToString() + 
                " a nombre de " + importador.nombre + " ha sido actualizada." );
            return RedirectToAction("index", new { detalleContingenteId = model.detalleContingenteId});
        }

        public ActionResult Reasignar(int detalleContingenteId)
        {
            var detalle = _context.DetallesContingente
                .Include(d => d.Contingente)
                .Include(t => t.Contingente.TipoContingente)
                .Single(d => d.detalleContingenteId == detalleContingenteId);

            ICollection<IndexSolicitudes> lista = new List<IndexSolicitudes>();

            var solicitudes = from t in _context.Solicitudes
                              where t.detalleContingenteId == detalleContingenteId
                              //&& t.retirarReasignacion == false
                              select t;
            //
            ViewBag.tipoContingente = detalle.Contingente.TipoContingente;
            ViewBag.detalle = detalle; //  this.contingentes.GetById(detalle.contingenteId);
            //
            //ViewBag.tratado = tratados.GetById(ViewBag.contingente.tratadoId);
            //var model = fracciones.ToList<FraccionTratado>();
            ApplicationDbContext ctx = new ApplicationDbContext();
            foreach (Solicitud sol in solicitudes)
            {
                var item = new IndexSolicitudes();
                item.Contribuyente = ctx.Contribuyentes
                    .Single(c => c.contribuyenteId == sol.contribuyenteId);
                item.FechaRegistro = sol.fechaRegistro;
                item.Fraccion = ctx.Fracciones.SingleOrDefault(f => f.fraccionId == sol.fraccionId);
                item.Id = sol.solicitudId.ToString();
                item.Historico = sol.esImportadorHistorico == "Y" ? "SI" : "NO";
                item.VolumenAsignado = (Double)sol.volumenAsignado;
                item.VolumenSolicitado = (Double)sol.volumenSolicitado;
                item.VolumenImportado = (Double)sol.volumenImportado;
                item.UnidadMedida = ctx.UnidadesMedida
                    .Single(u => u.unidadMedidaId == sol.unidadMedidaId);
                item.VolumenARedistribuir = item.VolumenAsignado - item.VolumenImportado;
                item.VolumenReasignacion = (Double)sol.volumenReasignacion;
                item.VolumenSolicitadoReasignacion = (Double)sol.volumenSolicitadoReasignacion;
                item.RetirarReasignacion = sol.retirarReasignacion;
                lista.Add(item);
            }

            var model = lista; //  tratado.fracciones;
            return View(model);
        }
        //[HttpPost]
        public ActionResult CalcularAsignacion()
        {
            //SDA.Services.ContingenteServices.Asignar(DateTime.Now.Year);
            //this.Flash("info","Proceso de Asignación finalizado. ");
            return RedirectToAction("SummaryContingentes", "Contingente",
                new { year = DateTime.Now.Year });
        }

        public ActionResult CalcularReasignacion()
        {
            //SDA.Services.ContingenteServices.Reasignar(DateTime.Now.Year);
            //this.Flash("info", "Proceso de reasignación finalizado. Número de solicitudes afectadas " +
            //    SDA.Services.ContingenteServices.CountSolicitudesReasignacion().ToString() +
            //    " y el total del volumen reasignado es de " +
            //    SDA.Services.ContingenteServices.SumSolicitudesReasignacion().ToString("###,###,##0.00"));
            return RedirectToAction("SummaryContingentes", "Contingente", new { year = DateTime.Now.Year });
        }

        public ActionResult IndexReasignacion(int detalleContingenteId)
        {
            var detalle = _context.DetallesContingente
                .Include(d => d.Contingente)
                .Include(d => d.Contingente.TipoContingente)
                .Single(d => d.detalleContingenteId == detalleContingenteId);

            ICollection<IndexSolicitudes> lista = new List<IndexSolicitudes>();

            var solicitudes = from t in _context.Solicitudes
                              where t.detalleContingenteId == detalleContingenteId
                              && t.reasignacion == false
                              select t;
            //
            ViewBag.tipoContingente = detalle.Contingente.TipoContingente;
            ViewBag.detalle = detalle; //  this.contingentes.GetById(detalle.contingenteId);
            //
            //ViewBag.tratado = tratados.GetById(ViewBag.contingente.tratadoId);
            //var model = fracciones.ToList<FraccionTratado>();
            foreach (Solicitud sol in solicitudes)
            {
                var item = new IndexSolicitudes();
                item.Contribuyente = _context.Contribuyentes
                    .Single(c => c.contribuyenteId == sol.contribuyenteId);
                item.FechaRegistro = sol.fechaRegistro;
                item.Fraccion = _context.Fracciones
                    .Single(f => f.fraccionId == sol.fraccionId);
                item.Id = sol.solicitudId.ToString();
                item.Historico = sol.esImportadorHistorico == "Y" ? "SI" : "NO";
                item.VolumenAsignado = (Double)sol.volumenAsignado;
                item.VolumenSolicitado = (Double)sol.volumenSolicitado;
                item.VolumenImportado = (Double)sol.volumenImportado;
                item.UnidadMedida = _context.UnidadesMedida.Single(
                    u => u.unidadMedidaId == sol.unidadMedidaId);
                item.VolumenARedistribuir = item.VolumenAsignado - item.VolumenImportado;
                item.FechaRetiroReasignacion = item.FechaRetiroReasignacion;

                //item.VolumenReasignacion = !item.RetirarReasignacion ? item.VolumenARedistribuir : 0.00;
                lista.Add(item);
            }

            var model = lista; //  tratado.fracciones;
            return View(model);
        }

        public ActionResult Remitir(int id)
        {
            var solicitud = _context.Solicitudes
                .Include(s => s.contribuyente)
                .Include(s => s.DetalleContingente.Contingente.TipoContingente)
                .Single(s => s.solicitudId == id);


            if (solicitud == null)
                return HttpNotFound();

            solicitud.estado = "R";
            solicitud.FechaEnvio = DateTime.Now;

            _context.SaveChanges();

            this.Flash("success", "La solicitud No. " +
                solicitud.solicitudId.ToString() +
                " ha sido remitida exitosamente.");
            return RedirectToAction("Dashboard", "Contribuyente");
        }

        public ActionResult DownloadNotificacion(int id)
        {
            var solicitud = _context.Solicitudes
                .Include(s => s.contribuyente)
                .SingleOrDefault(s => s.solicitudId == id);

            if (solicitud != null)
            {
                if (solicitud.RutaArchivoNotificacion != null)
                    return File(solicitud.RutaArchivoNotificacion, "application/pdf", 
                        solicitud.solicitudId.ToString() + "-" + solicitud.contribuyente.nit + "-" + solicitud.contribuyente.nombre + 
                        "-Notificacion.pdf");
                else return null;
            }
            else
            {
                return null;
            }
        }

        public ActionResult New(int id)
        {

            var detalleContingente = _context.DetallesContingente
                .Include(d => d.Contingente.TipoContingente.UnidadMedida)
                .SingleOrDefault(d => d.detalleContingenteId == id);

            if(detalleContingente == null)
            {
                return HttpNotFound();
            }

            var fraccionesTipo = _context.FraccionesTipoContingente
                .Include(f => f.Fraccion)
                .Where(f => f.tipoContingenteId == detalleContingente.Contingente.tipoContingenteId)
                .ToList();

            List<Fraccion> fracciones = new List<Fraccion>();

            foreach (var fr in  fraccionesTipo)
            {
                fracciones.Add(fr.Fraccion);
            }

            var unidades = (from u in _context.UnidadesMedida
                              where u.unidadMedidaId == detalleContingente.Contingente.TipoContingente.unidadMedidaId 
                              || u.unidadMedidaBaseId == detalleContingente.Contingente.TipoContingente.unidadMedidaId
                              select u).ToList();

            var importador =
                _context.Contribuyentes
                .SingleOrDefault(c => c.nit == User.Identity.Name);

            var vm = new SolicitudNewViewModel
            {
                Solicitud = new Solicitud
                {
                    contribuyenteId = importador.contribuyenteId
                },
                DetalleContingente = detalleContingente,
                Importador = _context.Contribuyentes
                .SingleOrDefault(c => c.nit == User.Identity.Name),                
                Fracciones = fracciones,
                UnidadesMedida = unidades,
                Requisitos = _context.Requisitos
                .Where(r => r.TipoContingenteId == detalleContingente.Contingente.tipoContingenteId)
                .ToList()
            };

            return View(vm);
        }
        [HttpPost]
        public ActionResult New(SolicitudNewViewModel model, HttpPostedFileBase[] files)
        {
            var existeSolicitud = _context.Solicitudes
                .Where(s => s.contribuyenteId == model.Solicitud.contribuyenteId 
                && s.detalleContingenteId == model.DetalleContingente.detalleContingenteId)
                .SingleOrDefault();

            if (existeSolicitud != null)
            {
                this.Flash("error", "Ya existe solicitud para el contingente seleccionado.");
                return RedirectToAction("Create", new { detalleContingenteId = model.DetalleContingente.detalleContingenteId });
            }
            //
            var detalle =
                _context.DetallesContingente
                .Include(d => d.Contingente.TipoContingente)
                .SingleOrDefault(d => d.detalleContingenteId == model.DetalleContingente.detalleContingenteId);

            var fracciones = (from f in _context.FraccionesTipoContingente
                              where f.tipoContingenteId == detalle.Contingente.tipoContingenteId
                              select f).ToList();
            //
            var sumaSolicitado = 0.00;
            foreach (var frac in fracciones)
            {
                var paramFraccion = "fraccion_" + frac.fraccionId;
                //
                //if (this.Request.QueryString[paramFraccion] != null)
                //{
                var valorSolicitado = HttpContext.Request.Params.Get(paramFraccion);

                if (valorSolicitado == "")
                {
                    valorSolicitado = "0.00";
                }
                if (valorSolicitado != "0.00")
                {
                    sumaSolicitado += double.Parse(valorSolicitado);
                }

                //}
            }
            //

            if(sumaSolicitado > detalle.monto)
            {
                this.Flash("error", "<h3>Monto solicitado supera el monto global del contingente</h3>");
                return RedirectToAction("New", new { detalleContingenteId = model.DetalleContingente.detalleContingenteId });

            }else if (sumaSolicitado <= 0)
            {
                this.Flash("error", "<h3>Monto solicitado debe ser mayor a 0.00</h3>");
                return RedirectToAction("New", new { detalleContingenteId = model.DetalleContingente.detalleContingenteId });

            }





            var solicitud = new Solicitud
            {
                detalleContingenteId = model.DetalleContingente.detalleContingenteId,
                fechaRegistro = DateTime.Now,
                estado = "S",
                esImportadorHistorico = "N",
                volumenAsignado = Decimal.Parse("0.00"),
                reasignacion = false,
                volumenReasignacion = Decimal.Parse("0.00"),
                volumenSolicitadoReasignacion = Decimal.Parse("0.00"),
                saldoReportadoImportador = Decimal.Parse("0.00"),
                valorRedistribucion = Decimal.Parse("0.00"),
                volumenImportado = Decimal.Parse("0.00"),
                volumenImportadoReasignacion = Decimal.Parse("0.00"),
                contribuyenteId = model.Solicitud.contribuyenteId,
                unidadMedidaId = detalle.Contingente.TipoContingente.unidadMedidaId
            };
            

            _context.Solicitudes.Add(solicitud);

            
            //
            if (detalle.Contingente.TipoContingente.EspecificarFraccion)
            {
                // Caso de contingentes para diferentes variedades de quesos
                fracciones = (from f in _context.FraccionesTipoContingente
                                  where f.tipoContingenteId == detalle.Contingente.tipoContingenteId
                                  select f).ToList();
                //
                sumaSolicitado = 0.00;
                foreach (var frac in fracciones)
                {
                    var paramFraccion = "fraccion_" + frac.fraccionId;
                    //
                    //if (this.Request.QueryString[paramFraccion] != null)
                    //{
                    var valorSolicitado = HttpContext.Request.Params.Get(paramFraccion);

                    if (valorSolicitado == "")
                    {
                        valorSolicitado = "0.00";
                    }
                    if (valorSolicitado != "0.00")
                    {
                        var detalleSolicitud = new DetalleSolicitud();
                        detalleSolicitud.Asignado = 0.00;
                        detalleSolicitud.FraccionId = frac.fraccionId;
                        detalleSolicitud.Solicitado = double.Parse(valorSolicitado);
                        detalleSolicitud.SolicitudId = solicitud.solicitudId;
                        _context.DetallesSolicitud.Add(detalleSolicitud);
                        sumaSolicitado += detalleSolicitud.Solicitado;
                    }

                    //}
                }
                solicitud.volumenSolicitado = (decimal)sumaSolicitado;
            }
            // Carga de Archivos - Requisitos
            //var requisitos = _context.Requisitos
            //    .Where(r => r.TipoContingenteId == detalle.Contingente.tipoContingenteId)
            //    .ToList();

            var reqs = _context.Requisitos
                .Where(r => r.TipoContingenteId == detalle.Contingente.tipoContingenteId)
                .ToList();

            int[] reqIDS = new int[reqs.Count];
            var idx = 0;
            foreach (var r in reqs)
            {
                reqIDS[idx] = r.Id;
                idx++;
            }



            //foreach (var r in reqs)
            //{
            //    int documentSize = 0;
            //    byte[] documentData = null;
            //    var archivo = Request.Form.Get["requisito_" + r.Id.ToString()];

            //    //attach the uploaded image to the object before saving to Database
            //    documentSize = archivo.ContentLength;
            //    documentData = new byte[archivo.ContentLength];
            //    archivo.InputStream.Read(documentData, 0, archivo.ContentLength);

            //    var req = new RequisitoSolicitud
            //    {
            //        Fecha = DateTime.Now,
            //        Comentarios = "",
            //        Documento = documentData,
            //        MimeType = archivo.ContentType,
            //        PictureName = archivo.FileName,
            //        RequisitoId = Request[,
            //        PictureSize = documentSize,
            //        SolicitudId = vm.Solicitud.solicitudId
            //    };

            //    _context.RequisitosSolicitud.Add(req);
            //}

            idx = 0;

            foreach (var archivo in files)
            {
                int documentSize = 0;
                byte[] documentData = null;

                //attach the uploaded image to the object before saving to Database
                documentSize = archivo.ContentLength;
                documentData = new byte[archivo.ContentLength];
                archivo.InputStream.Read(documentData, 0, archivo.ContentLength);

                var req = new RequisitoSolicitud
                {
                    Fecha = DateTime.Now,
                    Comentarios = "",
                    Documento = documentData,
                    MimeType = archivo.ContentType,
                    PictureName = archivo.FileName,
                    RequisitoId = reqIDS[idx],
                    PictureSize = documentSize,
                    SolicitudId = solicitud.solicitudId
                };

                idx++;

                _context.RequisitosSolicitud.Add(req);
            }

            _context.SaveChanges();
            this.Flash("success", "<h2>Solicitud No. <strong>" + solicitud.solicitudId.ToString() + 
                "</strong> ha sido registrada</h2>");
            return RedirectToAction("Dashboard", "Contribuyente", 
                new { });
        }


        public ActionResult Remove(int id)
        {
            var solicitud = _context.Solicitudes
                .Include(s => s.contribuyente)
                .SingleOrDefault(s => s.solicitudId == id);

            if(solicitud == null)
            {
                return HttpNotFound();
            }

            var vm = new RemoveSolicitudViewModel
            {
                Solicitud = solicitud
            };
            return View(vm);
        }
        [HttpPost]
        public ActionResult Remove(RemoveSolicitudViewModel model)
        {
            var solicitud = _context.Solicitudes
                .SingleOrDefault(s => s.solicitudId == model.Solicitud.solicitudId);

            if (!ModelState.IsValid)
            {
                
                if (solicitud == null)
                {
                    return HttpNotFound();
                }

                var vm = new RemoveSolicitudViewModel
                {
                    Solicitud = solicitud
                };
                return View(vm);
            }
            var user = _context.Users
                .SingleOrDefault(u => u.UserName == User.Identity.Name);

            if (user == null)
            {
                return HttpNotFound();
            }
            solicitud.estado = "N";
            solicitud.FechaRetiro = DateTime.Now;
            solicitud.UsuarioRetiro = user.Id;
            solicitud.ObservacionesRetiro = model.Solicitud.ObservacionesRetiro;

            _context.SaveChanges();

            this.Flash("success", "La solicitud No. " + solicitud.solicitudId.ToString() +
                " ha sido retirada del proceso de asignación de cuotas");

            return RedirectToAction("Index", "Solicitud", new { detalleContingenteId = model.Solicitud.detalleContingenteId });
        }

        //public ActionResult Create(int detalleContringenteId, int contribuyenteId)
        //{
        //    var model = new Solicitud();
        //    model.contribuyenteId = contribuyenteId;
        //    model.detalleContingenteId = detalleContringenteId;
        //    return View(model);

        //}
    }
}