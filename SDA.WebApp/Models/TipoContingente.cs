using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.WebApp.Models
{
    public class TipoContingente
    {
        public int tipoContingenteId { get; set; }

        [DisplayName("Nombre")]
        [Required(ErrorMessage = "Debe digitar el nombre del tipo de contingente.")]
        public string nombre { get; set; }


        [DisplayName("Tipo nomenclatura")]
        [Required(ErrorMessage = "Debe ingresar el tipo de nomneclatura a utilizar.")]
        public int tipoNomenclaturaId { get; set; }

        public TipoNomenclatura TipoNomenclatura { get; set; }

        [DisplayName("Unidad de medida")]
        public int unidadMedidaId { get; set; }

        [DisplayName("Crecimiento anual")]
        public Double crecimiento { get; set; }

        [DisplayName("Mecanismo de Subasta?")]
        public Boolean mecanismoSubasta { get; set; }

        public UnidadMedida UnidadMedida { get; set; }

        [Display(Name = "Especificar fracción")]
        public bool EspecificarFraccion { get; set; }

        [Display(Name = "Distribuir por fracción")]
        public bool DistribuirPorFraccion { get; set; }

        [Display(Name = "Separar históricos")]
        public bool SepararHistoricos { get; set; }
        [Display(Name = "% Máximo de Distribución")]
        public double PorcentajeMaximo { get; set; }
        [Display(Name = "No. de Meses para Vto. de Licencias")]
        public int? MesesVencimientoLicencia { get; set; }

        public string TemplateActa { get; set; }

        public string NombreImagen { get; set; }

    }
}
