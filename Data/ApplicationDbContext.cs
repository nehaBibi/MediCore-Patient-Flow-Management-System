using Microsoft.EntityFrameworkCore;
using MediCorePatientFlow.Models;

namespace MediCorePatientFlow.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Admission> Admissions { get; set; }
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Patient>().HasKey(p => p.PatientID);
            modelBuilder.Entity<Doctor>().HasKey(d => d.DoctorID);
            modelBuilder.Entity<Admission>().HasKey(a => a.AdmissionID);

            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Admissions)
                .WithOne(a => a.Patient!)
                .HasForeignKey(a => a.PatientID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Doctor>()
                .HasMany(d => d.Admissions)
                .WithOne(a => a.Doctor!)
                .HasForeignKey(a => a.DoctorID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
