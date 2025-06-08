using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EducaEFRT.Models.ViewModels
{
    public class NotaEstudianteViewModel
    {
        public int IdMatricula { get; set; }
        public string CodigoEstudiante { get; set; }
        public string NombreCompleto { get; set; }
        public string NombreCurso { get; set; }  
        public string Seccion { get; set; }  
        public decimal? NotaT1 { get; set; }
        public decimal? NotaT2 { get; set; }
        public decimal? NotaEF { get; set; }
        public decimal? Promedio => (NotaT1 * 0.25m) + (NotaT2 * 0.25m) + (NotaEF * 0.50m);
    }
}