using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.WebApp.Models
{
    public class ImportacionExtraordinaria
    {
        public int importacionExtraordinariaId { get; set; }

        [DisplayName("Fecha inicio")]
        [Required(ErrorMessage = "Debe ingresar la fecha de inicio")]
        public DateTime fechaInicio { get; set; }
        [DisplayName("Fecha final")]
        public DateTime fechaFinal { get; set; }

        [DisplayName("Descripción")]
        [Required(ErrorMessage = "Debe brindar una descripción")]
        public string nombre { get; set; }


        [DisplayName("Unidad de medida")]
        [Required(ErrorMessage = "Debe ingresar la unidad de medida en la que se expresaran los volumenes")]
        public UnidadMedida unidadMedida { get; set;}
        [DisplayName("Detalles")]
        public string detalles { get; set; }
    }
}
