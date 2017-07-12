using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDA.WebApp.Models
{
    public class DetalleContingenteVario
    {

        public int Id { get; set; }

        public int ContingenteVarioId { get; set; }

        public ContingenteVario ContingenteVario { get; set; }

        public int ContribuyenteId { get; set; }

        public Contribuyente Contribuyente { get; set; }

        public double PorcentajeRecepcion { get; set; }

        public double Recepcion { get; set; }

        public double VolumenPrimeraFase {
            get
            {
                if (this.ContingenteVario != null)
                {
                    if (this.ContingenteVario.PorcentajePrimeraFase != null)
                    {
                        var porcentaje = (double)this.ContingenteVario.PorcentajePrimeraFase / 100;
                        return porcentaje * this.Recepcion;
                    }
                    else return 0.00;
                        
                }
                else return 0.00;
                
            }
        }
        public double VolumenSegundaFase
        {
            get
            {
                if (this.ContingenteVario != null)
                {
                    if (this.ContingenteVario.PorcentajePrimeraFase != null)
                    {
                        var porcentaje = 1 - (double)this.ContingenteVario.PorcentajePrimeraFase / 100;
                        return porcentaje * this.Recepcion;
                    }
                    else return 0.00;
                    
                }
                else return 0.00;
                    
            }
        }
    }
}