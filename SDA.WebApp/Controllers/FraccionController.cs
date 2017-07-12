using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDA.WebApp.Controllers
{
    public class FraccionController : Controller
    {

        private ApplicationDbContext _context;

        public FraccionController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Fraccion
        public ActionResult Index()
        {
            //var context = new DataContext();
            //var productos = context.Fracciones.ToList<Fraccion>();
            var model = _context.Fracciones.ToList(); //  productos.Where(p => p.fraccionId > 7199);
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new Fraccion();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(Fraccion model)
        {
            //var context = new DataContext();
            //IRepositoryBase<Fraccion> fracciones = 
            //    (IRepositoryBase<Fraccion>)context.Fracciones.ToList<Fraccion>();
            ////
            //fracciones.Insert(model);
            //fracciones.commit();
            //var fraccion = fracciones.Find(p => p.fraccionId == model.fraccionId);

            //fracciones.Insert(model);
            //fracciones.commit();
            _context.Fracciones.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id) {

            //var context = new DataContext();
            //IRepositoryBase<Fraccion> fracciones =
            //    (IRepositoryBase<Fraccion>)context.Fracciones.ToList<Fraccion>();
            var model = _context.Fracciones.Single(f => f.fraccionId == id);
            return View(model);
        }
    }
}