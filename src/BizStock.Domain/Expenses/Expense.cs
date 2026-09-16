using BizStock.Domain.Common;
using BizStock.Domain.Catalog;
using BizStock.Domain.Identity;

namespace BizStock.Domain.Expenses;

/// <summary>Expense category for reporting.</summary>
public class ExpenseCategory : BaseEntity, IActivatable
{
    /// <summary>Unique category name.</summary>
    public string Name { get; set; } = default!;

    /// <summary>Whether active.</summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>Business expense record.</summary>
public class Expense : BaseEntity, IBusinessScoped
{
    /// <summary>Owning business identifier.</summary>
    public Guid? BusinessId { get; set; }

    /// <summary>Unique expense number, e.g. EXP-000001.</summary>
    public string ExpenseNumber { get; set; } = default!;

    /// <summary>Expense date (UTC).</summary>
    public DateTime DateUtc { get; set; }

    /// <summary>Category id.</summary>
    public Guid? ExpenseCategoryId { get; set; }

    /// <summary>Category name snapshot.</summary>
    public string CategoryName { get; set; } = default!;

    /// <summary>Payee.</summary>
    public string? PaidTo { get; set; }

    /// <summary>Amount.</summary>
    public decimal Amount { get; set; }

    /// <summary>GST input credit eligible portion.</summary>
    public decimal TaxAmount { get; set; }

    /// <summary>Payment method.</summary>
    public PaymentMethod PaymentMethod { get; set; }

    /// <summary>Reference (bill no, voucher no).</summary>
    public string? Reference { get; set; }

    /// <summary>Description.</summary>
    public string? Description { get; set; }

    /// <summary>Receipt/attachment path.</summary>
    public string? AttachmentPath { get; set; }

    /// <summary>Category navigation.</summary>
    public ExpenseCategory? Category { get; set; }
}
