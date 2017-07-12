using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDA.WebApp.Models
{
    public class DetalleLicencia
    {

        public int Id { get; set; }
        [Display(Name = "Licencia")]
        public int LicenciaId { get; set; }
        
        public Licencia Licencia { get; set; }
        [Display(Name = "Fracción")]
        public int FraccionId { get; set; }

        public Fraccion Fraccion { get; set; }
        [Display(Name = "Monto")]
        public double Volumen { get; set; }

        public double Importado { get; set; }
    }
}