namespace BizStock.Domain.Common;

/// <summary>Base exception for all domain rule violations.</summary>
public class DomainException : Exception
{
    /// <summary>Creates a domain exception with a user-facing message.</summary>
    public DomainException(string message) : base(message)
    {
    }

    /// <summary>Creates a domain exception with a message and inner exception.</summary>
    public DomainException(string message, Exception inner) : base(message, inner)
    {
    }
}

/// <summary>Thrown when an operation would take stock below zero while negative stock is disabled.</summary>
public sealed class InsufficientStockException : DomainException
{
    /// <summary>Creates the exception with details of the requested and available quantities.</summary>
    public InsufficientStockException(string productName, decimal requested, decimal available)
        : base($"Insufficient stock for '{productName}'. Requested {requested}, available {available}.")
    {
        ProductName = productName;
        Requested = requested;
        Available = available;
    }

    /// <summary>Name of the product involved.</summary>
    public string ProductName { get; }

    /// <summary>Quantity requested.</summary>
    public decimal Requested { get; }

    /// <summary>Quantity available.</summary>
    public decimal Available { get; }
}

/// <summary>Thrown when a journal entry's debits do not equal its credits.</summary>
public sealed class UnbalancedJournalException : DomainException
{
    /// <summary>Creates the exception with debit/credit totals.</summary>
    public UnbalancedJournalException(decimal debit, decimal credit)
        : base($"Journal entry is not balanced. Debit {debit:0.00} != Credit {credit:0.00}.")
    {
        Debit = debit;
        Credit = credit;
    }

    /// <summary>Total debit.</summary>
    public decimal Debit { get; }

    /// <summary>Total credit.</summary>
    public decimal Credit { get; }
}

/// <summary>Thrown when a unique value (code, barcode, number…) already exists.</summary>
public sealed class DuplicateEntityException : DomainException
{
    /// <summary>Creates the exception naming the field and value.</summary>
    public DuplicateEntityException(string field, string value)
        : base($"A record with {field} '{value}' already exists.")
    {
        Field = field;
        Value = value;
    }

    /// <summary>Field that must be unique.</summary>
    public string Field { get; }

    /// <summary>Duplicated value.</summary>
    public string Value { get; }
}

/// <summary>Thrown for any other business rule violation.</summary>
public sealed class BusinessRuleViolationException : DomainException
{
    /// <summary>Creates the exception with the violated rule description.</summary>
    public BusinessRuleViolationException(string message) : base(message)
    {
    }
}
