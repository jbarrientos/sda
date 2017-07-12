using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDA.WebApp.Controllers
{
    public class ProductoController : Controller
    {
        ApplicationDbContext _context;

        public ProductoController()
        {
            _context = new ApplicationDbContext();

        }

        // GET: Producto
        public ActionResult Index()
        {
            var model = _context.Productos.ToList();
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new Producto();
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(Producto model)
        {
            _context.Productos.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int Id)
        {
            var model = _context.Productos.Single(p => p.productoId == Id);
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(Producto model)
        {
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Details(int Id)
        {
            var model = _context.Productos.Single(p => p.productoId == Id);
            return View(model);
        }

    }
}