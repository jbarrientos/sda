using SDA.WebApp.Models;
using SDA.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDA.WebApp.Controllers
{
    public class DetalleSolicitudController : Controller
    {

        ApplicationDbContext _context;
        public DetalleSolicitudController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Details(int id)
        {

            var solicitud = _context.Solicitudes
                .Include(s => s.contribuyente)
                .Include(s => s.DetalleContingente.Contingente.TipoContingente)
                .SingleOrDefault(s => s.solicitudId == id);

            if (solicitud == null)
                return HttpNotFound();

            var fracciones = _context.DetallesSolicitud
                .Include(d => d.Fraccion)
                .Include(d => d.Solicitud.DetalleContingente.Contingente.TipoContingente)
                .Where(d => d.SolicitudId == id)
                .ToList();

            var vm = new DetalleSolicitudDetailsViewModel
            {
                Fracciones = fracciones,
                Solicitud = solicitud,
                TotalSolicitado = _context.DetallesSolicitud
                .Where(d => d.SolicitudId == id)
                .Sum(d => d.Solicitado),
                TotalAsignado = _context.DetallesSolicitud
                .Where(d => d.SolicitudId == id)
                .Sum(d => d.Asignado)
            };

            return View(vm);
        }

        public ActionResult ConsultaPorFraccion(int id)
        {

            var detalle = _context.DetallesContingente
                .Include(d => d.Contingente.TipoContingente)
                .SingleOrDefault(d => d.detalleContingenteId == id);

            if (detalle == null)
                return HttpNotFound();

            var fracciones = _context.FraccionesTipoContingente
                .Include(f => f.Fraccion)
                .Where(f => f.tipoContingenteId == detalle.Contingente.tipoContingenteId)
                .ToList();

            
            var solicitudes = _context.Solicitudes
                .Include(s => s.contribuyente)
                .Where(s => s.detalleContingenteId == id)
                .ToList();
            List<Detalle> valores = new List<Detalle>();
            foreach(var sol in solicitudes)
            {
                var det = new Detalle
                {
                    Id = sol.solicitudId,
                    Nombre = sol.contribuyente.nombre,
                    Asignados = new List<double>()
                };
                double total = 0.00;
                foreach(var frac in fracciones)
                {
                    var detalleSolicitud = _context.DetallesSolicitud
                                .Include(d => d.Solicitud.contribuyente)
                                .Include(d => d.Solicitud.DetalleContingente)
                                .SingleOrDefault(d => d.SolicitudId == sol.solicitudId && 
                                d.FraccionId == frac.fraccionId);
                    if(detalleSolicitud != null)
                    {
                        total += detalleSolicitud.Asignado;
                        det.Asignados.Add(detalleSolicitud.Asignado);
                    }else
                    {
                        det.Asignados.Add(0.00);
                    }
                }
                det.Asignados.Add(total);
                valores.Add(det);
                

            }

            var vm = new DetalleSolicitudIndexViewModel
            {
                Fracciones = fracciones,
                Detalles = valores,
                Contingente = detalle
            };


            return View(vm);
        }
    }
}