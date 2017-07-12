using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDA.WebApp.ViewModels
{
    public class IndexSolicitudes
    {

        public string Id { get; set; }

        [DisplayName("Fecha de Registro")]
        public DateTime FechaRegistro { get; set; }

        public string Estado { get; set; }

        [DisplayName("Fecha retiro reasig.")]
        public DateTime FechaRetiroReasignacion { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0.00}")]
        [DisplayName("Solicitado")]
        public Double VolumenSolicitado { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0.00}")]
        [DisplayName("Asignado")]
        public Double VolumenAsignado { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0.00}")]
        [DisplayName("Importado")]
        public Double VolumenImportado { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0.00}")]
        [DisplayName("Saldo")]
        public Double VolumenARedistribuir
        {
            get; set;
        }

        [DisplayName("Retirar?")]
        public Boolean RetirarReasignacion
        {
            get; set;
        }

        [DisplayFormat(DataFormatString = "{0:###,##0.00}")]
        [DisplayName("Reasig.")]
        public Double VolumenReasignacion
        {
            get; set;
        }

        [DisplayFormat(DataFormatString = "{0:###,##0.00}")]
        [DisplayName("Reasig. Importado")]
        public Double VolumenReasignacionImportado
        {
            get; set;
        }

        public Double SaldoReasignacion
        {
            get
            {
                return this.VolumenReasignacion - this.VolumenReasignacionImportado;
            }
        }


        [DisplayFormat(DataFormatString = "{0:###,##0.00}")]
        [DisplayName("Solic. Reasig.")]
        public Double VolumenSolicitadoReasignacion
        {
            get; set;
        }

        [DisplayName("Importador")]
        public Contribuyente Contribuyente { get; set; }

        [DisplayName("Histórico?")]
        public string Historico { get; set; }

        [DisplayName("Fracción")]
        public Fraccion Fraccion { get; set; }

        [DisplayName("Unidad de Medida")]
        public UnidadMedida UnidadMedida { get; set; }

        public int Periodo { get; set; }

        public bool Incrementa
        {
            get
            {
                return this.VolumenSolicitadoReasignacion > (this.VolumenAsignado - this.VolumenImportado) && this.VolumenSolicitadoReasignacion != 0.00;
            }
        }

        public DetalleContingente DetalleContigente { get; set; }

        public int NumNotificaciones { get; set; }

        public TipoContingente TipoContingente { get; set; }

        [Display(Name = "Certificado")]
        public string CertificadoExportacion { get; set; }
        [Display(Name = "Fecha de Certificado")]
        public DateTime? FechaCertificado { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0.00}")]
        public double? SumaOtorgadoLicencias { get; set; }

        public bool Decrementa
        {
            get
            {
                return this.VolumenSolicitadoReasignacion < (this.VolumenAsignado - this.VolumenImportado) && this.VolumenSolicitadoReasignacion != 0.00;
            }
        }
    }
}