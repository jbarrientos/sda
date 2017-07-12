using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SDA.WebApp.Controllers.Api
{
    public class ContribuyentesController : ApiController
    {

        private ApplicationDbContext _context;

        public ContribuyentesController()
        {
            _context = new ApplicationDbContext();
        }

        public IHttpActionResult GetContribuyentes(string query = null)
        {

            var customersQuery = _context.Contribuyentes
                .Where(c => c.nombre.Contains(query))
                .ToList();

            //if (!String.IsNullOrWhiteSpace(query))
            //    customersQuery = customersQuery
            //        .Where(c => c.nombre.Contains(query));

            //var customerDtos = customersQuery
            //    .ToList()
            //    .Select(Mapper.Map<Customer, CustomerDto>);

            return Ok(customersQuery);
        }

        [HttpPut]
        public IHttpActionResult CreateContribuyente(int id)
        {

            var preregistro = _context.Preregistros.Single(p => p.Id == id);

            if (preregistro == null)
                return NotFound();

            var importador = new Contribuyente
            {
                ActividadEconomicaId = preregistro.ActividadEconomicaId,
                dui = preregistro.Dui,
                email = preregistro.Email,
                EmailAlternativo = preregistro.EmailAlterno,
                nit = preregistro.Nit,
                nombre = preregistro.Nombre,
                telefonoFijo = preregistro.Telefono,
                telefonoCelular = preregistro.Celular,
                tipoPersona = preregistro.TipoPersona,
                direccion = preregistro.Direccion,
                CargoRepresentanteLegal = preregistro.RepresentanteLegal,
                DUIRepresentanteLegal = preregistro.DUIRepresentanteLegal,
                NitRepresentanteLegal = preregistro.NitRepresentanteLegal,
                RepresentanteLegal = preregistro.RepresentanteLegal,
                NombreNotificacion = preregistro.Nombre,
                CargoNotificacion = preregistro.CargoRepresentanteLegal                
            };

            preregistro.Status = "F";

            _context.Contribuyentes.Add(importador);
            _context.SaveChanges();
            return Ok();
        }
    }
}
