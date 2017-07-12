using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SDA.WebApp.ViewModels
{
    public class PreregistroFormViewModel
    {
        [Display(Name = "NIT")]
        [Required(ErrorMessage = "El Número de NIT es requerido")]
        [Remote("ValidarNIT", "Preregistro")]
        public string Nit { get; set; }
        [Display(Name = "DUI")]
        public string Dui { get; set; }
        [Display(Name = "Correo Electrónico")]
        [Required(ErrorMessage = "Correo Electrónico es requerido")]
        [EmailAddress(ErrorMessage = "Cuenta de correo no válida")]
        public string Email { get; set; }
        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "Teléfono es requerido")]
        public string Telefono { get; set; }

        public string Celular { get; set; }

        public string Comentarios { get; set; }
        [Display(Name = "Nombre o Razón Social")]
        [Required(ErrorMessage = "Nombre o Razón Social es requerido")]
        public string Nombre { get; set; }

        [Display(Name = "Persona Natural o Jurídica")]
        [Required(ErrorMessage = "Persona Natural o Jurídica es requerido")]
        public string TipoPersona { get; set; }
    }
}