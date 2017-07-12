using SDA.WebApp.Models;
using SDA.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDA.WebApp.Controllers
{
    public class NotificacionController : Controller
    {

        public ApplicationDbContext _context;
        public NotificacionController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Notificacion
        public ActionResult Index(int id)
        {

            var solicitud = _context.Solicitudes.SingleOrDefault(s => s.solicitudId == id);

            if (solicitud == null)
                return HttpNotFound();

            var notificaciones = _context.Notificaciones
                .Where(n => n.SolicitudId == id && !n.Visto)
                .ToList();

            var vm = new NotificacionIndexViewModel
            {
                Solicitud = solicitud,
                Notificaciones = notificaciones
            };

            foreach(var nota in notificaciones)
            {
                nota.Visto = true;                
            }
            _context.SaveChanges();
            return View(vm);
        }
    }
}