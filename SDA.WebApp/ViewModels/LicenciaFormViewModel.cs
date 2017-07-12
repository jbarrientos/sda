using SDA.WebApp.Models;

namespace SDA.WebApp.ViewModels
{
    public class LicenciaFormViewModel
    {
        public Licencia Licencia { get; set; }
        public Solicitud Solicitud { get; set; }
        public string Title { get; internal set; }
    }
}