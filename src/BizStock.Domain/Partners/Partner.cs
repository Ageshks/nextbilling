using BizStock.Domain.Common;

namespace BizStock.Domain.Partners;

/// <summary>Customer with credit tracking and running ledger balance.</summary>
public class Customer : BaseEntity, IActivatable
{
    /// <summary>Unique customer code, e.g. C0001.</summary>
    public string CustomerCode { get; set; } = default!;

    /// <summary>Customer name (person or business).</summary>
    public string Name { get; set; } = default!;

    /// <summary>Mobile number.</summary>
    public string? Phone { get; set; }

    /// <summary>Alternate contact.</summary>
    public string? AlternatePhone { get; set; }

    /// <summary>Email.</summary>
    public string? Email { get; set; }

    /// <summary>GSTIN for B2B invoices.</summary>
    public string? GSTIN { get; set; }

    /// <summary>Street address.</summary>
    public string? Address { get; set; }

    /// <summary>City.</summary>
    public string? City { get; set; }

    /// <summary>State name.</summary>
    public string? State { get; set; }

    /// <summary>State code; drives intra/inter-state GST.</summary>
    public string? StateCode { get; set; }

    /// <summary>PIN code.</summary>
    public string? Pincode { get; set; }

    /// <summary>Positive = customer owes us; negative = advance/credit note balance.</summary>
    public decimal OutstandingBalance { get; set; }

    /// <summary>Maximum credit allowed before warning.</summary>
    public decimal CreditLimit { get; set; }

    /// <summary>Agreed credit days.</summary>
    public int CreditDays { get; set; }

    /// <summary>Whether the customer is active.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Optional notes.</summary>
    public string? Notes { get; set; }
}

/// <summary>Supplier with payables tracking.</summary>
public class Supplier : BaseEntity, IActivatable
{
    /// <summary>Unique supplier code, e.g. S0001.</summary>
    public string SupplierCode { get; set; } = default!;

    /// <summary>Supplier name.</summary>
    public string Name { get; set; } = default!;

    /// <summary>Contact person.</summary>
    public string? ContactPerson { get; set; }

    /// <summary>Phone.</summary>
    public string? Phone { get; set; }

    /// <summary>Email.</summary>
    public string? Email { get; set; }

    /// <summary>GSTIN.</summary>
    public string? GSTIN { get; set; }

    /// <summary>Street address.</summary>
    public string? Address { get; set; }

    /// <summary>City.</summary>
    public string? City { get; set; }

    /// <summary>State name.</summary>
    public string? State { get; set; }

    /// <summary>State code.</summary>
    public string? StateCode { get; set; }

    /// <summary>PIN code.</summary>
    public string? Pincode { get; set; }

    /// <summary>Positive = we owe supplier; negative = advance paid.</summary>
    public decimal OutstandingBalance { get; set; }

    /// <summary>Agreed credit days.</summary>
    public int CreditDays { get; set; }

    /// <summary>Whether the supplier is active.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Optional notes.</summary>
    public string? Notes { get; set; }
}
