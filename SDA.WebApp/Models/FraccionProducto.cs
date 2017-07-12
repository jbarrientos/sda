using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.WebApp.Models
{
    public class FraccionProducto
    {
        [DisplayName("ID")]
        public int fraccionProductoId { get; set; }

        [DisplayName("Fraccion ID")]
        public int fraccionId { get; set; }

        [DisplayName("Producto ID")]
        public int productoId { get; set; }

        public Fraccion Fraccion { get; set; }

        public Producto Producto { get; set; }
    }
}
