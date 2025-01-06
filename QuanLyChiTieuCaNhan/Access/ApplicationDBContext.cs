using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuanLyChiTieuCaNhan.Models;
using System.Reflection.Emit;

namespace QuanLyChiTieuCaNhan.Access
{
    public class ApplicationDBContext :IdentityDbContext<User>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) 
        { }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ExpenseTransaction> ExpenseTransactions{ get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Category>().HasQueryFilter(x => !x.IsDelete);
            builder.Entity<ExpenseTransaction>().HasQueryFilter(x => !x.IsDelete);
            builder.Entity<Budget>().HasQueryFilter(x => !x.IsDeleted);

            builder.Entity<ExpenseTransaction>()
                .HasOne(et => et.User)
                .WithMany(u => u.ExpenseTransactions)
                .HasForeignKey(et => et.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Category>()
                .HasOne(c => c.User)
                .WithMany(u => u.Categories)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Budget>()
                .HasOne(b => b.User)
                .WithMany(u => u.Budgets)
                .HasForeignKey(b=> b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<User>().ToTable("Users");
               
        }
    }
}
