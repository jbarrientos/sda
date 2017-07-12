using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using MvcFlashMessages;
using SDA.Services;
using SDA.Services.Helpers;
using SDA.WebApp.Helpers;
using SDA.WebApp.Models;
using SDA.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace SDA.WebApp.Controllers
{
    public class LicenciaController : Controller
    {
        ApplicationDbContext _context;




        public LicenciaController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Licencia
        public ActionResult Index(int solicitudId)
        {
            ICollection<IndexLicenciasViewModel> lista = new List<IndexLicenciasViewModel>();
            var licencias = _context.Licencias
                .Include(l => l.unidadMedida)
                .Where(l => l.solicitudId == solicitudId && l.paraReasignacion == "N")
                .ToList();
            var solicitud = _context.Solicitudes
                .Include(s => s.DetalleContingente.Contingente.TipoContingente.UnidadMedida)
                .Include(s => s.ContingenteVario.TipoContingente.UnidadMedida)
                .Single(s => s.solicitudId == solicitudId);
            var fraccion = _context.Fracciones
                .SingleOrDefault(f => f.fraccionId == solicitud.fraccionId);
            var viewSolicitud = new IndexSolicitudes();
            //
            viewSolicitud.Contribuyente = _context.Contribuyentes.Single(c => c.contribuyenteId == solicitud.contribuyenteId);
            viewSolicitud.FechaRegistro = solicitud.fechaRegistro;
            viewSolicitud.Periodo = solicitud.DetalleContingente == null ?
                solicitud.ContingenteVario.Anio : solicitud.DetalleContingente.anio;
            viewSolicitud.Fraccion = fraccion;
            viewSolicitud.Historico = solicitud.esImportadorHistorico;
            viewSolicitud.Id = solicitud.solicitudId.ToString();
            viewSolicitud.TipoContingente = solicitud.DetalleContingente == null ?
                solicitud.ContingenteVario.TipoContingente : 
                solicitud.DetalleContingente.Contingente.TipoContingente;
            viewSolicitud.UnidadMedida = _context.UnidadesMedida.Single(u => u.unidadMedidaId == solicitud.unidadMedidaId);
            viewSolicitud.VolumenAsignado = (Double)solicitud.volumenAsignado;
            viewSolicitud.VolumenSolicitado = (Double)solicitud.volumenSolicitado;
            viewSolicitud.VolumenImportado = (Double)solicitud.volumenImportado;
            ViewBag.solicitud = viewSolicitud;
            //
            foreach (Licencia licencia in licencias)
            {
                var item = new IndexLicenciasViewModel();
                item.Id = licencia.licenciaId.ToString();
                item.Volumen = licencia.volumen;
                item.UnidadMedida = licencia.unidadMedida;
                item.Fecha = licencia.fecha;
                item.Observaciones = licencia.observaciones;
                item.codigo = licencia.codigo;
                item.acuerdo = licencia.noAcuerdo;
                item.Estado = licencia.estado;
                item.licenciaId = licencia.licenciaId;
                item.Impresa = licencia.Impresa;
                item.UploadedLicense = licencia.SignedLicenseUploaded;
                item.FechaVencimiento = (DateTime)licencia.fechaVencimiento;
                item.FechaAcuerdo = licencia.FechaAcuerdo;
                //
                lista.Add(item);
            }
            //
            var model = lista; //  contingentes.ToList<Contingente>();
            return View(model);
        }

        public ActionResult IndexOtrosContingentes(int id)
        {

            var solicitud = _context.Solicitudes
                .Include(s => s.ContingenteVario.TipoContingente)
                .Include(s => s.contribuyente)
                .SingleOrDefault(s => s.solicitudId == id);

            if (solicitud == null)
                return HttpNotFound();

            var licencias = _context.Licencias
                .Include(l => l.unidadMedida)
                .Where(l => l.solicitudId == id)
                .ToList();

            var vm = new LicenciaIndexViewModel
            {
                Solicitud = solicitud,
                Licencias = licencias
            };
            return View(vm);
        }

        public ActionResult IndexImportador(int solicitudId)
        {
            ICollection<IndexLicenciasViewModel> lista = new List<IndexLicenciasViewModel>();
            //var licencias = from t in _context.Licencias
            //                where t.solicitudId == solicitudId
            //                && t.paraReasignacion == "N"
            //                select t;
            var licencias = _context.Licencias
                .Include(l => l.unidadMedida)
                .Where(l => l.solicitudId == solicitudId && l.paraReasignacion == "N")
                .ToList();

            var solicitud = _context.Solicitudes
                .Include(s => s.DetalleContingente.Contingente.TipoContingente)
                .Include(s => s.ContingenteVario.TipoContingente)
                .Single(s => s.solicitudId == solicitudId);
            //var fraccion = _context.Fracciones.Single(f => f.fraccionId == solicitud.fraccionId);
            var viewSolicitud = new IndexSolicitudes();
            //
            viewSolicitud.Contribuyente = _context.Contribuyentes.Single(c => c.contribuyenteId == solicitud.contribuyenteId);
            viewSolicitud.FechaRegistro = solicitud.fechaRegistro;
            //viewSolicitud.Fraccion = fraccion;
            viewSolicitud.Historico = solicitud.esImportadorHistorico;
            viewSolicitud.Id = solicitud.solicitudId.ToString();
            viewSolicitud.UnidadMedida = _context.UnidadesMedida.Single(u => u.unidadMedidaId == solicitud.unidadMedidaId);
            viewSolicitud.VolumenAsignado = (Double)solicitud.volumenAsignado;
            viewSolicitud.VolumenSolicitado = (Double)solicitud.volumenSolicitado;
            viewSolicitud.VolumenImportado = (Double)solicitud.volumenImportado;
            ViewBag.solicitud = viewSolicitud;

            var objDetalle = solicitud.DetalleContingente;
            // _context.DetallesContingente.Single(d => d.detalleContingenteId == solicitud.detalleContingenteId);
            // var objContingente = _context.Contingentes.Single(c => c.contingenteId == objDetalle.contingenteId);
            var objTipo = objDetalle != null ? objDetalle.Contingente.TipoContingente :
                solicitud.ContingenteVario.TipoContingente;
            //_context.TiposContingente
            //    .Single(t => t.tipoContingenteId == objContingente.tipoContingenteId);
            //
            ViewBag.periodo = objDetalle == null ? solicitud.ContingenteVario.Anio : objDetalle.anio;
            ViewBag.tipo = objTipo;
            foreach (Licencia licencia in licencias)
            {
                var item = new IndexLicenciasViewModel();
                item.Id = licencia.licenciaId.ToString();
                item.Volumen = licencia.volumen;
                item.UnidadMedida = licencia.unidadMedida;  // _context.UnidadesMedida.Single(u => u.unidadMedidaId == licencia.unidadMedidaId);
                item.Fecha = licencia.fecha;
                item.Observaciones = licencia.observaciones;
                item.codigo = licencia.codigo;
                item.acuerdo = licencia.noAcuerdo;
                item.FechaVencimiento = (DateTime)licencia.fechaVencimiento;
                item.UploadedLicense = licencia.SignedLicenseUploaded;
                item.FechaAcuerdo = licencia.FechaAcuerdo;
                item.VolumenImportado = (double)licencia.volumenImportado;
                //item.DetalleContingente = objDetalle;
                //item.TipoContingente = objTipo;
                item.Estado = (licencia.estado == "S" ? "Solicitada" : 
                    licencia.estado == "N" ? "Anulada" : "Generada" );
                //
                lista.Add(item);
            }
            //
            var model = lista; //  contingentes.ToList<Contingente>();
            return View(model);
        }
        // GET: Licencia
        public ActionResult IndexReasignacion(int solicitudId)
        {
            ICollection<IndexLicenciasViewModel> lista = new List<IndexLicenciasViewModel>();
            var licencias = from t in _context.Licencias
                            where t.solicitudId == solicitudId
                            && t.paraReasignacion == "Y"
                            select t;
            var solicitud = _context.Solicitudes.Single(s => s.solicitudId == solicitudId);
            var fraccion = _context.Fracciones.Single(f => f.fraccionId == solicitud.fraccionId);
            var viewSolicitud = new IndexSolicitudes();
            var importador = _context.Contribuyentes.Single(c => c.contribuyenteId == solicitud.contribuyenteId);
            ViewBag.importador = importador;
            //
            viewSolicitud.Contribuyente = _context.Contribuyentes
                .Single(c => c.contribuyenteId == solicitud.contribuyenteId);
            viewSolicitud.FechaRegistro = solicitud.fechaRegistro;
            viewSolicitud.Fraccion = fraccion;
            viewSolicitud.Historico = solicitud.esImportadorHistorico;
            viewSolicitud.Id = solicitud.solicitudId.ToString();
            viewSolicitud.UnidadMedida = _context.UnidadesMedida
                .Single(u => u.unidadMedidaId == solicitud.unidadMedidaId);
            viewSolicitud.VolumenAsignado = (Double)solicitud.volumenAsignado;
            viewSolicitud.VolumenSolicitado = (Double)solicitud.volumenSolicitado;
            viewSolicitud.VolumenImportado = (Double)solicitud.volumenImportado;
            viewSolicitud.VolumenReasignacion = (Double)solicitud.volumenReasignacion;
            viewSolicitud.VolumenReasignacionImportado = (Double)solicitud.volumenImportadoReasignacion;
            ViewBag.solicitud = viewSolicitud;
            //
            foreach (Licencia licencia in licencias)
            {
                var item = new IndexLicenciasViewModel();
                item.Id = licencia.licenciaId.ToString();
                item.Volumen = licencia.volumen;
                item.UnidadMedida = _context.UnidadesMedida.Single(u => u.unidadMedidaId == licencia.unidadMedidaId);
                item.Fecha = licencia.fecha;
                item.Observaciones = licencia.observaciones;
                item.codigo = licencia.codigo;
                item.acuerdo = licencia.noAcuerdo;
                item.FechaVencimiento = (DateTime)licencia.fechaVencimiento;
                //
                lista.Add(item);
            }
            //
            var model = lista; //  contingentes.ToList<Contingente>();
            return View(model);
        }

        public ActionResult CreateVarios(int id)
        {

            var solicitud = _context.Solicitudes
                .Include(s => s.contribuyente)
                .Include(s => s.ContingenteVario.TipoContingente.UnidadMedida)
                .SingleOrDefault(s => s.solicitudId == id);

            if (solicitud == null)
                return HttpNotFound();

            //
            var vm = new LicenciaFormViewModel
            {
                Solicitud = solicitud,
                Licencia = new Licencia
                {
                    solicitudId = solicitud.solicitudId,
                    licenciaId = 0                    
                },
                Title = "Nueva Licencia"
            };

            return View("FormLicencia",vm);
        }

        public ActionResult Save(LicenciaFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var vm = new LicenciaFormViewModel
                {
                    Solicitud = _context.Solicitudes
                    .Single(s => s.solicitudId == model.Licencia.solicitudId),
                    Licencia = new Licencia
                    {
                        solicitudId = model.Licencia.solicitudId,
                        licenciaId = model.Licencia.licenciaId,
                        observaciones = model.Licencia.observaciones
                    },
                    Title = model.Licencia.licenciaId == 0 ? "Nueva Licencia" : "Editar Licencia"
                };

                return View("FormLicencia", vm);
            }
            var solicitud = _context.Solicitudes
                .Include(s => s.ContingenteVario.TipoContingente.UnidadMedida)
                .Single(s => s.solicitudId == model.Licencia.solicitudId);
            //
            if(model.Licencia.licenciaId == 0)
            {
                var licencia = new Licencia
                {
                    estado = "R",
                    fecha = DateTime.Today,
                    noAcuerdo = "",
                    codigo = "",
                    observaciones = model.Licencia.observaciones,
                    solicitudId = model.Licencia.solicitudId,
                    unidadMedidaId = solicitud.ContingenteVario.TipoContingente.unidadMedidaId,
                    volumen = model.Licencia.volumen
                };
                _context.Licencias.Add(licencia);
            }else
            {
                var lic = _context.Licencias
                    .Single(l => l.licenciaId == model.Licencia.licenciaId);
                lic.observaciones = model.Licencia.observaciones;
                lic.volumen = model.Licencia.volumen;

            }

            _context.SaveChanges();

            this.Flash("success", "Licencia para solicitud No. " + solicitud.solicitudId.ToString() +
                " ha sido creada exitosamente.");

            return RedirectToAction("IndexOtrosContingentes", new { id = solicitud.solicitudId });
        }

        public ActionResult Create(int solicitudId)
        {
            var model = new Licencia();
            Solicitud solicitud = _context.Solicitudes.Single(s => s.solicitudId == solicitudId);
            Contribuyente importador = _context.Contribuyentes.Single(c => c.contribuyenteId == solicitud.contribuyenteId);
            ViewBag.importador = importador;
            ViewBag.solicitud = solicitudId;
            DetalleContingente detalle = _context.DetallesContingente
                .Single(d => d.detalleContingenteId == solicitud.detalleContingenteId);
            Contingente contingente = _context.Contingentes
                .Single(c => c.contingenteId == detalle.contingenteId);
            //
            List<UnidadMedida> listaUnidades = new List<UnidadMedida>();
            var tipo = _context.TiposContingente.Single(t => t.tipoContingenteId == contingente.tipoContingenteId);
            int unidadMedidaId = tipo.unidadMedidaId;

            var dbUnidades = (from u in _context.UnidadesMedida
                              where u.unidadMedidaId == unidadMedidaId || u.unidadMedidaBaseId == unidadMedidaId
                              select u).ToList();

            foreach (UnidadMedida uni in dbUnidades)
            {
                listaUnidades.Add(_context.UnidadesMedida.Single(u => u.unidadMedidaId == uni.unidadMedidaId));
            }

            model.solicitudId = solicitudId;
            ViewBag.unidades = listaUnidades;


            return View(model);
        }
        [HttpPost]
        public ActionResult Create(Licencia model)
        {
            Solicitud solicitud = _context.Solicitudes.Single(s => s.solicitudId == model.solicitudId);
            DetalleContingente detalle = _context.DetallesContingente.Single(d => d.detalleContingenteId == solicitud.detalleContingenteId);
            Contingente contingente = _context.Contingentes.Single(c => c.contingenteId == detalle.contingenteId);
            TipoContingente tipo = _context.TiposContingente.Single(t => t.tipoContingenteId == contingente.tipoContingenteId);
            //
            model.fecha = DateTime.Now;
            model.fechaVencimiento = model.fecha.AddMonths(3);
            model.paraReasignacion = "N";
            model.estado = "S";
            model.volumenImportado = 0.00;

            //
            //model.codigo = Utils.GetCorrelativo(
            //    detalle.anio, 
            //    tipo.tipoNomenclaturaId,
            //    _context);
            model.codigo = "";
            _context.Licencias.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Index", new { solicitudId = model.solicitudId});
        }

        public ActionResult CreateReasignacion(int solicitudId)
        {
            var model = new Licencia();
            Solicitud solicitud = _context.Solicitudes.Single(s => s.solicitudId == solicitudId);
            Contribuyente importador = _context.Contribuyentes.Single(c => c.contribuyenteId == solicitud.contribuyenteId);
            ViewBag.importador = importador;
            ViewBag.solicitud = solicitudId;
            DetalleContingente detalle = _context.DetallesContingente
                .Single(d => d.detalleContingenteId == solicitud.detalleContingenteId);
            Contingente contingente = _context.Contingentes
                .Single(c => c.contingenteId == detalle.contingenteId);
            //
            List<UnidadMedida> listaUnidades = new List<UnidadMedida>();
            var tipo = _context.TiposContingente
                .Single(t => t.tipoContingenteId == contingente.tipoContingenteId);
            int unidadMedidaId = tipo.unidadMedidaId;

            var dbUnidades = (from u in _context.UnidadesMedida
                              where u.unidadMedidaId == unidadMedidaId || u.unidadMedidaBaseId == unidadMedidaId
                              select u).ToList();

            foreach (UnidadMedida uni in dbUnidades)
            {
                listaUnidades.Add(_context.UnidadesMedida
                    .Single(u => u.unidadMedidaId == uni.unidadMedidaId));
            }

            model.solicitudId = solicitudId;
            ViewBag.unidades = listaUnidades;


            return View(model);
        }
        [HttpPost]
        public ActionResult CreateReasignacion(Licencia model)
        {
            
            Solicitud solicitud = _context.Solicitudes.Single(s => s.solicitudId == model.solicitudId);
            if((solicitud.volumenReasignacion - solicitud.volumenImportadoReasignacion) < decimal.Parse(model.volumen.ToString()))
            {
                this.Flash("warning", "El saldo de la solicitud No. " + solicitud.solicitudId.ToString() +
                " no posee disponibilidad con respecto a lo solicitado en la licencia.");
                return RedirectToAction("CreateReasignacion", new { solicitudId = solicitud.solicitudId });
            }
            DetalleContingente detalle = _context.DetallesContingente
                .Single(d => d.detalleContingenteId == solicitud.detalleContingenteId);
            Contingente contingente = _context.Contingentes
                .Single(c => c.contingenteId == detalle.contingenteId);
            TipoContingente tipo = _context.TiposContingente
                .Single(t => t.tipoContingenteId == contingente.tipoContingenteId);
            //
            model.fecha = DateTime.Now;
            model.fechaVencimiento = model.fecha.AddMonths(3);
            model.paraReasignacion = "Y";
            model.estado = "S";

            //
            model.codigo = Utils.GetCorrelativo(detalle.anio, 
                tipo.tipoNomenclaturaId, _context);
            _context.Licencias.Add(model);
            _context.SaveChanges();
            return RedirectToAction("IndexReasignacion", new { solicitudId = model.solicitudId });
        }

        public ActionResult ImrprimirLicencia(int id)
        {
            var licencia = _context.Licencias
                .Include(l => l.solicitud.contribuyente)
                .Include(l => l.solicitud.DetalleContingente.Contingente.TipoContingente)
                .Include(l => l.solicitud.ContingenteVario.TipoContingente)
                .SingleOrDefault(l => l.licenciaId == id);

            if (licencia == null)
                return HttpNotFound();

            TipoContingente tipoContingente = null;
            if (licencia.solicitud.detalleContingenteId != null)
                tipoContingente = licencia.solicitud.DetalleContingente.Contingente.TipoContingente;
            else
                tipoContingente = licencia.solicitud.ContingenteVario.TipoContingente;
            //
            var vm = new ImprimirLicenciaViewModel
            {
                Licencia = licencia,
                TipoContingente = tipoContingente
            };

            return View(vm);
        }


        public ActionResult PrintReceipt(int id)
        {

            var licencia = _context.Licencias
                .Include(l => l.solicitud.contribuyente)
                .SingleOrDefault(l => l.licenciaId == id);

            if (licencia == null)
                return HttpNotFound();

            var vm = new PrintReceiptViewModel
            {
                Licencia = licencia,
                Responsable = "",
                LicenciaConducir = "",
                DUI = ""
            };


            return View(vm);
        }

        [HttpPost]
        public ActionResult PrintReceipt(PrintReceiptViewModel model)
        {
            var licencia = _context.Licencias
                .Include(l => l.solicitud.contribuyente)
                .Include(l => l.unidadMedida)
                .Include(l => l.solicitud.DetalleContingente.Contingente.TipoContingente)
                .Include(l => l.solicitud.ContingenteVario.TipoContingente)
                .SingleOrDefault(l => l.licenciaId == model.Licencia.licenciaId);

            if (licencia == null)
                return HttpNotFound();

            TipoContingente tipoContingente = null;

            var fracciones = "";

            var detalleSolicitud = _context.DetallesSolicitud
                .Include(d => d.Fraccion)
                .Where(d => d.SolicitudId == licencia.solicitudId)
                .ToList();

            foreach(var fr in detalleSolicitud)
            {
                fracciones += fr.Fraccion.codigo;
            }

            var utils = new Utils();

            if (licencia.solicitud.DetalleContingente == null)
                tipoContingente = licencia.solicitud.ContingenteVario.TipoContingente;
            else
                tipoContingente = licencia.solicitud.DetalleContingente.Contingente.TipoContingente;
            // Logos
            var body = tipoContingente.TemplateActa;
            var user = _context.Users.SingleOrDefault(
                u => u.Email == User.Identity.Name);
            // Binding
            var sdaPath = Utils.GetParameter("SDA_URL", _context);
            body = body.Replace("{{logoMinecPng}}", sdaPath + "/Content/img/logo_minec.png");
            body = body.Replace("{{logoMinecJpg}}", sdaPath + "/Content/img/logo_minec.jpg");
            body = body.Replace("{{logoDatcoJpg}}", sdaPath + "/Content/img/logo_datco.jpg");

            //body = body.Replace("{{logoMinec}}", Utils.GetParameter(Constant.IMG_LOGO_MINEC, _context));
            //body = body.Replace("{{logoDatco}}", Utils.GetParameter(Constant.IMG_LOGO_DATCO, _context));
            body = body.Replace("{{horaLetras}}", NumerosALetras.ToCardinal(DateTime.Now.Hour));
            body = body.Replace("{{minutoLetras}}", NumerosALetras.ToCardinal(DateTime.Now.Minute));
            body = body.Replace("{{diaLetras}}", NumerosALetras.ToCardinal(DateTime.Now.Day));
            body = body.Replace("{{nombreMes}}", utils.Months[DateTime.Now.Month].Item2);
            body = body.Replace("{{anioLetras}}", NumerosALetras.ToCardinal(DateTime.Now.Year));
            body = body.Replace("{{importador}}", licencia.solicitud.contribuyente.nombre);
            body = body.Replace("{{responsable}}", model.Responsable);
            body = body.Replace("{{tipoContingente}}", tipoContingente.nombre);
            body = body.Replace("{{volumenLicencia}}", licencia.volumen.ToString("###,##0.00"));
            body = body.Replace("{{unidadMedida}}", licencia.unidadMedida.nombre);
            body = body.Replace("{{diaAcuerdo}}", NumerosALetras.ToCardinal(licencia.FechaAcuerdo.Value.Day));
            body = body.Replace("{{mesAcuerdo}}", utils.Months[licencia.FechaAcuerdo.Value.Month].Item2);
            body = body.Replace("{{anioAcuerdo}}", licencia.FechaAcuerdo.Value.Year.ToString());
            body = body.Replace("{{acuerdo}}", licencia.noAcuerdo);
            body = body.Replace("{{fracciones}}", fracciones);
            body = body.Replace("{{usuario}}", user.nombre);
            if (model.DUI != "")
            {
                body = body.Replace("{{documentoID}}", "DUI No. " + model.DUI);
            }else
            {
                body = body.Replace("{{documentoID}}", "Licencia de Conducir No. " + model.LicenciaConducir);
            }


            //"<table><tr><td><img src='" +  +
            //    "' width=120/></td><td><img src='" + Utils.GetParameter(Constant.IMG_LOGO_DATCO, _context) +
            //    "' width=120/></td></tr></table>";
            //body += "<p align='center'>ACTA DE NOTIFICACIÓN</p><br />";
            //body += "<p>En la ciudad de San Salvador, Departamento de San Salvador, a las <strong><u>" +
            //     + "</u></strong> horas y <strong><u>" +
            //    NumerosALetras.ToCardinal(DateTime.Now.Minute) + "</u></strong> minutos del día <strong><u>" +
            //    NumerosALetras.ToCardinal(DateTime.Now.Day) + "</u></strong> de <strong><u>" +
            //    NumerosALetras.ToCardinal(DateTime.Now.Month) + "</u></strong> del año <strong><u>" +
            //    NumerosALetras.ToCardinal(DateTime.Now.Year) + "</u></strong>. Le notifique a la empresa <strong><u>" +
            //    licencia.solicitud.contribuyente.nombre + "</u></strong>, a través del Sr(a). <strong><u>" +
            //    model.Responsable + "</u></strong> su respectiva asignación de cuota en el " +
            //    tipoContingente.nombre + ", la cúal asciende a un volumen de <strong><u>" +
            //    licencia.volumen + "</u></strong> haciendose efectiva mediante licencia de importación de Acuerdo " +
            //    " Ejecutivo del Ministerio de Economía No.";

            //string HTMLContent = "<html><head><style>" +
            //    "</style></head><body>" + body + "</body></html>";

            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + 
                licencia.solicitud.contribuyente.nit + "-" + 
                licencia.codigo + ".pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.BinaryWrite(PDFServices.GetPDF(body));
            Response.End();


            return RedirectToAction("Index", new { solicitudId = licencia.solicitudId });
        }

        public void Print(int licenciaId)
        {

            var licencia = _context.Licencias
                .Include(l => l.solicitud.DetalleContingente.Contingente.TipoContingente.UnidadMedida)
                .Include(l => l.solicitud.ContingenteVario.TipoContingente.UnidadMedida)
                .SingleOrDefault(l => l.licenciaId == licenciaId);

            //

            var tipoContingente = licencia.solicitud.DetalleContingente == null ?
                licencia.solicitud.ContingenteVario.TipoContingente :
                licencia.solicitud.DetalleContingente.Contingente.TipoContingente;
            //

            Solicitud solicitud = _context.Solicitudes
                .Single(s => s.solicitudId == licencia.solicitudId);
            string plantilla = "";
            // TipoContingente tipoContingente = null;
            string anio = "";
            //
            Contribuyente importador = _context.Contribuyentes
                .Single(c => c.contribuyenteId == solicitud.contribuyenteId);
            UnidadMedida unidad = _context.UnidadesMedida
                    .Single(u => u.unidadMedidaId == licencia.unidadMedidaId);
            //
            if (solicitud.detalleContingenteId != null)
            {   
                DetalleContingente detalle = _context.DetallesContingente
                    .Single(d => d.detalleContingenteId == solicitud.detalleContingenteId);
                Contingente contingente = _context.Contingentes
                    .Include(c => c.TipoContingente)
                    .Single(c => c.contingenteId == detalle.contingenteId);
                
                plantilla = contingente.TemplateLicencia;
                //tipoContingente = contingente.TipoContingente;
                anio = detalle.anio.ToString();
                
                licencia.fechaVencimiento = DateTime.Today.AddMonths(
                (int)tipoContingente.MesesVencimientoLicencia);

            }
            else
            {
                var contingenteVario = _context.ContingentesVarios
                    .Include(v => v.TipoContingente)
                    .Single(v => v.Id == solicitud.ContingenteVarioId);

                plantilla = contingenteVario.TemplateLicencia;
                //tipoContingente = contingenteVario.TipoContingente;
                anio = contingenteVario.Anio.ToString();
                //
                if (tipoContingente.mecanismoSubasta)
                {
                    licencia.fechaVencimiento = contingenteVario.FechaFinal;
                }
            }
            licencia.FechaImpresion = DateTime.Now;
            licencia.fecha = DateTime.Today;

            var parametros = _context.Parametros
                .Where(p => p.Codigo == Constant.CODIGO_PARAMETRO_NOMBRE_MINISTRO)
                .SingleOrDefault();

            string nombreDelegado = "";

            if (parametros != null)
                nombreDelegado = parametros.Valor;

            parametros = _context.Parametros
                .Where(p => p.Codigo == Constant.CODIGO_PUESTO_FIRMA_LICENCIAS)
                .SingleOrDefault();

            string puestoDelegado = "";

            if (parametros != null)
                puestoDelegado = parametros.Valor;



            _context.SaveChanges();

            var sdaPath = Utils.GetParameter("SDA_URL", _context);

            plantilla = plantilla.Replace("{{logoMinecPng}}", sdaPath + "/Content/img/logo_minec.png");
            plantilla = plantilla.Replace("{{logoMinecJpg}}", sdaPath + "/Content/img/logo_minec.jpg");
            plantilla = plantilla.Replace("{{logoDatcoJpg}}", sdaPath + "/Content/img/logo_datco.jpg");

            plantilla = 
                plantilla.Replace("{{numLicencia}}", licencia.codigo)
                .Replace("{{acuerdo}}", licencia.noAcuerdo)
                .Replace("{{nombreImportador}}", importador.nombre.ToUpper())
                .Replace("{{direccionImportador}}", importador.direccion)
                .Replace("{{nitImportador}}", importador.nit)
                .Replace("{{volumenNumeros}}",licencia.volumen.ToString("##,##0.00"))
                .Replace("{{unidadMedida}}", unidad.nombre)
                .Replace("{{nombreDelegado}}", nombreDelegado)
                .Replace("{{puestoDelegado}}", puestoDelegado)
                .Replace("{{contingente}}", tipoContingente.nombre)
                .Replace("{{diaLetras}}", NumerosALetras.ToCardinal(DateTime.Now.Day).ToUpper())
                .Replace("{{mesLetras}}", NumerosALetras.ToMes(DateTime.Now.Month).ToUpper()).Replace("{{anioLetras}}", NumerosALetras.ToCardinal(DateTime.Now.Year).ToUpper()).Replace("{{categoriaImportador}}", solicitud.esImportadorHistorico == "Y" ? "IMPORTADOR HISTORICO": "NUEVO IMPORTADOR")
                .Replace("{{rangoFechas}}", licencia.fecha.ToString("dd/MM/yyyy") + " al " + 
                  licencia.fechaVencimiento?.ToString("dd/MM/yyyy"))
                .Replace("{{volumenLetras}}", NumerosALetras.ToCardinal(licencia.volumen).ToUpper());
            // Fracciones Contingente
            var fraccionesContingente = _context.FraccionesTipoContingente
                        .Include(f => f.Fraccion)
                        // .Where(f => f.tipoContingenteId == licencia.solicitud.DetalleContingente.Contingente.tipoContingenteId)
                        .Where(f => f.tipoContingenteId == tipoContingente.tipoContingenteId)
                        .ToList();
            var counter = fraccionesContingente.Count();
            var idx = 0;
            //
            var fracciones = "";
            foreach (var frac in fraccionesContingente)
            {
                idx++;
                if (idx == counter && counter > 1)
                {
                    fracciones = fracciones.Substring(0, fracciones.Length - 2);
                    fracciones += " y ";
                }

                fracciones += frac.Fraccion.codigo;
                if (idx < counter && counter > 1)
                {
                    fracciones += ", ";
                }

            }

            plantilla = plantilla.Replace("{{fraccionesContingente}}", fracciones);
            // 
            fracciones = "";
            //if (licencia.solicitud.DetalleContingente.Contingente.TipoContingente.DistribuirPorFraccion)
            if (tipoContingente.DistribuirPorFraccion)
            {
                var detalleSolicitud = _context.DetallesSolicitud
                    .Include(d => d.Fraccion)
                    .Where(d => d.SolicitudId == licencia.solicitud.solicitudId)
                    .ToList();

                foreach (var det in detalleSolicitud)
                {
                    fracciones += "<tr><td align=\"center\">" + det.Fraccion.codigo +
                        "</td><td>" + det.Fraccion.nombre +
                        "</td><td align=\"center\">" + det.Asignado.ToString("###,##0.00") + "</td></tr>";
                }

            }
            else if (licencia.solicitud.fraccionId != null && licencia.solicitud.fraccionId > 0)
            {
                var fraccion = _context.Fracciones
                    .SingleOrDefault(f => f.fraccionId == licencia.solicitud.fraccionId);

                fracciones += "<tr><td align=\"center\">" + fraccion.codigo +
                        "</td><td>" + fraccion.nombre +
                        "</td><td align=\"center\">" + licencia.solicitud.volumenAsignado.ToString("###,##0.00") + "</td></tr>";
            }
            plantilla = plantilla.Replace("{{fracciones}}", fracciones);
            plantilla = plantilla.Replace("{{abreviaturaUnidadMedida}}", tipoContingente.UnidadMedida.Abreviatura);
            //
            string HTMLContent = "<html><head><style>" +
                "</style></head><body>" + plantilla + "</body></html>";
            //
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + importador.nit + "-" + licencia.codigo + ".pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.BinaryWrite(PDFServices.GetPDF(HTMLContent));
            Response.End();
            // Enviar Notificacion
            var notificacion = new Notificacion
            {
                Fecha = DateTime.Now,
                SolicitudId = licencia.solicitudId,                
                Nota = "Por este medio se le informa la emisión de la licencia solicitada para " +
                "la solicitud No. " + licencia.solicitudId.ToString("0000000000") +
                ", para el contingente " + tipoContingente.nombre + " " + anio + 
                ", puede pasar a recogerla a partir del siguiente dia hábil, luego de recibida la presente notificación."
            };
            //
            licencia.FechaImpresion = DateTime.Now;
            licencia.Impresa = true;
            //
            _context.Notificaciones.Add(notificacion);
            _context.SaveChanges();
        }


        public ActionResult PrintLicence(ImprimirLicenciaViewModel model)
        {

            var licencia = _context.Licencias
                .Include(l => l.solicitud.ContingenteVario.TipoContingente.TipoNomenclatura)
                .Include(l => l.solicitud.DetalleContingente.Contingente.TipoContingente.TipoNomenclatura)
                .SingleOrDefault(l => l.licenciaId == model.Licencia.licenciaId);

            //

            int intAnio = DateTime.Now.Year;
            TipoNomenclatura nomenclatura = null;

            if (licencia.solicitud.detalleContingenteId == null)
            {
                intAnio = licencia.solicitud.ContingenteVario.Anio;
                nomenclatura = licencia.solicitud.ContingenteVario
                    .TipoContingente.TipoNomenclatura;
            }
            else
            {
                intAnio = licencia.solicitud.DetalleContingente.anio;
                nomenclatura = licencia.solicitud.DetalleContingente
                    .Contingente.TipoContingente.TipoNomenclatura;
            }
                

            licencia.noAcuerdo = model.Licencia.noAcuerdo;
            licencia.codigo = Utils.GetCorrelativo(intAnio,
                nomenclatura.tipoNomenclaturaId, _context);
            licencia.FechaAcuerdo = model.Licencia.FechaAcuerdo;
            
            _context.SaveChanges();

            Solicitud solicitud = _context.Solicitudes
                .Single(s => s.solicitudId == licencia.solicitudId);
            //string plantilla = "";
            //string tipoContingente = "";
            //string anio = intAnio.ToString();
            ////
            //Contribuyente importador = _context.Contribuyentes
            //    .Single(c => c.contribuyenteId == solicitud.contribuyenteId);
            //UnidadMedida unidad = _context.UnidadesMedida
            //        .Single(u => u.unidadMedidaId == licencia.unidadMedidaId);
            ////
            //if (solicitud.detalleContingenteId != null)
            //{
            //    DetalleContingente detalle = _context.DetallesContingente
            //        .Single(d => d.detalleContingenteId == solicitud.detalleContingenteId);
            //    Contingente contingente = _context.Contingentes
            //        .Include(c => c.TipoContingente)
            //        .Single(c => c.contingenteId == detalle.contingenteId);

            //    plantilla = contingente.TemplateLicencia;
            //    tipoContingente = contingente.TipoContingente.nombre;
            //    anio = detalle.anio.ToString();
            //}
            //else
            //{
            //    var contingenteVario = _context.ContingentesVarios
            //        .Include(v => v.TipoContingente)
            //        .Single(v => v.Id == solicitud.ContingenteVarioId);

            //    plantilla = contingenteVario.TemplateLicencia;
            //    tipoContingente = contingenteVario.TipoContingente.nombre;
            //    anio = contingenteVario.Anio.ToString();
            //}

            //plantilla =
            //    plantilla.Replace("{{numLicencia}}", licencia.codigo)
            //    .Replace("{{acuerdo}}", licencia.noAcuerdo)
            //    .Replace("{{nombreImportador}}", importador.nombre)
            //    .Replace("{{direccionImportador}}", importador.direccion)
            //    .Replace("{{nitImportador}}", importador.nit)
            //    .Replace("{{volumenNumeros}}", licencia.volumen.ToString("##,##0.00"))
            //    .Replace("{{unidadMedida}}", unidad.nombre)
            //    .Replace("{{contingente}}", tipoContingente)
            //    .Replace("{{diaLetras}}", NumerosALetras.ToCardinal(DateTime.Now.Day).ToUpper())
            //    .Replace("{{mesLetras}}", NumerosALetras.ToMes(DateTime.Now.Month).ToUpper()).Replace("{{anioLetras}}", NumerosALetras.ToCardinal(DateTime.Now.Year).ToUpper()).Replace("{{categoriaImportador}}", solicitud.esImportadorHistorico == "Y" ? "IMPORTADOR HISTORICO" : "NUEVO IMPORTADOR")
            //    .Replace("{{rangoFechas}}", licencia.fecha.ToString("dd/MM/yyyy") + " al " +
            //      licencia.fechaVencimiento?.ToString("dd/MM/yyyy"))
            //    .Replace("{{volumenLetras}}", NumerosALetras.ToCardinal(licencia.volumen).ToUpper());

            //string HTMLContent = "<html><head><style>" +
            //    "</style></head><body>" + plantilla + "</body></html>";


            //Response.Clear();
            //Response.ContentType = "application/pdf";
            //Response.AddHeader("content-disposition", "attachment;filename=" + importador.nit + "-" + licencia.codigo + ".pdf");
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.BinaryWrite(GetPDF(HTMLContent));
            //Response.End();

            // Enviar Notificacion
            //var notificacion = new Notificacion
            //{
            //    Fecha = DateTime.Now,
            //    SolicitudId = licencia.solicitudId,
            //    Nota = "Por este medio se le informa la emisión de la licencia solicitada para " +
            //    "la solicitud No. " + licencia.solicitudId.ToString("0000000000") +
            //    ", para el contingente " + tipoContingente + " " + anio +
            //    ", puede pasar a recogerla a partir del siguiente dia hábil, luego de recibida la presente notificación."
            //};

            //_context.Notificaciones.Add(notificacion);
            //_context.SaveChanges();
            this.Flash("success", "Licencia ha sido actualizada correctamente.");
            if (solicitud.detalleContingenteId != null)
                return RedirectToAction("Index", "Licencia",
                    new { solicitudId = solicitud.solicitudId });
            else
                return RedirectToAction("IndexOtrosContingentes", "Licencia",
                    new { id = solicitud.solicitudId });


        }

        //public byte[] GetPDF(string pHTML)
        //{
        //    byte[] bPDF = null;

        //    MemoryStream ms = new MemoryStream();
        //    TextReader txtReader = new StringReader(pHTML);

        //    // 1: create object of a itextsharp document class
        //    Document doc = new Document(PageSize.LETTER, 70f, 70f, 50f, 50f);

            
            

        //    // 2: we create a itextsharp pdfwriter that listens to the document and directs a XML-stream to a file
        //    PdfWriter oPdfWriter = PdfWriter.GetInstance(doc, ms);
            

        //    // 3: we create a worker parse the document
        //    HTMLWorker htmlWorker = new HTMLWorker(doc);

        //    // 4: we open document and start the worker on the document
        //    doc.Open();
        //    htmlWorker.StartDocument();

        //    // 5: parse the html into the document
        //    htmlWorker.Parse(txtReader);

        //    // 6: close the document and the worker
        //    htmlWorker.EndDocument();
        //    htmlWorker.Close();
        //    doc.Close();

        //    bPDF = ms.ToArray();

        //    return bPDF;
        //}
        //
        public ActionResult Solicitar(int solicitudId)
        {
            var model = new Licencia();
            Solicitud solicitud = _context.Solicitudes
                .Include(s => s.contribuyente)
                .Include(s => s.DetalleContingente.Contingente.TipoContingente)
                .Include(s => s.ContingenteVario.TipoContingente)
                .Include(s => s.unidadMedida)
                .Single(s => s.solicitudId == solicitudId);
            //Contribuyente importador = _context.Contribuyentes.Single(c => c.contribuyenteId == solicitud.contribuyenteId);
            //ViewBag.importador = importador;
            //ViewBag.solicitud = solicitudId;
            DetalleContingente detalle = solicitud.DetalleContingente;
            //_context.DetallesContingente.Single
            //    (d => d.detalleContingenteId == solicitud.detalleContingenteId);
            //Contingente contingente = _context.Contingentes
            //    .Single(c => c.contingenteId == detalle.contingenteId);
            //
            List<UnidadMedida> listaUnidades = new List<UnidadMedida>();
            var tipo = detalle == 
                null ? solicitud.ContingenteVario.TipoContingente : detalle.Contingente.TipoContingente;
                //_context.TiposContingente
                //.Single(t => t.tipoContingenteId == contingente.tipoContingenteId);
            int unidadMedidaId = tipo.unidadMedidaId;

            var dbUnidades = (from u in _context.UnidadesMedida
                              where u.unidadMedidaId == unidadMedidaId || u.unidadMedidaBaseId == unidadMedidaId
                              select u).ToList();

            var detalleSolicitues = _context.DetallesSolicitud
                .Include(d => d.Fraccion)
                .Where(d => d.SolicitudId == solicitud.solicitudId)
                .ToList();
            List<Fraccion> fracciones = new List<Fraccion>();
            //
            if (tipo.EspecificarFraccion)
            {
                foreach (var det in detalleSolicitues)
                {
                    fracciones.Add(det.Fraccion);
                }
            }

            var vm = new FormSolicitarLicenciaViewModel
            {
                Solicitud = solicitud,
                Unidades = dbUnidades,
                TipoContingente = tipo,
                Disponible = solicitud.volumenAsignado - solicitud.volumenImportado,
                Fracciones = fracciones,
                Licencia = new Licencia
                {
                    solicitudId = solicitud.solicitudId,
                    volumen = (double)(solicitud.volumenAsignado - solicitud.volumenImportado)
                }
            };

            //foreach (UnidadMedida uni in dbUnidades)
            //{
            //    listaUnidades.Add(_context.UnidadesMedida
            //        .Single(u => u.unidadMedidaId == uni.unidadMedidaId));
            //}

            //model.solicitudId = solicitudId;
            //ViewBag.unidades = listaUnidades;
            //ViewBag.tipo = tipo;
            //ViewBag.detalle = detalle;


            return View(vm);
        }
        [HttpPost]
        public ActionResult Solicitar(FormSolicitarLicenciaViewModel model)
        {
            Solicitud solicitud = _context.Solicitudes
                .Include(s => s.contribuyente)
                .Include(s => s.DetalleContingente.Contingente.TipoContingente)
                .Include(s => s.ContingenteVario.TipoContingente)
                .Include(s => s.unidadMedida)
                .Single(s => s.solicitudId == model.Licencia.solicitudId);
            DetalleContingente detalle = solicitud.DetalleContingente;
            //Contingente contingente = _context.Contingentes
            //    .Include(d => d.TipoContingente.UnidadMedida)
            //    .Single(c => c.contingenteId == detalle.contingenteId);
            TipoContingente tipo = solicitud.DetalleContingente == null ? 
                solicitud.ContingenteVario.TipoContingente :
                solicitud.DetalleContingente.Contingente.TipoContingente;
            //
            var unidadMedidaId = tipo.UnidadMedida.unidadMedidaId;
            var solicitado = model.Licencia.volumen;
            //if(model.Licencia.unidadMedidaId != tipo.unidadMedidaId)
            //{
            //    var unidadMedida = _context.UnidadesMedida
            //        .Single(u => u.unidadMedidaId == unidadMedidaId);
            //    solicitado = model.Licencia.volumen / unidadMedida.factor; 
            //}
            //
            var licencia = new Licencia
            {
                fecha = DateTime.Now,
                fechaVencimiento = DateTime.Now.AddMonths((int)tipo.MesesVencimientoLicencia),
                paraReasignacion = "N",
                solicitudId = solicitud.solicitudId,
                noAcuerdo = "",
                codigo = "",
                volumen = solicitado,
                unidadMedidaId = unidadMedidaId,
                estado = "S",
                observaciones = model.Licencia.observaciones
            };
            //
            _context.Licencias.Add(licencia);
            //
            if (tipo.EspecificarFraccion)
            {
                // Caso de contingentes para diferentes variedades de quesos
                var fracciones = _context.DetallesSolicitud
                    .Include(d => d.Fraccion)
                    .Where(d => d.SolicitudId == solicitud.solicitudId)
                    .ToList();
                //
                var sumaSolicitado = 0.00;
                foreach (var frac in fracciones)
                {
                    var paramFraccion = "fraccion_" + frac.FraccionId;
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
                        var detalleLicencia = new DetalleLicencia();
                        detalleLicencia.FraccionId = frac.FraccionId;
                        detalleLicencia.Volumen = double.Parse(valorSolicitado);
                        detalleLicencia.LicenciaId = licencia.licenciaId;
                        detalleLicencia.Importado = 0.00;
                        //
                        _context.DetallesLicencia.Add(detalleLicencia);
                        sumaSolicitado += detalleLicencia.Volumen;
                    }

                    //}
                }
                licencia.volumen = sumaSolicitado;
            }

            _context.SaveChanges();
            this.Flash("success", "Estamos trabajando en su solicitud de licencia, se le notificara por correo electrónico cuando se encuentre lista para venir por ella a nuestras oficinas");
            return RedirectToAction("IndexImportador", new { solicitudId = model.Licencia.solicitudId });
        }

        public ActionResult Completar(int id)
        {

            var licencia = _context.Licencias
                .Include(l => l.unidadMedida)
                .Include(l => l.solicitud.contribuyente)
                .Include(l => l.solicitud.DetalleContingente.Contingente.TipoContingente)
                .SingleOrDefault(l => l.licenciaId == id);

            if (licencia == null)
                return HttpNotFound();

            ViewBag.unidades = _context.UnidadesMedida
                .Where(u => u.unidadMedidaId == licencia.solicitud.DetalleContingente.Contingente.TipoContingente.unidadMedidaId || u.unidadMedidaBaseId == licencia.solicitud.DetalleContingente.Contingente.TipoContingente.unidadMedidaId)
                .ToList();

            return View("Create",licencia);
        }

        [HttpPost]
        public ActionResult Completar(Licencia model)
        {
            var licencia = _context.Licencias
                .Include(l => l.solicitud.DetalleContingente.Contingente.TipoContingente)
                .SingleOrDefault(l => l.licenciaId == model.licenciaId);

            if (licencia == null)
                return HttpNotFound();

            licencia.codigo = Utils.GetCorrelativo(
                licencia.solicitud.DetalleContingente.anio,
                licencia.solicitud.DetalleContingente.Contingente.TipoContingente.tipoNomenclaturaId,
                _context);
            licencia.noAcuerdo = model.noAcuerdo;
            licencia.estado = "A";
            licencia.fecha = DateTime.Now;
            licencia.fechaVencimiento = licencia.fecha.AddMonths(3);
            _context.SaveChanges();

            return RedirectToAction("Index", new { solicitudId = licencia.solicitudId });
        }

        // Upload Signed License
        public ActionResult UploadSigned(int id)
        {
            var licencia = _context.Licencias
                .Include(l => l.solicitud.contribuyente)
                .SingleOrDefault(l => l.licenciaId == id);

            if (licencia == null)
                return HttpNotFound();

            var vm = new UploadLicenciaViewmodel
            {
                Licencia = licencia,
                Comentarios = ""
            };


            return View(vm);
        }

        [HttpPost]
        public ActionResult UploadSigned(UploadLicenciaViewmodel vm, HttpPostedFileBase documento)
        {

            if (!ModelState.IsValid)
            {

                var viewModel = new UploadLicenciaViewmodel
                {
                    Licencia =
                    _context.Licencias
                    .SingleOrDefault(c => c.licenciaId == vm.Licencia.licenciaId),
                    Comentarios = vm.Comentarios
                };


                return View(viewModel);

            }

            int documentSize = 0;
            byte[] documentData = null;
            if (documento != null)
            {
                //attach the uploaded image to the object before saving to Database
                documentSize = documento.ContentLength;
                documentData = new byte[documento.ContentLength];
                documento.InputStream.Read(documentData, 0, documento.ContentLength);
            }

            var licencia = _context.Licencias
                .Include(l => l.solicitud.contribuyente)
                .Include(l => l.solicitud.DetalleContingente.Contingente.TipoContingente)
                .Include(l => l.solicitud.ContingenteVario.TipoContingente)
                .Single(l => l.licenciaId == vm.Licencia.licenciaId);

            licencia.MimeType = documento.ContentType;
            licencia.DocumentName = documento.FileName;
            licencia.DocumentSize = documentSize;
            licencia.LicenciaFirmada = documentData;
            licencia.SignedLicenseUploaded = true;
            licencia.SignedLicenseUploadedDate = DateTime.Now;
            
            _context.SaveChanges();

            TipoContingente tipoContingente = null;
            if (licencia.solicitud.DetalleContingente != null)
                tipoContingente = licencia.solicitud.DetalleContingente.Contingente.TipoContingente;
            else
                tipoContingente = licencia.solicitud.ContingenteVario.TipoContingente;

            if (tipoContingente == null)
                tipoContingente = licencia.solicitud.ContingenteVario.TipoContingente;
            // Envio de correo electrónico a importador
            if (licencia.solicitud.contribuyente.email != "")
            {
                
                var email = new Email();
                email.From = Constant.DEFAULT_EMAIL_ACCOUNT;
                email.To.Add("jorge.barrientos@gmail.com");

                // email.To.Add(solicitud.contribuyente.email);
                //if(solicitud.contribuyente.EmailAlternativo != "")
                //{
                //    email.To.Add(solicitud.contribuyente.EmailAlternativo);
                //}
                email.Subject = "Ministerio de Economía - Emisión de Licencia";
                email.Body = "<h2>" + licencia.solicitud.contribuyente.nombre + " - " + 
                    licencia.solicitud.contribuyente.nit + "</h2>";
                email.Body += "<h3>Aviso de emisión de licencia</h3>";
                email.Body +=
                    "<p>Por este medio se le notifica que la licencia de importación solicitada " +
                    "para el contingente: " + 
                    tipoContingente.nombre + " esta lista para ser descargada.</p>";
                email.Send();
            }

            this.Flash("success", "Licencia firmada para el importador " +
                licencia.solicitud.contribuyente.nombre + " ha sido registrada en el sistema exitosamente.");

            if(licencia.solicitud.detalleContingenteId != null)
                return RedirectToAction("Index", new { solicitudId = licencia.solicitudId });
            else
                return RedirectToAction("IndexOtrosContingentes", new { id = licencia.solicitudId });
        }

        public FileContentResult DownloadSigned(int id)
        {
            var licencia = _context.Licencias.FirstOrDefault(p => p.licenciaId == id);
            if (licencia != null)
            {
                if (licencia.MimeType != null)
                    return File(licencia.LicenciaFirmada, licencia.MimeType);
                else return null;
            }
            else
            {
                return null;
            }
        }

        public ActionResult Renovar(int id)
        {
            var licencia = _context.Licencias
                .Include(l => l.solicitud.contribuyente)
                .SingleOrDefault(l => l.licenciaId == id);

            if (licencia == null)
                return HttpNotFound();

            var nuevaLicencia = new Licencia
            {
                LicenciaRenovadaId = id,
                solicitudId = licencia.solicitudId,
                unidadMedidaId = licencia.unidadMedidaId,
                volumen = (double)(licencia.volumen - licencia.volumenImportado)                
            };

            var vm = new RenovarLicenciaViewModel
            {
                Licencia = nuevaLicencia,
                Importador = licencia.solicitud.contribuyente
            };

            return View(vm);
        }
        [HttpPost]
        public ActionResult Renovar(RenovarLicenciaViewModel model)
        {
            var licencia = _context.Licencias
                .Include(l => l.solicitud.contribuyente)
                .Include(l => l.solicitud.DetalleContingente.Contingente.TipoContingente)
                .SingleOrDefault(l => l.licenciaId == model.Licencia.LicenciaRenovadaId);

            if (!ModelState.IsValid)
            {
                
                if (licencia == null)
                    return HttpNotFound();

                var nuevaLicencia = new Licencia
                {
                    estado = "R",
                    fecha = DateTime.Now,
                    LicenciaRenovadaId = model.Licencia.LicenciaRenovadaId,
                    solicitudId = licencia.solicitudId,
                    unidadMedidaId = licencia.unidadMedidaId,
                    volumen = (double)(licencia.volumen - licencia.volumenImportado)
                };

                var vm = new RenovarLicenciaViewModel
                {
                    Licencia = nuevaLicencia
                };

                return View(vm);

            }

            var licenciaRenovada = new Licencia
            {
                noAcuerdo = model.Licencia.noAcuerdo,
                estado = "R",
                fecha = DateTime.Now,
                FechaAcuerdo = model.Licencia.FechaAcuerdo,
                fechaVencimiento =
                model.Licencia.FechaAcuerdo.Value.AddMonths((int)licencia.solicitud
                .DetalleContingente.Contingente.TipoContingente.MesesVencimientoLicencia) >
                licencia.solicitud.DetalleContingente.fechaReasignacion.AddDays(-1) ?
                licencia.solicitud.DetalleContingente.fechaReasignacion.AddDays(-1) :
                model.Licencia.FechaAcuerdo.Value.AddMonths((int)licencia.solicitud
                .DetalleContingente.Contingente.TipoContingente.MesesVencimientoLicencia),
                LicenciaRenovadaId = model.Licencia.LicenciaRenovadaId,
                unidadMedidaId = licencia.unidadMedidaId,
                volumen = model.Licencia.volumen,
                volumenImportado = 0.00,
                solicitudId = licencia.solicitudId,
                paraReasignacion = "N",
                codigo = ""

            };

            var detalleLicencia = _context.DetallesLicencia
                .Where(d => d.LicenciaId == model.Licencia.LicenciaRenovadaId)
                .ToList();

            _context.Licencias.Add(licenciaRenovada);

            foreach (var det in detalleLicencia)
            {
                var nuevoDetalle = new DetalleLicencia
                {
                    FraccionId = det.FraccionId,
                    Importado = 0.00,
                    Volumen = det.Volumen - det.Importado,
                    LicenciaId = licenciaRenovada.licenciaId                                     
                };
                _context.DetallesLicencia.Add(nuevoDetalle);
            }

            licencia.estado = "E";     

            _context.SaveChanges();

            this.Flash("success",
                "Renovación de licencia No. " + licencia.licenciaId.ToString() + " ha sido realizada exitosamente.");

            return RedirectToAction("Index",new { solicitudId = licencia.solicitudId });
        }
    }
}