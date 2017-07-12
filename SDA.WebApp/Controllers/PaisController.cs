using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDA.WebApp.Controllers
{
    public class PaisController : Controller
    {

        ApplicationDbContext _context;

        public PaisController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Pais
        public ActionResult Index()
        {
            var model = _context.Paises.ToList();
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new Pais();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(Pais model)
        {
            _context.Paises.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}