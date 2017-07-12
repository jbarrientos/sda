using MvcFlashMessages;
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
    public class AsignacionesController : Controller
    {

        ApplicationDbContext _context;

        public AsignacionesController()
        {
            _context = new ApplicationDbContext();
        }
        // GET: Asignaciones
        public ActionResult Index(int id)
        {
            var contingente = _context
                .DetallesContingente
                .Include(d => d.Contingente)
                .Include(d => d.Contingente.TipoContingente)
                .Single(d => d.detalleContingenteId == id);
            var viewModel = new AsignacionIndexViewModel
            {
                DetalleContingente = contingente,
                Asignaciones = _context.Asignaciones
                .Where(a => a.DetalleContingenteId == id)
                .OrderByDescending(a => a.Id)
                .ToList() 
            };
            return View(viewModel);
        }

        public ActionResult New(int id)
        {

            var contingente = _context.DetallesContingente
                .Include(d => d.Contingente.TipoContingente)
                .Single(d => d.detalleContingenteId == id);

            var asignacion = new Asignacion
            {
                DetalleContingenteId = id,
                FechaEjecucion = DateTime.Now,
                UserId = User.Identity.Name,
                Cerrada = false
            };

            _context.Asignaciones.Add(asignacion);

            // Solicitudes Nuevos
            var solicitudes = _context.Solicitudes
                .Include(s => s.DetalleContingente.Contingente.TipoContingente)
                .Where(s => s.detalleContingenteId == id
                && s.estado == "R"
                && s.esImportadorHistorico == "N"
                && s.DetalleContingente.Contingente.TipoContingente.SepararHistoricos
                && (double)s.volumenSolicitado > 0.00).ToList();
            //
            double montoTotal = contingente.volumenNuevo;
            var numSolicitudes = solicitudes.Count;
            double totalSolicitado = (double)solicitudes.Sum(s => s.volumenSolicitado);
            double asignado = montoTotal / numSolicitudes;
            //
            Asignar(solicitudes, asignado, montoTotal, asignacion, false);
            // Historicos
            solicitudes = _context.Solicitudes
                .Include(s => s.contribuyente)
                .Include(s => s.DetalleContingente.Contingente.TipoContingente)
                .Where(s => s.detalleContingenteId == id
                && s.estado == "R"
                && s.esImportadorHistorico == "Y"
                && s.DetalleContingente.Contingente.TipoContingente.SepararHistoricos
                && (double)s.volumenSolicitado > 0.00).ToList();
            //
            montoTotal = contingente.monto - contingente.volumenNuevo;
            numSolicitudes = solicitudes.Count;
            totalSolicitado = (double)solicitudes.Sum(s => s.volumenSolicitado);
            asignado = montoTotal / numSolicitudes;
            //
            AsignarHistoricos(solicitudes, asignado, montoTotal, asignacion, true);
            if (contingente.Contingente.TipoContingente.EspecificarFraccion 
                && contingente.Contingente.TipoContingente.SepararHistoricos)
            {
                // Proceso de Asignacion de contingentes como el de quesos
                AsignacionPorFraccion(contingente, asignacion);
            }

            _context.SaveChanges();
            // Solicitudes para Contingentes Sin distincion de nuevos e historicos
            // Historicos
            solicitudes = _context.Solicitudes
                .Include(s => s.contribuyente)
                .Include(s => s.DetalleContingente.Contingente.TipoContingente)
                .Where(s => s.detalleContingenteId == id
                && s.estado == "R"
                && !s.DetalleContingente.Contingente.TipoContingente.SepararHistoricos
                && (double)s.volumenSolicitado > 0.00).ToList();
            //
            AsignarSinDistincion(solicitudes, asignacion);
            //

            this.Flash("success", "Proceso de Asignación realizado exitosamente.");

            return RedirectToAction("Index", "DetalleAsignaciones", new { id = asignacion.Id });
        }

        private void AsignacionPorFraccion(
            DetalleContingente contingente,
            Asignacion asignacion)
        {

            var fracciones = _context.FraccionesTipoContingente
                .Include(f => f.Fraccion)
                .Where(f => f.tipoContingenteId == contingente.Contingente.tipoContingenteId)
                .ToList();
            var detallesAsignacion = _context.DetallesAsignaciones
                    .Include(d => d.Solicitud)
                    .Where(d => d.AsignacionId == asignacion.Id && d.Solicitud.estado == "R")
                    .ToList();
            //
            foreach(var detalle in detallesAsignacion)
            {
                double totalFracciones = 0.00;
                foreach (var frac in fracciones)
                {
                    var detalleFraccion = _context.DetallesSolicitud
                        .Include(d => d.Solicitud)
                        .Where(d => d.SolicitudId == detalle.SolicitudId &&
                        d.FraccionId == frac.fraccionId).SingleOrDefault();

                    if(detalleFraccion != null)
                    {
                        var solicitadoFraccion = (double)detalleFraccion.Solicitado;
                        // var asignado = detalle.Asignado;
                        var porcentaje = 0.00;
                        if(solicitadoFraccion > 0.00)
                            porcentaje = solicitadoFraccion / (double)detalleFraccion.Solicitud.volumenSolicitado;
                        
                        //

                        //
                        var valorAsignado = detalle.Asignado * porcentaje;
                        //
                        detalleFraccion.Asignado = valorAsignado;
                        detalleFraccion.PorcentajeAplicado = porcentaje;
                        totalFracciones += valorAsignado;
                        //
                        
                    }

                }
                //detalle.Asignado = totalFracciones;
                _context.SaveChanges();
            }
            
            
        }

        public ActionResult Close(int id)
        {
            var asignacion = _context.Asignaciones.SingleOrDefault(a => a.Id == id);

            if (asignacion == null)
                return HttpNotFound();

            var detalles = _context.DetallesAsignaciones
                .Where(d => d.AsignacionId == id).ToList();

            foreach (var detalle in detalles)
            {

                var solicitud = _context.Solicitudes
                    .Single(s => s.solicitudId == detalle.SolicitudId);

                solicitud.volumenAsignado = (decimal)detalle.Asignado;
                

            }
            asignacion.Cerrada = true;
            _context.SaveChanges();
            this.Flash("success", "Proceso de Cierre de Asignación realizado exitosamente.");
            return RedirectToAction("Index","Home");
        }

        private void AsignarSinDistincion(
            List<Solicitud> solicitudes,
            Asignacion asignacion)
        {
            var fracciones = _context.FraccionesTipoContingente
                .Where(f => f.tipoContingenteId == asignacion.DetalleContingente.Contingente.tipoContingenteId)
                .ToList();
            //
            var distribucionPorCodigo = asignacion.DetalleContingente.monto / fracciones.Count();
            //
            var maximoPorFraccion = asignacion.DetalleContingente.monto * 
                (asignacion.DetalleContingente.Contingente.TipoContingente.PorcentajeMaximo / 100);
            //
            foreach(var fraccion in fracciones)
            {
                var sols = _context.DetallesSolicitud
                    .Include(d => d.Solicitud)
                    .Where(d => d.Solicitud.detalleContingenteId == asignacion.DetalleContingenteId
                    && d.FraccionId == fraccion.fraccionId)
                    .ToList();
                var remanenteFraccion = 0.00;
                var totalSolicitado = 0.00;
                if(sols.Count() > 0)
                    totalSolicitado = _context.DetallesSolicitud
                    .Include(d => d.Solicitud)
                    .Where(d => d.Solicitud.detalleContingenteId == asignacion.DetalleContingenteId
                    && d.FraccionId == fraccion.fraccionId)
                    .Sum(d => d.Solicitado);
                //
                var valorPorSolicitud = maximoPorFraccion / sols.Count();
                if (totalSolicitado < maximoPorFraccion)
                    valorPorSolicitud = distribucionPorCodigo / sols.Count();
                //
                foreach (var sol in sols)
                {
                    if (sol.Solicitado < valorPorSolicitud)
                    {
                        sol.Asignado = sol.Solicitado;
                        remanenteFraccion += valorPorSolicitud - sol.Asignado;
                    }                        
                    else
                        sol.Asignado = valorPorSolicitud;
                }
                _context.SaveChanges();
                if (remanenteFraccion > 0 && totalSolicitado < maximoPorFraccion)
                {
                    var solicitudesConFaltante = _context.DetallesSolicitud
                    .Include(d => d.Solicitud)
                    .Where(d => d.Solicitud.detalleContingenteId == asignacion.DetalleContingenteId
                    && d.FraccionId == fraccion.fraccionId
                    && d.Asignado < d.Solicitado)
                    .ToList();
                    //
                    foreach (var solicFaltante in solicitudesConFaltante)
                    {

                        remanenteFraccion -= (solicFaltante.Solicitado - solicFaltante.Asignado);
                        solicFaltante.Asignado = solicFaltante.Solicitado;

                    }
                }
                _context.SaveChanges();
            }
            

            foreach (var solicitud in solicitudes)
            {
                var asignado = _context.DetallesSolicitud
                    .Where(d => d.SolicitudId == solicitud.solicitudId)
                    .Sum(d => d.Asignado);
                //    
                var detalleAsignacion = new DetalleAsignacion
                {
                    AsignacionId = asignacion.Id,
                    SolicitudId = solicitud.solicitudId,
                    Solicitado = (double)solicitud.volumenSolicitado,
                    Comentarios = asignado.ToString(),
                    Asignado = asignado,
                    Finalizado = false
                };
                
                _context.DetallesAsignaciones.Add(detalleAsignacion);
                
            }
            _context.SaveChanges();
            
        }
        private void Asignar(
            List<Solicitud> solicitudes, 
            double asignado,
            double disponible, 
            Asignacion asignacion, 
            bool historicos)
        {
            foreach (var solicitud in solicitudes)
            {
                

                var existeAsignacion = _context
                        .DetallesAsignaciones
                        .Include(a => a.Asignacion)
                        .SingleOrDefault(a => a.SolicitudId == solicitud.solicitudId
                        && a.Asignacion.Id == asignacion.Id);

                if (existeAsignacion == null)
                {
                    

                    var detalleAsignacion = new DetalleAsignacion
                    {
                        AsignacionId = asignacion.Id,
                        SolicitudId = solicitud.solicitudId,
                        Solicitado = (double)solicitud.volumenSolicitado,
                        Comentarios = asignado.ToString(),
                        Finalizado = false
                    };
                    if((double)solicitud.volumenSolicitado <= asignado)
                    {
                        detalleAsignacion.Asignado = (double)solicitud.volumenSolicitado;
                        detalleAsignacion.Finalizado = true;
                        disponible -= (double)solicitud.volumenSolicitado;
                    }else
                    {
                        detalleAsignacion.Asignado = asignado;
                        disponible -= asignado;
                    }
                    
                    _context.DetallesAsignaciones.Add(detalleAsignacion);
                }else if(!existeAsignacion.Finalizado)
                {
                    
                    //if(existeAsignacion == null)
                    //{
                    //    var detAsignacion = new DetalleAsignacion
                    //    {
                    //        AsignacionId = asignacion.Id,
                    //        SolicitudId = solicitud.solicitudId,
                    //        Solicitado = (double)solicitud.volumenSolicitado
                    //    };
                    //    if ((double)solicitud.volumenSolicitado <= asignado)
                    //    {
                    //        detAsignacion.Asignado = (double)solicitud.volumenSolicitado;
                    //        detAsignacion.Finalizado = true;
                    //        disponible -= (double)solicitud.volumenSolicitado;
                    //        detAsignacion.Comentarios = solicitud.volumenSolicitado.ToString();
                    //    }
                    //    else
                    //    {
                    //        detAsignacion.Asignado = asignado;
                    //        detAsignacion.Comentarios = asignado.ToString();
                    //        disponible -= asignado;
                    //    }
                    //    _context.DetallesAsignaciones.Add(detAsignacion);
                    //}
                    //else
                    //{
                    if((double)solicitud.volumenSolicitado <= 
                            existeAsignacion.Asignado + asignado)
                        {
                            existeAsignacion.Asignado = (double)solicitud.volumenSolicitado;
                            disponible -= (double)solicitud.volumenSolicitado - 
                                existeAsignacion.Asignado;
                            
                            existeAsignacion.Finalizado = true;
                        }else
                        {
                            existeAsignacion.Asignado += asignado;
                            disponible -= asignado;
                        }
                        existeAsignacion.Comentarios += "|" + asignado.ToString();
                    
                }
            }
            _context.SaveChanges();
            // if(disponible > 0.00)
            if (disponible > 0.00
                && asignacion.DetalleContingente.Contingente.TipoContingente.SepararHistoricos)
            {
                var sols = _context.Solicitudes
                .Where(s => s.detalleContingenteId == asignacion.DetalleContingenteId
                && s.estado == "R"
                && s.esImportadorHistorico == (historicos ? "Y" : "N")
                && (double)s.volumenSolicitado > 0.00).ToList();

                //disponible = _context.DetallesAsignaciones
                //    .Where(d => d.AsignacionId == asignacion.Id)
                //    .Sum(d => d.Asignado);
                var dets = _context.DetallesAsignaciones
                    .Include(d => d.Solicitud)
                    .Where(d => d.AsignacionId == asignacion.Id 
                    //&& d.Solicitado > d.Asignado 
                    && !d.Finalizado
                    && d.Solicitud.esImportadorHistorico == (historicos ? "Y" : "N"))
                    .Count();

                var totAsignado = _context.DetallesAsignaciones
                    .Where(d => d.AsignacionId == asignacion.Id
                    && d.Solicitud.esImportadorHistorico == (historicos ? "Y" : "N"))
                    .Sum(d => d.Asignado);

                disponible = 
                    (historicos ? 
                    asignacion.DetalleContingente.monto - asignacion.DetalleContingente.montoNuevo 
                    : asignacion.DetalleContingente.montoNuevo) - totAsignado;
                asignado = disponible / dets;
                //
                if(dets > 0 && asignado > 0.00)
                    Asignar(sols, asignado, disponible, asignacion, historicos);
            }
        }

        // Asignacion Historicos
        private void AsignarHistoricos(
            List<Solicitud> solicitudes,
            double asignado,
            // ref double disponible,
            double disponible,
            Asignacion asignacion,
            bool historicos)
        {
            var promedioTotal = 0.00;

            double montoTotal = (asignacion.DetalleContingente.monto -
                               asignacion.DetalleContingente.montoNuevo);

            if (asignacion.DetalleContingente.Contingente.TipoContingente.EspecificarFraccion)
            {
                promedioTotal = SumaPromedioTotalImportaciones(asignacion.DetalleContingente);
            }else if(asignacion.DetalleContingente.Contingente.aniosAnteriores > 0)
            {
                promedioTotal = SumaPromedioTotalImportacionesHist(asignacion.DetalleContingente);
            }
            //
            foreach (var solicitud in solicitudes)
            {


                var existeAsignacion = _context
                        .DetallesAsignaciones
                        .Include(a => a.Asignacion)
                        .SingleOrDefault(a => a.SolicitudId == solicitud.solicitudId
                        && a.Asignacion.Id == asignacion.Id);

                if (existeAsignacion == null)
                {
                    Solicitud solicitudAnterior = SolicitudPeriodoAnterior(solicitud);
                    bool finalizar = false;
                    if(solicitudAnterior != null)
                    {
                        if(solicitud.DetalleContingente.Contingente.aniosAnteriores > 0)
                        {
                            double avgImportaciones = PromedioImportacionesI(solicitud);
                            double avg = avgImportaciones / promedioTotal;
                            //
                            
                            asignado = avg * montoTotal;
                            string trace  = "PromedioImportaciones: " + avgImportaciones.ToString() +
                                " Porcentaje: " + avg.ToString() +
                                " Asignado: " + asignado.ToString();
                            finalizar = true;
                        }
                        else
                        {
                            double porcentajeUtilizacion =
                            (double)(solicitudAnterior.volumenImportado / solicitudAnterior.volumenAsignado);

                            double totalAnterior = TotalImportacionesPeriodoAnterior(solicitud, true);
                            double porcentajeImportaciones = (double)solicitudAnterior.volumenImportado / totalAnterior;

                            if(porcentajeUtilizacion <
                                (
                                (double)solicitud.DetalleContingente
                                .Contingente.porcentajeVolumenHistorico) / 100.00
                                )
                            {
                                finalizar = true;
                            }
                            asignado = porcentajeImportaciones *
                               (solicitud.DetalleContingente.monto -
                               solicitud.DetalleContingente.montoNuevo);
                        }
                        

                    }
                    


                    var detalleAsignacion = new DetalleAsignacion
                    {
                        AsignacionId = asignacion.Id,
                        SolicitudId = solicitud.solicitudId,
                        Solicitado = (double)solicitud.volumenSolicitado,
                        Comentarios = asignado.ToString(),
                        Finalizado = finalizar
                    };
                    //if ((double)solicitud.volumenSolicitado <= asignado)
                    //{
                    //    detalleAsignacion.Asignado = (double)solicitud.volumenSolicitado;
                    //    detalleAsignacion.Finalizado = true;
                    //    disponible -= (double)solicitud.volumenSolicitado;
                    //}
                    //else
                    //{
                        detalleAsignacion.Asignado = asignado;
                        disponible -= asignado;
                    //}

                    _context.DetallesAsignaciones.Add(detalleAsignacion);
                }
                else if (!existeAsignacion.Finalizado)
                {

                    //if(existeAsignacion == null)
                    //{
                    //    var detAsignacion = new DetalleAsignacion
                    //    {
                    //        AsignacionId = asignacion.Id,
                    //        SolicitudId = solicitud.solicitudId,
                    //        Solicitado = (double)solicitud.volumenSolicitado
                    //    };
                    //    if ((double)solicitud.volumenSolicitado <= asignado)
                    //    {
                    //        detAsignacion.Asignado = (double)solicitud.volumenSolicitado;
                    //        detAsignacion.Finalizado = true;
                    //        disponible -= (double)solicitud.volumenSolicitado;
                    //        detAsignacion.Comentarios = solicitud.volumenSolicitado.ToString();
                    //    }
                    //    else
                    //    {
                    //        detAsignacion.Asignado = asignado;
                    //        detAsignacion.Comentarios = asignado.ToString();
                    //        disponible -= asignado;
                    //    }
                    //    _context.DetallesAsignaciones.Add(detAsignacion);
                    //}
                    //else
                    //{
                    if ((double)solicitud.volumenSolicitado <=
                            existeAsignacion.Asignado + asignado)
                    {
                        existeAsignacion.Asignado = (double)solicitud.volumenSolicitado;
                        disponible -= (double)solicitud.volumenSolicitado -
                            existeAsignacion.Asignado;

                        existeAsignacion.Finalizado = true;
                    }
                    else
                    {
                        existeAsignacion.Asignado += asignado;
                        disponible -= asignado;
                    }
                    existeAsignacion.Comentarios += "|" + asignado.ToString();

                }
            }
            _context.SaveChanges();
            if (disponible > 0.00 
                && asignacion.DetalleContingente.Contingente.TipoContingente.SepararHistoricos)
            {
                var sols = _context.Solicitudes
                .Where(s => s.detalleContingenteId == asignacion.DetalleContingenteId
                && s.estado == "R"
                && s.esImportadorHistorico == (historicos ? "Y" : "N")
                && (double)s.volumenSolicitado > 0.00).ToList();

                //disponible = _context.DetallesAsignaciones
                //    .Where(d => d.AsignacionId == asignacion.Id)
                //    .Sum(d => d.Asignado);
                var dets = _context.DetallesAsignaciones
                    .Include(d => d.Solicitud)
                    .Where(d => d.AsignacionId == asignacion.Id
                    //&& d.Solicitado > d.Asignado 
                    && !d.Finalizado
                    && d.Solicitud.esImportadorHistorico == (historicos ? "Y" : "N"))
                    .Count();

                var totAsignado = _context.DetallesAsignaciones
                    .Where(d => d.AsignacionId == asignacion.Id
                    && d.Solicitud.esImportadorHistorico == (historicos ? "Y" : "N"))
                    .Sum(d => d.Asignado);

                disponible =
                    (historicos ?
                    asignacion.DetalleContingente.monto - asignacion.DetalleContingente.montoNuevo
                    : asignacion.DetalleContingente.montoNuevo) - totAsignado;
                asignado = disponible / dets;
                //
                if (dets > 0 && asignado > 0.00)
                    AsignarHistoricos(sols, asignado, disponible, asignacion, historicos);
            }
        }

        private double PorcentajeUtilizacionPeriodoAnterior(
            Solicitud solicitud)
        {
            var solicitudAnterior = _context.Solicitudes
                .Include(s => s.DetalleContingente.Contingente)
                .SingleOrDefault(s => s.contribuyenteId == solicitud.contribuyenteId
                && s.DetalleContingente.contingenteId == solicitud.DetalleContingente.contingenteId
                && s.DetalleContingente.anio == (solicitud.DetalleContingente.anio - 1));

            if (solicitudAnterior == null)
                return 0.00;

            return (double)(solicitudAnterior.volumenImportado / solicitudAnterior.volumenAsignado);
        }

        public double TotalImportacionesPeriodoAnterior(Solicitud solicitud, bool historicos)
        {
            var totalAnterior = _context.Solicitudes
                .Include(s => s.DetalleContingente.Contingente)
                .Where(
                s => s.DetalleContingente.contingenteId == solicitud.DetalleContingente.contingenteId
                && s.esImportadorHistorico == (historicos ? "Y" : "N")
                && s.DetalleContingente.anio == (solicitud.DetalleContingente.anio - 1))
                .Sum(s => s.volumenImportado);

            if (totalAnterior == null)
                return 0.00;

            return (double)totalAnterior;
        }



        public Solicitud SolicitudPeriodoAnterior(Solicitud solicitud)
        {
            var solicitudAnterior = _context.Solicitudes
                .Include(s => s.DetalleContingente.Contingente)
                .SingleOrDefault(s => s.contribuyenteId == solicitud.contribuyenteId
                && s.DetalleContingente.contingenteId == solicitud.DetalleContingente.contingenteId
                && s.DetalleContingente.anio == (solicitud.DetalleContingente.anio - 1));

            return solicitudAnterior;
        }

        public double PromedioImportaciones(Solicitud solicitud)
        {
            var promedio = _context.Solicitudes
                .Include(s => s.DetalleContingente.Contingente)
                .Where(s => s.contribuyenteId == solicitud.contribuyenteId
                && s.DetalleContingente.contingenteId == solicitud.DetalleContingente.contingenteId
                && s.DetalleContingente.anio >= 
                (solicitud.DetalleContingente.anio - solicitud.DetalleContingente.Contingente.aniosAnteriores)
                && s.DetalleContingente.anio != solicitud.DetalleContingente.anio
                && s.esImportadorHistorico == "Y")
                .Average(s => s.volumenImportado);

            if (promedio == null)
                return 0.00;

            //if (promedio > solicitud.volumenSolicitado)
            //    return (double)solicitud.volumenSolicitado;

            return (double)promedio;
        }
        public double SumaPromedioTotalImportaciones(DetalleContingente detalleContingente)
        {
            //var promedio = _context.Solicitudes
            //    .Include(s => s.DetalleContingente.Contingente)
            //    .Where(s => s.DetalleContingente.contingenteId == detalleContingente.contingenteId
            //    && s.DetalleContingente.anio >= 
            //    (detalleContingente.anio - detalleContingente.Contingente.aniosAnteriores)
            //    && s.DetalleContingente.anio != detalleContingente.anio)
            //    .Average(s => s.volumenImportado);

            var solicitudes = _context.Solicitudes
                .Where(s => s.detalleContingenteId == detalleContingente.detalleContingenteId
                && s.esImportadorHistorico == "Y")
                .ToList();
            double suma = 0.00;
            foreach(var sol in solicitudes)
            {
                var promedio = PromedioImportaciones(sol);
                suma += promedio;
            }

            return suma;
        }
        public double PromedioImportacionesI(Solicitud solicitud)
        {
            var suma = _context.Solicitudes
                .Include(s => s.DetalleContingente.Contingente)
                .Where(s => s.contribuyenteId == solicitud.contribuyenteId
                && s.DetalleContingente.contingenteId == solicitud.DetalleContingente.contingenteId
                && s.DetalleContingente.anio >=
                (solicitud.DetalleContingente.anio - solicitud.DetalleContingente.Contingente.aniosAnteriores)
                && s.DetalleContingente.anio != solicitud.DetalleContingente.anio)
                .Sum(s => s.volumenImportado);

            if (suma == null)
                return 0.00;

            var promedio = suma / solicitud.DetalleContingente.Contingente.aniosAnteriores;

            //if (promedio > solicitud.volumenSolicitado)
            //    return (double)solicitud.volumenSolicitado;

            return (double)promedio;
        }
        public double SumaPromedioTotalImportacionesHist(
            DetalleContingente detalleContingente)
        {


            var solicitudes = _context.Solicitudes
                .Where(s => s.detalleContingenteId == detalleContingente.detalleContingenteId
                && s.esImportadorHistorico == "Y")
                .ToList();
            double suma = 0.00;
            foreach (var sol in solicitudes)
            {
                var promedio = PromedioImportacionesI(sol);
                suma += promedio;
            }

            return suma;
        }
    }
}