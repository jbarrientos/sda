using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using SDA.WebApp.Models;
using SDA.WebApp.ViewModels;
using System.Data.Entity;
using MvcFlashMessages;

namespace SDA.WebUI.Controllers
{
    public class ContingenteController : Controller
    {

        ApplicationDbContext _context;

        public ContingenteController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Contingente
        //public ActionResult Index(int categoriaId)
        //{
        //    DAL.Data.DataContext db = new DAL.Data.DataContext();
        //    ICollection<IndexContingenteModel> lista = new List<IndexContingenteModel>();
        //    var contingentes = from t in db.Contingentes
        //                 where t.categoriaId == categoriaId
        //                 select t;
        //    //var corte = this.cortes.GetById(categoriaId);
        //    ViewBag.categoria = this.categorias.GetById(categoriaId);
        //    ViewBag.tratado = this.tratados.GetById(ViewBag.categoria.tratadoId);
        //    //
        //    foreach (Contingente fr in contingentes)
        //    {
        //        var item = new IndexContingenteModel();
        //        item.FechaFinal = fr.fechaFin;
        //        item.FechaFinSolicitudes = fr.fechaFinSolicitudes;
        //        item.FechaInicio = fr.fechaInicio;
        //        item.Id = fr.contingenteId.ToString();
        //        item.FechaInicioSolicitudes = fr.fechaInicioSolicitudes;
        //        item.FechaReasignacion = fr.fechaReasignacion;
        //        item.UnidadMedida = this.unidades.GetById(fr.unidadMedidaId);
        //        item.Volumen = fr.volumen;
        //        item.Descripcion = fr.nombre;
        //        lista.Add(item);
        //    }
        //    //
        //    var model = lista; //  contingentes.ToList<Contingente>();
        //    return View(model);
        //}

        public ActionResult Index(int tratadoId)
        {
            
            ICollection<IndexContingenteModel> lista = new List<IndexContingenteModel>();
            var contingentes = _context.Contingentes
                .Include(c => c.TipoContingente)
                .Include(c => c.TipoContingente.UnidadMedida)
                .Include(n => n.TipoContingente.TipoNomenclatura)
                .Where(c => c.tratadoId == tratadoId).ToList();
            //var corte = this.cortes.GetById(categoriaId);
            ViewBag.tratado = _context.Tratados.SingleOrDefault(t => t.tratadoId == tratadoId);
            //
            foreach (Contingente fr in contingentes)
            {
                var item = new IndexContingenteModel();
                //item.FechaFinal = fr.fechaFin;
                //item.FechaFinSolicitudes = fr.fechaFinSolicitudes;
                //item.FechaInicio = fr.fechaInicio;
                item.Id = fr.contingenteId.ToString();
                //item.FechaInicioSolicitudes = fr.fechaInicioSolicitudes;
                //item.FechaReasignacion = fr.fechaReasignacion;
                //item.UnidadMedida = this.unidades.GetById(fr.unidadMedidaId);
                item.Volumen = fr.volumen;
                item.TipoContingente = fr.TipoContingente;
                item.UnidadMedida = fr.TipoContingente.UnidadMedida;
                item.TipoNomenclatura = fr.TipoContingente.TipoNomenclatura;
                //
                item.VolumenHistoricosPercent = (Double)fr.porcentajeVolumenHistorico;
                item.VolumenNuevosPercent = 100.00 - (Double)fr.porcentajeVolumenHistorico;
                lista.Add(item);
            }
            //
            
            var model = lista; //  contingentes.ToList<Contingente>();
            return View(model);
        }

