using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using WCAPP.Models.Database;

namespace WCAPP
{
    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ERP> ERPs { get; set; }
        public DbSet<Procedure> Procedures { get; set; }
        public DbSet<Seam> Seams { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<AuthorityR> Authorities { get; set; }
        public DbSet<Process> Processes { get; set; }
        public DbSet<SeamParam1> SeamParam1s { get; set; }
        public DbSet<SeamParam2> SeamParam2s { get; set; }
        public DbSet<ReportFile> ReportFiles { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Approve> Approves { get; set; }

        public DbSet<DispatchMessage> DispatchMessages { get; set; }

        public DbSet<SynchroTable> SynchroTables { get; set; }

        public DbSet<TestModel> TestModels { get; set; }

        public Context() : base("name=DbContext")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuthorityR>().HasRequired(x => x.User).WithMany(x => x.AuthorityRs).WillCascadeOnDelete(true);
            base.OnModelCreating(modelBuilder);
        }
    }
}