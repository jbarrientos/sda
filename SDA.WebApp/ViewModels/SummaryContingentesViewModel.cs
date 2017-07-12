using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDA.WebApp.ViewModels
{
    public class SummaryContingentesViewModel
    {
        [DisplayName("Id")]
        public string Id { get; set; }

        [DisplayName("Id Contingente")]
        public int DetalleContingenteId { get; set; }

        [DisplayName("Año")]
        public int Anio { get; set; }

        [DisplayName("Nombre")]
        public string Nombre { get; set; }

        [DisplayName("Unidad Medida")]
        public UnidadMedida UnidadMedida { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0.00}")]
        [DisplayName("Total")]
        public Double MontoTotal { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0.00}")]
        [DisplayName("Nuevos")]
        public Double MontoNuevos { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0.00}")]
        [DisplayName("Hist.")]
        public Double MontoHistoricos { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0.00}")]
        [DisplayName("Asignado")]
        public Double Asignado { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0.00}")]
        [DisplayName("Solicitado")]
        public Double Solicitado { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0.00}")]
        [DisplayName("Importado")]
        public Double Importado { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0.00}")]
        [DisplayName("Saldo")]
        public Double Redistribucion { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0.00}")]
        [DisplayName("Reasign.")]
        public Double Reasignacion { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0.00}")]
        [DisplayName("Disp. Hist.")]
        public Double DisponibleHist { get; set; }

        [DisplayFormat(DataFormatString = "{0:###,##0.00}")]
        [DisplayName("Disp. Nuevos")]
        public Double DisponibleNuevos { get; set; }
    }
}