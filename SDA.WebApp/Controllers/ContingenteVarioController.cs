using MvcFlashMessages;
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
    public class ContingenteVarioController : Controller
    {

        public ApplicationDbContext _context;

        public ContingenteVarioController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: ContingenteVario
        public ActionResult Index()
        {
            var contingentes = _context.ContingentesVarios
                .Include(c => c.TipoContingente)
                .Include(c => c.Tratado)
                .OrderByDescending(c => c.Anio)
                .ToList();
            
            return View(contingentes);
        }

        public ActionResult Create()
        {
            var vm = new ContingenteVarioFormViewModel
            {
                Contingente = new ContingenteVario
                {
                    Id = 0,
                    Anio = DateTime.Now.Year
                },
                TiposContingente = _context.TiposContingente
                    .ToList(),
                Tratados = _context.Tratados.ToList(),
                Titulo = "Nuevo Contingente"
            };

            return View("FormContingenteVario",vm);
        }

        public ActionResult Details(int id)
        {

            var contingente = _context.ContingentesVarios
                .Include(v => v.TipoContingente)
                .SingleOrDefault(c => c.Id == id);

            if (contingente == null)
                return HttpNotFound();

            
            return View(contingente);
        }

        public ActionResult Edit(int id)
        {
            var contingente = _context.ContingentesVarios
                .SingleOrDefault(v => v.Id == id);

            if (contingente == null)
                return HttpNotFound();

            var vm = new ContingenteVarioFormViewModel
            {
                Contingente = contingente,
                TiposContingente = _context.TiposContingente
                    .ToList(),
                Tratados = _context.Tratados.ToList(),
                Titulo = "Propiedades de Contingente"
            };
            return View("FormContingenteVario", vm);
        }

        
        [HttpPost]
        public ActionResult Save(ContingenteVarioFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var vm = new ContingenteVarioFormViewModel
                {
                    Contingente = model.Contingente,
                    TiposContingente = _context.TiposContingente
                    .ToList(),
                    Tratados = _context.Tratados.ToList(),
                    Titulo = model.Contingente.Id == 0 ? "Nuevo Contingente" : "Propiedades de Contingente"
                };

                return View("FormContingenteVario", vm);
            }
            if(model.Contingente.Id == 0)
            {
                var contingente = new ContingenteVario
                {
                    TipoContingenteId = model.Contingente.TipoContingenteId,
                    Anio = model.Contingente.Anio,
                    TotalRecepcion = model.Contingente.TotalRecepcion,
                    FechaInicio = model.Contingente.FechaInicio,
                    FechaFinal = model.Contingente.FechaFinal,
                    Descripcion = model.Contingente.Descripcion,
                    TratadoId = model.Contingente.TratadoId,
                    CalcularPorcentaje = model.Contingente.CalcularPorcentaje,
                    FechaLicenciaSegundaFase = model.Contingente.GenerarLicencias ?
                    model.Contingente.FechaLicenciaSegundaFase : null,
                    GenerarLicencias = model.Contingente.GenerarLicencias,
                    PorcentajePrimeraFase = model.Contingente.PorcentajePrimeraFase
                };
                _context.ContingentesVarios.Add(contingente);
                //
            }else
            {
                var vario = _context.ContingentesVarios
                    .SingleOrDefault(v => v.Id == model.Contingente.Id);
                vario.Anio = model.Contingente.Anio;
                vario.CalcularPorcentaje = model.Contingente.CalcularPorcentaje;
                vario.Descripcion = model.Contingente.Descripcion;
                vario.FechaFinal = model.Contingente.FechaFinal;
                vario.FechaGeneracionSolicitudes = model.Contingente.FechaGeneracionSolicitudes;
                vario.FechaInicio = model.Contingente.FechaInicio;
                vario.TipoContingenteId = model.Contingente.TipoContingenteId;
                vario.TotalRecepcion = model.Contingente.TotalRecepcion;
                vario.TratadoId = model.Contingente.TratadoId;
                vario.GenerarLicencias = model.Contingente.GenerarLicencias;
                vario.FechaLicenciaSegundaFase = model.Contingente.GenerarLicencias ?
                    model.Contingente.FechaLicenciaSegundaFase : null;
                vario.PorcentajePrimeraFase = model.Contingente.PorcentajePrimeraFase;
                //
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult EditTemplate(int id)
        {
            var contingente = _context.ContingentesVarios
                .Include(c => c.TipoContingente)
                .SingleOrDefault(l => l.Id == id);

            if (contingente == null)
                return HttpNotFound();


            return View(contingente);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditTemplate(ContingenteVario model)
        {
            var contingente = _context.ContingentesVarios
                .Include(c => c.TipoContingente)
                .SingleOrDefault(c => c.Id == model.Id);

            contingente.TemplateLicencia = model.TemplateLicencia;
            _context.SaveChanges();

            this.Flash("success", "Plantilla para licencias de " +
                contingente.TipoContingente.nombre + ", ha sido actualizada correctamente.");

            return RedirectToAction("Details", new { id = contingente.Id });
        }
    }
}