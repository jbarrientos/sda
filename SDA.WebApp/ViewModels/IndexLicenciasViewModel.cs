using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDA.WebApp.ViewModels
{
    public class IndexLicenciasViewModel
    {
        [DisplayName("Id")]
        public string Id { get; set; }

        [DisplayName("Número de solicitud de licencia")]
        public int licenciaId { get; set; }

        [DisplayName("Código de la licencia")]
        public string codigo { get; set; }

        [DisplayName("Número de Acuerdo")]
        public string acuerdo { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Fecha de solicitud de la licencia")]
        public DateTime Fecha { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Fecha de vencimiento de la licencia")]
        public DateTime FechaVencimiento { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Fecha de emisión de la licencia")]
        public DateTime? FechaAcuerdo { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0.00}")]
        [DisplayName("Monto Otorgado")]
        public Double Volumen { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0.00}")]
        [DisplayName("Volumen importado de la licencia")]
        public Double VolumenImportado { get; set; }

        [DisplayName("Observaciones")]
        public string Observaciones { get; set; }

        [DisplayName("Unidad de Medida")]
        public UnidadMedida UnidadMedida { get; set; }

        public string Estado { get; set; }

        public TipoContingente TipoContingente { get; set; }

        public DetalleContingente DetalleContingente { get; set; }

        [DisplayName("Impresa?")]
        public bool Impresa { get; set; }
        [DisplayName("Fecha de impresión")]
        public DateTime? FechaImpresion { get; set; }

        public bool UploadedLicense { get; set; }


    }
}