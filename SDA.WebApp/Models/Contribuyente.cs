using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.WebApp.Models
{
    public class Contribuyente
    {
        public int contribuyenteId { get; set; }

        [DisplayName("NIT")]
        [Required(ErrorMessage="Debe ingresar número de NIT")]
        public string nit { get; set; }

        [DisplayName("DUI")]
        //[Required(ErrorMessage = "Debe ingresar número de DUI")]
        public string dui { get; set; }


        [DisplayName("Nombre o razón social")]
        [Required(ErrorMessage = "Debe ingresar nombre o razón social de la empresa")]
        public string nombre { get; set; }
        [DisplayName("Dirección")]
        public string direccion { get; set; }

        [DisplayName("Tipo de persona")]
        public string tipoPersona { get; set; }
        

        [DisplayName("Email")]
        public string email { get; set; }

        [DisplayName("Telefono Fijo")]
        public string telefonoFijo { get; set; }

        [DisplayName("Telefono Celular")]
        public string telefonoCelular { get; set; }

        public bool TieneUsuario { get; set; }

        public int? ActividadEconomicaId { get; set; }

        public ActividadEconomica ActividadEconomica { get; set; }

        public string EmailAlternativo { get; set; }
        [DisplayName("Nombre Notificación")]
        public string NombreNotificacion { get; set; }
        [DisplayName("Cargo Notificación")]
        public string CargoNotificacion { get; set; }

        [Display(Name = "Representante Legal")]
        public string RepresentanteLegal { get; set; }
        [Display(Name = "Cargo")]
        public string CargoRepresentanteLegal { get; set; }

        [Display(Name = "NIT Representante Legal")]
        public string NitRepresentanteLegal { get; set; }

        [Display(Name = "DUI Representante Legal")]
        public string DUIRepresentanteLegal { get; set; }



    }
}
