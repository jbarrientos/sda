using System.Collections.Generic;
using SDA.WebApp.Models;

namespace SDA.WebApp.ViewModels
{
    public class LicenciaIndexViewModel
    {
        public List<Licencia> Licencias { get; set; }
        public Solicitud Solicitud { get; set; }
    }
}