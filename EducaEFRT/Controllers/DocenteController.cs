using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EducaEFRT.Models.DB.Repositories;

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
                ViewBag.NombreUsuario = Session["NombreUsuario"];
                return View(cursos);
            }
        }
    }
}
