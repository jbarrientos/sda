using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SDA.WebApp.ViewModels
{
    public class IndexFraccionModel
    {

        public string Id { get; set; }

        [DisplayName("ID Categoría")]
        public int categoriaId { get; set; }

        [DisplayName("ID Fracción")]
        public int fraccionId { get; set; }

        [DisplayName("Arancel")]
        public decimal arancel { get; set; }
        [DisplayName("Categoría")]
        public string NombreCategoria
        {
            get
            {
                return Categoria.nombre;
            }
        }

        public Fraccion Fraccion { get; set; }
        public Categoria Categoria { get; set; }

        public Tratado Tratado { get; set; }

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