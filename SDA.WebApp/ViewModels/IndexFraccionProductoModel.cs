using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SDA.WebApp.ViewModels
{
    public class IndexFraccionProductoModel
    {

        public string Id { get; set; }

        [DisplayName("ID Fracción")]
        public int FraccionId { get; set; }

        public int ProductoId { get; set; }

        public Fraccion Fraccion { get; set; }

        [DisplayName("Fracción")]
        public string NombreFraccion
        {
            get
            {
                return Fraccion.nombre;
            }
        }

        [DisplayName("Código")]
        public string CodigoSAC
        {
            get
            {
                return Fraccion.codigo;
            }
        }
    }
}