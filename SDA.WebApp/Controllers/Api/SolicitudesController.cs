using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SDA.WebApp.Controllers.Api
{
    public class SolicitudesController : ApiController
    {
        private ApplicationDbContext _context;

        public SolicitudesController()
        {
            _context = new ApplicationDbContext();
        }

        public object Mapper { get; private set; }

        [HttpPut]
        public void UpdateSolicitud(int id)
        {

            var solicitudInDb = _context.Solicitudes.SingleOrDefault(m => m.solicitudId == id);
            if (solicitudInDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            solicitudInDb.estado = "A";

            _context.SaveChanges();
        }
    }

}

