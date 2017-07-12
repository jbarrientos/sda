using System.Collections.Generic;
using SDA.WebApp.Models;

namespace SDA.WebApp.ViewModels
{
    public class CambiosAsignacionIndexViewModel
    {
        public List<CambioAsignacion> CambiosAsignacion { get; set; }
        public DetalleAsignacion DetalleAsignacion { get; set; }
    }
}