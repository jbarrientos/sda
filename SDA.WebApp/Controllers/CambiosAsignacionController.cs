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
    public class CambiosAsignacionController : Controller
    {

        public ApplicationDbContext _context;


        public CambiosAsignacionController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: CambiosAsignacion
        public ActionResult Index(int id)
        {

            var detalleAsignacion = _context
                .DetallesAsignaciones
                .Include(d => d.Asignacion.DetalleContingente.Contingente.TipoContingente)
                .Include(d => d.Solicitud.contribuyente)
                .SingleOrDefault(d => d.Id == id);

            if (detalleAsignacion == null)
                return HttpNotFound();

            var vm = new CambiosAsignacionIndexViewModel
            {
                DetalleAsignacion = detalleAsignacion,
                CambiosAsignacion = _context.CambiosAsignacion
                .Include(c => c.ApplicationUser)
                .Where(c => c.DetalleAsignacionId == id)
                .ToList()
            };

            return View(vm);
        }

        public ActionResult Create(int id)
        {

            var detalleAsignacion = _context
                 .DetallesAsignaciones
                 .Include(d => d.Asignacion.DetalleContingente.Contingente.TipoContingente.UnidadMedida)
                 .Include(d => d.Solicitud.contribuyente)
                 .SingleOrDefault(d => d.Id == id);

            if (detalleAsignacion == null)
                return HttpNotFound();

            var vm = new CambiosAsignacionFormViewModel
            {
                DetalleAsignacion = detalleAsignacion,
                CambioAsignacion = new CambioAsignacion
                {
                    Id = 0,
                    DetalleAsignacionId = id
                }

            };

            return View("CambioAsignacionForm", vm);
        }

        public ActionResult Save(CambiosAsignacionFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var vm = new CambiosAsignacionFormViewModel
                {
                    DetalleAsignacion = _context
                    .DetallesAsignaciones
                    .SingleOrDefault(d => d.Id == model.CambioAsignacion.DetalleAsignacionId),
                    CambioAsignacion = new CambioAsignacion
                    {
                        Id = 0,
                        DetalleAsignacionId = model.CambioAsignacion.DetalleAsignacionId
                    }

                };

                return View("CambioAsignacionForm", vm);
            }
            var det = _context
                .DetallesAsignaciones
                .SingleOrDefault(d => d.Id == model.CambioAsignacion.DetalleAsignacionId);

            if (det == null)
                return HttpNotFound();

            if(model.CambioAsignacion.Id == 0)
            {
                // New
                var user = _context.Users
                .SingleOrDefault(u => u.UserName == User.Identity.Name);

                if (user == null)
                {
                    return HttpNotFound();
                }
                //
                _context.CambiosAsignacion.Add(new CambioAsignacion
                {
                    Comentarios = model.CambioAsignacion.Comentarios,
                    DetalleAsignacionId = model.CambioAsignacion.DetalleAsignacionId,
                    FechaCambio = DateTime.Now,
                    ValorActualizado = model.CambioAsignacion.ValorActualizado,
                    ValorOriginal = det.Asignado,
                    ApplicationUserId = user.Id
                });
                det.Asignado = model.CambioAsignacion.ValorActualizado;
            }
            _context.SaveChanges();
            return RedirectToAction("Index","CambiosAsignacion",
                new { id = model.CambioAsignacion.DetalleAsignacionId });
        }
    }
}