using System.Collections.Generic;
using SDA.WebApp.Models;

namespace SDA.WebApp.ViewModels
{
    public class ContingenteVarioFormViewModel
    {
        public ContingenteVario Contingente { get; set; }
        public List<TipoContingente> TiposContingente { get; set; }
        public List<Tratado> Tratados { get; set; }

        public string Titulo { get; set; }
    }
}