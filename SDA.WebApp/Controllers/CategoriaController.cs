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
    public class CategoriaController : Controller
    {

        ApplicationDbContext _context;

        public CategoriaController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Categoria
        public ActionResult Index(int tratadoId)
        {
            var model = _context.Categorias.Where(c => c.tratadoId == tratadoId).ToList();
            //
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            // IRepositoryBase<Categoria> categoria = new CategoriaRepository(new DAL.Data.DataContext());
            var model = _context.Categorias.Single(c => c.categoriaId == id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Categoria model)
        {
            //IRepositoryBase<Categoria> categoria = new CategoriaRepository(new DAL.Data.DataContext());
            _context.SaveChanges();
            return RedirectToAction("IndexCategories", "Tratado", new { id = model.tratadoId });
        }


        public ActionResult Details(int id)
        {
            //
            ViewBag.tieneCiclos = 
                (from ciclos in _context.AniosCategoria
                 where ciclos.categoriaId == id select ciclos).Count() > 0;
            //
            var model = _context.Categorias.Include(c => c.tratado).Single(c => c.categoriaId == id);
            ViewBag.tratado = model.tratado;
            return View(model);
        }

        
        public ActionResult AddFraccion(int id)
        {
            
            //
            //IRepositoryBase<Categoria> categoria = new CategoriaRepository(context);
            //

            var model = new AddFraccionViewModel();

            model.CategoriaId = id;
            ViewBag.categoria = _context.Categorias.Single(c => c.categoriaId == id);
            ViewBag.fracciones = (from t in _context.Fracciones 
                                   where (t.codigo.Length) >= 12
                                   select t).ToList<Fraccion>();
            //
            return View(model);
        }

        [HttpPost]
        public ActionResult AddFraccion(FraccionTratado model)
        {
            _context.FraccionesTratado.Add(model);
            _context.SaveChanges();
            return RedirectToAction("IndexFracciones", new { id = model.categoriaId });

        }

        public ActionResult IndexFracciones(int id)
        {
            var categoria = _context.Categorias
                .Include(c => c.tratado)
                .Single(c => c.categoriaId == id);
            ICollection<IndexFraccionModel> lista = new List<IndexFraccionModel>();

            var fracciones = from t in _context.FraccionesTratado
                             where t.categoriaId == id
                             select t;
            //
            ViewBag.categoria = categoria;
            ViewBag.tratado = categoria.tratado;
            //var model = fracciones.ToList<FraccionTratado>();
            foreach (FraccionTratado fr in fracciones)
            {
                var item = new IndexFraccionModel();
                item.arancel = fr.arancel;
                item.categoriaId = fr.categoriaId;
                item.fraccionId = fr.fraccionId;
                item.Id = fr.fraccionTratadoId.ToString();
                item.Fraccion = _context.Fracciones.Single(f => f.fraccionId == fr.fraccionId);
                item.Categoria = _context.Categorias.Single(c => c.categoriaId == fr.categoriaId);
                lista.Add(item);
            }

            var model = lista; //  tratado.fracciones;
            return View(model);
        }


        public ActionResult GenerarCortes(int id)
        {
            //
            var model = _context.Categorias
                .Include(c => c.tratado)
                .SingleOrDefault(c => c.categoriaId == id);

            if (model == null)
                return HttpNotFound();  
            
            ViewBag.tratado = model.tratado;
            return View(model);
        }

        [HttpPost]
        public ActionResult GenerarCortes(Categoria model)
        {
            var tratado = _context.Tratados
                .SingleOrDefault(t => t.tratadoId == model.tratadoId);
            List<AnioCategoria> anioRepo = new List<AnioCategoria>();
            //
            for (int y = 0;y < model.aniosDesgravacion;y++)
            {
                var anio = new AnioCategoria();
                
                anio.categoriaId = model.categoriaId;
                anio.arancel = 0.00;
                if(y == 0)
                {
                    anio.fecha = tratado.fechaInicio;
                }
                else
                {
                    anio.fecha = new DateTime(tratado.fechaInicio.Year+y, 1, 1);
                }
                anio.formula = "";
                //                
                _context.AniosCategoria.Add(anio);
            }
            _context.SaveChanges();

            return RedirectToAction("Index", "AnioCategoria", new { categoryId = model.categoriaId });

        }


    }
}