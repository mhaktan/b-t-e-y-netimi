using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ButceYonetimi.EntityFrameworkCore;
using ButceYonetimi.EntityFrameworkCore.Seed;
using ButceYonetimi.Entities;

namespace ButceYonetimi.Web.Host
{
    /// <summary>
    /// Background service that runs migration + seed once at startup
    /// without blocking the HTTP pipeline.
    /// </summary>
    public class MigrationHostedService : IHostedService
    {
        private readonly IConfiguration _config;

        public MigrationHostedService(IConfiguration config)
        {
            _config = config;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var connStr = _config.GetConnectionString("Default") ?? "";
            if (string.IsNullOrEmpty(connStr)) return;

            // Run in background to avoid blocking host startup (prevents EF tooling timeout)
            _ = Task.Run(async () =>
            {
                // Small delay to ensure host is fully started before DB operations
                await Task.Delay(1000, cancellationToken);
                try
                {
                    var optionsBuilder = new DbContextOptionsBuilder();
                    optionsBuilder.UseNpgsql(connStr);

                    using (var db = new ButceYonetimiDbContext(optionsBuilder.Options))
                    {
                        // EnsureCreated bos veritabaninda en guncel semayi kurar ve true doner.
                        // Dolu veritabaninda hicbir sey yapmaz — o durumda bekleyen migration'lar
                        // SchemaMigrations.Apply icinde sirayla calistirilir.
                        var freshlyCreated = db.Database.EnsureCreated();
                        Console.WriteLine("[Migration] Migration tanimi yok — sema EnsureCreated ile yonetiliyor.");
                        Console.WriteLine("[Migration] Database is up to date.");

                        // Seed sample data — wrapped in its own try so a failure here doesn't block RBAC seed below.
                        try
                        {
                    if (!db.BudgetPeriods.Any())
                    {
                        db.BudgetPeriods.AddRange(
                    new BudgetPeriod { Id = 1, Name = "Bütçe Dönemi 1", Year = 42, StartDate = DateTime.Today.AddDays(-3), EndDate = DateTime.Today.AddDays(-3), Description = "Yıllık plan kapsamında hazırlandı.", IsActive = true },
                    new BudgetPeriod { Id = 2, Name = "Bütçe Dönemi 2", Year = 17, StartDate = DateTime.Today.AddDays(-24), EndDate = DateTime.Today.AddDays(-24), Description = "Departman talebi doğrultusunda güncellendi.", IsActive = false }
                        );
                    }
                    if (!db.Departments.Any())
                    {
                        db.Departments.AddRange(
                    new Department { Id = 3, Name = "Departman 1", Code = "KOD-001", Description = "Yıllık plan kapsamında hazırlandı." },
                    new Department { Id = 4, Name = "Departman 2", Code = "KOD-002", Description = "Departman talebi doğrultusunda güncellendi." }
                        );
                    }
                    if (!db.BudgetCategories.Any())
                    {
                        db.BudgetCategories.AddRange(
                    new BudgetCategory { Id = 5, Name = "Bütçe Kategorisi 1", Code = "KOD-001", Description = "Yıllık plan kapsamında hazırlandı." },
                    new BudgetCategory { Id = 6, Name = "Bütçe Kategorisi 2", Code = "KOD-002", Description = "Departman talebi doğrultusunda güncellendi." }
                        );
                    }
                    if (!db.ExchangeRates.Any())
                    {
                        db.ExchangeRates.AddRange(
                    new ExchangeRate { Id = 7, Name = "Döviz Kuru 1", FromCurrency = (ExchangeRateFromCurrency)0, ToCurrency = (ExchangeRateToCurrency)0, Rate = 12500.00m, EffectiveDate = DateTime.Today.AddDays(-3) },
                    new ExchangeRate { Id = 8, Name = "Döviz Kuru 2", FromCurrency = (ExchangeRateFromCurrency)1, ToCurrency = (ExchangeRateToCurrency)1, Rate = 48250.00m, EffectiveDate = DateTime.Today.AddDays(-24) }
                        );
                    }
                    if (!db.BudgetRequests.Any())
                    {
                        db.BudgetRequests.AddRange(
                    new BudgetRequest { Id = 11, Title = "Pazarlama bütçesi 2026", RequestNo = "KOD-001", Description = "Yıllık plan kapsamında hazırlandı.", Justification = "Örnek 1", TotalAmount = 12500.00m, Currency = (BudgetRequestCurrency)0, TotalAmountTRY = 12500.00m, RequestDate = DateTime.Today.AddDays(-3), Status = (BudgetRequestStatus)0, RevisionNote = "Yıllık plan kapsamında hazırlandı.", FinanceApprovalThreshold = 12500.00m, ManagerApproverId = 1000L, FinanceApproverRole = "Örnek 1", BudgetId = 9, DepartmentId = 3 },
                    new BudgetRequest { Id = 12, Title = "Yazılım lisans yenileme", RequestNo = "KOD-002", Description = "Departman talebi doğrultusunda güncellendi.", Justification = "Örnek 2", TotalAmount = 48250.00m, Currency = (BudgetRequestCurrency)1, TotalAmountTRY = 48250.00m, RequestDate = DateTime.Today.AddDays(-24), Status = (BudgetRequestStatus)3, RevisionNote = "Departman talebi doğrultusunda güncellendi.", FinanceApprovalThreshold = 48250.00m, ManagerApproverId = 2000L, FinanceApproverRole = "Örnek 2", BudgetId = 10, DepartmentId = 4 }
                        );
                    }
                    if (!db.BudgetRequestItems.Any())
                    {
                        db.BudgetRequestItems.AddRange(
                    new BudgetRequestItem { Id = 13, Description = "Yıllık plan kapsamında hazırlandı.", Amount = 12500.00m, Quantity = 12500.00m, UnitPrice = 12500.00m, PlannedDate = DateTime.Today.AddDays(-3), Notes = "Yıllık plan kapsamında hazırlandı.", BudgetRequestId = 11, BudgetCategoryId = 5 },
                    new BudgetRequestItem { Id = 14, Description = "Departman talebi doğrultusunda güncellendi.", Amount = 48250.00m, Quantity = 48250.00m, UnitPrice = 48250.00m, PlannedDate = DateTime.Today.AddDays(-24), Notes = "Departman talebi doğrultusunda güncellendi.", BudgetRequestId = 12, BudgetCategoryId = 6 }
                        );
                    }
                    if (!db.Budgets.Any())
                    {
                        db.Budgets.AddRange(
                    new Budget { Id = 9, Title = "Pazarlama bütçesi 2026", AllocatedAmount = 12500.00m, Currency = (BudgetCurrency)0, AllocatedAmountTRY = 12500.00m, Description = "Yıllık plan kapsamında hazırlandı.", IsAutoCreated = true, Status = (BudgetStatus)0, RevisionNote = "Yıllık plan kapsamında hazırlandı.", FinanceDirectorId = 1000L, BudgetPeriodId = 1, DepartmentId = 3, BudgetCategoryId = 5 },
                    new Budget { Id = 10, Title = "Yazılım lisans yenileme", AllocatedAmount = 48250.00m, Currency = (BudgetCurrency)1, AllocatedAmountTRY = 48250.00m, Description = "Departman talebi doğrultusunda güncellendi.", IsAutoCreated = false, Status = (BudgetStatus)2, RevisionNote = "Departman talebi doğrultusunda güncellendi.", FinanceDirectorId = 2000L, BudgetPeriodId = 2, DepartmentId = 4, BudgetCategoryId = 6 }
                        );
                    }
                    if (!db.Expenses.Any())
                    {
                        db.Expenses.AddRange(
                    new Expense { Id = 15, Description = "Yıllık plan kapsamında hazırlandı.", Amount = 12500.00m, Currency = (ExpenseCurrency)0, AmountTRY = 12500.00m, ExpenseDate = DateTime.Today.AddDays(-3), ReceiptNo = "KOD-001", Notes = "Yıllık plan kapsamında hazırlandı.", Status = (ExpenseStatus)0, RevisionNote = "Yıllık plan kapsamında hazırlandı.", ExpenseApproverId = 1000L, BudgetId = 9, BudgetCategoryId = 5 },
                    new Expense { Id = 16, Description = "Departman talebi doğrultusunda güncellendi.", Amount = 48250.00m, Currency = (ExpenseCurrency)1, AmountTRY = 48250.00m, ExpenseDate = DateTime.Today.AddDays(-24), ReceiptNo = "KOD-002", Notes = "Departman talebi doğrultusunda güncellendi.", Status = (ExpenseStatus)2, RevisionNote = "Departman talebi doğrultusunda güncellendi.", ExpenseApproverId = 2000L, BudgetId = 10, BudgetCategoryId = 6 }
                        );
                    }
                            db.SaveChanges();
                            Console.WriteLine("[Seed] Sample data created.");
                        }
                        catch (Exception sampleEx)
                        {
                            Console.WriteLine($"[Seed] Sample data skipped: {sampleEx.GetType().Name}: {sampleEx.Message}");
                            // Carry on — RBAC seed must still run so admin/123qwe is usable.
                        }
                        // Sync identity sequences to MAX(Id). Seeded rows carry explicit Ids which do NOT
                        // advance Postgres identity sequences → nextval collides with a seed row and the
                        // first few inserts fail with a duplicate-key 500. Runs every startup; idempotent.
                        try
                        {
                            foreach (var et in db.Model.GetEntityTypes())
                            {
                                var pk = et.FindPrimaryKey();
                                var table = et.GetTableName();
                                if (pk == null || pk.Properties.Count != 1 || table == null) continue;
                                var pkType = Nullable.GetUnderlyingType(pk.Properties[0].ClrType) ?? pk.Properties[0].ClrType;
                                if (pkType != typeof(int) && pkType != typeof(long)) continue;
                                var col = pk.Properties[0].GetColumnName();
                                try
                                {
                                    db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"" + table + "\"', '" + col + "'), (SELECT COALESCE(MAX(\"" + col + "\"), 0) FROM \"" + table + "\") + 1, false);");
                                }
                                catch (Exception seqTableEx)
                                {
                                    Console.WriteLine($"[Seed] Sequence sync skipped for {table}: {seqTableEx.Message}");
                                }
                            }
                            Console.WriteLine("[Seed] Identity sequences synced.");
                        }
                        catch (Exception seqEx)
                        {
                            Console.WriteLine($"[Seed] Sequence sync skipped: {seqEx.GetType().Name}: {seqEx.Message}");
                        }
                    }
                    // RBAC seed (Admin/User roles + permissions + admin user) runs through ABP DI
                    // so PermissionRegistry can be injected. SeedHelper is idempotent.
                    SeedHelper.SeedHostDb(Abp.Dependency.IocManager.Instance);
                    Console.WriteLine("[Seed] RBAC seed complete (Admin role + admin user).");
                }
                catch (Exception ex)
                {
                    // Full diagnostic — surface the real cause so silent seed failures are debuggable.
                    Console.WriteLine($"[Migration] FAILED: {ex.GetType().Name}: {ex.Message}");
                    if (ex.InnerException != null)
                        Console.WriteLine($"[Migration] InnerException: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
                    Console.WriteLine("[Migration] StackTrace:");
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine("[Migration] App continues without migration — admin user will not exist.");
                }
            }, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    public class Program
    {
        // Runtime entry: WebHost is required because ABP Startup returns IServiceProvider.
        public static void Main(string[] args)
        {
            // Npgsql 7+ requires UTC DateTimes — enable legacy behavior for ABP compatibility
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build()
                .Run();
        }

        // Design-time entry for EF Core tools (dotnet ef migrations).
        // Without this, EF tools wait 5 minutes for IHost build (resolver default timeout)
        // and then SIGTERM any running dotnet process — killing live dev servers.
        // We expose a minimal IHost that EF tools resolve in milliseconds; the actual
        // DbContext is built by IDesignTimeDbContextFactory in the EntityFrameworkCore project.
        public static IHostBuilder CreateHostBuilder(string[] args)
            => Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args);
    }
}
