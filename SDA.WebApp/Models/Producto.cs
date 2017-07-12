using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.WebApp.Models
{
    public class Producto
    {
        [DisplayName("ID")]
        public int productoId { get; set; }

        [DisplayName("Nombre")]
        public int nombre { get; set; }
    }
}
