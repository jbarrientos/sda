using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.WebApp.Models
{
    public class FraccionImportar
    {
        public int fraccionImportarId { get; set; }

        [DisplayName("Importación extraordinaria")]
        public ImportacionExtraordinaria importacionExtraordinaria { get; set; }

        [DisplayName("Fracción a importar")]
        public Fraccion fraccion { get; set; }

        [DisplayName("Volumen a importar")]
        public decimal volumen { get; set; }
    }
}
