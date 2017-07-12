using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SDA.WebApp.Controllers.Api
{
    public class DocumentoContingenteController : ApiController
    {

        ApplicationDbContext _context;

        public DocumentoContingenteController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpDelete]
        public IHttpActionResult DeleteDocument(int id)
        {
            var documento = _context.DocumentosContingente
                .SingleOrDefault(r => r.Id == id);

            if (documento == null)
                return NotFound();

            _context.DocumentosContingente.Remove(documento);
            _context.SaveChanges();

            return Ok();
        }
    }
}
