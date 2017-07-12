using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SDA.WebApp.ViewModels
{
    public class IndexTipoContingenteModel
    {
        public string Id { get; set; }

        [DisplayName("Nombre")]
        public string Nombre { get; set; }

        [DisplayName("Unidad medida")]
        public UnidadMedida UnidadMedida { get; set; }

        [DisplayName("Tipo Nomenclatura")]
        public TipoNomenclatura TipoNomenclatura { get; set; }
    }
}