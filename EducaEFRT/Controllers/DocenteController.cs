using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

           /* public ActionResult ListaEstudiantes(int idAsignacion)
            {
                if (Session["IdUsuario"] == null)
                    return RedirectToAction("Login", "Account");

                using (var repo = new EstudianteRepository())
                {
                    var model = repo.ObtenerListaEstudiantes(idAsignacion);

                    if (model == null)
                        return HttpNotFound();

                    return View(model);
                }
            }*/


            public ActionResult ListaEstudiantes(int idAsignacion)
            {
                if (Session["IdUsuario"] == null)
                    return RedirectToAction("Login", "Account");

                using (var repo = new AsistenciaRepository())
                {
                    var model = repo.ObtenerListaAsistencia(idAsignacion);

                    if (model == null)
                        return HttpNotFound();

                    return View(model);
                }
            }

        [HttpPost]
        public ActionResult ListaEstudiantes(ListaAsistenciaViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var repo = new AsistenciaRepository())
                    {
                        // Verificar que hay estudiantes para guardar
                        if (model.Estudiantes == null || model.Estudiantes.Count == 0)
                        {
                            ModelState.AddModelError("", "No hay estudiantes para registrar asistencia");
                            return View(model);
                        }

                        // Filtrar solo estudiantes con alguna opción seleccionada
                        var estudiantesConAsistencia = model.Estudiantes
                            .Where(e => e.Asistio || e.Tardanza || e.Falto)
                            .ToList();

                        if (estudiantesConAsistencia.Count == 0)
                        {
                            ModelState.AddModelError("", "Debe seleccionar al menos una opción de asistencia");
                            return View(model);
                        }

                        if (repo.RegistrarAsistencia(estudiantesConAsistencia))
                        {
                            TempData["Mensaje"] = "Asistencia registrada correctamente";
                            return RedirectToAction("CursosAsignados");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Loggear el error
                    System.Diagnostics.Debug.WriteLine("Error al guardar asistencia: " + ex.ToString());

                    // Mostrar mensaje de error más específico
                    ModelState.AddModelError("", "Ocurrió un error al guardar la asistencia. Detalles: " + ex.Message);
                }
            }
            else
            {
                // Agregar mensaje de error de validación del modelo
                ModelState.AddModelError("", "Hay errores en los datos enviados");
            }

            // Si llegamos aquí, hubo un error - recargar los datos necesarios
            using (var repo = new AsistenciaRepository())
            {
                var datosActuales = repo.ObtenerListaAsistencia(model.IdAsignacion);
                if (datosActuales != null)
                {
                    model.NombreCurso = datosActuales.NombreCurso;
                    model.Seccion = datosActuales.Seccion;
                    model.Turno = datosActuales.Turno;
                }
            }

            return View(model);
        }


    }
}
