using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.WebApp.Models
{
    public class Capitulo
    {
        public int capituloId { get; set; }
        public Seccion seccion { get; set; }

        [DisplayName("Nombre")]
        [Required(ErrorMessage = "Debe digitar el nombre del capitulo.")]
        public string nombre { get; set; }

        [DisplayName("Código")]
        [Required(ErrorMessage = "Debe digitar el código correspondiente.")]
        public string codigo { get; set; }

        public ICollection<Partida> partidas { get; set; }
    }
}
