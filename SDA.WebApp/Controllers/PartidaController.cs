using SDA.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDA.WebApp.Controllers
{
    public class PartidaController : Controller
    {

        ApplicationDbContext _context;

        public PartidaController()  
        {
            _context = new ApplicationDbContext();
        }



        // GET: Partida
        public ActionResult Index()
        {
            var model = _context.Partidas.ToList();
            return View(model);
        }
    }
}