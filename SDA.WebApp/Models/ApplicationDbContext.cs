using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace SDA.WebApp.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<AnioCategoria> AniosCategoria { get; set; }
        public DbSet<Capitulo> Capitulos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Contingente> Contingentes { get; set; }
        public DbSet<DetalleContingente> DetallesContingente { get; set; }
        public DbSet<Fraccion> Fracciones { get; set; }
        public DbSet<FraccionTratado> FraccionesTratado { get; set; }
        public DbSet<Pais> Paises { get; set; }
        public DbSet<Partida> Partidas { get; set; }
        public DbSet<Seccion> Secciones { get; set; }
        public DbSet<Subpartida> Subpartidas { get; set; }
        public DbSet<Tratado> Tratados { get; set; }
        public DbSet<UnidadMedida> UnidadesMedida { get; set; }
        public DbSet<TipoNomenclatura> TiposNomenclatura { get; set; }
        public DbSet<CorrelativoAnual> CorrelativosAnual { get; set; }
        public DbSet<TipoContingente> TiposContingente { get; set; }
        public DbSet<FraccionTipoContingente> FraccionesTipoContingente { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<FraccionProducto> FraccionesProducto { get; set; }

        public DbSet<HeaderAduana> HeadersAduana { get; set; }
        public DbSet<DetalleAduana> DetallesAduana { get; set; }

        public DbSet<Licencia> Licencias { get; set; }


        //
        public DbSet<PaisTratado> PaisesTratado { get; set; }
        public DbSet<Contribuyente> Contribuyentes { get; set; }

        public DbSet<Solicitud> Solicitudes { get; set; }
        public DbSet<ImportacionExtraordinaria> ImportacionesExtraordinarias { get; set; }
        public DbSet<FraccionImportar> FraccionesImportar { get; set; }
        public DbSet<Invitacion> Invitaciones { get; set; }

        public DbSet<ActividadEconomica> ActividadesEconomicas { get; set; }

        public DbSet<Preregistro> Preregistros { get; set; }

        public DbSet<NotificacionInterna> NotificacionesInternas { get; set; }

        public DbSet<Requisito> Requisitos { get; set; }

        public DbSet<RequisitoSolicitud> RequisitosSolicitud { get; set; }

        public DbSet<Asignacion> Asignaciones { get; set; }

        public DbSet<DetalleAsignacion> DetallesAsignaciones { get; set; }

        public DbSet<CambioAsignacion> CambiosAsignacion { get; set; }

        public DbSet<Notificacion> Notificaciones { get; set; }

        public DbSet<DetalleSolicitud> DetallesSolicitud { get; set; }

        public DbSet<ContingenteVario> ContingentesVarios { get; set; }

        public DbSet<DetalleContingenteVario> DetallesContingenteVario { get; set; }

        public DbSet<DocumentoContingente> DocumentosContingente { get; set; }

        public DbSet<Parametro> Parametros { get; set; }

        public DbSet<DetalleLicencia> DetallesLicencia { get; set; }


        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {

        
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}