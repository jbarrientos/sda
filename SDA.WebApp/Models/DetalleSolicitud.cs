using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDA.WebApp.Models
{
    public class DetalleSolicitud
    {

        public int Id { get; set; }

        public int SolicitudId { get; set; }

        public Solicitud Solicitud { get; set; }

        public int FraccionId { get; set; }

        public Fraccion Fraccion { get; set; }

        public double Solicitado { get; set; }

        public double Asignado { get; set; }

        public double PorcentajeAplicado { get; set; }


    }
}