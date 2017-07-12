using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.WebApp.Models
{
    public class Fraccion
    {
        [DisplayName("ID")]
        public int fraccionId { get; set; }

        [DisplayName("Sub Partida")]
        public Subpartida subpartida { get; set; }

        [DisplayName("Partida")]
        public Partida partida { get; set; }

        [DisplayName("Fracción")]
        [Required(ErrorMessage = "Debe digitar el nombre de la fracción.")]
        public string nombre { get; set; }

        [DisplayName("Código")]
        [Required(ErrorMessage = "Debe digitar el código de la fracción.")]
        public string codigo { get; set; }

        [DisplayName("DAI %")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N1}")]
        public Decimal dai { get; set; }

        [DisplayName("Parte")]
        public Parte parte { get; set; }

        [DisplayName("Fraccion dependiente")]
        public int? fraccionParentId { get; set; }

        public string DisplayName { get
            {
                return this.codigo + " - " + this.nombre;
            } }

    }
}
