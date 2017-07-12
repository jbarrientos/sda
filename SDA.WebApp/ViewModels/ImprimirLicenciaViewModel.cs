using SDA.WebApp.Models;

namespace SDA.WebApp.ViewModels
{
    public class ImprimirLicenciaViewModel
    {
        public Licencia Licencia { get; set; }
        public TipoContingente TipoContingente { get; internal set; }
    }
}