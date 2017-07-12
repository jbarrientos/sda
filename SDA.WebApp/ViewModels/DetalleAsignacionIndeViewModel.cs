using System.Collections.Generic;
using SDA.WebApp.Models;

namespace SDA.WebApp.ViewModels
{
    public class DetalleAsignacionIndeViewModel
    {
        public Asignacion Asignacion { get; set; }
        public List<DetalleAsignacion> Detalles { get; set; }
    }
}