        public ActionResult Manager()
        {
            var periodos = from t in _context.DetallesContingente
                               where t.anio == DateTime.Now.Year
                               select t;

            ICollection<IndexManagerContingenteModel> lista = new List<IndexManagerContingenteModel>();

            ViewBag.year = DateTime.Now.Year;
            //
            foreach (DetalleContingente fr in periodos)
            {
                var contingente = _context.Contingentes.SingleOrDefault(c => c.contingenteId == fr.contingenteId);
                //
                var item = new IndexManagerContingenteModel();
                item.Id = fr.contingenteId.ToString();
                item.Tratado = _context.Tratados.Single(t => t.tratadoId == contingente.tratadoId);
                item.TipoContingente = _context.TiposContingente.Single(t => t.tipoContingenteId == contingente.tipoContingenteId);
                item.FechaFinalSolicitudes = fr.fechaFinSolicitudes;
                item.FechaFinalReasignacion = fr.fechaFinSolicitudesRe;
                item.FechaInicioReasignacion = fr.fechaInicioSolicitudesRe;
                item.FechaInicioSolicitudes = fr.fechaInicioSolicitudes;
                item.Monto = (Double)fr.monto;
                item.MontoNuevos = (Double)fr.montoNuevo;
                item.Periodo = fr.anio;
                lista.Add(item);
            }
            //
            var model = lista; //  contingentes.ToList<Contingente>();
            return View(model);
        }

        public ActionResult EditTemplate(int id)
        {
            var contingente = _context.Contingentes
                .Include(c => c.TipoContingente)
                .Include(c => c.Tratado)
                .SingleOrDefault(l => l.contingenteId == id);

            if (contingente == null)
                return HttpNotFound();


            return View(contingente);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditTemplate(Contingente model)
        {
            var contingente = _context.Contingentes
                .Include(c => c.TipoContingente)
                .SingleOrDefault(c => c.contingenteId == model.contingenteId);

            contingente.TemplateLicencia = model.TemplateLicencia;
            contingente.TemplateNotificacion = model.TemplateNotificacion;
            _context.SaveChanges();

            this.Flash("success", "Plantilla para licencias de " + 
                contingente.TipoContingente.nombre + ", ha sido actualizada correctamente.");

            return RedirectToAction("Index", new { tratadoId = contingente.tratadoId });
        }

        public ActionResult Create(int tratadoId)
        {
            //var model = new CreateContingenteModel();
            //model.TratadoId = tratadoId;
            //ViewBag.tratado = _context.Tratados.Single(t => t.tratadoId == tratadoId);
            //var fracs = from t in _context.FraccionesTratado
            //                        where t.tratadoId == tratadoId
            //                        select t;
            //ViewBag.unidades = _context.UnidadesMedida.ToList();
            //ViewBag.nomenclaturas = _context.TiposNomenclatura.ToList();

            // Fracciones a seleccionar
            //List<SelectListItem> listSelectListItems = new List<SelectListItem>();

            //foreach (FraccionTratado fraccion in fracs.ToList())
            //{
            //    var fracc = this.fracciones.GetById(fraccion.fraccionId);
            //    //
            //    SelectListItem selectList = new SelectListItem()
            //    {
            //        Text = fracc.codigo + " : " + fracc.nombre,
            //        Value = fracc.fraccionId.ToString(),
            //        //Selected = city.IsSelected
            //    };
            //    listSelectListItems.Add(selectList);
            //}
            //

            var tratado = _context.Tratados
                .SingleOrDefault(t => t.tratadoId == tratadoId);
            //
            var vm = new ContingenteFormViewModel
            {
                Tratado = tratado,
                TiposContingente = _context.TiposContingente.ToList(),
                Categorias = _context.Categorias
                .Where(c => c.tratadoId == tratadoId)
                .ToList(),
                Contingente = new Contingente
                {
                    tratadoId = tratadoId,
                    contingenteId = 0
                }
            };
            return View("FormContingente",vm);
        }

        [HttpPost]
        public ActionResult Save(ContingenteFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var tratado = _context.Tratados
                .SingleOrDefault(t => t.tratadoId == model.Contingente.tratadoId);
                //
                var vm = new ContingenteFormViewModel
                {
                    Tratado = tratado,
                    Categorias = _context.Categorias
                    .Where(c => c.tratadoId == model.Contingente.tratadoId)
                    .ToList(),
                    TiposContingente = _context.TiposContingente.ToList(),
                    Contingente = new Contingente
                    {
                        tratadoId = tratado.tratadoId,
                        contingenteId = 0
                    }
                };
            }
            if(model.Contingente.contingenteId == 0)
            {
                _context.Contingentes.Add(new Contingente
                {
                    aniosAnteriores = model.Contingente.aniosAnteriores,
                    aumentoVolumen = model.Contingente.aumentoVolumen,
                    categoriaId = model.Contingente.categoriaId,
                    cuotaUnica = model.Contingente.cuotaUnica,
                    porcentajeMinimoImportacion = model.Contingente.porcentajeMinimoImportacion,
                    porcentajeVolumenHistorico = model.Contingente.porcentajeVolumenHistorico,
                    tipoContingenteId = model.Contingente.tipoContingenteId,
                    volumen = model.Contingente.volumen,
                    tratadoId = model.Contingente.tratadoId,
                    nombre = model.Contingente.nombre
                });
            }else
            {
                var contingente = _context.Contingentes
                    .SingleOrDefault(c => c.contingenteId == model.Contingente.contingenteId);
                contingente.aniosAnteriores = model.Contingente.aniosAnteriores;
                contingente.aumentoVolumen = model.Contingente.aumentoVolumen;
                contingente.categoriaId = model.Contingente.categoriaId;
                contingente.cuotaUnica = model.Contingente.cuotaUnica;
                contingente.porcentajeMinimoImportacion = model.Contingente.porcentajeMinimoImportacion;
                contingente.porcentajeVolumenHistorico = model.Contingente.porcentajeVolumenHistorico;
                contingente.nombre = model.Contingente.nombre;
            }
            
            _context.SaveChanges();
            return RedirectToAction("Index", new { tratadoId = model.Contingente.tratadoId});
        }

