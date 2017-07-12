using SDA.WebApp.Models;

namespace SDA.WebApp.ViewModels
{
    public class RequisitoFormViewModel
    {
        public TipoContingente TipoContingente { get; set; }

        public string Nombre { get; set; }

        public bool Indispensable { get; set; }
    }
}