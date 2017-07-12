using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.WebApp.Models
{
    public class FraccionTratado
    {
        public int fraccionTratadoId { get; set; }
        
        [DisplayName("Arancel")]
        public Decimal arancel { get; set; }

        
        public Categoria categoria { get; set; }

        public int categoriaId { get; set; }

        [DisplayName("Producto excluido?")]
        public bool excluido { get; set; }

        [DisplayName("Tratado")]
        public int tratadoId { get; set; }

        [DisplayName("Fracción")]
        public int fraccionId { get; set; }

        public Fraccion Fraccion { get; set; }

        public Tratado Tratado { get; set; }

       
    }
}