        public ActionResult Details(int contingenteId)
        {
            var contin = _context.Contingentes.Single(c => c.contingenteId == contingenteId);
            if (contin == null)
                return HttpNotFound();

            ViewBag.detalleContingente = (from t in _context.DetallesContingente
                                                     where t.contingenteId == contingenteId
                                                     select t).ToList();
            var model = new IndexContingenteModel();
            model.Id = contingenteId.ToString();
            model.TipoContingente = _context.TiposContingente.Single(t => t.tipoContingenteId == contin.tipoContingenteId);
            model.TipoNomenclatura = _context.TiposNomenclatura.Single(n => n.tipoNomenclaturaId == model.TipoContingente.tipoNomenclaturaId);
            model.UnidadMedida = _context.UnidadesMedida.Single(u => u.unidadMedidaId == model.TipoContingente.unidadMedidaId);
            model.Volumen = contin.volumen;
            model.VolumenHistoricosPercent = (Double)contin.porcentajeVolumenHistorico;
            model.VolumenNuevosPercent = 100.00 - (Double)contin.porcentajeVolumenHistorico;
            return View(model);
        }
        public ActionResult SummaryContingentes(int year)
        {
            /*
            select anio, t.nombre, u.nombre, monto, montoNuevo, 
sum(case when s.unidadMedidaId = t.unidadMedidaId then s.volumenAsignado else s.volumenAsignado / us.factor end) Asignado, 
sum(case when s.unidadMedidaId = t.unidadMedidaId then s.volumenSolicitado else s.volumenSolicitado / us.factor end) Solicitado
  from DetalleContingente d join Contingente c on d.contingenteId = c.contingenteId
join TipoContingente t on c.tipoContingenteId = t.tipoContingenteId
join UnidadMedida u on t.unidadMedidaId = u.unidadMedidaId
join Solicitud s on d.detalleContingenteId = s.detalleContingenteId
join UnidadMedida us on s.unidadMedidaId = us.unidadMedidaId
group by anio, t.nombre, u.nombre, monto, montoNuevo
*/
            int referenceYear;
            if(year == 0)
            {
                referenceYear = DateTime.Now.Year;
            }
            else
            {
                referenceYear = year;
            }
            
            ViewBag.year = referenceYear;

            ICollection<SummaryContingentesViewModel> lista = new List<SummaryContingentesViewModel>();
            var summ = from d in _context.DetallesContingente
                       join c in _context.Contingentes on d.contingenteId equals c.contingenteId
                       join t in _context.TiposContingente on c.tipoContingenteId equals t.tipoContingenteId
                       join u in _context.UnidadesMedida on t.unidadMedidaId equals u.unidadMedidaId
                       join s in _context.Solicitudes on d.detalleContingenteId equals s.detalleContingenteId
                       join us in _context.UnidadesMedida on s.unidadMedidaId equals us.unidadMedidaId
                       //join l in db.Licencias on s.solicitudId equals l.solicitudId
                       //join da in db.DetallesAduana on l.codigo equals da.cuota
                       where d.anio == referenceYear
                       group new { s.volumenAsignado, s.volumenSolicitado, s.volumenImportado, s.volumenReasignacion }
                       by new { d.detalleContingenteId, t.unidadMedidaId, d.anio, t.nombre, d.monto, d.montoNuevo, d.disponibleRedistribucion, d.disponibleRedistHistoricos } into grp
                       select new
                       {
                           Anio = grp.Key.anio,
                           TipoContingente = grp.Key.nombre,
                           Monto = grp.Key.monto,
                           MontoNuevos = grp.Key.montoNuevo,
                           Asignado = grp.Sum(s => s.volumenAsignado),
                           Solicitado = grp.Sum(s => s.volumenSolicitado),
                           Importado = grp.Sum(s => s.volumenImportado),
                           DetalleContingenteId = grp.Key.detalleContingenteId,
                           UnidadMedidaId = grp.Key.unidadMedidaId,
                           //UnidadMedida = _context.UnidadesMedida.SingleOrDefault(grp.Key.unidadMedidaId),
                           Reasignado = grp.Sum(s => s.volumenReasignacion),
                           DisponibleNuevos = grp.Key.disponibleRedistribucion,
                           DisponibleHistoricos = grp.Key.disponibleRedistHistoricos
                       };
            ApplicationDbContext ctx = new ApplicationDbContext();
            foreach (var summary in summ)
            {
                var item = new SummaryContingentesViewModel();
                item.Anio = DateTime.Now.Year;
                item.Id = summary.TipoContingente;
                item.Asignado = (Double)summary.Asignado;
                item.MontoNuevos = summary.MontoNuevos;
                item.MontoTotal = summary.Monto;
                item.Nombre = summary.TipoContingente;
                item.Solicitado = (Double)summary.Solicitado;
                item.Importado = (Double)summary.Importado;
                item.Redistribucion = item.Asignado - item.Importado;
                item.MontoHistoricos = item.MontoTotal - item.MontoNuevos;
                item.UnidadMedida = ctx.UnidadesMedida.Single(u => u.unidadMedidaId == summary.UnidadMedidaId);
                item.DetalleContingenteId = summary.DetalleContingenteId;
                item.Reasignacion = (Double)summary.Reasignado;
                
                item.DisponibleNuevos = summary.DisponibleNuevos == null ? 0.00 : (Double)summary.DisponibleNuevos; //  SDA.Services.ContingenteServices.SumDisponibleReasignacion(item.DetalleContingenteId);
                item.DisponibleHist = summary.DisponibleHistoricos == null ? 0.00 : (Double)summary.DisponibleHistoricos;
                //
                lista.Add(item);
            }

            var model = lista;

            return View(model);
        }

