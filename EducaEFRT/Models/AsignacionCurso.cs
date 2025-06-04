using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EducaEFRT.Models;

[Table("AsignacionCurso")]
public class AsignacionCurso
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_asignacion")]
    public int IdAsignacion { get; set; }

    [Required]
    [Column("id_docente")]
    public int IdDocente { get; set; }

    [Required]
    [Column("id_curso")]
    public int IdCurso { get; set; }

    [Required]
    [Column("id_seccion")]
    public int IdSeccion { get; set; }

    [Required]
    [Column("id_turno")]
    public int IdTurno { get; set; }
}
