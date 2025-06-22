namespace Optum.VendingMachineApp.Configurations;

public class CoinSpecificationOptions
{
	public const String SectionName = "CoinSpecifications";

	public String CoinType { get; set; }
	public Decimal MonetaryValue { get; set; }
	public Double WeightInGrams { get; set; }
	public Double DiameterInMillimeters { get; set; }
	public Double Tolerance { get; set; } = 0.001;
}