        //
        public ActionResult ResumeChart(int detalleContingenteId, int year)
        {
            
            ICollection<SummaryContingentesViewModel> lista = new List<SummaryContingentesViewModel>();
            var summ = from d in _context.DetallesContingente
                       join c in _context.Contingentes on d.contingenteId equals c.contingenteId
                       join t in _context.TiposContingente on c.tipoContingenteId equals t.tipoContingenteId
                       join u in _context.UnidadesMedida on t.unidadMedidaId equals u.unidadMedidaId
                       join s in _context.Solicitudes on d.detalleContingenteId equals s.detalleContingenteId
                       join us in _context.UnidadesMedida on s.unidadMedidaId equals us.unidadMedidaId
                       where d.anio == year
                       && d.detalleContingenteId == detalleContingenteId
                       group new { s.volumenAsignado, s.volumenSolicitado, s.volumenImportado, s.volumenReasignacion }
                       by new { d.detalleContingenteId, t.unidadMedidaId, d.anio, t.nombre, d.monto, d.montoNuevo, d.disponibleRedistribucion, d.disponibleRedistHistoricos } into grp
                       select new
                       {
                           Anio = grp.Key.anio,
                           TipoContingente = grp.Key.nombre,
                           Monto = grp.Key.monto,
                           MontoNuevos = grp.Key.montoNuevo,
                           Asignado = grp.Sum(s => s.volumenAsignado),
                           Solicitado = grp.Sum(s => s.volumenSolicitado),
                           Importado = grp.Sum(s => s.volumenImportado),
                           DetalleContingenteId = grp.Key.detalleContingenteId,
                           UnidadMedidaId = grp.Key.unidadMedidaId,
                           Reasignado = grp.Sum(s => s.volumenReasignacion),
                           DisponibleNuevos = grp.Key.disponibleRedistribucion,
                           DisponibleHistoricos = grp.Key.disponibleRedistHistoricos
                       }
                        ;
            ApplicationDbContext ctx = new ApplicationDbContext();
            foreach (var summary in summ)
            {
                var item = new SummaryContingentesViewModel();
                item.Anio = DateTime.Now.Year;
                item.Id = summary.TipoContingente;
                item.Asignado = (Double)summary.Asignado;
                item.MontoNuevos = summary.MontoNuevos;
                item.MontoTotal = summary.Monto;
                item.Nombre = summary.TipoContingente + " - " + year.ToString();
                item.Solicitado = (Double)summary.Solicitado;
                item.Importado = (Double)summary.Importado;
                item.Redistribucion = item.Asignado - item.Importado;
                item.MontoHistoricos = item.MontoTotal - item.MontoNuevos;
                item.UnidadMedida = ctx.UnidadesMedida.Single(u => u.unidadMedidaId == summary.UnidadMedidaId);
                item.DetalleContingenteId = summary.DetalleContingenteId;
                item.Reasignacion = (Double)summary.Reasignado;
                item.DisponibleNuevos = (Double?)summary.DisponibleNuevos ?? 0.00; //  SDA.Services.ContingenteServices.SumDisponibleReasignacion(item.DetalleContingenteId);
                item.DisponibleHist = (Double?)summary.DisponibleHistoricos ?? 0.00;
                //
                lista.Add(item);
            }

            var model = lista;

            int[] test = { 3, 5, 6, 7, 3, 8, 9 };

            
            ViewBag.intArray = test;
            return View(model);
        }

        public void DownloadPDF()
        {
            string HTMLContent = "Licencia";

            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + "PDFfile.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.BinaryWrite(GetPDF(HTMLContent));
            Response.End();
        }

        public byte[] GetPDF(string pHTML)
        {
            byte[] bPDF = null;

            MemoryStream ms = new MemoryStream();
            TextReader txtReader = new StringReader(pHTML);

            // 1: create object of a itextsharp document class
            Document doc = new Document(PageSize.A4, 25, 25, 25, 25);

            // 2: we create a itextsharp pdfwriter that listens to the document and directs a XML-stream to a file
            PdfWriter oPdfWriter = PdfWriter.GetInstance(doc, ms);

            // 3: we create a worker parse the document
            HTMLWorker htmlWorker = new HTMLWorker(doc);

            // 4: we open document and start the worker on the document
            doc.Open();
            htmlWorker.StartDocument();

            // 5: parse the html into the document
            htmlWorker.Parse(txtReader);

            // 6: close the document and the worker
            htmlWorker.EndDocument();
            htmlWorker.Close();
            doc.Close();

            bPDF = ms.ToArray();

            return bPDF;
        }
    }
}