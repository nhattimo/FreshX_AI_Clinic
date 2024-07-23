using Microsoft.EntityFrameworkCore;
using API.Models;
using API.Server.Models;

namespace API
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<AccountCus> AccountCus { get; set; }
        public DbSet<AccountEmp> AccountEmp { get; set; }
        public DbSet<Advice> Advice { get; set; }
        public DbSet<Appointment> Appointment { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Diagnosis> Diagnosis { get; set; }
        public DbSet<DiagnosisSymptom> DiagnosisSymptom { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Exercise> Exercise { get; set; }
        public DbSet<Insurance> Insurance { get; set; }
        public DbSet<Meal> Meal { get; set; }
        public DbSet<MedicalRecord> MedicalRecord { get; set; }
        public DbSet<Note> Note { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Symptom> Symptom { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // AccountCus configuration
            modelBuilder.Entity<AccountCus>()
                .HasOne(ac => ac.Customer)
                .WithMany(c => c.AccountCus)
                .HasForeignKey(ac => ac.CustomerId);

            // AccountEmp configuration
            modelBuilder.Entity<AccountEmp>()
                .HasOne(ae => ae.Employee)
                .WithMany(e => e.AccountEmp)
                .HasForeignKey(ae => ae.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict); // Tránh xóa nhân viên khi xóa AccountEmp

            modelBuilder.Entity<AccountEmp>()
                .HasOne(ae => ae.Role)
                .WithMany(r => r.AccountEmp)
                .HasForeignKey(ae => ae.RoleId)
                .OnDelete(DeleteBehavior.Restrict); // Tránh xóa vai trò khi xóa AccountEmp

            // Advice configuration
            modelBuilder.Entity<Advice>()
                .HasOne(a => a.Diagnosis)
                .WithMany(d => d.Advices)
                .HasForeignKey(a => a.DiagnosisId);

            // Appointment configuration
            modelBuilder.Entity<Appointment>()
                .HasOne(ap => ap.Customer)
                .WithMany(c => c.Appointments)
                .HasForeignKey(ap => ap.CustomerId);
            modelBuilder.Entity<Appointment>()
                .HasOne(ap => ap.Employee)
                .WithMany(e => e.Appointments)
                .HasForeignKey(ap => ap.EmployeeId);

            // DiagnosisSymptom configuration
            modelBuilder.Entity<DiagnosisSymptom>()
                .HasKey(ds => new { ds.DiagnosisId, ds.SymptomId });
            modelBuilder.Entity<DiagnosisSymptom>()
                .HasOne(ds => ds.Diagnosis)
                .WithMany(d => d.DiagnosisSymptoms)
                .HasForeignKey(ds => ds.DiagnosisId);
            modelBuilder.Entity<DiagnosisSymptom>()
                .HasOne(ds => ds.Symptom)
                .WithMany(s => s.DiagnosisSymptoms)
                .HasForeignKey(ds => ds.SymptomId);

            // Exercise configuration
            modelBuilder.Entity<Exercise>()
                .HasOne(e => e.Diagnosis)
                .WithMany(d => d.Exercises)
                .HasForeignKey(e => e.DiagnosisId);

            // Meal configuration
            modelBuilder.Entity<Meal>()
                .HasOne(m => m.Diagnosis)
                .WithMany(d => d.Meals)
                .HasForeignKey(m => m.DiagnosisId);

            // MedicalRecord configuration
            modelBuilder.Entity<MedicalRecord>()
                .HasOne(mr => mr.Customer)
                .WithMany(c => c.MedicalRecords)
                .HasForeignKey(mr => mr.CustomerId);

            // Note configuration
            modelBuilder.Entity<Note>()
                .HasOne(n => n.MedicalRecord)
                .WithMany(mr => mr.Notes)
                .HasForeignKey(n => n.MedicalRecordId);

            // Role configuration
            modelBuilder.Entity<Role>()
                .HasMany(r => r.AccountEmp)
                .WithOne(ae => ae.Role)
                .HasForeignKey(ae => ae.RoleId)
                .OnDelete(DeleteBehavior.Restrict); // Tránh xóa vai trò khi xóa AccountEmp

            // Symptom configuration
            modelBuilder.Entity<Symptom>()
                .HasMany(s => s.DiagnosisSymptoms)
                .WithOne(ds => ds.Symptom)
                .HasForeignKey(ds => ds.SymptomId);
        }
    }
}
