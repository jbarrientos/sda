using System.Collections.Generic;
using SDA.WebApp.Models;

namespace SDA.WebApp.ViewModels
{
    public class FormSolicitarLicenciaViewModel
    {
        public decimal? Disponible { get; set; }
        public List<Fraccion> Fracciones { get; set; }
        public Licencia Licencia { get; set; }
        public Solicitud Solicitud { get; set; }
        public TipoContingente TipoContingente { get; set; }
        public List<UnidadMedida> Unidades { get; set; }
    }
}