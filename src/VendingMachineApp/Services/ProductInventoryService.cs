namespace Optum.VendingMachineApp.Services;

public sealed class ProductInventoryService : IInventoryService
{
	private readonly Dictionary<Product, Int32> _inventory = [];

	public void AddProduct(Product product, Int32 quantity)
	{
		if (quantity < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be non-negative.");
		}

		if (_inventory.TryGetValue(product, out var existingQuantity))
		{
			_inventory[product] = existingQuantity + quantity;
			return;
		}

		_inventory[product] = quantity;
	}
	public void DeductProduct(Product product)
	{
		if (!_inventory.TryGetValue(product, out var quantity) || quantity <= 0)
		{
			throw new InvalidOperationException("Product is not available or out of stock.");
		}

		_inventory[product] = --quantity;
	}
	public Boolean IsAvailable(Product product)
	{
		return
			_inventory.TryGetValue(product, out var quantity) &&
			quantity > 0;
	}
}
