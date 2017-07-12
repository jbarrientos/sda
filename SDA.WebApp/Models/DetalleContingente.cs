using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.WebApp.Models
{
    public class DetalleContingente
    {
        public int detalleContingenteId { get; set; }
        [DisplayName("Año")]
        public int anio { get; set; }
        [DisplayName("Monto")]
        public Double monto { get; set; }
        [DisplayName("Monto Nuevo Imp.")]
        public double montoNuevo { get; set; }

        [DisplayName("Vol.Nuevo Imp.")]
        public Double volumenNuevo { get; set; }

        [DisplayName("Contingente ID")]
        public int contingenteId { get; set; }

        //
        [DisplayName("Fecha inicio")]
        public DateTime fechaInicio { get; set; }
        [DisplayName("Fecha final")]
        public DateTime fechaFin { get; set; }
        [DisplayName("Fecha de reasignación")]
        public DateTime fechaReasignacion { get; set; }
        [DisplayName("Inicio rec. solic.")]
        public DateTime fechaInicioSolicitudes { get; set; }
        [DisplayName("Fin rec. solic.")]
        public DateTime fechaFinSolicitudes { get; set; }
        // Fechas solicitudes reasignacion
        [DisplayName("Inicio recepción de solicitudes (Reasig.)")]
        public DateTime fechaInicioSolicitudesRe { get; set; }
        [DisplayName("Fin recepción de solicitudes (Reasig.)")]
        public DateTime fechaFinSolicitudesRe { get; set; }
        //public int fraccionTratadoId { get; set; }
        //public FraccionTratado FraccionTratado { get; set; }
        //public DetalleContingente DetalleContingente { get; set; }

        [DisplayName("Disponible Redist.")]
        public Double? disponibleRedistribucion { get; set; }

        [DisplayName("Disponible Redist. Hist.")]
        public Double? disponibleRedistHistoricos { get; set; }

        public Contingente Contingente { get; set; }

        public double MontoHistoricos {
            get
            {
                return this.monto - this.montoNuevo;
            }
        }

        public bool NotificacionesGeneradas { get; set; }

        public DateTime? FechaGeneracionNotificaciones { get; set; }

        public bool NotificacionesCargadas { get; set; }

        public DateTime? FechaCargaNotificaciones { get; set; }

        public bool NotificacionesEnviadas { get; set; }
        public DateTime? FechaEnvioNotificaciones { get; set; }


    }
}
