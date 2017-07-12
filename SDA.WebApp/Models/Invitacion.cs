using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.WebApp.Models
{
    public class Invitacion
    {
        public int invitacionId
        {
            get; set;
        }

        public Contribuyente contribuyente { get; set; }


        public FraccionImportar fraccionImportar { get; set; }

        public bool aceptada { get; set; }
        public int contribuyenteId { get; set; }
        public int fraccionImportarId { get; set; }
    }


}
