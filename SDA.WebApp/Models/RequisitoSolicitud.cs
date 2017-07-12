using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDA.WebApp.Models
{
    public class RequisitoSolicitud
    {

        public int Id { get; set; }

        public int SolicitudId { get; set; }

        public Solicitud Solicitud { get; set; }


        [Display(Name = "Documento Requisito")]
        public int RequisitoId { get; set; }

        public string Comentarios { get; set; }

        public Requisito Requisito { get; set; }

        public DateTime Fecha { get; set; }

        public byte[] Documento { get; set; }

        public string MimeType { get; set; }

        public int? PictureSize { get; set; }

        public string PictureName { get; set; }
    }
}