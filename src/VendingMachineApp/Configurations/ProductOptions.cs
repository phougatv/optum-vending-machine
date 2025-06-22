namespace Optum.VendingMachineApp.Configurations;

public class ProductOptions
{
	public const String SectionName = "Products";

	public String Name { get; set; }
	public Decimal Price { get; set; }
	public Int32 InitialQuantity { get; set; }
}

