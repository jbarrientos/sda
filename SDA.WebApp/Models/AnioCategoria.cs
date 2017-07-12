using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.WebApp.Models
{
    public class AnioCategoria
    {
        public int anioCategoriaId { get; set; }

        //public Categoria categoria { get; set; }

        [DisplayName("Formúla")]
        public string formula { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Fecha")]
        public DateTime fecha { get; set; }

        [DisplayName("Arancel")]
        public Double arancel { get; set; }

        [DisplayName("Categoria")]
        public int categoriaId { get; set; }

        public Categoria Categoria { get; set; }
    }
}
