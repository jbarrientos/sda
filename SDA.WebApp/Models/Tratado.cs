using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SDA.WebApp.Models
{
    public class Tratado
    {
        public int tratadoId { get; set; }

        [DisplayName("Nombre")]
        [Required(ErrorMessage="Debe digitar el nombre del tratado.")]
        [MaxLength(100, ErrorMessage = "El nombre del tratado no debe superar los 100 caracteres.")]
        public string nombre { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Fecha inicio")]
        [Required(ErrorMessage = "Debe ingresar fecha de inicio del tratado.")]
        public DateTime fechaInicio { get; set; }

        [DisplayName("Plazo máximo de desgravación (años)")]
        [Range(1,100,ErrorMessage = "El número de años del tratado debe ser mayor a 0.")]
        public Int16 numeroAnios { get; set; }

        public ICollection<Pais> paises { get; set; }
        public ICollection<Categoria> categorias { get; set; }
        public ICollection<FraccionTratado> fracciones { get; set; }
    }
}
