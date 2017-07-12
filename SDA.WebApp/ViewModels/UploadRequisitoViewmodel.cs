using SDA.WebApp.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SDA.WebApp.ViewModels
{
    public class UploadRequisitoViewmodel
    {
        public int Tipo { get; set; }

        public Solicitud Solicitud { get; set; }

        [Display(Name = "Requisito a subir")]
        public int RequisitoId { get; set; }

        [Display(Name = "Requisito")]
        public List<Requisito> Requisitos { get; set; }

        public string Comentarios { get; set; }
    }
}