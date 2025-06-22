namespace Optum.VendingMachineApp.Configurations;

public class CoinSpecificationOptions
{
	public String CoinType { get; set; }
	public Decimal MonetaryValue { get; set; }
	public Double WeightInGrams { get; set; }
	public Double DiameterInMillimeters { get; set; }
	public Double Tolerance { get; set; } = 0.001;
}

public class ProductOptions
{
	public string Name { get; set; }
	public decimal Price { get; set; }
	public int InitialQuantity { get; set; }
}

