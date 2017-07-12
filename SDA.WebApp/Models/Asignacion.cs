using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDA.WebApp.Models
{
    public class Asignacion
    {

        public int Id { get; set; }

        public int DetalleContingenteId { get; set; }

        public DetalleContingente DetalleContingente { get; set; }

        public DateTime FechaEjecucion { get; set; }

        public string UserId { get; set; }

        public bool Cerrada { get; set; }


    }
}