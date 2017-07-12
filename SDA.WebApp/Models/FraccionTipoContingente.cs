using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.WebApp.Models
{
    public class FraccionTipoContingente
    {
        public int fraccionTipoContingenteId { get; set; }

        [DisplayName("Fracción")]
        [Required(ErrorMessage = "El ID de Fracción es requerido.")]
        public int fraccionId { get; set; }

        [DisplayName("Tipo Contingente")]
        [Required(ErrorMessage = "El tipo de contingente es requerido.")]
        public int tipoContingenteId { get; set; }

        public Fraccion Fraccion { get; set; }

        public TipoContingente TipoContingente { get; set; }
    }
}
