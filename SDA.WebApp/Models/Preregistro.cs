using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDA.WebApp.Models
{
    public class Preregistro
    {

        public int Id { get; set; }

        public string Nombre { get; set; }

        public string Nit { get; set; }
        [Display(Name = "DUI")]
        public string Dui { get; set; }
        [EmailAddress(ErrorMessage = "Cuenta de correo electrónico no válida")]
        public string Email { get; set; }

        public string Telefono { get; set; }

        public string Celular { get; set; }

        [Display(Name = "Fecha de envio")]
        public DateTime FechaEnvio { get; set; }

        [StringLength(1)]
        public string Status { get; set; }

        public string Comentarios { get; set; }
        [Display(Name = "Registro de IVA")]
        //[Required(ErrorMessage = "Debe ingresar Registro de IVA")]
        public string RegistroIVA { get; set; }


        [Display(Name = "Actividad Económica")]
        public int? ActividadEconomicaId { get; set; }
        [Display(Name = "Correo electrónico alterno")]
        [EmailAddress(ErrorMessage = "Cuenta de correo electrónico no válida")]
        public string EmailAlterno { get; set; }
        [Display(Name = "Nombre de Contacto")]
        public string Contacto { get; set; }

        [Display(Name = "Tipo de persona")]
        [Required(ErrorMessage = "Debe ingresar la Dirección")]
        public string TipoPersona { get; set; }
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }


        [Display(Name = "Representante Legal")]
        public string RepresentanteLegal { get; set; }
        [Display(Name = "Cargo")]
        public string CargoRepresentanteLegal { get; set; }

        [Display(Name = "NIT Representante Legal")]
        public string NitRepresentanteLegal { get; set; }

        [Display(Name = "DUI Representante Legal")]
        public string DUIRepresentanteLegal { get; set; }

        public string Token { get; set; }



    }
}