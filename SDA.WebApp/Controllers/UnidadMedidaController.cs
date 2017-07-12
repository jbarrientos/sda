using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDA.WebApp.Controllers
{
    public class UnidadMedidaController : Controller
    {

        ApplicationDbContext _context;
        public UnidadMedidaController()
        {

            _context = new ApplicationDbContext();

        }



        // GET: UnidadMedida
        public ActionResult Index()
        {

            var model = _context.UnidadesMedida.ToList();
            return View(model);
        }

        public ActionResult Create()
        {

            var model = new UnidadMedida();
            var baseUnits = _context.UnidadesMedida.Where(item => item.unidadBase == true).ToList();
            ViewBag.unidadesBase = baseUnits;
            return View(model);

        }

        [HttpPost]
        public ActionResult Create(UnidadMedida model)
        {
            _context.UnidadesMedida.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Details", new { id = model.unidadMedidaId });
        }

        public ActionResult Edit(int Id)
        {
            var model = _context.UnidadesMedida.Single(u => u.unidadMedidaId == Id);
            var baseUnits = _context.UnidadesMedida.Where(item => item.unidadBase == true).ToList();
            ViewBag.unidadesBase = baseUnits;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(UnidadMedida model)
        {
            _context.SaveChanges();
            return RedirectToAction("Index");

        }

        public ActionResult Details(int id)
        {
            var model = _context.UnidadesMedida.Single(u => u.unidadMedidaId == id);
            //
            return View(model);
        }
    }
}