using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.WebApp.Models
{
    public class DetalleAduana
    {
        public int detalleAduanaId { get; set; }

        public HeaderAduana headerAduana { get; set; }

        [DisplayName("Id Header")]
        public int headerAduanaId { get; set; }

        [DisplayName("D. Aduana")]
        public string aduana { get; set; }

        [DisplayName("Agente Aduana")]
        public string agenteAduana { get; set; }

        [DisplayName("Serial")]
        public string serial { get; set; }

        [DisplayName("Número DM")]
        public string numeroDM { get; set; }

        [DisplayName("NIT")]
        public string empresa { get; set; }

        [DisplayName("Nombre")]
        public string nombreEmpresa { get; set; }

        [DisplayName("Consignatario")]
        public string consignatario { get; set; }

        [DisplayName("Exportador")]
        public string exportador { get; set; }

        [DisplayName("Fecha Registro")]
        public DateTime fechaRegistro { get; set; }

        [DisplayName("Fecha Liquidación")]
        public DateTime fechaLiquidacion { get; set; }

        [DisplayName("Regimen")]
        public string regimen { get; set; }

        [DisplayName("Total Items")]
        public int totalItems { get; set; }

        [DisplayName("Preferencia")]
        public string preferencia { get; set; }

        [DisplayName("Cuota")]
        public string cuota { get; set; }

        [DisplayName("Licencia")]
        public string licencia { get; set; }

        [DisplayName("Subpartida")]
        public string subpartida { get; set; }

        [DisplayName("Desc. Mercancia")]
        public string descMercancia { get; set; }

        [DisplayName("País Destino")]
        public string paisDestino { get; set; }

        [DisplayName("País Procedencia")]
        public string paisProcedencia { get; set; }

        [DisplayName("País Origen")]
        public string paisOrigen { get; set; }

        [DisplayName("Cuantía")]
        public Double cuantia { get; set; }

        [DisplayName("Peso Neto")]
        public Double pesoNeto { get; set; }

        [DisplayName("Peso Bruto")]
        public Double pesoBruto { get; set; }

        [DisplayName("FOB Partida")]
        public Double fobPartida { get; set; }

        [DisplayName("CIF Partida")]
        public Double cifPartida { get; set; }

        [DisplayName("% DAI")]
        public Double porcentajeDAI { get; set; }

        [DisplayName("Valor DAI")]
        public Double dai { get; set; }

        [DisplayName("% IVA")]
        public Double porcentajeIVA { get; set; }

        [DisplayName("Monto IVA")]
        public Double iva { get; set; }
    }
}
