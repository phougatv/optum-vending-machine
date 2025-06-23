namespace Optum.VendingMachineApp.Services;

public sealed class ProductInitializationService
{
    private readonly List<ProductOptions> _productOptions;

    public ProductInitializationService(List<ProductOptions> productOptions)
    {
        _productOptions = productOptions;
    }

    public void InitializeInventory(IInventoryService inventoryService)
    {
        foreach (var option in _productOptions)
        {
            var product = Product.Create(option.Name, new Price(option.Price));
            inventoryService.AddProduct(product, option.InitialQuantity);
        }
    }
}
