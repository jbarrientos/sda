using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDA.WebApp.ViewModels
{
    public class GenerarContingentesViewModel
    {

        public string Id { get; set; }

        [DisplayName("Tratado ID")]
        public int TratadoId { get; set; }

        [DisplayName("Tipo de Contingente")]
        public int TipoContingenteId { get; set; }

        [DisplayName("Años para históricos")]
        public int AniosParaHistoricos { get; set; }

        [DisplayName("Volumen Total")]
        public Double volumen { get; set; }

        [DisplayName("Aumento Volumen Anual")]
        public Double PorcentajeAumentoVolumen { get; set; }

        [DisplayName("% Volumen para históricos")]
        public Double PorcentajeVolumenHistoricos { get; set; }

        [DisplayName("% Minimo para importación")]
        public Double PorcentajeMinimoImportacion { get; set; }

        [DisplayName("Cuota Única?")]
        public Boolean CuotaUnica { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Fecha inicio")]
        public DateTime FechaInicio { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Fecha final")]
        public DateTime FechaFinal { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Fecha inicio Solicitudes")]
        public DateTime FechaInicioSolicitudes { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Fecha final Solicitudes")]
        public DateTime FechaFinalSolicitudes { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Fecha inicio Solicitudes (Reasignación)")]
        public DateTime FechaInicioSolicitudesReasignacion { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Fecha final Solicitudes (Reasignación)")]
        public DateTime FechaFinalSolicitudesReasignacion { get; set; }

        public Tratado Tratado { get; set; }
        public TipoContingente TipoContingente { get; set; }
    }
}