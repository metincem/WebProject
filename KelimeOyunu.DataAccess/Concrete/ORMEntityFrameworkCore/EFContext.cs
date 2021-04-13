using KelimeOyunu.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace KelimeOyunu.DataAccess.Concrete.ORMEntityFrameworkCore
{
    public class EFContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=sql.poseidon.domainhizmetleri.com;Initial Catalog=metince1_kelimeoyunu;User ID=******;Password=******;");
        }

        public DbSet<Kelime> Kelimeler { get; set; }
        public DbSet<Oturum> Oturumlar { get; set; }
        public DbSet<Surec> Surecler { get; set; }
    }
}
