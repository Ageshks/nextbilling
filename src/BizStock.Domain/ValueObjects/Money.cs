namespace BizStock.Domain.ValueObjects;

/// <summary>
/// Money value object with decimal-safe rounding used across all monetary calculations.
/// Indian invoicing convention: two decimal places, half-up rounding.
/// </summary>
public readonly record struct Money
{
    /// <summary>Monetary amount.</summary>
    public decimal Amount { get; }

    /// <summary>Creates a money value after canonical rounding.</summary>
    public Money(decimal amount)
    {
        Amount = MoneyRound(amount);
    }

    /// <summary>Rounds a decimal to 2 places, half away from zero (standard for INR invoicing).</summary>
    public static decimal MoneyRound(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);

    /// <summary>Rounds a decimal to the given scale, half away from zero.</summary>
    public static decimal Round(decimal value, int decimals = 2) =>
        Math.Round(value, decimals, MidpointRounding.AwayFromZero);

    public static Money operator +(Money a, Money b) => new(a.Amount + b.Amount);
    public static Money operator -(Money a, Money b) => new(a.Amount - b.Amount);
    public static Money operator *(Money a, decimal factor) => new(a.Amount * factor);

    /// <summary>Formats as Indian rupee currency.</summary>
    public override string ToString() => Amount.ToString("N2");
}
