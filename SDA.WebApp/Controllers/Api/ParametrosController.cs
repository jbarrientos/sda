using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SDA.WebApp.Controllers.Api
{
    public class ParametrosController : ApiController
    {

        ApplicationDbContext _context;

        public ParametrosController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpDelete]
        public IHttpActionResult RemoveParameter(int id)
        {
            var parametro = _context.Parametros.SingleOrDefault(p => p.Id == id);

            if (parametro == null)
                return NotFound();

            _context.Parametros.Remove(parametro);
            _context.SaveChanges();
            return Ok();
        }
    }
}
