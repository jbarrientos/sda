using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDA.WebApp.Models
{
    public class NotificacionInterna
    {

        public int Id { get; set; }

        public string Nombre { get; set; }

        public string Email { get; set; }

        [StringLength(1)]
        public string Tipo { get; set; }
    }
}