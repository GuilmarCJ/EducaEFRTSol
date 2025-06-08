using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EducaEFRT.Models.ViewModels
{
	public class RegistrarNotasViewModel
	{
        public int IdAsignacion { get; set; }
        public string NombreCurso { get; set; }
        public string Seccion { get; set; }
        public string Turno { get; set; }
        [Display(Name = "Buscar por código")]
        public string CodigoBusqueda { get; set; }
        public List<EstudianteNotasViewModel> Estudiantes { get; set; }
    }
}