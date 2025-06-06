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



    }
}
