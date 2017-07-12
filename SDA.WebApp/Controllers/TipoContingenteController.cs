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
    public class TipoContingenteController : Controller
    {
        ApplicationDbContext _context;

        public TipoContingenteController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: TipoContingente
        public ActionResult Index()
        {
            //var model = this.tiposContingente.GetAll();
            //var model2 = new IndexTipoContingenteModel();
            //return View(model);

            
            ICollection<IndexTipoContingenteModel> lista = new List<IndexTipoContingenteModel>();
            var tiposContingente = _context.TiposContingente
                .Include(t => t.TipoNomenclatura)
                .Include(t => t.UnidadMedida).ToList();
            //
            foreach (TipoContingente fr in tiposContingente)
            {
                var item = new IndexTipoContingenteModel();
                item.Id = fr.tipoContingenteId.ToString();
                item.Nombre = fr.nombre;
                item.TipoNomenclatura = fr.TipoNomenclatura;
                item.UnidadMedida = fr.UnidadMedida;
                lista.Add(item);
            }
            //
            var model = lista; 
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new TipoContingente();
            ViewBag.nomenclaturas = _context.TiposNomenclatura.ToList();
            ViewBag.unidades = _context.UnidadesMedida.ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(TipoContingente model)
        {

            _context.TiposContingente.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int Id)
        {
            var model = _context.TiposContingente.Single(t => t.tipoContingenteId == Id);
            ViewBag.nomenclaturas = _context.TiposNomenclatura.ToList();
            ViewBag.unidades = _context.UnidadesMedida.ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(TipoContingente model)
        {
            var m = _context.TiposContingente.SingleOrDefault(t => t.tipoContingenteId == model.tipoContingenteId);

            if (m == null)
                return HttpNotFound();

            m.nombre = model.nombre;
            m.tipoNomenclaturaId = model.tipoNomenclaturaId;
            m.unidadMedidaId = model.unidadMedidaId;
            m.crecimiento = model.crecimiento;
            m.mecanismoSubasta = model.mecanismoSubasta;
            m.EspecificarFraccion = model.EspecificarFraccion;
            m.DistribuirPorFraccion = model.DistribuirPorFraccion;
            m.SepararHistoricos = model.SepararHistoricos;
            m.PorcentajeMaximo = model.PorcentajeMaximo;
            m.MesesVencimientoLicencia = model.MesesVencimientoLicencia;
            m.NombreImagen = model.NombreImagen;
           
            //
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}