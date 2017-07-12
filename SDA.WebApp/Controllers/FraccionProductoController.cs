using SDA.WebApp.Models;
using SDA.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDA.WebApp.Controllers
{
    public class FraccionProductoController : Controller
    {

        ApplicationDbContext _context;


        public FraccionProductoController()
        {
            _context = new ApplicationDbContext();
        }


        // GET: FraccionProducto
        public ActionResult Index(int productoId)
        {
            
            var producto = _context.Productos.Single(p => p.productoId == productoId);
            ICollection<IndexFraccionProductoModel> lista = new List<IndexFraccionProductoModel>();

            var fracciones = from t in _context.FraccionesProducto
                             where t.productoId == productoId
                             select t;
            //
            ViewBag.producto = producto;
            foreach (FraccionProducto fr in fracciones)
            {
                var item = new IndexFraccionProductoModel();
                item.Id = fr.fraccionProductoId.ToString();
                item.Fraccion = _context.Fracciones.Single(f => f.fraccionId == fr.fraccionId);
                item.ProductoId = fr.productoId;
                lista.Add(item);
            }

            var model = lista; //  tratado.fracciones;
            return View(model);
        }
    }
}