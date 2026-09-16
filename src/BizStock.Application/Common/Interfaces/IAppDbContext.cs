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

namespace BizStock.Application.Common.Interfaces;

/// <summary>
/// EF Core database context abstraction. Owns all entity sets, audit timestamps
/// and save-time conventions (UTC normalization, business scoping).
/// </summary>
public interface IAppDbContext
{
    /// <summary>Users.</summary>
    DbSet<User> Users { get; }

    /// <summary>Roles.</summary>
    DbSet<Role> Roles { get; }

    /// <summary>Permissions.</summary>
    DbSet<Permission> Permissions { get; }

    /// <summary>User-role joins.</summary>
    DbSet<UserRole> UserRoles { get; }

    /// <summary>Role-permission joins.</summary>
    DbSet<RolePermission> RolePermissions { get; }

    /// <summary>Businesses.</summary>
    DbSet<Business> Businesses { get; }

    /// <summary>Business settings.</summary>
    DbSet<BusinessSettings> BusinessSettings { get; }

    /// <summary>Financial years.</summary>
    DbSet<FinancialYear> FinancialYears { get; }

    /// <summary>Key/value settings.</summary>
    DbSet<SettingEntry> SettingEntries { get; }

    /// <summary>Audit trail.</summary>
    DbSet<AuditLog> AuditLogs { get; }

    /// <summary>Backup history.</summary>
    DbSet<BackupLog> BackupLogs { get; }

    /// <summary>Categories.</summary>
    DbSet<Category> Categories { get; }

    /// <summary>Brands.</summary>
    DbSet<Brand> Brands { get; }

    /// <summary>Units.</summary>
    DbSet<Unit> Units { get; }

    /// <summary>Tax rates.</summary>
    DbSet<TaxRate> TaxRates { get; }

    /// <summary>Products.</summary>
    DbSet<Product> Products { get; }

    /// <summary>Customers.</summary>
    DbSet<Customer> Customers { get; }

    /// <summary>Customer ledger.</summary>
    DbSet<CustomerLedger> CustomerLedgers { get; }

    /// <summary>Suppliers.</summary>
    DbSet<Supplier> Suppliers { get; }

    /// <summary>Supplier ledger.</summary>
    DbSet<SupplierLedger> SupplierLedgers { get; }

    /// <summary>Stock balances.</summary>
    DbSet<StockSummary> StockSummaries { get; }

    /// <summary>Stock ledger.</summary>
    DbSet<StockMovement> StockMovements { get; }

    /// <summary>Stock adjustments.</summary>
    DbSet<StockAdjustment> StockAdjustments { get; }

    /// <summary>Adjustment lines.</summary>
    DbSet<StockAdjustmentItem> StockAdjustmentItems { get; }

    /// <summary>Sales.</summary>
    DbSet<Sale> Sales { get; }

    /// <summary>Sale lines.</summary>
    DbSet<SaleItem> SaleItems { get; }

    /// <summary>Sale payments.</summary>
    DbSet<SalePayment> SalePayments { get; }

    /// <summary>Sales returns.</summary>
    DbSet<SalesReturn> SalesReturns { get; }

    /// <summary>Return lines.</summary>
    DbSet<SalesReturnItem> SalesReturnItems { get; }

    /// <summary>Held bills.</summary>
    DbSet<HeldSale> HeldSales { get; }

    /// <summary>Purchases.</summary>
    DbSet<Purchase> Purchases { get; }

    /// <summary>Purchase lines.</summary>
    DbSet<PurchaseItem> PurchaseItems { get; }

    /// <summary>Purchase returns.</summary>
    DbSet<PurchaseReturn> PurchaseReturns { get; }

    /// <summary>Purchase return lines.</summary>
    DbSet<PurchaseReturnItem> PurchaseReturnItems { get; }

    /// <summary>Expense categories.</summary>
    DbSet<ExpenseCategory> ExpenseCategories { get; }

    /// <summary>Expenses.</summary>
    DbSet<Expense> Expenses { get; }

    /// <summary>Chart of accounts.</summary>
    DbSet<Account> Accounts { get; }

    /// <summary>Journals.</summary>
    DbSet<JournalEntry> JournalEntries { get; }

    /// <summary>Journal lines.</summary>
    DbSet<JournalLine> JournalLines { get; }

    /// <summary>Employees.</summary>
    DbSet<Employee> Employees { get; }

    /// <summary>Attendance.</summary>
    DbSet<Attendance> Attendances { get; }

    /// <summary>Leave requests.</summary>
    DbSet<LeaveRequest> LeaveRequests { get; }

    /// <summary>Payroll runs.</summary>
    DbSet<PayrollRun> PayrollRuns { get; }

    /// <summary>Payslips.</summary>
    DbSet<Payslip> Payslips { get; }

    /// <summary>Salary advances.</summary>
    DbSet<SalaryAdvance> SalaryAdvances { get; }

    /// <summary>Billing machines.</summary>
    DbSet<BillingMachine> BillingMachines { get; }

    /// <summary>Dashboard snapshots.</summary>
    DbSet<DashboardSnapshot> DashboardSnapshots { get; }

    /// <summary>Saves all tracked changes as one atomic transaction.</summary>
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

/// <summary>Unit of work wrapping <see cref="IAppDbContext.SaveChangesAsync"/> for explicit transaction control.</summary>
public interface IUnitOfWork
{
    /// <summary>Database context.</summary>
    IAppDbContext Context { get; }

    /// <summary>
    /// Executes the action inside a single database transaction.
    /// Nested calls join the existing transaction.
    /// </summary>
    Task<T> ExecuteInTransactionAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken ct = default);

    /// <summary>Transaction overload without a return value.</summary>
    Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken ct = default);
}
