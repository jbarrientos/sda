using System.Collections.Generic;
using SDA.WebApp.Models;

namespace SDA.WebApp.ViewModels
{
    public class SolicitudConsultaViewModel
    {
        public List<RequisitoSolicitud> Requisitos { get; set; }
        public Solicitud Solicitud { get; set; }

        public List<Requisito> Pendientes { get; set; }
        public List<DetalleSolicitud> DetallesFraccion { get; set; }
        public TipoContingente TipoContingente { get; set; }
        public List<Licencia> Licencias { get; set; }
    }
}