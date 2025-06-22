namespace Optum.VendingMachineApp.UnitTest.Services;

public class ProductInventoryServiceTest
{
	[Fact]
	public void AddProduct_ShouldAddProduct_WhenValidInput()
	{
		//Arrange
		var service = new ProductInventoryService();
		var coke = Product.Create("Coke", 1.00m);
		var quantity = 1;

		//Assert before Act
		Assert.False(service.IsAvailable(coke));

		//Act
		service.AddProduct(coke, quantity);

		//Assert
		Assert.True(service.IsAvailable(coke));
	}

	[Fact]
	public void AddProduct_ShouldThrowException_WhenQuantityIsNegative()
	{
		//Arrange
		var service = new ProductInventoryService();
		var coke = Product.Create("Coke", 1.00m);
		var quantity = -1;

		//Act & Assert
		Assert.Throws<ArgumentOutOfRangeException>(() => service.AddProduct(coke, quantity));
	}

	[Fact]
	public void DeductProduct_ShouldDeductProduct_WhenAvailable()
	{
		//Arrange
		var service = new ProductInventoryService();
		var coke = Product.Create("Coke", 1.00m);
		service.AddProduct(coke, 1);

		//Assert before Act
		Assert.True(service.IsAvailable(coke));

		//Act
		service.DeductProduct(coke);

		//Assert
		Assert.False(service.IsAvailable(coke));
	}

	[Fact]
	public void DeductProduct_ShouldThrowException_WhenProductIsNotAvailable()
	{
		//Arrange
		var service = new ProductInventoryService();
		var coke = Product.Create("Coke", 1.00m);

		//Act & Assert
		Assert.Throws<InvalidOperationException>(() => service.DeductProduct(coke));
	}

	[Fact]
	public void IsAvailable_ShouldReturnTrue_WhenProductIsAvailable()
	{
		//Arrange
		var service = new ProductInventoryService();
		var coke = Product.Create("Coke", 1.00m);
		service.AddProduct(coke, 1);

		//Act
		var isAvailable = service.IsAvailable(coke);

		//Assert
		Assert.True(isAvailable);
	}

	[Fact]
	public void IsAvailable_ShouldReturnFalse_WhenProductIsNotAvailable()
	{
		//Arrange
		var service = new ProductInventoryService();
		var coke = Product.Create("Coke", 1.00m);
		service.AddProduct(coke, 0); // Adding with zero quantity

		//Act
		var isAvailable = service.IsAvailable(coke);

		//Assert
		Assert.False(isAvailable);
	}
}
