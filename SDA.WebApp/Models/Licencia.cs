using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.WebApp.Models
{
    public class Licencia
    {
        [DisplayName("ID")]
        public int licenciaId { get; set; }


        [DisplayName("Solicitud ID")]
        public int solicitudId { get; set; }

        [DisplayName("Solicitud")]
        public Solicitud solicitud { get; set; }

        [DisplayName("Código")]
        public string codigo { get; set; }

        [DisplayName("No. Acuerdo")]
        public string noAcuerdo { get; set; }

        [DisplayName("Observaciones")]
        public string observaciones { get; set; }

        [DisplayName("Fecha")]
        public DateTime fecha { get; set; }

        [DisplayName("Fecha Vencimiento")]
        public DateTime? fechaVencimiento { get; set; }

        [Display(Name = "Volumen")]
        [Required(ErrorMessage = "Debe ingresar el volumen de importación de la licencia.")]
        public Double volumen { get; set; }

        [Display(Name = "Monto Importado")]
        public Double? volumenImportado { get; set; }

        [DisplayName("Unidad de Medida")]
        public int unidadMedidaId { get; set; }

        [DisplayName("Unidad de Medida")]
        public UnidadMedida unidadMedida { get; set; }

        [DisplayName("Para reasignación")]
        public string paraReasignacion { get; set; }
        [DisplayName("Estado")]
        public string estado { get; set; }
        [DisplayName("Impresa?")]
        public bool Impresa { get; set; }
        [DisplayName("Fecha de impresión")]
        public DateTime? FechaImpresion { get; set; }

        public bool Imprimir {
            get
            {
                var comp = this.fecha.Date.CompareTo(DateTime.Today.Date);
                return  comp <= 0;
            }
        }
        [DisplayName("Licencia Firmada")]
        public byte[] LicenciaFirmada { get; set; }

        public string MimeType { get; set; }

        public int DocumentSize { get; set; }

        public string DocumentName { get; set; }

        public bool SignedLicenseUploaded { get; set; }

        public DateTime? SignedLicenseUploadedDate { get; set; }

        [DisplayName("Fecha de Acuerdo")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaAcuerdo { get; set; }

        [Display(Name = "ID Licencia a Renovar")]
        public int? LicenciaRenovadaId { get; set; }

        public Licencia LicenciaRenovada { get; set; }



    }
}
