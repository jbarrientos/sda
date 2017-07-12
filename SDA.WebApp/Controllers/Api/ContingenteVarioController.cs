using SDA.WebApp.Helpers;
using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SDA.WebApp.Controllers.Api
{
    public class ContingenteVarioController : ApiController
    {

        public ApplicationDbContext _context;

        public ContingenteVarioController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpPost]
        public IHttpActionResult GenerateRequests(int id)
        {

            var contingente = _context.ContingentesVarios
                .Include(c => c.TipoContingente)
                .SingleOrDefault(c => c.Id == id);

            if(contingente == null)
            {
                return NotFound();
            }

            var detalles = _context.DetallesContingenteVario
                .Where(d => d.ContingenteVarioId == id)
                .ToList();

            foreach (var detalle in detalles)
            {
                var solicitud = new Solicitud
                {
                    comentarios = "Solicitud generada automáticamente por el Sistema",
                    contribuyenteId = detalle.ContribuyenteId,
                    ContingenteVarioId = contingente.Id,
                    esImportadorHistorico = "I",
                    estado = "R",
                    fechaRegistro = DateTime.Now,
                    unidadMedidaId = contingente.TipoContingente.unidadMedidaId,
                    volumenAsignado = (decimal)detalle.Recepcion,
                    volumenImportado = (decimal)0.00,
                    volumenSolicitado = (decimal)detalle.Recepcion
                };
                _context.Solicitudes.Add(solicitud);
            }
            contingente.SolicitudesGeneradas = true;
            _context.SaveChanges();
            // Generar licencias
            if (contingente.GenerarLicencias)
            {
                var solicitudes = _context.Solicitudes
                    .Where(s => s.ContingenteVarioId == contingente.Id)
                    .ToList();
                foreach(var solic in solicitudes)
                {
                    var licencia1 = new Licencia
                    {
                        codigo = "",
                        estado = "R",
                        fecha = DateTime.Today,
                        fechaVencimiento = 
                        contingente.FechaLicenciaSegundaFase.Value.AddDays(-1),
                        noAcuerdo = "",
                        observaciones = "Licencia Generada Automáticamente. 1a. Fase.",
                        solicitudId = solic.solicitudId,
                        unidadMedidaId = contingente.TipoContingente.unidadMedidaId,
                        volumen = (double)solic.volumenAsignado * 
                        (double)(contingente.PorcentajePrimeraFase / 100)
                    };
                    _context.Licencias.Add(licencia1);
                    //var licencia2 = new Licencia
                    //{
                    //    codigo = Utils.GetCorrelativo(contingente.Anio,
                    //    contingente.TipoContingente.tipoNomenclaturaId,
                    //    _context),
                    //    estado = "R",
                    //    fecha = (DateTime)contingente.FechaLicenciaSegundaFase,
                    //    fechaVencimiento = contingente.FechaFinal,
                    //    noAcuerdo = "",
                    //    observaciones = "Licencia Generada Automáticamente. 2da Fase.",
                    //    solicitudId = solic.solicitudId,
                    //    unidadMedidaId = contingente.TipoContingente.unidadMedidaId,
                    //    volumen = (double)solic.volumenAsignado *
                    //    (double)((100 - contingente.PorcentajePrimeraFase) / 100)
                    //};
                    //_context.Licencias.Add(licencia2);
                }
            }
            _context.SaveChanges();
            return Ok();
        }
    }
}
