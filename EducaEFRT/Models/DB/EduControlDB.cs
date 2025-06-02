using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EducaEFRT.Models.DB
{
    public class EduControlDB : DbContext
    {
        public EduControlDB() : base("name=EduControlDB")
        {
            // Configuración adicional para evitar problemas
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

        public virtual DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Elimina esta línea si estás usando atributos en la clase
            // modelBuilder.Entity<Usuario>().ToTable("Usuario");

            base.OnModelCreating(modelBuilder);
        }
    }
}