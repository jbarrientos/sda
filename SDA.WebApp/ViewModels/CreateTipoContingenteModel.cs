using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SDA.WebApp.ViewModels
{
    public class CreateTipoContingenteModel
    {
        public string Id { get; set; }

        [DisplayName("Nombre")]
        public string Nombre { get; set; }

        [DisplayName("Tipo Nomenclatura")]
        public int TipoNomenclaturaId { get; set; }

        [DisplayName("Unidad de Medida")]
        public int UnidadDeMedidaId { get; set; }

        [DisplayName("Crecimiento")]
        public Double Crecimiento { get; set; }
    }
}