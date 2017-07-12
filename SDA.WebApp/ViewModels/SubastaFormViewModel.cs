using System.Linq;
using SDA.WebApp.Models;

namespace SDA.WebApp.ViewModels
{
    public class SubastaFormViewModel
    {
        public DetalleContingenteVario Detalle { get; internal set; }
        
        public Solicitud Solicitud { get; set; }
    }
}