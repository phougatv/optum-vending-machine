namespace Optum.VendingMachineApp.UnitTest.Factories;

public class StateFactoryTests
{
	[Fact]
	public void CreateHasCoinState_ShouldReturnHasCoinState()
	{
		// Arrange
		var coinValidator = Substitute.For<ICoinValidator>();
		var inventoryService = Substitute.For<IInventoryService>();
		var factory = new StateFactory(coinValidator, inventoryService);
		var machine = new VendingMachine(factory);

		// Act
		var state = factory.CreateHasCoinState(machine);

		// Assert
		Assert.IsType<HasCoinState>(state);
	}

	[Fact]
	public void CreateNoCoinState_ShouldReturnNoCoinState()
	{
		// Arrange
		var coinValidator = Substitute.For<ICoinValidator>();
		var inventoryService = Substitute.For<IInventoryService>();
		var factory = new StateFactory(coinValidator, inventoryService);
		var machine = new VendingMachine(factory);

		// Act
		var state = factory.CreateNoCoinState(machine);

		// Assert
		Assert.IsType<NoCoinState>(state);
	}

	[Fact]
	public void CreateProductDispensedState_ShouldReturnProductDispensedState()
	{
		// Arrange
		var coinValidator = Substitute.For<ICoinValidator>();
		var inventoryService = Substitute.For<IInventoryService>();
		var factory = new StateFactory(coinValidator, inventoryService);
		var machine = new VendingMachine(factory);

		// Act
		var state = factory.CreateProductDispensedState(machine);

		// Assert
		Assert.IsType<ProductDispensedState>(state);
	}

	[Fact]
	public void CreateProductSelectedState_ShouldReturnProductSelectedState()
	{
		// Arrange
		var coinValidator = Substitute.For<ICoinValidator>();
		var inventoryService = Substitute.For<IInventoryService>();
		var factory = new StateFactory(coinValidator, inventoryService);
		var machine = new VendingMachine(factory);

		// Act
		var state = factory.CreateProductSelectedState(machine);

		// Assert
		Assert.IsType<ProductSelectedState>(state);
	}
}
