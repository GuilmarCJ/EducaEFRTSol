using EducaEFRT.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EducaEFRT.Models.DB.Repositories
{
    public class NotasRepository : IDisposable
    {
        private EduControlDB db = new EduControlDB();

        public bool RegistrarNotas(List<EstudianteNotasViewModel> estudiantes)
        {
            try
            {
                foreach (var est in estudiantes)
                {
                    if (est.NotaT1.HasValue &&
                    (est.NotaT1.Value < 0 || est.NotaT1.Value > 20 ||
                     Decimal.Round(est.NotaT1.Value, 2) != est.NotaT1.Value))
                    {
                        throw new ArgumentException("Nota T1 inválida");
                    }
                    if (est.NotaT2.HasValue &&
                    (est.NotaT2.Value < 0 || est.NotaT2.Value > 20 ||
                     Decimal.Round(est.NotaT2.Value, 2) != est.NotaT2.Value))
                    {
                        throw new ArgumentException("Nota T2 inválida");
                    }
                    if (est.NotaEF.HasValue &&
                    (est.NotaEF.Value < 0 || est.NotaEF.Value > 20 ||
                     Decimal.Round(est.NotaEF.Value, 2) != est.NotaEF.Value))
                    {
                        throw new ArgumentException("Nota EF inválida");
                    }
                    var notaExistente = db.Notas.FirstOrDefault(n => n.IdMatricula == est.IdMatricula);

                    if (notaExistente == null)
                    {
                        // Crear nueva nota
                        var nuevaNota = new Notas
                        {
                            IdMatricula = est.IdMatricula,
                            NotaT1 = est.NotaT1,
                            NotaT2 = est.NotaT2,
                            NotaEF = est.NotaEF,
                            Promedio = null,
                            Estado = "Pendiente" // El trigger lo actualizará
                        };
                        db.Notas.Add(nuevaNota);
                    }
                    else
                    {
                        // Actualizar nota existente
                        notaExistente.NotaT1 = est.NotaT1;
                        notaExistente.NotaT2 = est.NotaT2;
                        notaExistente.NotaEF = est.NotaEF;
                    }
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al guardar notas", ex);
            }
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}