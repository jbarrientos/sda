using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using MvcFlashMessages;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Helpers;
using SDA.WebApp.Models;
using SDA.WebApp.ViewModels;
using Microsoft.AspNet.Identity;
using SDA.Services.Helpers;
using System.Data.Entity;
using System.Web.Security;

namespace SDA.WebApp.Controllers
{
    public class ContribuyenteController : Controller
    {

        
        private ApplicationDbContext _context;

        private ApplicationUserManager _userManager;

        public ContribuyenteController()
        {
            _context = new ApplicationDbContext();
        }


        // GET: Contribuyente
        public ActionResult Index()
        {
            var model = _context.Contribuyentes.ToList();
            return View(model);
        }

        public ActionResult Details(int id)
        {
            var contribuyente = _context.Contribuyentes
                .SingleOrDefault(c => c.contribuyenteId == id);

            if (contribuyente == null)
                return HttpNotFound();

            var vm = new ContribuyenteDetailViewModel
            {
                Contribuyente = contribuyente,
                Solicitudes = _context.Solicitudes
                .Include(s => s.DetalleContingente.Contingente.TipoContingente)
                .Include(s => s.unidadMedida)
                .Where(s => s.contribuyenteId == id && s.detalleContingenteId != null)
                .OrderByDescending(s => s.DetalleContingente.anio)
                .ToList()
                

            };
            return View(vm);
        }

        public ActionResult Dashboard()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            var importador =
                _context.Contribuyentes
                .SingleOrDefault(c => c.nit == User.Identity.Name);
            
            //
            var model = new ChartViewModel
            {
                Chart = GetChart()
            };
            //

            var solics = _context.Solicitudes
                .Include(s => s.DetalleContingente.Contingente.TipoContingente.UnidadMedida)
                .Include(s => s.ContingenteVario.TipoContingente.UnidadMedida)
                .Where(c => c.contribuyenteId == importador.contribuyenteId)
                .OrderByDescending(c => c.solicitudId).ToList();
            //
            List<IndexSolicitudes> sols = new List<IndexSolicitudes>();
            foreach(Solicitud sol in solics)
            {
                //var detalleContingente = sol.detalleContingenteId != null ?
                //    _context.DetallesContingente.Single(d => d.detalleContingenteId == sol.detalleContingenteId) :
                //    _context.ContingentesVarios.Single(c => c.Id == sol.ContingenteVarioId);
                //var contingente = _context.Contingentes.Single(c => c.contingenteId == detalleContingente.contingenteId);

                var tipoContingente = sol.detalleContingenteId == null ?
                    sol.ContingenteVario.TipoContingente : sol.DetalleContingente.Contingente.TipoContingente;

                //var tipoContingente =
                //    _context.TiposContingente
                //    .Single(t => t.tipoContingenteId == contingente.tipoContingenteId);
                bool incluir = true;
                if (sol.DetalleContingente != null)
                    incluir = sol.DetalleContingente.anio == DateTime.Now.Year;
                else
                    incluir = sol.ContingenteVario.Anio == DateTime.Now.Year;

                if (incluir)
                {
                    var sumaLicencias = 
                        _context.Licencias
                        .Where(l => l.solicitudId == sol.solicitudId)
                        .Sum(l => (double?)l.volumen) ?? 0.00;
                    sols.Add(new IndexSolicitudes
                    {
                        FechaRegistro = sol.fechaRegistro,
                        UnidadMedida = _context.UnidadesMedida.Single(u => u.unidadMedidaId == sol.unidadMedidaId),
                        VolumenSolicitado = Double.Parse(sol.volumenSolicitado.ToString()),
                        VolumenAsignado = Double.Parse(sol.volumenAsignado.ToString()),
                        VolumenImportado = Double.Parse(sol.volumenImportado.ToString()),
                        Id = sol.solicitudId.ToString(),
                        NumNotificaciones = _context.Notificaciones
                    .Where(n => n.SolicitudId == sol.solicitudId && !n.Visto)
                    .Count(),
                        VolumenSolicitadoReasignacion =
                    (Double?)sol.volumenSolicitadoReasignacion ?? 0.00,
                        VolumenReasignacion =
                    (Double?)sol.volumenReasignacion ?? 0.00,
                        DetalleContigente = sol.DetalleContingente,
                        Periodo = sol.DetalleContingente == null ? sol.ContingenteVario.Anio : sol.DetalleContingente.anio,
                        TipoContingente = tipoContingente,
                        SumaOtorgadoLicencias = (sumaLicencias == null ? 0.00 : sumaLicencias),
                        Estado = sol.estado == "A" ? "Autorizada" : 
                        (sol.estado == "R" ? "En Proceso" : (sol.estado == "S" ? "Solicitada" : "Pendiente")),
                        VolumenARedistribuir = Double.Parse((sol.volumenAsignado - sol.volumenImportado).ToString())
                    });

                }
                
            }
            //
            List<DetalleContingente> contins = _context.DetallesContingente
                .Where(c => DateTime.Now >= c.fechaInicioSolicitudes && DateTime.Now <= c.fechaFinSolicitudes)                
                .ToList();

