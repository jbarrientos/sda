using System.Collections.Generic;
using SDA.WebApp.Models;

namespace SDA.WebApp.ViewModels
{
    public class HeaderAduanaFormViewModel
    {
        public List<DetalleAduana> Detalle { get; internal set; }
        public HeaderAduana HeaderAduana { get; set; }
    }
}