using System.Collections.Generic;
using SDA.WebApp.Models;

namespace SDA.WebApp.ViewModels
{
    public class SolicitudFormViewModel
    {
        public List<Fraccion> Fracciones { get; set; }
        public Solicitud Solicitud { get; set; }
        public List<UnidadMedida> UnidadesMedida { get; set; }
    }
}