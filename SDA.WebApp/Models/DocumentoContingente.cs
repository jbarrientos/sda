using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDA.WebApp.Models
{
    public class DocumentoContingente
    {

        public int Id { get; set; }

        public int? ContingenteVarioId { get; set; }

        public int? DetalleContingenteId { get; set; }

        public ContingenteVario ContingenteVario { get; set; }

        public DetalleContingente DetalleContingente { get; set; }


        public string Comentarios { get; set; }

        public DateTime Fecha { get; set; }

        public byte[] Documento { get; set; }

        public string MimeType { get; set; }

        public int? DocumentSize { get; set; }

        public string DocumentName { get; set; }
    }
}