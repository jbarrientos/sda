using System.Collections.Generic;
using SDA.WebApp.Models;

namespace SDA.WebApp.ViewModels
{
    public class NotificacionIndexViewModel
    {
        public List<Notificacion> Notificaciones { get; set; }
        public Solicitud Solicitud { get; set; }
    }
}