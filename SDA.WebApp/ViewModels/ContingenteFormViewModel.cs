using System.Collections.Generic;
using SDA.WebApp.Models;

namespace SDA.WebApp.ViewModels
{
    public class ContingenteFormViewModel
    {
        public List<Categoria> Categorias { get; internal set; }
        public Contingente Contingente { get; set; }
        public List<TipoContingente> TiposContingente { get; internal set; }
        public Tratado Tratado { get; set; }
    }
}