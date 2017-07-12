using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.WebApp.Models
{
    public class Parte
    {
        public int parteId { get; set; }

        [DisplayName("Nombre")]
        [Required(ErrorMessage = "Debe digitar el nombre de la parte SAC.")]
        public string nombre { get; set; }

        [DisplayName("Código")]
        [Required(ErrorMessage = "Debe digitar el código de la parte SAC.")]
        public string codigo { get; set; }
    }
}