            List<IndexDetalleContingenteModel> contingenteModdels = new List<IndexDetalleContingenteModel>();

            foreach(DetalleContingente con in contins)
            {
                Contingente c = _context.Contingentes.Single(_ => _.contingenteId == con.contingenteId);
                int numSols = 
                    _context.Solicitudes.Count(s => s.detalleContingenteId == con.detalleContingenteId && s.contribuyenteId == importador.contribuyenteId);
                var asignado = _context.Asignaciones
                    .Where(a => a.DetalleContingenteId == con.detalleContingenteId && a.Cerrada)
                    .SingleOrDefault();
                //
                if(asignado == null)
                {
                    contingenteModdels.Add(new IndexDetalleContingenteModel
                    {
                        Contingente = c,
                        TipoContingente = _context.TiposContingente.Single(t => t.tipoContingenteId == c.tipoContingenteId),
                        Id = con.detalleContingenteId.ToString(),
                        Tratado = _context.Tratados.Single(t => t.tratadoId == c.tratadoId),
                        Periodo = con.anio,
                        FechaInicio = con.fechaInicioSolicitudes,
                        FechaFinal = con.fechaFinSolicitudes,
                        YaSolicito = numSols > 0
                    });
                }

                
            }

            var viewModel = new DashboardImportadorViewModel {
                Solicitudes = sols,
                Importador = importador,
                Contingentes = contingenteModdels,
                Anio = DateTime.Now.Year
            };
            var roleManager = new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(_context));

            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var esAdmin = User.IsInRole("Admin");

            
            //             roles = userManager.GetRoles(User.);
            //var roles = roleManager.Roles.ToList();
            //admin.Users.Single(User.Identity.GetUserName());
            //var roles = roleManager.Roles.ToList();
            //ViewBag.roles = roles;
            //
            return View(viewModel);
        }

        private Chart GetChart()
        {
            
            var summ = from d in _context.DetallesContingente
                       join c in _context.Contingentes on d.contingenteId equals c.contingenteId
                       join t in _context.TiposContingente on c.tipoContingenteId equals t.tipoContingenteId
                       join u in _context.UnidadesMedida on t.unidadMedidaId equals u.unidadMedidaId
                       join s in _context.Solicitudes on d.detalleContingenteId equals s.detalleContingenteId
                       join us in _context.UnidadesMedida on s.unidadMedidaId equals us.unidadMedidaId
                       where d.anio == DateTime.Now.Year
                       group new { s.volumenAsignado, s.volumenSolicitado }
                       by new { d.anio, t.nombre, d.monto, d.montoNuevo } into grp
                       select new
                       {
                           Anio = grp.Key.anio,
                           TipoContingente = grp.Key.nombre,
                           Monto = grp.Key.monto,
                           MontoNuevos = grp.Key.montoNuevo,
                           Asignado = grp.Sum(s => s.volumenAsignado),
                           Solicitado = grp.Sum(s => s.volumenSolicitado)

                       }
                        ;

            foreach (var summary in summ)
            {

            }
            return new Chart(600, 400, ChartTheme.Blue)
                .AddTitle("Contingentes")
                .AddLegend()
                .AddSeries(
                    name: "WebSite",
                    chartType: "Pie",
                    xValue: new[] { "Asigando", "Solicitado" },
                    yValues: new[] { "2399.97823661221", "5845.30726383685" });

        }

        public ActionResult Create()
        {
            var model = new Contribuyente();
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(Contribuyente model)
        {
            _context.Contribuyentes.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult CreateUsuario(int id)
        {
            var importador = _context.Contribuyentes.Single(c => c.contribuyenteId == id);
            var pass = Membership.GeneratePassword(12, 1);
            //
            var viewModel = new LoginContribuyenteViewModel
            {
                contribuyenteId = id,
                email = importador.email,
                nombre = importador.nombre,
                nit = importador.nit,
                Password = pass,
                ConfirmPassword = pass,
                telefonoCelular = importador.telefonoCelular,
                telefonoFijo = importador.telefonoFijo
            };
            return View(viewModel);
        }
        [HttpPost]
        public async Task<ActionResult> CreateUsuario(LoginContribuyenteViewModel importador)
        {
            // ApplicationDbContext db = new ApplicationDbContext();


            var contribuyente = _context.Contribuyentes.Single(i => i.contribuyenteId == importador.contribuyenteId);


            var existeUser = _context.Users.SingleOrDefault(c => c.UserName == importador.nit);
            if (existeUser == null)
            {
                var user = new ApplicationUser
                {

                    UserName = importador.nit,
                    Email = importador.email,
                    direccion = contribuyente.direccion,
                    dui = contribuyente.dui,
                    nit = contribuyente.nit,
                    fechaRegistro = DateTime.Now,
                    nombre = contribuyente.nombre

                    //UserName = importador.nit,
                    //Email = importador.email,
                    //nit = importador.nit,
                    //nombre = importador.nombre,
                    //fechaRegistro = DateTime.Now
                };

                var result = await UserManager.CreateAsync(user, importador.Password);
                //var result = UserManager.Create(user, importador.Password);
                if (result.Succeeded)
                {
                    // var cont = _context.Contribuyentes.Single(c => c.contribuyenteId == importador.contribuyenteId);
                    
                    await UserManager.AddToRoleAsync(user.Id, RoleName.IMPORTADOR);

                    contribuyente.TieneUsuario = true;
                    _context.SaveChanges();

                    // Email
                    Email email = new Email();
                    email.To.Add(contribuyente.email);
                    if (contribuyente.EmailAlternativo != "" && contribuyente.EmailAlternativo != null)
                        email.CC.Add(contribuyente.EmailAlternativo);
                    email.From = Constant.DEFAULT_EMAIL_ACCOUNT;
                    email.Subject = "Ministerio de Economía - Creación de Usuario";
                    email.Body = "<p>Estimado(a):</p><p>" + contribuyente.nombre + "</p>";
                    email.Body += "<p>Muchas gracias por registrarse en el Sistema de Gestión de Contingentes Arancelarios del Ministerio de Economía.</p>";
                    email.Body += "<p><strong>Su cuenta ha sido creada exitosamente.</strong></p>";
                    email.Body += "<p>Para acceder al sistema, por favor ingrese a la siguiente dirección "+
                        "<a href='" +
                        Url.Action("Index", "Home", null, Request.Url.Scheme, null)
                        + "'>Sistema de Gestión de Contingentes Arancelarios</a> y seleccione la opción <strong>INGRESAR</strong>.</p>";
                    email.Body += "<p><strong>Sus credenciales de acceso a su cuenta son:<strong></p>";
                    email.Body += "<p><strong>Usuario:</strong> " + contribuyente.nit + "</p>";
                    email.Body += "<p><strong>Contraseña:</strong> " + importador.Password + "</p>";
                    email.Body += "<p>En caso de requerir asistencia, puede contactarnos a los teléfonos 2590-5788/5790 o al correo electrónico datco@minec.gob.sv.</p>";
                    email.Body += "<p>Atentamente,</p>";
                    email.Body += "<p>Dirección de Administración de Tratados Comerciales<br/>Ministerio de Economía.</p>";
                    email.Send();
                    //
                    this.Flash("success",
                        "Se creó usuario para el importador " + importador.nombre);
                }
                else
                {
                    this.Flash("warning",
                        "No se pudo crear usuario para el importador " + importador.nombre);
                }


            }
            return RedirectToAction("Index");
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult Edit(int id)
        {
            var model = _context.Contribuyentes.Single(c => c.contribuyenteId == id);

            var viewModel = new ImportadorFormViewModel
            {
                Contribuyente = model,
                ActividadesEconomicas = _context.ActividadesEconomicas.ToList()
            };
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult Edit(ImportadorFormViewModel model)
        {
            var importador = _context.Contribuyentes.Single(i => i.contribuyenteId == model.Contribuyente.contribuyenteId);
            if (importador == null)
                return HttpNotFound();

            importador.ActividadEconomicaId = model.Contribuyente.ActividadEconomicaId;
            importador.direccion = model.Contribuyente.direccion;
            importador.dui = model.Contribuyente.dui;
            importador.email = model.Contribuyente.email;
            importador.EmailAlternativo = model.Contribuyente.EmailAlternativo;
            importador.nit = model.Contribuyente.nit;
            importador.nombre = model.Contribuyente.nombre;
            importador.telefonoCelular = model.Contribuyente.telefonoCelular;
            importador.telefonoFijo = model.Contribuyente.telefonoFijo;
            importador.tipoPersona = model.Contribuyente.tipoPersona;
            importador.NombreNotificacion = model.Contribuyente.NombreNotificacion;
            importador.CargoNotificacion = model.Contribuyente.CargoNotificacion;
            importador.CargoRepresentanteLegal = model.Contribuyente.CargoRepresentanteLegal;
            //
            _context.SaveChanges();
            return RedirectToAction("index");
        }

        //public ActionResult Details(int id)
        //{
        //    var model = _context.Contribuyentes.Single(c => c.contribuyenteId == id);
        //    return View(model);
        //}

        public ActionResult Delete(int id)
        {
            var model = _context.Contribuyentes.Single(c => c.contribuyenteId == id);
            return View(model);
        }
        [HttpPost]
        public ActionResult Delete(int id, Contribuyente mod)
        {
            var model = _context.Contribuyentes.Single(c => c.contribuyenteId == id);
            _context.Contribuyentes.Remove(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Desktop()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            

            var contribuyente = _context.Contribuyentes
                .SingleOrDefault(c => c.nit == User.Identity.Name);

            var detalles = from t in _context.DetallesContingente
                           where t.anio == DateTime.Now.Year
                           select t;
            ICollection<IndexDetalleContingenteModel> lista = new List<IndexDetalleContingenteModel>();

            //
            ViewBag.contribuyente = contribuyente;
            //
            foreach (DetalleContingente detalle in detalles)
            {
                var contingente = _context.Contingentes.Single(c => c.contingenteId == detalle.contingenteId);
                
                //
                var item = new IndexDetalleContingenteModel();
                item.Contingente = contingente;
                item.Estado = (DateTime.Now >= detalle.fechaInicioSolicitudes && DateTime.Now <= detalle.fechaFinSolicitudes) ?
                    "Recep. Solic." : (
                    (DateTime.Now >= detalle.fechaInicio && DateTime.Now <= detalle.fechaInicioSolicitudesRe ? "En proceso" :
                    "Reasign.")
                    );
                item.FechaFinal = detalle.fechaFin;
                item.Id = detalle.detalleContingenteId.ToString();
                item.FechaInicio = detalle.fechaInicio;
                item.Periodo = detalle.anio;
                item.TipoContingente = _context.TiposContingente
                    .Single(t => t.tipoContingenteId == contingente.tipoContingenteId);
                //item.Tratado = tratado;
                lista.Add(item);
            }
            var model = lista;
            return View(model);
        }


        public ActionResult IndexSolicitudes(int detalleContingenteId, int id)
        {
            var contribuyente = _context.Contribuyentes.Single(c => c.contribuyenteId == id);
            ICollection<IndexSolicitudes> lista = new List<IndexSolicitudes>();

            var solicitudes = from t in _context.Solicitudes
                              where t.detalleContingenteId == detalleContingenteId
                              select t;
            //
            ViewBag.contribuyente = contribuyente;
            ViewBag.detalleContingenteId = detalleContingenteId;
            //
            foreach (Solicitud sol in solicitudes)
            {
                var item = new IndexSolicitudes();
                //item.Contribuyente = this.contribuyentes.GetById(sol.contribuyenteId);
                item.FechaRegistro = sol.fechaRegistro;
                item.Fraccion = _context.Fracciones.Single(f => f.fraccionId == sol.fraccionId);
                item.Id = sol.solicitudId.ToString();
                item.Historico = sol.esImportadorHistorico == "Y" ? "SI" : "NO";
                item.VolumenAsignado = (Double)sol.volumenAsignado;
                item.VolumenSolicitado = (Double)sol.volumenSolicitado;
                item.VolumenImportado = (Double)sol.volumenImportado;
                item.UnidadMedida = _context.UnidadesMedida.Single(u => u.unidadMedidaId == sol.unidadMedidaId);
                item.VolumenARedistribuir = item.VolumenAsignado - item.VolumenImportado;
                lista.Add(item);
            }

            var model = lista; //  tratado.fracciones;
            return View(model);
        }

        public ActionResult DisplayChart(int id)
        {
            var solicitud = _context.Solicitudes
                .Include(s => s.DetalleContingente.Contingente)
                .Include(s => s.contribuyente)
                .SingleOrDefault(s => s.solicitudId == id);
            //
            if(solicitud == null)
            {
                return HttpNotFound();
            }
            //
            var resumen = _context.Solicitudes
                .Include(s => s.DetalleContingente)
                .Where(s => s.DetalleContingente.Contingente.tipoContingenteId == solicitud.DetalleContingente.Contingente.tipoContingenteId
                && s.contribuyenteId == solicitud.contribuyenteId)
                .OrderBy(s => s.DetalleContingente.anio)
                .ToList();
            //

            var vm = new ContribuyenteDisplayChartViewModel
            {
                Resumen = resumen,
                Contribuyente = solicitud.contribuyente
            };

            return View(vm);
        }
    }
}