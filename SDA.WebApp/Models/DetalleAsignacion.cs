using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDA.WebApp.Models
{
    public class DetalleAsignacion
    {

        public int Id { get; set; }

        public int SolicitudId { get; set; }

        public Solicitud Solicitud { get; set; }

        public int AsignacionId { get; set; }

        public Asignacion Asignacion { get; set; }

        public double Asignado { get; set; }

        public double Solicitado { get; set; }

        public string Comentarios { get; set; }

        public bool Finalizado { get; set; }
        public double PorcentajeAplicado { get; set; }
    }
}