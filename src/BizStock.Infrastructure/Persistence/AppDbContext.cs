using BizStock.Application.Common.Interfaces;
using BizStock.Domain.Auditing;
using BizStock.Domain.Billing;
using BizStock.Domain.Business;
using BizStock.Domain.Catalog;
using BizStock.Domain.Expenses;
using BizStock.Domain.Finance;
using BizStock.Domain.Identity;
using BizStock.Domain.Inventory;
using BizStock.Domain.Partners;
using BizStock.Domain.Purchases;
using BizStock.Domain.Reports;
using BizStock.Domain.Sales;
using BizStock.Domain.Settings;
using BizStock.Domain.Staff;
using Microsoft.EntityFrameworkCore;

namespace BizStock.Infrastructure.Persistence;

/// <summary>EF Core database context. Applies UTC timestamps and business scoping on save.</summary>
public partial class AppDbContext : DbContext, IAppDbContext
{
    /// <summary>Creates the context with the given options.</summary>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <inheritdoc />
    public DbSet<User> Users => Set<User>();
    /// <inheritdoc />
    public DbSet<Role> Roles => Set<Role>();
    /// <inheritdoc />
    public DbSet<Permission> Permissions => Set<Permission>();
    /// <inheritdoc />
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    /// <inheritdoc />
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    /// <inheritdoc />
    public DbSet<Business> Businesses => Set<Business>();
    /// <inheritdoc />
    public DbSet<BusinessSettings> BusinessSettings => Set<BusinessSettings>();
    /// <inheritdoc />
    public DbSet<FinancialYear> FinancialYears => Set<FinancialYear>();
    /// <inheritdoc />
    public DbSet<SettingEntry> SettingEntries => Set<SettingEntry>();
    /// <inheritdoc />
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    /// <inheritdoc />
    public DbSet<BackupLog> BackupLogs => Set<BackupLog>();
    /// <inheritdoc />
    public DbSet<Category> Categories => Set<Category>();
    /// <inheritdoc />
    public DbSet<Brand> Brands => Set<Brand>();
    /// <inheritdoc />
    public DbSet<Unit> Units => Set<Unit>();
    /// <inheritdoc />
    public DbSet<TaxRate> TaxRates => Set<TaxRate>();
    /// <inheritdoc />
    public DbSet<Product> Products => Set<Product>();
    /// <inheritdoc />
    public DbSet<Customer> Customers => Set<Customer>();
    /// <inheritdoc />
    public DbSet<CustomerLedger> CustomerLedgers => Set<CustomerLedger>();
    /// <inheritdoc />
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    /// <inheritdoc />
    public DbSet<SupplierLedger> SupplierLedgers => Set<SupplierLedger>();
    /// <inheritdoc />
    public DbSet<StockSummary> StockSummaries => Set<StockSummary>();
    /// <inheritdoc />
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    /// <inheritdoc />
    public DbSet<StockAdjustment> StockAdjustments => Set<StockAdjustment>();
    /// <inheritdoc />
    public DbSet<StockAdjustmentItem> StockAdjustmentItems => Set<StockAdjustmentItem>();
    /// <inheritdoc />
    public DbSet<Sale> Sales => Set<Sale>();
    /// <inheritdoc />
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    /// <inheritdoc />
    public DbSet<SalePayment> SalePayments => Set<SalePayment>();
    /// <inheritdoc />
    public DbSet<SalesReturn> SalesReturns => Set<SalesReturn>();
    /// <inheritdoc />
    public DbSet<SalesReturnItem> SalesReturnItems => Set<SalesReturnItem>();
    /// <inheritdoc />
    public DbSet<HeldSale> HeldSales => Set<HeldSale>();
    /// <inheritdoc />
    public DbSet<Purchase> Purchases => Set<Purchase>();
    /// <inheritdoc />
    public DbSet<PurchaseItem> PurchaseItems => Set<PurchaseItem>();
    /// <inheritdoc />
    public DbSet<PurchaseReturn> PurchaseReturns => Set<PurchaseReturn>();
    /// <inheritdoc />
    public DbSet<PurchaseReturnItem> PurchaseReturnItems => Set<PurchaseReturnItem>();
    /// <inheritdoc />
    public DbSet<ExpenseCategory> ExpenseCategories => Set<ExpenseCategory>();
    /// <inheritdoc />
    public DbSet<Expense> Expenses => Set<Expense>();
    /// <inheritdoc />
    public DbSet<Account> Accounts => Set<Account>();
    /// <inheritdoc />
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    /// <inheritdoc />
    public DbSet<JournalLine> JournalLines => Set<JournalLine>();
    /// <inheritdoc />
    public DbSet<Employee> Employees => Set<Employee>();
    /// <inheritdoc />
    public DbSet<Attendance> Attendances => Set<Attendance>();
    /// <inheritdoc />
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    /// <inheritdoc />
    public DbSet<PayrollRun> PayrollRuns => Set<PayrollRun>();
    /// <inheritdoc />
    public DbSet<Payslip> Payslips => Set<Payslip>();
    /// <inheritdoc />
    public DbSet<SalaryAdvance> SalaryAdvances => Set<SalaryAdvance>();
    /// <inheritdoc />
    public DbSet<BillingMachine> BillingMachines => Set<BillingMachine>();
    /// <inheritdoc />
    public DbSet<DashboardSnapshot> DashboardSnapshots => Set<DashboardSnapshot>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
