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
    public class FraccionTipoContingenteController : Controller
    {
        ApplicationDbContext _context;

        public FraccionTipoContingenteController()
        {
            _context = new ApplicationDbContext();
        }


        // GET: FraccionTipoContingente
        public ActionResult Index(int tipoContingenteId)
        {
            
            var tipo = _context.TiposContingente.Single(t => t.tipoContingenteId == tipoContingenteId);
            ICollection<IndexFraccionTipoContingenteModel> lista = new List<IndexFraccionTipoContingenteModel>();

            var fracciones = _context.FraccionesTipoContingente
                .Include(f => f.Fraccion)
                .Where(f => f.tipoContingenteId == tipoContingenteId)
                .ToList();
                
                //from t in _context.FraccionesTipoContingente
                //             where t.tipoContingenteId == tipoContingenteId
                //             select t;
            //
            ViewBag.tipoContingente = tipo;
            foreach (FraccionTipoContingente fr in fracciones)
            {
                var item = new IndexFraccionTipoContingenteModel();
                item.Id = fr.fraccionTipoContingenteId.ToString();
                item.Fraccion = fr.Fraccion;
                item.TipoContingenteId = fr.tipoContingenteId;
                lista.Add(item);
            }

            var model = lista; //  tratado.fracciones;
            return View(model);
        }

        public ActionResult AddFraccion(int tipoContingenteId)
        {
            
            var frac = from t in _context.Fracciones
                             where t.codigo.Length >= 10
                             select t;
            //
            var model = new AddFraccionTipoContingente();
            model.tipoContingenteId = tipoContingenteId;
            ViewBag.fracciones = frac.ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult AddFraccion(AddFraccionTipoContingente viewModel)
        {
            var model = new FraccionTipoContingente();
            model.fraccionId = viewModel.SelectedFraccionId;
            model.tipoContingenteId = viewModel.tipoContingenteId;
            _context.FraccionesTipoContingente.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Index", new { tipoContingenteId = viewModel.tipoContingenteId});
        }
    }
}