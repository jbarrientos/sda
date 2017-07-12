using System.Collections.Generic;
using SDA.WebApp.Models;

namespace SDA.WebApp.ViewModels
{
    public class DocumentoContingenteVarioIndexViewModel
    {
        public ContingenteVario Contingente { get; set; }
        public List<DocumentoContingente> Documentos { get; set; }
    }
}