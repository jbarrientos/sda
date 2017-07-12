using MvcFlashMessages;
using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDA.WebApp.Controllers
{
    public class ParametrosController : Controller
    {

        public ApplicationDbContext _context;

        public ParametrosController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Parametros
        public ActionResult Index()
        {

            var parametros = _context.Parametros.ToList();

            return View(parametros);
        }

        public ActionResult Create()
        {

            var parametro = new Parametro();

            return View(parametro);
        }

        [HttpPost]
        public ActionResult Create(Parametro model)
        {

            _context.Parametros.Add(model);
            _context.SaveChanges();
            this.Flash("success", "Parametro de sistema creado exitosamente.");
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var parametro = _context.Parametros
                .SingleOrDefault(p => p.Id == id);

            if (parametro == null)
                return HttpNotFound();

            return View(parametro);
        }
        [HttpPost]
        public ActionResult Edit(Parametro model)
        {
            var parametro = _context.Parametros
                .SingleOrDefault(p => p.Id == model.Id);

            if (parametro == null)
                return HttpNotFound();

            parametro.Codigo = model.Codigo;
            parametro.Valor = model.Valor;
            _context.SaveChanges();
            this.Flash("success", "Parametro de sistema actualizado exitosamente.");
            return RedirectToAction("Index");
        }
    }
}