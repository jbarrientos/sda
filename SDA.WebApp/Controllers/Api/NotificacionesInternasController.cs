using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SDA.WebApp.Controllers.Api
{
    public class NotificacionesInternasController : ApiController
    {

        ApplicationDbContext _context;

        public NotificacionesInternasController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpDelete]
        public IHttpActionResult DeleteNotificacion(int id)
        {
            var notificacion = _context.NotificacionesInternas.Single(r => r.Id == id);

            if (notificacion == null)
                return NotFound();

            _context.NotificacionesInternas.Remove(notificacion);
            _context.SaveChanges();

            return Ok();
        }
    }
}
