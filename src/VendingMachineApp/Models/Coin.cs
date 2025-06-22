namespace Optum.VendingMachineApp.Models;

public readonly struct Coin
{
	public Double WeightInGrams { get; }
	public Double DiameterInMillimeters { get; }

	private Coin(Double weight, Double diameter)
	{
		WeightInGrams = weight;
		DiameterInMillimeters = diameter;
	}

	public static Coin Create(Double weight, Double diameter)
	{
		if (weight <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(weight), "Weight must be greater than zero.");
		}

		if (diameter <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(diameter), "Diameter must be greater than zero.");
		}

		return new Coin(weight, diameter);
	}
}
