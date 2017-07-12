using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SDA.WebApp.ViewModels
{
    public class LoginContribuyenteViewModel
    {
        public int contribuyenteId { get; set; }

        
        public string nit { get; set; }

       
        [DisplayName("Nombre o razón social")]
         public string nombre { get; set; }
              

        [DisplayName("Email")]
        public string email { get; set; }

        [DisplayName("Telefono Fijo")]
        public string telefonoFijo { get; set; }

        [DisplayName("Telefono Celular")]
        public string telefonoCelular { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El {0} bebe contener al menos {2} caracteres de longitud.",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar password")]
        [Compare("Password", ErrorMessage = "El password y el password de confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }
    }
}