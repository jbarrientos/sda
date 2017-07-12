using System.Collections.Generic;
using SDA.WebApp.Models;

namespace SDA.WebApp.ViewModels
{
    public class DetalleContingenteVarioIndexViewModel
    {
        public ContingenteVario Contingente { get; set; }
        public List<DetalleContingenteVario> Detalles { get; set; }
        public double PorcentajeAsignado { get; internal set; }
        public double Saldo { get; internal set; }
        public double TotalAsignado { get; internal set; }
    }
}