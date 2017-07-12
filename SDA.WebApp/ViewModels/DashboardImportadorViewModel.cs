using SDA.WebApp.Models;
using System.Collections.Generic;

namespace SDA.WebApp.ViewModels
{
    public class DashboardImportadorViewModel
    {
        public IEnumerable<IndexSolicitudes> Solicitudes  { get; set; }

        public Contribuyente Importador { get; set; }

        public IEnumerable<IndexDetalleContingenteModel> Contingentes { get; set; }
        public int Anio { get; internal set; }
    }
}