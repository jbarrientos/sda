using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SDA.WebApp.ViewModels
{
    public class IndexContingenteModel
    {

        public string Id { get; set; }
        [DisplayName("Tipo Contingente")]
        public TipoContingente TipoContingente { get; set; }

        [DisplayName("Nomenclatura")]
        public TipoNomenclatura TipoNomenclatura { get; set; }

        [DisplayName("Unidad Medida")]
        public UnidadMedida UnidadMedida { get; set; }

        [DisplayName("Volumen Inicial")]
        public Double Volumen { get; set; }

        [DisplayName("% Volumen Históricos")]
        public Double VolumenHistoricosPercent { get; set; }

        [DisplayName("% Volumen Nuevos")]
        public Double VolumenNuevosPercent { get; set; }
    }
}