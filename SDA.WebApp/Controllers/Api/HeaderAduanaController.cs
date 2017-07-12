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
    public class HeaderAduanaController : ApiController
    {

        public ApplicationDbContext _context;
        public HeaderAduanaController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpPost]
        public IHttpActionResult Apply(int id)
        {
            var header = _context.HeadersAduana
                .SingleOrDefault(h => h.headerAduanaId == id);

            if (header == null)
                return NotFound();

            var lineas = _context.DetallesAduana
                .Where(d => d.headerAduanaId == id)
                .ToList();

            foreach (var linea in lineas)
            {
                var licencia = _context.Licencias
                    .Include(l => l.solicitud.DetalleContingente
                    .Contingente.TipoContingente.UnidadMedida)
                    .SingleOrDefault(l => l.codigo == linea.licencia);
                if(licencia != null)
                {
                    var valorImportado = 0.00;
                    if (Constant.ID_UNIDAD_MEDIDA_ADUANA ==
                        licencia.solicitud.DetalleContingente.Contingente.TipoContingente.unidadMedidaId)
                    {
                        valorImportado = linea.pesoNeto;
                    }else
                    {
                        var unidadMedidaAduana = _context.UnidadesMedida
                            .SingleOrDefault(u => u.unidadMedidaId == Constant.ID_UNIDAD_MEDIDA_ADUANA);
                        valorImportado = linea.pesoNeto / unidadMedidaAduana.factor;
                         
                    }
                    licencia.solicitud.volumenImportado += (decimal)valorImportado;
                }
            }
            header.Aplicado = true;
            _context.SaveChanges();
            return Ok();
        }
    }
}
