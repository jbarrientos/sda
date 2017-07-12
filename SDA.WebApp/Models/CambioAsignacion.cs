using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDA.WebApp.Models
{
    public class CambioAsignacion
    {

        public int Id { get; set; }

        public int DetalleAsignacionId { get; set; }

        public DetalleAsignacion DetalleAsignacion { get; set; }

        public double ValorOriginal { get; set; }

        public double ValorActualizado { get; set; }

        public DateTime FechaCambio { get; set; }

        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public string Comentarios { get; set; }

    }
}