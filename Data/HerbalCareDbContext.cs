using Microsoft.EntityFrameworkCore;
using HerbalMedicalCare.Models;

namespace HerbalMedicalCare.Data
{
    public class HerbalCareDbContext : DbContext
    {
        public HerbalCareDbContext(DbContextOptions<HerbalCareDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Plant> Plants { get; set; }


        public DbSet<Disease> Diseases { get; set; }
        public DbSet<Remedy> Remedies { get; set; }
        public DbSet<Variant> Variants { get; set; }
        public DbSet<Precaution> Precautions { get; set; }
        public DbSet<Why> Whys { get; set; }
        public DbSet<RelatedDisease> RelatedDiseases { get; set; }

        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Note> Notes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔥 Disease PK
            modelBuilder.Entity<Disease>()
                .HasKey(d => d.Id);

            // 🔥 Relationships
            modelBuilder.Entity<Remedy>()
                .HasOne(r => r.Disease)
                .WithMany(d => d.Remedies)
                .HasForeignKey(r => r.DiseaseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Variant>()
                .HasOne(v => v.Disease)
                .WithMany(d => d.Variants)
                .HasForeignKey(v => v.DiseaseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}