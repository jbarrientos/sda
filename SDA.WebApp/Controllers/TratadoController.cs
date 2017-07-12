using SDA.WebApp.Models;
using SDA.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDA.WebApp.Controllers
{
    public class TratadoController : Controller
    {

        ApplicationDbContext _context;

        string fechaIni = "01-01";
        string fechaFin = "12-31";
        string fechaIniSolic = "12-01";
        string fechaFinSolic = "12-21";
        string fechaIniSolicR = "09-01";
        string fechaFinSolicR = "09-15";

        
        public TratadoController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Tratado
        public ActionResult Index()
        {
            var model = _context.Tratados.ToList();
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new Tratado();
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(Tratado model)
        {
            _context.Tratados.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var model = _context.Tratados.Single(t => t.tratadoId == id);
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(Tratado model)
        {
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            
            var model = _context.Tratados.Single(t => t.tratadoId == id);
            var paisesTratado = from t in _context.PaisesTratado
                                where t.tratado.tratadoId == id
                                select t.pais;
            //
            ViewBag.paises = paisesTratado;
            return View(model);
        }

        public ActionResult IndexCategories(int id)
        {
            
            var model = _context.Tratados.Single(t => t.tratadoId == id);
            var categoriasTratado = from t in _context.Categorias
                                where t.tratado.tratadoId == id
                                select t;
            //
            ViewBag.categorias = categoriasTratado.ToList<Categoria>();
            return View(model);
        }

        public ActionResult IndexFracciones(int id)
        {
            
            var tratado = _context.Tratados.Single(t => t.tratadoId == id);
            //var frac = from t in db.Fracciones select t;
            //var fracc = frac.ToList<Fraccion>();
            ICollection<IndexFraccionModel> lista = new List<IndexFraccionModel>();
            
            var fracciones = from t in _context.FraccionesTratado
                                    where t.tratadoId == id
                                    select t;
            //
            ViewBag.tratado = tratado;
            //var model = fracciones.ToList<FraccionTratado>();
            foreach(FraccionTratado fr in fracciones)
            {
                var item = new IndexFraccionModel();
                item.arancel = fr.arancel;
                item.categoriaId = fr.categoriaId;
                item.fraccionId = fr.fraccionId;
                item.Id = fr.fraccionTratadoId.ToString();
                item.Fraccion = _context.Fracciones.Single(f => f.fraccionId == fr.fraccionId);
                item.Categoria = _context.Categorias.Single(c => c.categoriaId == fr.categoriaId);
                lista.Add(item);
            }

            var model = lista; //  tratado.fracciones;
            return View(model);
        }


        public ActionResult CreateCategory(int tratadoId)
        {
            //DAL.Data.DataContext db = new DAL.Data.DataContext();
            var model = new Categoria();
            model.tratadoId = tratadoId;
            ViewBag.tratado = _context.Tratados.Single(t => t.tratadoId == tratadoId);
            //
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateCategory(Categoria model)
        {
            _context.Categorias.Add(model);
            _context.SaveChanges();
            return RedirectToAction("IndexCategories", new { id = model.tratadoId });

        }

        public ActionResult GenerarContingentes(int tratadoId)
        {
            var model = new GenerarContingentesViewModel();
            model.Tratado = _context.Tratados.Single(t => t.tratadoId == tratadoId);
            model.TratadoId = tratadoId;
            ViewBag.tiposContingente = _context.TiposContingente.ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult GenerarContingentes(GenerarContingentesViewModel viewModel)
        {
            // Primero Encabezado de Contingente
            var contingente = new Contingente();
            contingente.cuotaUnica = viewModel.CuotaUnica;

            /*
            contingente.fechaFin = viewModel.FechaFinal;
            contingente.fechaFinSolicitudes = viewModel.FechaFinalSolicitudes;
            contingente.fechaFinSolicitudesRe = viewModel.FechaFinalSolicitudesReasignacion;
            */
            contingente.porcentajeMinimoImportacion = (Decimal)viewModel.PorcentajeMinimoImportacion;
            contingente.porcentajeVolumenHistorico = (Decimal)viewModel.PorcentajeVolumenHistoricos;
            contingente.tipoContingenteId = viewModel.TipoContingenteId;
            contingente.tratadoId = viewModel.TratadoId;
            contingente.volumen = viewModel.volumen;
            contingente.aumentoVolumen = viewModel.PorcentajeAumentoVolumen;
            /*
            contingente.fechaInicio = viewModel.FechaInicio;
            contingente.fechaInicioSolicitudes = viewModel.FechaInicioSolicitudes;
            contingente.fechaInicioSolicitudesRe = viewModel.FechaInicioSolicitudesReasignacion;
            */
            _context.Contingentes.Add(contingente);
            _context.SaveChanges();
            // Detalle
            var tratado = _context.Tratados.Single(t => t.tratadoId == viewModel.TratadoId);
            int anioInicial = tratado.fechaInicio.Year;
            int anioFinal = anioInicial + tratado.numeroAnios;
            Double aumento = 0.00;
            Double acumulado = 0.00;
            //
            for (int y = anioInicial; anioFinal > y; y++)
            {
                //
                var periodo = new DetalleContingente();
                periodo.anio = y;
                periodo.contingenteId = contingente.contingenteId;
                //periodo.fechaFin = DateTime.Parse(y.ToString() + "-" + viewModel.FechaFinal.ToString("MM") + "-" + viewModel.FechaFinal.ToString("dd"));
                //periodo.fechaFinSolicitudes = DateTime.Parse(y.ToString() + "-" + viewModel.FechaFinalSolicitudes.ToString("MM") + "-" + viewModel.FechaFinalSolicitudes.ToString("dd"));
                //periodo.fechaFinSolicitudesRe = DateTime.Parse(y.ToString() + "-" + viewModel.FechaFinalSolicitudesReasignacion.ToString("MM") + "-" + viewModel.FechaFinalSolicitudesReasignacion.ToString("dd"));
                //periodo.fechaInicio = DateTime.Parse(y.ToString() + "-" + viewModel.FechaInicio.ToString("MM") + "-" + viewModel.FechaInicio.ToString("dd"));
                //periodo.fechaInicioSolicitudes = DateTime.Parse(y.ToString() + "-" + viewModel.FechaInicioSolicitudes.ToString("MM") + "-" + viewModel.FechaInicioSolicitudes.ToString("dd"));
                //periodo.fechaInicioSolicitudesRe = DateTime.Parse(y.ToString() + "-" + viewModel.FechaInicioSolicitudesReasignacion.ToString("MM") + "-" + viewModel.FechaInicioSolicitudesReasignacion.ToString("dd"));
                periodo.fechaFin = DateTime.Parse(y.ToString() + "-" + this.fechaFin);
                periodo.fechaReasignacion = DateTime.Parse(y.ToString() + "-" + this.fechaIniSolicR);
                periodo.fechaFinSolicitudes = DateTime.Parse(y.ToString() + "-" + this.fechaFinSolic);
                periodo.fechaFinSolicitudesRe = DateTime.Parse(y.ToString() + "-" + this.fechaFinSolicR);
                periodo.fechaInicio = DateTime.Parse(y.ToString() + "-" + this.fechaIni);
                periodo.fechaInicioSolicitudes = DateTime.Parse(y.ToString() + "-" + this.fechaIniSolic);
                periodo.fechaInicioSolicitudesRe = DateTime.Parse(y.ToString() + "-" + this.fechaIniSolicR);
                //
                acumulado = acumulado == 0.00 ? viewModel.volumen : acumulado;
                //
                periodo.monto = acumulado;
                periodo.volumenNuevo = periodo.monto * ((100.00 - (Double)contingente.porcentajeVolumenHistorico) / 100.00);
                periodo.montoNuevo = (int)periodo.volumenNuevo;
                acumulado += viewModel.PorcentajeAumentoVolumen;
                _context.DetallesContingente.Add(periodo);
                //
                
            }

            _context.SaveChanges();
            

            

            return RedirectToAction("Index");
        }

    }
}