using System.ComponentModel.DataAnnotations;

namespace SDA.WebApp.ViewModels
{
    public class NotificacionFormViewModel
    {
        public string Email { get; set; }
        public string Nombre { get; set; }
        [Display(Name = "Contingentes?")]
        public string Tipo { get; set; }
    }
}