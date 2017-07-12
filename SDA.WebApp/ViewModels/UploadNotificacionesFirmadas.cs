using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDA.WebApp.ViewModels
{
    public class UploadNotificacionesFirmadas
    {
        [Display(Name = "Nombre de Archivo")]
        public string NombreArchivo { get; set; }

        public DetalleContingente DetalleContingente { get; set; }
    }
}