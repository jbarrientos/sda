using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.WebApp.Models
{
    public class CorrelativoAnual
    {
        public int correlativoAnualId { get; set; }

        [DisplayName("Año")]
        [Required(ErrorMessage = "Debe digitar el año.")]
        public int anio { get; set; }

        [DisplayName("Correlativo")]
        public int correlativo { get; set; }

    }
}
