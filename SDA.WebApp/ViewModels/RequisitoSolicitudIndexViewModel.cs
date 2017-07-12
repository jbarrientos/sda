using System.Collections.Generic;
using SDA.WebApp.Models;

namespace SDA.WebApp.ViewModels
{
    public class RequisitoSolicitudIndexViewModel
    {
        public List<RequisitoSolicitud> Requisitos { get; set; }

        public List<Requisito> Pendientes { get; set; }
        public Solicitud Solicitud { get; set; }
    }
}