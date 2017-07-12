using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.WebApp.Models
{
    public class Partida
    {
        public int partidaId { get; set; }
        public Capitulo capitulo { get; set; }

        [DisplayName("Nombre")]
        [Required(ErrorMessage = "Debe digitar el nombre de la partida.")]
        public string nombre { get; set; }

        [DisplayName("Código")]
        [Required(ErrorMessage = "Debe digitar el código de la partida.")]
        public string codigo { get; set; }

        public ICollection<Subpartida> subpartidas { get; set; }

    }
}
