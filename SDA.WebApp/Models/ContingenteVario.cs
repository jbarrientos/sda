using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDA.WebApp.Models
{
    public class ContingenteVario
    {

        public int Id { get; set; }
        [Display(Name = "Tipo de Contingente")]
        public int TipoContingenteId { get; set; }

        public TipoContingente TipoContingente { get; set; }
        [Display(Name = "Tratado")]
        public int? TratadoId { get; set; }

        public Tratado Tratado { get; set; }
        [Display(Name = "Año")]
        public int Anio { get; set; }
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
        [Display(Name = "Fecha de inicio")]
        public DateTime FechaInicio { get; set; }
        [Display(Name = "Fecha final")]
        public DateTime FechaFinal { get; set; }
        [Display(Name = "Total Contingente")]
        public double TotalRecepcion { get; set; }
        [Display(Name = "Solicitudes Generadas?")]
        public bool SolicitudesGeneradas { get; set; }
        [Display(Name = "Fecha de generación solicitudes")]
        public DateTime? FechaGeneracionSolicitudes { get; set; }
        [Display(Name = "Calcular porcentaje?")]
        public bool  CalcularPorcentaje { get; set; }

        [Display(Name = "Generar Licencias")]
        public bool GenerarLicencias { get; set; }

        [Display(Name = "% Primera Fase")]
        public double? PorcentajePrimeraFase { get; set; }

        [Display(Name = "Fecha Licencias Segunda Fase")]
        public DateTime? FechaLicenciaSegundaFase { get; set; }

        public string TemplateLicencia { get; set; }


    }
}