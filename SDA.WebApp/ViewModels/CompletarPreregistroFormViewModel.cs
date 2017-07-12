using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDA.WebApp.ViewModels
{
    public class CompletarPreregistroFormViewModel
    {

        public Preregistro Preregistro { get; set; }

        public List<ActividadEconomica> ActividadesEconomicas { get; set; }
    }
}