using System.Collections.Generic;
using SDA.WebApp.Models;

namespace SDA.WebApp.ViewModels
{
    public class DetalleSolicitudIndexViewModel
    {
        public DetalleContingente Contingente { get; internal set; }
        public List<Detalle> Detalles { get; set; }
        public List<FraccionTipoContingente> Fracciones { get; set; }
    }

    public class Detalle
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public List<double> Asignados { get; set; }
    }
}