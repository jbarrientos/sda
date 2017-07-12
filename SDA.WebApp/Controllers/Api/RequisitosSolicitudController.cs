using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SDA.WebApp.Controllers.Api
{
    public class RequisitosSolicitudController : ApiController
    {

        ApplicationDbContext _context;

        public RequisitosSolicitudController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpDelete]
        public IHttpActionResult DeleteRequisito(int id)
        {
            var requisito = _context.RequisitosSolicitud.Single(r => r.Id == id);

            if (requisito == null)
                return NotFound();

            _context.RequisitosSolicitud.Remove(requisito);
            _context.SaveChanges();

            return Ok();
        }
    }
}
