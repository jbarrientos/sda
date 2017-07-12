using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDA.WebApp.ViewModels
{
    public class CreateContingenteModel
    {

        public string Id { get; set; }
        public int TratadoId { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Fecha Inicio")]
        public DateTime FechaInicio { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Fecha Final")]
        public DateTime FechaFinal { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Fecha Reasignación")]
        public DateTime FechaReasignacion { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Fecha Inicio Solicitudes")]
        public DateTime FechaInicioSolicitudes { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Fecha Fin Solicitudes")]
        public DateTime FechaFinSolicitudes { get; set; }

        [DisplayName("Unidad de medida")]
        public int UnidadMedidaId { get; set; }
        [DisplayName("Volumen")]
        public Double Volumen { get; set; }
        [DisplayName("Unidad medida")]
        public UnidadMedida UnidadMedida { get; set; }
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }
        [DisplayName("Tipo de nomenclatura")]
        public int TipoNomenclaturaId { get; set; }

        public IEnumerable<string> FraccionesSeleccionadas { get; set; }
        public IEnumerable<SelectListItem> Fracciones { get; set; }
    }
}