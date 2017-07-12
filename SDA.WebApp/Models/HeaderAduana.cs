using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.WebApp.Models
{
    public class HeaderAduana
    {
        public int headerAduanaId { get; set; }
        [DisplayName("Año")]
        //[Required(ErrorMessage = "Debe digitar el año reportado.")]
        public int? anio { get; set; }

        [DisplayName("Mes")]
        //[Required(ErrorMessage = "Debe digitar el mes reportado.")]
        public int? mes { get; set; }

        [DisplayName("Status")]
        public string status { get; set; }

        [DisplayName("Fecha de carga")]
        public DateTime fechaCarga { get; set; }


        [DisplayName("Fecha de Aplicación")]
        public DateTime? FechaAplicacion { get; set; }

        [DisplayName("ID Detalle Contingente")]
        public int? detalleContingenteId { get; set; }

        [DisplayName("Detalle Contingente")]
        public DetalleContingente detalleContingente { get; set; }
        [DisplayName("Aplicado?")]
        public bool Aplicado { get; set; }
        [DisplayName("Nombre del Archivo")]
        public string NombreArchivo { get; set; }

        public string Comentarios { get; set; }
    }
}
