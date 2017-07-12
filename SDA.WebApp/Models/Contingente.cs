using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.WebApp.Models
{
    public class Contingente
    {
        public int contingenteId { get; set; }
        //public AnioCategoria anioCategoria { get; set; }
        //public UnidadMedida unidadMedida { get; set; }
        //////[DisplayName("Fecha inicio")]
        //////public DateTime fechaInicio { get; set; }
        //////[DisplayName("Fecha final")]
        //////public DateTime fechaFin { get; set; }
        //////[DisplayName("Fecha de reasignación")]
        //////public DateTime fechaReasignacion { get; set; }
        //////[DisplayName("Inicio recepción de solicitudes")]
        //////public DateTime fechaInicioSolicitudes { get; set; }
        //////[DisplayName("Fin recepción de solicitudes")]
        //////public DateTime fechaFinSolicitudes { get; set; }
        //////// Fechas solicitudes reasignacion
        //////[DisplayName("Inicio recepción de solicitudes (Reasig.)")]
        //////public DateTime fechaInicioSolicitudesRe { get; set; }
        //////[DisplayName("Fin recepción de solicitudes (Reasig.)")]
        //////public DateTime fechaFinSolicitudesRe { get; set; }
        //public string tipoContingente { get; set; }
        //[DisplayName("Tipo decontingente")]
        //public string indTipoContingente { get; set; }
        //public int? anioCategoriaId { get; set; }
        //[DisplayName("Unidad de medida")]
        //public int unidadMedidaId { get; set; }

        [DisplayName("Categoria")]
        public int? categoriaId { get; set; }

        [DisplayName("Volumen")]
        public Double volumen { get; set; }

        [DisplayName("Aumento Volumen Anual")]
        public Double aumentoVolumen { get; set; }

        [DisplayName("Descripción")]
        public string nombre { get; set; }

        [DisplayName("Tratado")]
        public int tratadoId { get; set; }

        [DisplayName("Años para considerarse importador histórico?")]
        public int aniosAnteriores { get; set; }

        [DisplayName("% de Importación para importadores históricos?")]
        public Decimal porcentajeVolumenHistorico { get; set; }

        [DisplayName("% Minimo de importación para históricos?")]
        public Decimal porcentajeMinimoImportacion { get; set; }

        //[DisplayName("Nomenclatura")]
        //public int tipoNomenclaturaId { get; set; }

        [DisplayName("Tipo contingente")]
        public int tipoContingenteId { get; set; }

        [DisplayName("Cuota Única")]
        public Boolean cuotaUnica { get; set; }



        //public AnioCategoria AnioCategoria { get; set; }
        //public UnidadMedida UnidadMedida { get; set; }
        //public TipoNomenclatura TipoNomenclatura { get; set; }
        public TipoContingente TipoContingente { get; set; }

        public ICollection<DetalleContingente> detallesContingente { get; set; }

        public string TemplateLicencia { get; set; }

        public string TemplateNotificacion { get; set; }

        public Tratado Tratado { get; set; }


    }
}
