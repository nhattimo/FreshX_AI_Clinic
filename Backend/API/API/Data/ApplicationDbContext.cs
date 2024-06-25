using API.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<AccountEmp> AccountEmps { get; set; }
        public DbSet<Employees> Employees { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductItem> ProductItems { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1 Employees : 1 AccountEmp
            modelBuilder.Entity<Employees>()
             .HasOne(e => e.AccountEmp)
             .WithOne(a => a.Employee)
             .HasForeignKey<Employees>(e => e.AccountEmpId);

            // 1 Employees : 1 Role
            modelBuilder.Entity<Employees>()
            .HasOne(e => e.Role)
            .WithOne(r => r.Employee)
            .HasForeignKey<Employees>(e => e.RoleId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
