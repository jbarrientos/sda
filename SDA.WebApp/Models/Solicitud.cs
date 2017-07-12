using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.WebApp.Models
{
    public class Solicitud
    {
        public int solicitudId { get; set; }

        [DisplayName("Contribuyente ID")]
        public int contribuyenteId { get; set; }

        [DisplayName("Contribuyente")]
        //[Required(ErrorMessage = "La solicitud debe estar asociada a un contribuyente")]
        public Contribuyente contribuyente { get; set; }

        [DisplayName("Fecha de registro")]
        public DateTime fechaRegistro { get; set; }

        [DisplayName("Periodo")]
        //[Required(ErrorMessage = "La solicitud debe estar asociada a una fracción de contingente")]
        public DetalleContingente DetalleContingente { get; set; }

        [DisplayName("Periodo ID")]
        public int? detalleContingenteId { get; set; }

        public int? ContingenteVarioId { get; set; }

        public ContingenteVario ContingenteVario { get; set; }

        [DisplayName("Fracción")]
        //[Required(ErrorMessage = "La solicitud debe estar asociada a una fracción de contingente")]
        public int? fraccionId { get; set; }


        [DisplayName("Volumen solicitado")]
        //[Range(0,999999999999, ErrorMessage = "El volumen de la solicitud debe ser mayor a 0")]
        public decimal volumenSolicitado { get; set; }

        [DisplayName("Volumen asignado")]
        public decimal volumenAsignado { get; set; }

        [DisplayName("Volumen importado")]
        public decimal? volumenImportado { get; set; }

        [DisplayName("Estado de la solicitud")]
        public string estado { get; set; }

        [DisplayName("Reasignación?")]
        public Boolean? reasignacion { get; set; }

        [DisplayName("Es importador histórico?")]
        public string esImportadorHistorico { get; set; }

        [DisplayName("Unidad Medida")]
        public int? unidadMedidaId{ get; set; }

        [DisplayName("Unidad Medida")]
        public UnidadMedida unidadMedida { get; set; }


        [DisplayName("Comentarios")]
        public string comentarios { get; set; }


        [DisplayName("Reasignación")]
        [DefaultValue(0.00)]
        public decimal? volumenReasignacion { get; set; }

        [DisplayName("Solicitado reasignación")]
        [DefaultValue(0.00)]
        public decimal? volumenSolicitadoReasignacion { get; set; }

        [DisplayName("Retirarse de reasignación?")]
        [DefaultValue(false)]
        public bool retirarReasignacion { get; set; }
        
        [DisplayName("Fecha retiro reasignación")]
        public DateTime? fechaRetiroReasignacion { get; set; }

        [DisplayName("Fecha solicitud reasignación")]
        public DateTime? fechaSolitudReasignacion { get; set; }

        [DisplayName("Saldo Reportado Importador")]
        [DefaultValue(0.00)]
        public decimal? saldoReportadoImportador { get; set; }

        [DisplayName("Valor Redistribuido")]
        [DefaultValue(0.00)]
        public decimal? valorRedistribucion { get; set; }

        public decimal? volumenImportadoReasignacion { get; set; }

        //public decimal? VolumenOriginal { get; set; }

        [Display(Name = "Fecha de envio")]
        public DateTime? FechaEnvio { get; set; }
        [Display(Name = "Certificado de Exportación")]
        public string CertificadoExportacion { get; set; }
        [Display(Name = "Fecha de Certificado")]
        public DateTime? FechaCertificado { get; set; }

        // public byte[] NotificacionFirmada { get; set; }

        public string RutaArchivoNotificacion { get; set; }


        public DateTime? FechaRetiro { get; set; }

        public string UsuarioRetiro { get; set; }
        [Display(Name = "Observaciones")]
        public string ObservacionesRetiro { get; set; }



    }
}
