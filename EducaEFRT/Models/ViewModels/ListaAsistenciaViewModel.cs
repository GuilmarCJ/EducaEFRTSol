using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EducaEFRT.Models.ViewModels
{
    public class EstudianteAsistenciaViewModel
    {
        public int IdMatricula { get; set; }
        public string CodigoEstudiante { get; set; }
        public string Apellidos { get; set; }
        public string Nombres { get; set; }
        public bool Asistio { get; set; }
        public bool Tardanza { get; set; }
        public bool Falto { get; set; }
    }

    public class ListaAsistenciaViewModel
    {
        public int IdAsignacion { get; set; }
        public string NombreCurso { get; set; }
        public string Seccion { get; set; }
        public string Turno { get; set; }
        public List<EstudianteAsistenciaViewModel> Estudiantes { get; set; }
    }
}