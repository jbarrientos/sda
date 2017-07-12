using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SDA.WebApp.Controllers.Api
{
    public class RequisitosController : ApiController
    {

        ApplicationDbContext _context;

        public RequisitosController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpDelete]
        public IHttpActionResult DeleteRequisito(int id)
        {
            var requisito = _context.Requisitos.Single(r => r.Id == id);

            if (requisito == null)
                return NotFound();

            _context.Requisitos.Remove(requisito);
            _context.SaveChanges();

            return Ok();
        }
    }
}
