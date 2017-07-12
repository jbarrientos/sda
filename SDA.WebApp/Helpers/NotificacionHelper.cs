using SDA.Services.Helpers;
using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDA.WebApp.Helpers
{
    public class NotificacionHelper
    {

        ApplicationDbContext _context;

        public NotificacionHelper()
        {
            _context = new ApplicationDbContext();
        }


        public void EnviarNotificaciones(string tipo, string subject, string body)
        {

            var notificaciones = _context.NotificacionesInternas.Where(n => n.Tipo == tipo).ToList();
            // Email
            Email email = new Email();
            
            foreach (var notificacion in notificaciones)
            {
                email.To.Add(notificacion.Email);
            };
            email.From = Constant.DEFAULT_EMAIL_ACCOUNT;
            email.Subject = subject;
            email.Body = body;

            email.Send();
        }

    }
}