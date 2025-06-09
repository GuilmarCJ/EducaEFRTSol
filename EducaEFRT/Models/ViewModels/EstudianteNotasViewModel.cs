using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EducaEFRT.Models.ViewModels
{
	public class EstudianteNotasViewModel
	{
        public int IdMatricula { get; set; }
        public string CodigoEstudiante { get; set; }
        public string NombreCompleto { get; set; }
        [Range(0, 20, ErrorMessage = "La nota debe estar entre 0 y 20")]
        [RegularExpression(@"^\d{1,2}(\.\d{1,2})?$", ErrorMessage = "Formato inválido (ej: 12.50)")]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public decimal? NotaT1 { get; set; }
        [Range(0, 20, ErrorMessage = "La nota debe estar entre 0 y 20")]
        [RegularExpression(@"^\d{1,2}(\.\d{1,2})?$", ErrorMessage = "Formato inválido (ej: 12.50)")]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public decimal? NotaT2 { get; set; }
        [Range(0, 20, ErrorMessage = "La nota debe estar entre 0 y 20")]
        [RegularExpression(@"^\d{1,2}(\.\d{1,2})?$", ErrorMessage = "Formato inválido (ej: 12.50)")]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public decimal? NotaEF { get; set; }
        public decimal? Promedio { get; set; }
        public string Estado { get; set; }
    }
}