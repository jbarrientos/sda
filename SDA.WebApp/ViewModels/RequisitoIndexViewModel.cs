using System.Linq;
using SDA.WebApp.Models;

namespace SDA.WebApp.ViewModels
{
    public class RequisitoIndexViewModel
    {
        public IQueryable<Requisito> Requisitos { get; set; }
        public TipoContingente TipoContingente { get; set; }
    }
}