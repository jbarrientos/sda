using System;
using System.Collections.Generic;

namespace SDA.WebApp.ViewModels
{
    public class HeaderAduanaIndexViewModel
    {
        public List<HeaderAduanaIndex> Headers { get; set; }
    }

    public struct HeaderAduanaIndex
    {
        public int Id { get; set; }
        public DateTime FechaCarga { get; set; }

        public string NombreArchivo { get; set; }

        public string Status { get; set; }

        public int NumLineas { get; set; }
    }
}