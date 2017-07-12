
using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDA.WebApp.Controllers
{
    public class CapituloController : Controller
    {
        ApplicationDbContext _context;

        public CapituloController()
        {
            _context = new ApplicationDbContext();
        }


        // GET: Capitulo
        public ActionResult Index()
        {
            var model = _context.Capitulos.ToList();
            return View(model);
        }
        
    }
}