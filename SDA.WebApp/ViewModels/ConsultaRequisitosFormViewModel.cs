using System.Collections.Generic;
using SDA.WebApp.Models;

namespace SDA.WebApp.ViewModels
{
    public class ConsultaRequisitosFormViewModel
    {
        public List<Requisito> Requisitos { get; set; }
        public TipoContingente TipoContingente { get; set; }
    }
}