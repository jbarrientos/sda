using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDA.WebApp.ViewModels
{
    public class IndexDetalleContingenteModel
    {
        public string Id { get; set; }

        [DisplayName("Tratado")]
        public Tratado Tratado { get; set; }

        [DisplayName("Contingente")]
        public Contingente Contingente { get; set; }

        [DisplayName("Tipo Contingente")]
        public TipoContingente TipoContingente { get; set; }

        [DisplayName("Periodo")]
        public int Periodo { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Inicio")]
        public DateTime FechaInicio { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Final")]
        public DateTime FechaFinal { get; set; }

        [DisplayName("Estado")]
        public string Estado { get; set; }

        public bool YaSolicito { get; set; }

    }
}