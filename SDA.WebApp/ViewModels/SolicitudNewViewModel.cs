using System.Collections.Generic;
using SDA.WebApp.Models;

namespace SDA.WebApp.ViewModels
{
    public class SolicitudNewViewModel
    {
        public DetalleContingente DetalleContingente { get; set; }
        public List<Requisito> Requisitos { get; set; }
        public Solicitud Solicitud { get; set; }

        public Contribuyente Importador { get; set; }

        public List<UnidadMedida> UnidadesMedida { get; set; }

        public List<Fraccion> Fracciones { get; set; }
    }
}