using SDA.WebApp.Models;
using SDA.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDA.WebApp.Controllers
{
    public class RequisitoController : Controller
    {

        public ApplicationDbContext _context;
        public RequisitoController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Requisito
        public ActionResult Index(int id)
        {

            var tipoContingente = _context.TiposContingente.Single(t => t.tipoContingenteId == id);

            if (tipoContingente == null)
                return HttpNotFound();

            var requisitos = _context.Requisitos.Where(r => r.TipoContingenteId == id);

            var vm = new RequisitoIndexViewModel
            {
                TipoContingente = tipoContingente,
                Requisitos = requisitos
            };
            return View(vm);
        }

        public ActionResult New(int id)
        {

            var tipoContingente = _context.TiposContingente.SingleOrDefault(t => t.tipoContingenteId == id);

            var vm = new RequisitoFormViewModel
            {
                TipoContingente = tipoContingente

            };

            return View(vm);
        }
        [HttpPost]
        public ActionResult Save(RequisitoFormViewModel vm)
        {

            if (!ModelState.IsValid)
            {
                var tipoContingente = 
                    _context.TiposContingente.SingleOrDefault(t => t.tipoContingenteId == vm.TipoContingente.tipoContingenteId);

                var viewModel = new RequisitoFormViewModel
                {
                    TipoContingente = tipoContingente,
                    Nombre = vm.Nombre,
                    Indispensable = vm.Indispensable

                };
                return View("New", viewModel);

            }

            var model = new Requisito();
            model.Indispensable = vm.Indispensable;
            model.Nombre = vm.Nombre;
            model.TipoContingenteId = vm.TipoContingente.tipoContingenteId;

            _context.Requisitos.Add(model);

            _context.SaveChanges();

            return RedirectToAction("Index", new { id = vm.TipoContingente.tipoContingenteId });
        }

        public ActionResult ConsultaRequisitos(int id)
        {
            var tipo = _context.TiposContingente
                .SingleOrDefault(t => t.tipoContingenteId == id);

            if (tipo == null)
                return HttpNotFound();

            var requisitos = _context.Requisitos
                .Where(r => r.TipoContingenteId == tipo.tipoContingenteId)
                .ToList();
            var vm = new ConsultaRequisitosFormViewModel
            {
                TipoContingente = tipo,
                Requisitos = requisitos
            };
    
            return View(vm);
        }
    }
}