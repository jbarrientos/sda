using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SDA.WebApp.ViewModels
{
    public class AddFraccionViewModel
    {

        public string Id { get; set; }

        [DisplayName("Arancel")]
        public decimal Arancel { get; set; }

        [DisplayName("Excluido?")]
        public Boolean Excluido { get; set; }

        [DisplayName("Fracción")]
        public IEnumerable<Fraccion> FraccionesList { get; set; }

        public int CategoriaId { get; set; }
    }
}