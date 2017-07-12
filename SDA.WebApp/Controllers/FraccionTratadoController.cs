using SDA.WebApp.Models;
using SDA.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDA.WebApp.Controllers
{
    public class FraccionTratadoController : Controller
    {

        ApplicationDbContext _context;

        public FraccionTratadoController()
        {
            _context = new ApplicationDbContext();
        }



        // GET: FraccionTratado
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(int id)
        {
            
            var fraccionesTratado = _context.FraccionesTratado.ToList<FraccionTratado>();
            var fraccion = fraccionesTratado.Find(p => p.fraccionTratadoId == id);
                
            var model = new IndexFraccionModel();
            model.arancel = fraccion.arancel;
            model.Categoria = _context.Categorias.ToList<Categoria>().Find(p => p.categoriaId == fraccion.categoriaId);
            model.Fraccion = _context.Fracciones.ToList<Fraccion>().Find(p => p.fraccionId == fraccion.fraccionId);
            model.Tratado = _context.Tratados.ToList<Tratado>().Find(p => p.tratadoId == fraccion.tratadoId);
            //
            return View(model);

        }

        public ActionResult Edit(int Id)
        {
            var model = _context.FraccionesTratado.Single(f => f.fraccionTratadoId == Id);
            ViewBag.tratado = _context.Tratados.Single(t => t.tratadoId == model.tratadoId);
            ViewBag.categoria = _context.Categorias.Single(c => c.categoriaId == model.categoriaId);
            ViewBag.fraccion = _context.Fracciones.Single(f => f.fraccionId == model.fraccionId);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(FraccionTratado model)
        {
            _context.SaveChanges();
            return RedirectToAction("IndexFracciones", "Categoria", new { id = model.categoriaId});
        }
    }
}