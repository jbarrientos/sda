using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.WebApp.Models
{
    public class PaisTratado
    {
        public int paisTratadoId { get; set; }
        [DisplayName("Pais")]
        public Pais pais { get; set; }
        [DisplayName("Tratado")]
        public Tratado tratado { get; set; }

        public int paisId { get; set; }
        public int tratadoId { get; set; }
    }
}
