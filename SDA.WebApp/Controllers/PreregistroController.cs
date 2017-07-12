using MvcFlashMessages;
using SDA.WebApp.Helpers;
using SDA.WebApp.Models;
using SDA.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDA.WebApp.Controllers
{
    public class PreregistroController : Controller
    {

        private ApplicationDbContext _context;

        public PreregistroController()
        {
            _context = new ApplicationDbContext();
        }


        // GET: Preregistro
        [Authorize(Roles = RoleName.ADMINISTRADOR)]
        public ActionResult Index()
        {

            var preregistros = _context.Preregistros.ToList();

            return View(preregistros);
        }

        public ActionResult New()
        {

            var viewModel = new PreregistroFormViewModel();
            return View();
        }
        [HttpPost]
        public ActionResult Save(PreregistroFormViewModel viewModel)
        {

            var preregistro = _context.Preregistros.SingleOrDefault(p => p.Nit == viewModel.Nit);
            var importador = _context.Contribuyentes.SingleOrDefault(_ => _.nit == viewModel.Nit);

            if(preregistro != null || importador != null)
            {
                var vm = new PreregistroFormViewModel
                {
                    Celular = viewModel.Celular,
                    Comentarios = viewModel.Comentarios,
                    //Dui = viewModel.Dui,
                    Nit = viewModel.Nit,
                    Email = viewModel.Email,
                    Nombre = viewModel.Nombre,
                    Telefono = viewModel.Telefono,
                    TipoPersona = viewModel.TipoPersona
                };

                this.Flash("error",
                "<h4>Lo sentimos, ya existe registro o importador registrado con el mismo NIT.</h4>");
                return View("New", vm);

            }


            var model = new Preregistro
            {
                Comentarios = viewModel.Comentarios,
                Dui = "",
                Email = viewModel.Email,
                Nit = viewModel.Nit,
                Status = "E",
                FechaEnvio = DateTime.Now,
                Telefono = viewModel.Telefono,
                Nombre = viewModel.Nombre,
                TipoPersona = viewModel.TipoPersona,
                Celular = viewModel.Celular,
                RegistroIVA = ""
                                
            };

            _context.Preregistros.Add(model);
            try
            {
                _context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entidad de tipo \"{0}\" en estado \"{1}\" tiene los siguientes errores de validacion:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;

            }
            

            NotificacionHelper helper = new NotificacionHelper();

            helper.EnviarNotificaciones(Constant.NOTIFICACION_CONTIGENTES,
                "SDA - Nuevo Preregistro",
                "<h3>Solicitud de Preregistro - " + model.Nombre + " - " + model.Nit + "</h3>"+
                "<hr /><p>Se ha creado una nueva solicitud para creación de usuario en SDA.</p>");
            
            this.Flash("info", 
                "<h4>Se ha enviado su solicitud, en 24 horas recibira " +
                "información con respecto al tramite de su solicitud.</h4>");
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Completar(int id, string token)
        {
            var model = _context.Preregistros
                .Where(p => p.Status == "A" && p.Token == token)
                .SingleOrDefault(p => p.Id == id);
            if (model == null)
                return HttpNotFound();

            var viewModel = new CompletarPreregistroFormViewModel
            {
                Preregistro = model,
                ActividadesEconomicas = _context.ActividadesEconomicas.OrderBy(a => a.Nombre).ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Completar(CompletarPreregistroFormViewModel viewModel)
        {

            var model = _context.Preregistros.Single(p => p.Id == viewModel.Preregistro.Id);

            model.RegistroIVA = viewModel.Preregistro.RegistroIVA;
            model.ActividadEconomicaId = viewModel.Preregistro.ActividadEconomicaId;
            model.Contacto = viewModel.Preregistro.Contacto;
            model.EmailAlterno = viewModel.Preregistro.EmailAlterno;
            model.Direccion = viewModel.Preregistro.Direccion;
            model.Dui = viewModel.Preregistro.Dui;
            model.DUIRepresentanteLegal = viewModel.Preregistro.DUIRepresentanteLegal;
            model.NitRepresentanteLegal = viewModel.Preregistro.NitRepresentanteLegal;
            model.CargoRepresentanteLegal = viewModel.Preregistro.CargoRepresentanteLegal;
            model.RepresentanteLegal = viewModel.Preregistro.RepresentanteLegal;

            model.Status = "R";
            _context.SaveChanges();

            NotificacionHelper helper = new NotificacionHelper();

            helper.EnviarNotificaciones(Constant.NOTIFICACION_CONTIGENTES,
                "SDA - Actualización de Datos",
                "<h3>" + model.Nombre + "</h3>" +
                "<hr /><p>Se le informa que el solicitante " + model.Nombre + 
                ", con NIT " + model.Nit + " ha completado los datos requeridos para la creación de usuario en el SDA.</p>");

            this.Flash("info",
                "Se han actualizado sus datos, posteriormente se le enviará un correo electrónico con las credenciales para poder acceder al Sistema de gestión de contingentes arancelarios." +
                " Muchas gracias!");

            return RedirectToAction("Index", "Home");
        }

        // Validacion de NIT
        public JsonResult ValidarNIT(string NIT)
        {
            int i = 0;
            int vSuma = 0;
            int vValor = 0;
            int digito = 0;
            int w_inttemp = 0;
            int vDiv = 0;
            int vMul = 0;
            int vResta = 0;
            bool nit_correcto = false;

            var strNIT = NIT.Replace("-", "").Replace("_","");

            if (string.IsNullOrEmpty(strNIT) | strNIT.Length < 14)
            {
                nit_correcto = false;
            }
            else
            {
                vValor = 14;
                w_inttemp = int.Parse(strNIT.Substring(10, 3));

                if ((w_inttemp < 100))
                {
                    for (i = 0; i <= 12; i++)
                    {
                        w_inttemp = int.Parse(strNIT.Substring(i, 1));
                        vSuma = vSuma + (w_inttemp * vValor);
                        vValor = vValor - 1;
                    }

                    vDiv = vSuma / 11;
                    vMul = vDiv * 11;
                    vResta = vSuma - vMul;
                    digito = int.Parse(strNIT.Substring(13, 1));
                    if ((vResta == 10))
                    {
                        vResta = 0;
                    }

                    if (vResta == digito)
                    {
                        nit_correcto = true;
                    }
                    else
                    {
                        nit_correcto = false;
                    }
                }
                else
                {
                    w_inttemp = int.Parse(strNIT.Substring(10, 3));
                    if (w_inttemp > 100)
                    {
                        vValor = 2;
                        for (i = 0; i <= 12; i++)
                        {
                            w_inttemp = int.Parse(strNIT.Substring(i, 1));
                            vSuma = vSuma + w_inttemp * vValor;
                            if (vValor == 2)
                            {
                                vValor = 7;
                            }
                            else
                            {
                                vValor = vValor - 1;
                            }
                        }
                        vDiv = (vSuma / 11);
                        vMul = vDiv * 11;
                        vResta = vSuma - vMul;

                        if (vResta > 1)
                        {
                            vResta = 11 - vResta;
                            w_inttemp = int.Parse(strNIT.Substring(13, 1));
                            if (vResta == w_inttemp)
                            {
                                nit_correcto = true;
                            }
                            else
                            {
                                w_inttemp = int.Parse(strNIT.Substring(13, 1));
                                if (((vResta == 11) & (w_inttemp == 0)))
                                {
                                    nit_correcto = true;
                                }
                                else
                                {
                                    nit_correcto = false;
                                }
                            }
                        }
                        else
                        {
                            vResta = 0;
                            w_inttemp = int.Parse(strNIT.Substring(13, 1));
                            if ((vResta == w_inttemp))
                            {
                                nit_correcto = true;
                            }
                            else
                            {
                                nit_correcto = false;
                            }
                        }
                    }
                    else
                    {
                        nit_correcto = false;
                    }
                }
            }

            if (!nit_correcto)
                return Json("NIT no es válido, por favor verifique.", JsonRequestBehavior.AllowGet);
            else
                return Json(true, JsonRequestBehavior.AllowGet);
        }
    }

    //

}