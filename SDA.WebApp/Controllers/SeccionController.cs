using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDA.WebApp.Controllers
{
    public class SeccionController : Controller
    {

        ApplicationDbContext _context;

        public SeccionController()
        {
            _context = new ApplicationDbContext();
        }


        // GET: Seccion
        public ActionResult Index()
        {
            var model = _context.Secciones.ToList();
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = _context.Secciones.Single(s => s.seccionId == id);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = _context.Secciones.Single(s => s.seccionId == id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Seccion model)
        {
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}