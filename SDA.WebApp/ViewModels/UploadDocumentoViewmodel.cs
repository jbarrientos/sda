using SDA.WebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace SDA.WebApp.ViewModels
{
    public class UploadDocumentoViewmodel
    {
        public string Comentarios { get; set; }
        [Display(Name = "Documento a subir")]
        public ContingenteVario Contingente { get; set; }
    }
}