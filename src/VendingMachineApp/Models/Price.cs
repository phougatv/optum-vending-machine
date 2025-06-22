namespace Optum.VendingMachineApp.Models;

public readonly record struct Price : IComparable<Price>
{
	public Decimal Value { get; }

	public Price(Decimal amount)
	{
		if (amount < 0)
			throw new ArgumentOutOfRangeException(nameof(amount), "Price cannot be negative.");

		Value = amount;
	}

	public override String ToString() => Value.ToString();
	public Int32 CompareTo(Price other) => Value.CompareTo(other.Value);

	#region Operator Overloads
	public static implicit operator Price(Decimal amount) => new Price(amount);
	public static implicit operator Decimal(Price price) => price.Value;
	public static Price operator +(Price left, Price right) => new Price(left.Value + right.Value);

	/// <summary>
	/// This operator allows subtracting one Price from another.
	/// </summary>
	/// <param name="left">The left</param>
	/// <param name="right">The right</param>
	/// <returns>The result of subtraction</returns>
	/// <exception cref="InvalidOperationException">Thrown when the left Price is less than the right Price.</exception>
	public static Price operator -(Price left, Price right)
	{
		if (left.Value < right.Value)
			throw new InvalidOperationException("Cannot subtract a larger price from a smaller one.");

		return new Price(left.Value - right.Value);
	}
	public static Boolean operator >(Price left, Price right) => left.CompareTo(right) > 0;
	public static Boolean operator <(Price left, Price right) => left.CompareTo(right) < 0;
	public static Boolean operator >=(Price left, Price right) => left.CompareTo(right) >= 0;
	public static Boolean operator <=(Price left, Price right) => left.CompareTo(right) <= 0;
	#endregion Operator Overloads
}
