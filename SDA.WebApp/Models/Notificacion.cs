using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDA.WebApp.Models
{
    public class Notificacion
    {

        public int Id { get; set; }

        public int SolicitudId { get; set; }

        public Solicitud Solicitud { get; set; }

        public DateTime Fecha { get; set; }

        public string Nota { get; set; }

        public bool Visto { get; set; }
    }
}