using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDA.WebApp.Models
{
    public class Requisito
    {

        public int Id { get; set; }

        public string Nombre { get; set; }

        public int TipoContingenteId { get; set; }

        public TipoContingente TipoContingente { get; set; }

        [Display(Name = "Indispensable?")]
        public bool Indispensable { get; set; }
    }
}