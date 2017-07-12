using SDA.WebApp.Models;
using System.ComponentModel;

namespace SDA.WebApp.ViewModels
{
    public class PrintReceiptViewModel
    {
        public string DUI { get; set; }
        public Licencia Licencia { get; set; }
        [DisplayName("Licencia de Conducir")]
        public string LicenciaConducir { get; set; }
        [DisplayName("Persona que recibe la licencia")]
        public string Responsable { get; set; }
    }
}