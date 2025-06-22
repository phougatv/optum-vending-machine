namespace Optum.VendingMachineApp.Services;

public interface IInventoryService
{
	void AddProduct(Product product, Int32 quantity);
	void DeductProduct(Product product);
	Boolean IsAvailable(Product product);
}
