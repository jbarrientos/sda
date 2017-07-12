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
    public class DetalleAsignacionesController : Controller
    {

        ApplicationDbContext _context;

        public DetalleAsignacionesController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: DetalleAsignaciones
        public ActionResult Index(int id)
        {
            var asignacion = _context.Asignaciones
                .Include(a => a.DetalleContingente)
                .Include(a => a.DetalleContingente.Contingente)
                .Include(a => a.DetalleContingente.Contingente.TipoContingente)
                .SingleOrDefault(a => a.Id == id);

            if (asignacion == null)
                return HttpNotFound();

            var detalle = _context
                .DetallesAsignaciones
                .Include(d => d.Solicitud)
                .Include(d => d.Solicitud.contribuyente)
                .Where(d => d.AsignacionId == id).ToList();

            var viewModel = new DetalleAsignacionIndeViewModel
            {
                Asignacion = asignacion,
                Detalles = detalle
            };

            return View(viewModel);
        }

        
    }
}