using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EducaEFRT.Models.ViewModels;
using EducaEFRT.Models.DB;
using System.Data.Entity;


namespace EducaEFRT.Models.DB.Repositories
{
   /* public class EstudianteRepository : IDisposable
    {
        private EduControlDB db = new EduControlDB();

        public ListaEstudiantesViewModel ObtenerListaEstudiantes(int idAsignacion)
        {
            // Corrected property name to match the EduControlDB definition
            var asignacion = db.AsignacionesCurso
                .Include("Curso")
                .Include("Seccion")
                .Include("Turno")
                .FirstOrDefault(a => a.IdAsignacion == idAsignacion);

            if (asignacion == null)
                return null;

            var estudiantes = db.Matriculas
                .Where(m => m.IdAsignacion == idAsignacion)
                .Select(m => new EstudianteViewModel
                {
                    CodigoEstudiante = m.Estudiante.CodigoEstudiante, // Corrected property name
                    Apellidos = m.Estudiante.Apellidos,              // Corrected property name
                    Nombres = m.Estudiante.Nombres                   // Corrected property name
                })
                .OrderBy(e => e.Apellidos)
                .ToList();

            return new ListaEstudiantesViewModel
            {
                NombreCurso = asignacion.Curso.NombreCurso, // Corrected property name
                Seccion = asignacion.Seccion.NombreSeccion, // Corrected property name
                Turno = asignacion.Turno.NombreTurno,       // Corrected property name
                Estudiantes = estudiantes
            };
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }*/
}