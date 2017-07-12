using System.Linq;
using SDA.WebApp.Models;
using System.Collections.Generic;

namespace SDA.WebApp.ViewModels
{
    public class ContribuyenteDetailViewModel
    {
        public Contribuyente Contribuyente { get; set; }
        public List<Solicitud> Solicitudes { get; set; }
    }
}