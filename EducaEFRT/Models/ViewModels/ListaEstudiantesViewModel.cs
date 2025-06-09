using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EducaEFRT.Models.ViewModels
{
    public class EstudianteViewModel
    {
        public string CodigoEstudiante { get; set; }
        public string Apellidos { get; set; }
        public string Nombres { get; set; }
    }

    public class ListaEstudiantesViewModel
    {
        public string NombreCurso { get; set; }
        public string Seccion { get; set; }
        public string Turno { get; set; }
        public List<EstudianteViewModel> Estudiantes { get; set; }
    }
}