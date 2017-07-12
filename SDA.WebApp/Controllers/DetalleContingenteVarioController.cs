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
    public class DetalleContingenteVarioController : Controller
    {

        ApplicationDbContext _context;

        public DetalleContingenteVarioController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: DetalleContingenteVario
        public ActionResult Index(int id)
        {

            var contingente = _context.ContingentesVarios
                .Include(c => c.TipoContingente)
                .SingleOrDefault(c => c.Id == id);

            if (contingente == null)
                return HttpNotFound();

            var detalle = _context.DetallesContingenteVario
                .Include(d => d.Contribuyente)
                .Where(d => d.ContingenteVarioId == id)
                .ToList();

            double? totalAsignado =
            _context.DetallesContingenteVario
                .Where(d => d.ContingenteVarioId == id)
                .Sum(d => (double?)d.Recepcion) ?? 0.00;
            double? totalPorcentajeAsignado =
            _context.DetallesContingenteVario
                .Where(d => d.ContingenteVarioId == id)
                .Sum(d => (double?)d.PorcentajeRecepcion) ?? 0.00;

            var vm = new DetalleContingenteVarioIndexViewModel
            {
                Contingente = contingente,
                Detalles = detalle,
                TotalAsignado = (double)totalAsignado,
                Saldo = contingente.TotalRecepcion - (double)totalAsignado,
                PorcentajeAsignado = (double)totalPorcentajeAsignado
            };
            return View(vm);
        }

        public ActionResult Add(int id)
        {

            var contingente = _context.ContingentesVarios
                .Include(c => c.TipoContingente)
                .SingleOrDefault(c => c.Id == id);

            if (contingente == null)
                return HttpNotFound();

            var vm = new DetalleContingenteVarioFormViewModel
            {
                Contingente = contingente,
                Detalle = new DetalleContingenteVario
                {
                    ContingenteVarioId = id,
                    Id = 0                    
                }
            };

            return View("FormDetalleContingenteVario", vm);
        }
    }
}