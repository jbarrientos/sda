using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDA.WebApp.Controllers
{
    public class TipoNomenclaturaController : Controller
    {

        ApplicationDbContext _context;

        public TipoNomenclaturaController()
        {
            _context = new ApplicationDbContext();
        }


        // GET: TipoNomenclatura
        public ActionResult Index()
        {
            var model = _context.TiposNomenclatura.ToList();
            return View(model);
        }
    }
}