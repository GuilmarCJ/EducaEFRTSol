using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EducaEFRT.Models;
using EducaEFRT.Models.DB;
using EducaEFRT.Models.DB.Repositories;
using EducaEFRT.Models.ViewModels;

namespace EducaEFRT.Controllers
{
    [Authorize]
    public class DocenteController : Controller
    {
        public ActionResult CursosAsignados()
        {
            if (Session["IdUsuario"] == null)
                return RedirectToAction("Login", "Account");

            int idUsuario = (int)Session["IdUsuario"];
            System.Diagnostics.Debug.WriteLine("idUsuario en sesión: " + idUsuario);

            using (var repo = new AsignacionCursoRepository())
            {
                var cursos = repo.ObtenerCursosPorUsuario(idUsuario);

                //Obtener nombre completo del docente
                using (var db = new EduControlDB())
                {
                    var docente = db.Docentes.FirstOrDefault(d => d.IdUsuario == idUsuario);
                    if (docente != null)
                    {
                        ViewBag.NombreUsuario = docente.Nombres + " " + docente.Apellidos;
                    }
                    else
                    {
                        ViewBag.NombreUsuario = "Usuario";
                    }
                }

                return View(cursos);
            }
        }
        public ActionResult RegistrarAsistencia(int idAsignacion)
        {
            using (var db = new EduControlDB())
            {
                var asignacion = db.AsignacionesCurso
                    .Include("Curso")
                    .Include("Seccion")
                    .Include("Turno")
                    .FirstOrDefault(a => a.IdAsignacion == idAsignacion);

                if (asignacion == null)
                    return HttpNotFound();

                var model = new RegistrarAsistenciaDocenteViewModel
                {
                    IdAsignacion = asignacion.IdAsignacion,
                    Curso = asignacion.Curso.NombreCurso,
                    Seccion = asignacion.Seccion.NombreSeccion,
                    Turno = asignacion.Turno.NombreTurno
                };

                return View(model);
            }
        }

        [HttpPost]
        public ActionResult RegistrarAsistencia(RegistrarAsistenciaDocenteViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool exito;
                using (var repo = new AsistenciaDocenteRepository())
                {
                    exito = repo.RegistrarAsistencia(model.IdAsignacion);
                }

                if (exito)
                {
                    TempData["MensajeAsistencia"] = "✅ Asistencia registrada correctamente.";
                }
                else
                {
                    TempData["MensajeAsistencia"] = "⚠️ Ya registraste tu asistencia para este curso hoy.";
                }

                return RedirectToAction("CursosAsignados");
            }

            return View(model);
        }


        public ActionResult RegistrarNotas(int idAsignacion, string codigoBusqueda = null)
        {
            if (Session["IdUsuario"] == null)
                return RedirectToAction("Login", "Account");

            using (var db = new EduControlDB())
            {
                // 1. Validar que la asignación pertenezca al docente logueado
                var idUsuario = (int)Session["IdUsuario"];
                var asignacion = db.AsignacionesCurso
                    .Include("Curso")
                    .Include("Seccion")
                    .Include("Turno")
                    .FirstOrDefault(a => a.IdAsignacion == idAsignacion &&
                                       a.Docente.IdUsuario == idUsuario);

                if (asignacion == null)
                    return HttpNotFound();
                var query = db.Matriculas
            .Where(m => m.IdAsignacion == idAsignacion);

                // Aplicar filtro si hay búsqueda
                if (!string.IsNullOrEmpty(codigoBusqueda))
                {
                    query = query.Where(m => m.Estudiante.CodigoEstudiante.Contains(codigoBusqueda));
                }

                var estudiantes = query
                    .Select(m => new EstudianteNotasViewModel
                    {
                        IdMatricula = m.IdMatricula,
                        CodigoEstudiante = m.Estudiante.CodigoEstudiante,
                        NombreCompleto = m.Estudiante.Apellidos + ", " + m.Estudiante.Nombres,
                        NotaT1 = db.Notas.FirstOrDefault(n => n.IdMatricula == m.IdMatricula) != null
                                ? db.Notas.FirstOrDefault(n => n.IdMatricula == m.IdMatricula).NotaT1
                                : (decimal?)null,
                        NotaT2 = db.Notas.FirstOrDefault(n => n.IdMatricula == m.IdMatricula) != null
                                ? db.Notas.FirstOrDefault(n => n.IdMatricula == m.IdMatricula).NotaT2
                                : (decimal?)null,
                        NotaEF = db.Notas.FirstOrDefault(n => n.IdMatricula == m.IdMatricula) != null
                                ? db.Notas.FirstOrDefault(n => n.IdMatricula == m.IdMatricula).NotaEF
                                : (decimal?)null,
                        Promedio = db.Notas.FirstOrDefault(n => n.IdMatricula == m.IdMatricula) != null
                                ? db.Notas.FirstOrDefault(n => n.IdMatricula == m.IdMatricula).Promedio
                                : (decimal?)null,
                        Estado =db.Notas.FirstOrDefault(n => n.IdMatricula == m.IdMatricula) != null
                                ? db.Notas.FirstOrDefault(n => n.IdMatricula == m.IdMatricula).Estado
                                : "Pendiente"
                    }).ToList();
                var model = new RegistrarNotasViewModel
                {
                    IdAsignacion = idAsignacion,
                    NombreCurso = asignacion.Curso.NombreCurso,
                    Seccion = asignacion.Seccion.NombreSeccion,
                    Turno = asignacion.Turno.NombreTurno,
                    Estudiantes = estudiantes,
                    CodigoBusqueda = codigoBusqueda
                };

                return View(model);
            }
        }

        [HttpPost]
        public ActionResult RegistrarNotas(RegistrarNotasViewModel model)
        {
            try
            {
                if (model.Estudiantes == null || !model.Estudiantes.Any())
                {
                    ModelState.AddModelError("", "No hay estudiantes para registrar notas.");
                    return View(model);
                }

                // Validación de rangos
                foreach (var e in model.Estudiantes)
                {
                    if (e.NotaT1 < 0 || e.NotaT1 > 20)
                        ModelState.AddModelError("", $"Nota T1 inválida para {e.NombreCompleto}");
                    if (e.NotaT2 < 0 || e.NotaT2 > 20) 
                        ModelState.AddModelError("", $"Nota T2 inválida para {e.NombreCompleto}");
                    if (e.NotaEF < 0 || e.NotaEF > 20) 
                        ModelState.AddModelError("", $"Nota EF inválida para {e.NombreCompleto}");

                    if (e.NotaT1.HasValue &&
                    BitConverter.GetBytes(decimal.GetBits(e.NotaT1.Value)[3])[2] > 2)
                    if (e.NotaT2.HasValue &&
                    BitConverter.GetBytes(decimal.GetBits(e.NotaT2.Value)[3])[2] > 2)
                    if (e.NotaEF.HasValue &&
                    BitConverter.GetBytes(decimal.GetBits(e.NotaEF.Value)[3])[2] > 2)
                    if (e.Promedio.HasValue &&
                    BitConverter.GetBytes(decimal.GetBits(e.Promedio.Value)[3])[2] > 2)
                                {
                        ModelState.AddModelError("", "Las notas solo pueden tener hasta 2 decimales");
                    }
                }

                if (ModelState.IsValid)
                {
                    using (var repo = new NotasRepository())
                    {
                        if (repo.RegistrarNotas(model.Estudiantes))
                        {
                            TempData["MensajeNotas"] = "✅ Notas registradas correctamente.";
                            return RedirectToAction("CursosAsignados");
                        }
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error crítico: " + ex.Message);
                return View(model);
            }
        }
    }
        }
