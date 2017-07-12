using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDA.WebApp.Controllers
{
    public class AnioCategoriaController : Controller
    {

        ApplicationDbContext _context;
        

        public AnioCategoriaController()
        {
            
            _context = new ApplicationDbContext();
        }

        // GET: AnioCategoria
        public ActionResult Index(int categoryId)
        {
            //var cortes = from t in _context.AniosCategoria
            //                 where t.categoriaId == categoryId
            //                 select t;

            var cortes = _context.AniosCategoria.Where(a => a.categoriaId == categoryId);
            var categoria = _context.Categorias.Single(c => c.categoriaId == categoryId);
            ViewBag.categoria = categoria;
            ViewBag.tratado = _context.Tratados
                .Single(t => t.tratadoId == categoria.tratadoId);
            //
            var model = cortes.ToList<AnioCategoria>();
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var model = _context.AniosCategoria.Single(a => a.anioCategoriaId == id);
            var categoria = _context.Categorias.Single(c => c.categoriaId == model.categoriaId);
            ViewBag.categoria = categoria;
            ViewBag.tratado = _context.Tratados.Single(t => t.tratadoId == categoria.tratadoId);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(AnioCategoria model)
        {

            _context.SaveChanges();
            return RedirectToAction("Details", new { id = model.anioCategoriaId });

        }

        public ActionResult Details(int id)
        {
            var model = _context.AniosCategoria
                .Include(a => a.Categoria)
                .Include(a => a.Categoria.tratado)
                .Single(a => a.anioCategoriaId == id);

            ViewBag.categoria = model.Categoria;
            ViewBag.tratado = model.Categoria.tratado;
            return View(model);
        }

        
    }
}