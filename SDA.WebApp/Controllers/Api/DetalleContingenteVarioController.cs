using SDA.WebApp.dtos;
using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SDA.WebApp.Controllers.Api
{
    public class DetalleContingenteVarioController : ApiController
    {

        ApplicationDbContext _context;

        public DetalleContingenteVarioController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpPost]
        public IHttpActionResult AddDetalle(NewDetalleContingenteVarioDto newDetalleDto)
        {
            var importador = _context.Contribuyentes
                .Single(c => c.contribuyenteId == newDetalleDto.ContribuyenteId);

            var contingente = _context.ContingentesVarios
                .SingleOrDefault(c => c.Id == newDetalleDto.ContingenteVarioId);

            var detalleContingente = new DetalleContingenteVario
            {
                ContingenteVarioId = contingente.Id,
                ContribuyenteId = importador.contribuyenteId,
                PorcentajeRecepcion = newDetalleDto.Porcentaje,
                Recepcion = 
                contingente.CalcularPorcentaje ?
                contingente.TotalRecepcion * newDetalleDto.Porcentaje / 100 : 
                newDetalleDto.Porcentaje
            };
            _context.DetallesContingenteVario.Add(detalleContingente);
            _context.SaveChanges();
            return Ok();
        }


        [HttpDelete]
        public IHttpActionResult RetirarImportador(int id)
        {
            var detalle = _context.DetallesContingenteVario
                .SingleOrDefault(r => r.Id == id);

            if (detalle == null)
                return NotFound();

            _context.DetallesContingenteVario.Remove(detalle);
            _context.SaveChanges();

            return Ok();
        }
    }
}
