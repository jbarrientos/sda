using SDA.WebApp.Models;
using SDA.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDA.WebApp.Controllers
{
    public class NotificacionInternaController : Controller
    {

        public ApplicationDbContext _context { get; set; }

        public NotificacionInternaController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: NotificacionInterna
        public ActionResult Index()
        {

            var notificaciones = _context.NotificacionesInternas.ToList();


            return View(notificaciones);
        }

        public ActionResult New()
        {                    

            return View(new NotificacionFormViewModel {
                Email = "",
                Nombre = ""
            });
        }
        [HttpPost]
        public ActionResult Save(NotificacionFormViewModel vm)
        {
            vm.Tipo = "C";
            if (!ModelState.IsValid)
            {
                
                var viewModel = new NotificacionFormViewModel
                {
                    Email = vm.Email,
                    Nombre = vm.Nombre,
                    Tipo = vm.Tipo

                };
                return View("New", viewModel);

            }

            var model = new NotificacionInterna();
            model.Email = vm.Email;
            model.Nombre = vm.Nombre;
            model.Tipo = vm.Tipo;
            

            _context.NotificacionesInternas.Add(model);

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}