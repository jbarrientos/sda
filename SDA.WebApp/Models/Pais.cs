using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.WebApp.Models
{
    public class Pais
    {
        public int paisId { get; set; }

        [DisplayName("Nombre")]
        [Required(ErrorMessage = "Debe digitar el nombre del país.")]
        public string nombre { get; set; }

        [DisplayName("Código ISO")]
        [Required(ErrorMessage = "Debe digitar el código ISO del país.")]
        public string isoCode { get; set; }
    }
}
