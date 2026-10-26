using Microsoft.EntityFrameworkCore;
using Abp.EntityFrameworkCore;
using ButceYonetimi.Entities;

namespace ButceYonetimi.EntityFrameworkCore
{
    public class ButceYonetimiDbContext : AbpDbContext
    {
        public DbSet<BudgetPeriod> BudgetPeriods { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<BudgetCategory> BudgetCategories { get; set; }
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        public DbSet<BudgetRequest> BudgetRequests { get; set; }
        public DbSet<BudgetRequestItem> BudgetRequestItems { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<AppRole> AppRoles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<ApprovalRecord> ApprovalRecords { get; set; }
        public DbSet<StatusChangeLog> StatusChangeLogs { get; set; }


        public ButceYonetimiDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Kayit basina tek bekleyen onay: es zamanli iki gonderim iki "Pending" acip onayciya
            // bir adimi atlatabiliyordu. Filtreli tekil indeks MySQL'de yok — orada uygulama kontrolu yeterli.
            var provider = Database.ProviderName ?? "";
            var pendingFilter = provider.Contains("Npgsql") ? "\"Status\" = 'Pending'" : provider.Contains("SqlServer") ? "[Status] = 'Pending'" : null;
            if (pendingFilter != null)
                modelBuilder.Entity<ApprovalRecord>()
                    .HasIndex(a => new { a.EntityType, a.EntityId })
                    .IsUnique()
                    .HasFilter(pendingFilter);

            // BudgetPeriod 1:N Budget
            modelBuilder.Entity<Budget>()
                .HasOne(x => x.BudgetPeriod)
                .WithMany(x => x.Budgets)
                .HasForeignKey(x => x.BudgetPeriodId)
                .OnDelete(DeleteBehavior.Restrict);

            // Department 1:N Budget
            modelBuilder.Entity<Budget>()
                .HasOne(x => x.Department)
                .WithMany(x => x.Budgets)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // BudgetCategory 1:N Budget
            modelBuilder.Entity<Budget>()
                .HasOne(x => x.BudgetCategory)
                .WithMany(x => x.Budgets)
                .HasForeignKey(x => x.BudgetCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Budget 1:N BudgetRequest
            modelBuilder.Entity<BudgetRequest>()
                .HasOne(x => x.Budget)
                .WithMany(x => x.BudgetRequests)
                .HasForeignKey(x => x.BudgetId)
                .OnDelete(DeleteBehavior.Restrict);

            // BudgetRequest 1:N BudgetRequestItem
            modelBuilder.Entity<BudgetRequestItem>()
                .HasOne(x => x.BudgetRequest)
                .WithMany(x => x.BudgetRequestItems)
                .HasForeignKey(x => x.BudgetRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            // BudgetCategory 1:N BudgetRequestItem
            modelBuilder.Entity<BudgetRequestItem>()
                .HasOne(x => x.BudgetCategory)
                .WithMany(x => x.BudgetRequestItems)
                .HasForeignKey(x => x.BudgetCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Budget 1:N Expense
            modelBuilder.Entity<Expense>()
                .HasOne(x => x.Budget)
                .WithMany(x => x.Expenses)
                .HasForeignKey(x => x.BudgetId)
                .OnDelete(DeleteBehavior.Restrict);

            // BudgetCategory 1:N Expense
            modelBuilder.Entity<Expense>()
                .HasOne(x => x.BudgetCategory)
                .WithMany(x => x.Expenses)
                .HasForeignKey(x => x.BudgetCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Department 1:N BudgetRequest
            modelBuilder.Entity<BudgetRequest>()
                .HasOne(x => x.Department)
                .WithMany(x => x.BudgetRequests)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);


            // RBAC: AppUser N:N AppRole via UserRole junction
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserRole>()
                .HasIndex(ur => new { ur.UserId, ur.RoleId })
                .IsUnique();

            // RolePermission: AppRole 1:N RolePermission
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<RolePermission>()
                .HasIndex(rp => new { rp.RoleId, rp.PermissionName })
                .IsUnique();

            // AppRole.Name unique
            modelBuilder.Entity<AppRole>()
                .HasIndex(r => r.Name)
                .IsUnique();

        }
    }
}
