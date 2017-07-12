using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.WebApp.Models
{
    public class TipoNomenclatura
    {
        public int tipoNomenclaturaId { get; set; }

        [DisplayName("Nombre")]
        [Required(ErrorMessage = "Debe digitar el nombre del tipo de nomenclatura.")]
        public string nombre { get; set; }

        [DisplayName("Código")]
        public string codigo { get; set; }

    }
}
