using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.WebApp.Models
{
    public class Subpartida
    {
        public int subpartidaId { get; set; }
        public Partida partida { get; set; }

        [DisplayName("Nombre")]
        [Required(ErrorMessage = "Debe digitar el nombre de la subpartida.")]
        public string nombre { get; set; }

        [DisplayName("Código")]
        [Required(ErrorMessage = "Debe digitar el código de la sección.")]
        public string codigo { get; set; }

        public ICollection<Fraccion> fracciones { get; set; }
    }
}
