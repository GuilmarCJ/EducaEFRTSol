using EducaEFRT.Models.DB;
using EducaEFRT.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;

namespace EducaEFRT.Models.DB.Repositories
{
    public class AsistenciaRepository : IDisposable
    {
        private EduControlDB db = new EduControlDB();

        public ListaAsistenciaViewModel ObtenerListaAsistencia(int idAsignacion)
        {
            var asignacion = db.AsignacionesCurso
                .Include("Curso")
                .Include("Seccion")
                .Include("Turno")
                .FirstOrDefault(a => a.IdAsignacion == idAsignacion);

            if (asignacion == null)
                return null;

            var estudiantes = db.Matriculas
                .Where(m => m.IdAsignacion == idAsignacion)
                .Select(m => new EstudianteAsistenciaViewModel
                {
                    IdMatricula = m.IdMatricula,
                    CodigoEstudiante = m.Estudiante.CodigoEstudiante,
                    Apellidos = m.Estudiante.Apellidos,
                    Nombres = m.Estudiante.Nombres,
                    Asistio = false,
                    Tardanza = false,
                    Falto = false
                })
                .OrderBy(e => e.Apellidos)
                .ToList();

            return new ListaAsistenciaViewModel
            {
                IdAsignacion = idAsignacion,
                NombreCurso = asignacion.Curso.NombreCurso,
                Seccion = asignacion.Seccion.NombreSeccion,
                Turno = asignacion.Turno.NombreTurno,
                Estudiantes = estudiantes
            };

        }

        public bool RegistrarAsistencia(List<EstudianteAsistenciaViewModel> estudiantes)
        {
            try
            {
                var fechaActual = DateTime.Today;

                // Verificar si ya existe asistencia registrada para hoy
                var idsMatricula = estudiantes.Select(e => e.IdMatricula).ToList();
                var existeAsistencia = db.AsistenciasEstudiante
                    .Any(a => a.Fecha == fechaActual && idsMatricula.Contains(a.IdMatricula));

                if (existeAsistencia)
                {
                    throw new ApplicationException("Ya se registró asistencia para algunos estudiantes hoy");
                }

                foreach (var est in estudiantes)
{
    string estado = "F"; // Por defecto
    if (est.Asistio) estado = "A";
    if (est.Tardanza) estado = "T";

    var asistencia = new AsistenciaEstudiante
    {
        IdMatricula = est.IdMatricula,
        Fecha = fechaActual,
        Hora = DateTime.Now.TimeOfDay,
        EstadoAsistencia = estado
    };

    db.AsistenciasEstudiante.Add(asistencia);
}


                db.SaveChanges();
                return true;
            }
            catch (DbEntityValidationException ex)
            {
                // Loggear errores de validación de entidad
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

                var fullErrorMessage = string.Join("; ", errorMessages);
                throw new ApplicationException("Error de validación: " + fullErrorMessage);
            }
            catch (Exception ex)
            {
                // Loggear el error completo
                System.Diagnostics.Debug.WriteLine("Error en RegistrarAsistencia: " + ex.ToString());
                throw;
            }
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
