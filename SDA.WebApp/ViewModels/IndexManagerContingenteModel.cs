using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDA.WebApp.ViewModels
{
    public class IndexManagerContingenteModel
    {
        public string Id { get; set; }

        [DisplayName("Tratado")]
        public Tratado Tratado { get; set; }

        [DisplayName("Tipo Contingente")]
        public TipoContingente TipoContingente { get; set; }

        [DisplayName("Periodo")]
        public int Periodo { get; set; }

        [DisplayName("Total")]
        public Double Monto { get; set; }

        [DisplayName("Nuevos")]
        public Double MontoNuevos { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Inicio Solic.")]
        public DateTime FechaInicioSolicitudes { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Final Solic.")]
        public DateTime FechaFinalSolicitudes { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Inicio Reasig.")]
        public DateTime FechaInicioReasignacion { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Final Reasig.")]
        public DateTime FechaFinalReasignacion { get; set; }
    }
}