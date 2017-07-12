using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.WebApp.Models
{
    public class Categoria
    {
        public int categoriaId { get; set; }

        [DisplayName("Categoría")]
        [Required(ErrorMessage = "Debe digitar el nombre de la categoria.")]
        public string nombre { get; set; }

        [DisplayName("Tratado")]
        public Tratado tratado { get; set; }

        [DisplayName("Tratado")]
        public int tratadoId { get; set; }

        [DisplayName("Descripción")]
        public string descripcion { get; set; }

        [DisplayName("Arancel")]
        [Required(ErrorMessage = "Debe digitar el arancel base de la categoria.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N1}")]
        public Decimal arancelBase { get; set; }

        [DisplayName("Años de gracia")]
        public Int16 aniosGracia { get; set; }

        [DisplayName("Número de cortes")]
        public Int16 aniosDesgravacion { get; set; }

        [DisplayName("Código")]
        public string codigo { get; set; }

        public ICollection<AnioCategoria> aniosCategoria { get; set; }
        public ICollection<FraccionTratado> fraccionesTratado { get; set; }
    }
}
