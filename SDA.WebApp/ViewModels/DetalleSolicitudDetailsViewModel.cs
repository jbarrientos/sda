using System.Collections.Generic;
using SDA.WebApp.Models;

namespace SDA.WebApp.ViewModels
{
    public class DetalleSolicitudDetailsViewModel
    {
        public List<DetalleSolicitud> Fracciones { get; set; }
        public Solicitud Solicitud { get; set; }
        public double TotalAsignado { get; internal set; }
        public double TotalSolicitado { get; internal set; }
    }
}