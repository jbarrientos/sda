using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.WebApp.Models
{
    public class UnidadMedida
    {
        public int unidadMedidaId { get; set; }

        [DisplayName("Nombre")]
        [Required(ErrorMessage = "Debe digitar el nombre de la unidad de medida.")]
        public string nombre { get; set; }

        [DisplayName("Unidad base?")]
        public bool unidadBase { get; set; }

        [DisplayName("Factor de conversión")]
        public Double factor { get; set; }

        [DisplayName("Unidad de medida base")]
        public int unidadMedidaBaseId { get; set; }

        public string Abreviatura { get; set; }

        //public UnidadMedida UnidadMedidaConversion { get; set; }

    }
}
