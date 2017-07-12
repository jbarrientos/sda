using System.Collections.Generic;
using SDA.WebApp.Models;

namespace SDA.WebApp.ViewModels
{
    public class ContribuyenteDisplayChartViewModel
    {
        public Contribuyente Contribuyente { get; set; }
        public List<Solicitud> Resumen { get; set; }
    }
}