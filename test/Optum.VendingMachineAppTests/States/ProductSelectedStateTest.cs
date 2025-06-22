namespace Optum.VendingMachineApp.UnitTest.States;

public class ProductSelectedStateTest
{
	private readonly ICoinValidator _coinValidator;
	private readonly IInventoryService _inventoryService;
	private readonly IStateFactory _stateFactory;
	private readonly CoinValidationResult _validResultForQuarter;
	private readonly CoinValidationResult _validResultForDime;
	private readonly VendingMachine _machine;
	private readonly Coin _quarter;
	private readonly Coin _dime;

	public ProductSelectedStateTest()
	{
		_dime = new Coin(10, 10);
		_quarter = new Coin(25, 25); // this coin will be inserted during the NoCoinState
		_coinValidator = Substitute.For<ICoinValidator>();
		_validResultForQuarter = CoinValidationResult.CreateValidResult("Quarter", 0.25m);
		_validResultForDime = CoinValidationResult.CreateValidResult("Dime", 0.10m);
		_coinValidator.Validate(_quarter).Returns(_validResultForQuarter);
		_coinValidator.Validate(_dime).Returns(_validResultForDime);

		_inventoryService = Substitute.For<IInventoryService>();
		_inventoryService.IsAvailable(Arg.Any<Product>()).Returns(true); // Assume all products are available for simplicity

		_stateFactory = Substitute.For<IStateFactory>();
		_stateFactory.CreateNoCoinState(Arg.Any<VendingMachine>()).Returns(m => new NoCoinState(m.Arg<VendingMachine>(), _coinValidator));
		_machine = new VendingMachine(_stateFactory);
		_stateFactory.CreateHasCoinState(_machine).Returns(new HasCoinState(_machine, _coinValidator, _inventoryService));
	}

	[Fact]
	public void OnEnter_ShouldSetMessage_WhenBalanceIsInsufficient()
	{
		// Arrange
		var coke = Product.Create("Coke", 1.00m);
		var productSelectedState = new ProductSelectedState(_machine, _coinValidator, _inventoryService);
		_stateFactory.CreateProductSelectedState(_machine).Returns(productSelectedState);
		_inventoryService.IsAvailable(coke).Returns(true);
		_machine.InsertCoin(_quarter);  // this simulates the transition from NoCoinState to HasCoinState
		_machine.SelectProduct(coke);   // this simulates the transition from HasCoinState to ProductSelectedState

		// Act & Assert
		_stateFactory.Received(1).CreateNoCoinState(_machine);
		_stateFactory.Received(1).CreateHasCoinState(_machine);
		_stateFactory.Received(1).CreateProductSelectedState(_machine);
		_stateFactory.Received(0).CreateProductDispensedState(_machine);

		Assert.Collection(_machine.MessageHistory,
			message => Assert.Equal("INSERT COIN", message),
			message => Assert.Equal("BALANCE: $0.25", message),
			message => Assert.Equal("SELECT PRODUCT", message),
			message => Assert.Equal("Coke - $1.00", message),
			message => Assert.Equal("BALANCE: $0.25", message),
			message => Assert.Equal("INSERT COIN", message));
		Assert.Equal("INSERT COIN", _machine.CurrentMessage);
		Assert.Equal(0.25m, _machine.CurrentBalance);
		Assert.Equal(coke.Price, _machine.SelectedProduct.Price);
		Assert.Equal(coke.Name, _machine.SelectedProduct.Name);
	}

	[Fact]
	public void InsertCoin_ShouldUpdateBalanceAndMessage_WhenBalanceIsInsufficientAfterValidCoinInsertion()
	{
		//Arrange
		var coke = Product.Create("Coke", 1.00m);
		var productSelectedState = new ProductSelectedState(_machine, _coinValidator, _inventoryService);
		_stateFactory.CreateProductSelectedState(_machine).Returns(productSelectedState);
		_inventoryService.IsAvailable(coke).Returns(true);
		_machine.InsertCoin(_quarter);      // this simulates the transition from NoCoinState to HasCoinState
		_machine.SelectProduct(coke);       // this simulates the transition from HasCoinState to ProductSelectedState

		//Act
		productSelectedState.InsertCoin(_quarter);

		// Assert
		_stateFactory.Received(1).CreateNoCoinState(_machine);
		_stateFactory.Received(1).CreateHasCoinState(_machine);
		_stateFactory.Received(1).CreateProductSelectedState(_machine);
		_stateFactory.Received(0).CreateProductDispensedState(_machine);

		Assert.Collection(_machine.MessageHistory,
			message => Assert.Equal("INSERT COIN", message),
			message => Assert.Equal("BALANCE: $0.25", message),
			message => Assert.Equal("SELECT PRODUCT", message),
			message => Assert.Equal("Coke - $1.00", message),
			message => Assert.Equal("BALANCE: $0.25", message),
			message => Assert.Equal("INSERT COIN", message),
			message => Assert.Equal("BALANCE: $0.50", message),
			message => Assert.Equal("INSERT COIN", message));
		Assert.Equal("INSERT COIN", _machine.CurrentMessage);
		Assert.Equal(0.50m, _machine.CurrentBalance);
		Assert.Equal(1, _machine.SelectedProduct.Price);
		Assert.Equal("Coke", _machine.SelectedProduct.Name);
	}

	[Fact]
	public void InsertCoin_ShouldTransitionToProductDispensedState_WhenBalanceIsSufficientAfterValidCoinInsertion()
	{
		//Arrange
		var coke = Product.Create("Coke", 1.00m);
		var productSelectedState = new ProductSelectedState(_machine, _coinValidator, _inventoryService);
		_stateFactory.CreateProductSelectedState(_machine).Returns(productSelectedState);
		_inventoryService.IsAvailable(coke).Returns(true);
		_machine.InsertCoin(_quarter);      // this simulates the transition from NoCoinState to HasCoinState
		_machine.SelectProduct(coke);       // this simulates the transition from HasCoinState to ProductSelectedState		

		//Act
		productSelectedState.InsertCoin(_quarter);  //balance becomes 0.50
		productSelectedState.InsertCoin(_quarter);  //balance becomes 0.75
		productSelectedState.InsertCoin(_quarter);  //balance becomes 1.00

		// Assert
		_stateFactory.Received(1).CreateNoCoinState(_machine);
		_stateFactory.Received(1).CreateHasCoinState(_machine);
		_stateFactory.Received(1).CreateProductSelectedState(_machine);
		_stateFactory.Received(1).CreateProductDispensedState(_machine);

		Assert.Collection(_machine.MessageHistory,
			message => Assert.Equal("INSERT COIN", message),
			message => Assert.Equal("BALANCE: $0.25", message),
			message => Assert.Equal("SELECT PRODUCT", message),
			message => Assert.Equal("Coke - $1.00", message),
			message => Assert.Equal("BALANCE: $0.25", message),
			message => Assert.Equal("INSERT COIN", message),
			message => Assert.Equal("BALANCE: $0.50", message),
			message => Assert.Equal("INSERT COIN", message),
			message => Assert.Equal("BALANCE: $0.75", message),
			message => Assert.Equal("INSERT COIN", message),
			message => Assert.Equal("BALANCE: $1.00", message),
			message => Assert.Equal("DISPENSING Coke...", message),
			message => Assert.Equal("Coke DISPENSED", message),
			message => Assert.Equal("THANK YOU", message));
		Assert.Equal("THANK YOU", _machine.CurrentMessage);
		Assert.Equal(0m, _machine.CurrentBalance);
		Assert.Equal(1.00m, (Decimal)_machine.SelectedProduct.Price);
		Assert.Equal("Coke", _machine.SelectedProduct.Name);
	}

	[Fact]
	public void ReturnCoins_ShouldReturnAllCoinsAndTransitionToNoCoinState()
	{
		//Arrange
		var productSelectedState = new ProductSelectedState(_machine, _coinValidator, _inventoryService);
		_stateFactory.CreateProductSelectedState(_machine).Returns(productSelectedState);
		_machine.InsertCoin(_quarter);  // this simulates the transition from NoCoinState to HasCoinState
		_machine.InsertCoin(_quarter);  // this updates current balance and keeps the HasCoinState

		var coke = Product.Create("Coke", 1.00m);
		_machine.SelectProduct(coke); // this simulates the transition from HasCoinState to ProductSelectedState

		//Act
		productSelectedState.ReturnCoins();

		//Assert
		_stateFactory.Received(2).CreateNoCoinState(_machine);
		_stateFactory.Received(1).CreateHasCoinState(_machine);
		_stateFactory.Received(1).CreateProductSelectedState(_machine);
		_stateFactory.Received(0).CreateProductDispensedState(_machine);

		Assert.Collection(_machine.MessageHistory,
			message => Assert.Equal("INSERT COIN", message));
		Assert.Equal("INSERT COIN", _machine.CurrentMessage);
		Assert.Equal(0m, _machine.CurrentBalance);
		Assert.Equal(0, _machine.SelectedProduct.Price);
		Assert.Null(_machine.SelectedProduct.Name);
	}

	[Fact]
	public void SelectProduct_ShouldTransitionToProductDispensedState_WhenBalanceIsSufficientForNewlySelectedProduct()
	{
		//Arrange
		var productSelectedState = new ProductSelectedState(_machine, _coinValidator, _inventoryService);
		_stateFactory.CreateProductSelectedState(_machine).Returns(productSelectedState);

		var coke = Product.Create("Coke", 1.00m);
		_machine.InsertCoin(_quarter);  // this simulates the transition from NoCoinState to HasCoinState
		_machine.SelectProduct(coke);   // this simulates the transition from HasCoinState to ProductSelectedState
		_machine.InsertCoin(_dime);     // this updates the current balance and keeps the ProductSelectState
		_machine.InsertCoin(_quarter);  // this updates the current balance and keeps the ProductSelectedState

		//Act
		var chips = Product.Create("Chips", 0.50m);
		_machine.SelectProduct(chips);

		//Assert
		_stateFactory.Received(1).CreateNoCoinState(_machine);
		_stateFactory.Received(1).CreateHasCoinState(_machine);
		_stateFactory.Received(1).CreateProductSelectedState(_machine);
		_stateFactory.Received(1).CreateProductDispensedState(_machine);

		Assert.Collection(_machine.MessageHistory,
			message => Assert.Equal("INSERT COIN", message),
			message => Assert.Equal("BALANCE: $0.25", message),
			message => Assert.Equal("SELECT PRODUCT", message),
			message => Assert.Equal("Coke - $1.00", message),
			message => Assert.Equal("BALANCE: $0.25", message),
			message => Assert.Equal("INSERT COIN", message),
			message => Assert.Equal("BALANCE: $0.35", message),
			message => Assert.Equal("INSERT COIN", message),
			message => Assert.Equal("BALANCE: $0.60", message),
			message => Assert.Equal("INSERT COIN", message),
			message => Assert.Equal("Chips - $0.50", message),
			message => Assert.Equal("DISPENSING Chips...", message),
			message => Assert.Equal("Chips DISPENSED", message),
			message => Assert.Equal("THANK YOU", message));
		Assert.Equal("THANK YOU", _machine.CurrentMessage);
		Assert.Equal(0.10m, _machine.CurrentBalance);
		Assert.Equal(0.50m, (Decimal)_machine.SelectedProduct.Price);
		Assert.Equal("Chips", _machine.SelectedProduct.Name);
	}

	[Fact]
	public void SelectProduct_ShouldNotTransitionToProductDispensedState_WhenBalanceIsInsufficientForNewlySelectedProduct()
	{
		//Arrange
		var productSelectedState = new ProductSelectedState(_machine, _coinValidator, _inventoryService);
		_stateFactory.CreateProductSelectedState(_machine).Returns(productSelectedState);

		var coke = Product.Create("Coke", 1.00m);
		_machine.InsertCoin(_quarter);  // this simulates the transition from NoCoinState to HasCoinState
		_machine.SelectProduct(coke);   // this simulates the transition from HasCoinState to ProductSelectedState
		_machine.InsertCoin(_quarter);  // this updates the current balance and keeps the ProductSelectedState

		var candy = Product.Create("Candy", 0.65m);

		//Act
		productSelectedState.SelectProduct(candy);

		//Assert
		_stateFactory.Received(1).CreateNoCoinState(_machine);
		_stateFactory.Received(1).CreateHasCoinState(_machine);
		_stateFactory.Received(1).CreateProductSelectedState(_machine);
		_stateFactory.Received(0).CreateProductDispensedState(_machine);

		Assert.Collection(_machine.MessageHistory,
			message => Assert.Equal("INSERT COIN", message),
			message => Assert.Equal("BALANCE: $0.25", message),
			message => Assert.Equal("SELECT PRODUCT", message),
			message => Assert.Equal("Coke - $1.00", message),
			message => Assert.Equal("BALANCE: $0.25", message),
			message => Assert.Equal("INSERT COIN", message),
			message => Assert.Equal("BALANCE: $0.50", message),
			message => Assert.Equal("INSERT COIN", message),
			message => Assert.Equal("Candy - $0.65", message),
			message => Assert.Equal("BALANCE: $0.50", message),
			message => Assert.Equal("INSERT COIN", message));
		Assert.Equal("INSERT COIN", _machine.CurrentMessage);
		Assert.Equal(0.50m, _machine.CurrentBalance);
		Assert.Equal(candy.Price, _machine.SelectedProduct.Price);
		Assert.Equal(candy.Name, _machine.SelectedProduct.Name);
	}

	[Fact]
	public void SelectProduct_ShouldDoNothing_WhenSameProductIsSelectedAgain()
	{
		//Arrange
		var productSelectedState = new ProductSelectedState(_machine, _coinValidator, _inventoryService);
		_stateFactory.CreateProductSelectedState(_machine).Returns(productSelectedState);

		var coke = Product.Create("Coke", 1.00m);
		_machine.InsertCoin(_quarter);  // this simulates the transition from NoCoinState to HasCoinState
		_machine.SelectProduct(coke);   // this simulates the transition from HasCoinState to ProductSelectedState

		//Act
		productSelectedState.SelectProduct(coke);

		//Assert
		_stateFactory.Received(1).CreateNoCoinState(_machine);
		_stateFactory.Received(1).CreateHasCoinState(_machine);
		_stateFactory.Received(1).CreateProductSelectedState(_machine);
		_stateFactory.Received(0).CreateProductDispensedState(_machine);

		Assert.Collection(_machine.MessageHistory,
			message => Assert.Equal("INSERT COIN", message),
			message => Assert.Equal("BALANCE: $0.25", message),
			message => Assert.Equal("SELECT PRODUCT", message),
			message => Assert.Equal("Coke - $1.00", message),
			message => Assert.Equal("BALANCE: $0.25", message),
			message => Assert.Equal("INSERT COIN", message),
			message => Assert.Equal("Coke ALREADY SELECTED", message),
			message => Assert.Equal("INSERT COIN", message));
		Assert.Equal("INSERT COIN", _machine.CurrentMessage);
		Assert.Equal(0.25m, _machine.CurrentBalance);
		Assert.Equal(coke.Price, _machine.SelectedProduct.Price);
		Assert.Equal(coke.Name, _machine.SelectedProduct.Name);
	}

	[Fact]
	public void SelectProduct_ShouldNotTransitionToProductDispensedState_WhenNewlySelectedProductIsOutOfStock()
	{
		//Arrange
		var productSelectedState = new ProductSelectedState(_machine, _coinValidator, _inventoryService);
		_stateFactory.CreateProductSelectedState(_machine).Returns(productSelectedState);

		var coke = Product.Create("Coke", 1.00m);
		_machine.InsertCoin(_quarter);  // this simulates the transition from NoCoinState to HasCoinState
		_machine.SelectProduct(coke);   // this simulates the transition from HasCoinState to ProductSelectedState

		var candy = Product.Create("Candy", 0.65m);
		_inventoryService.IsAvailable(candy).Returns(false); // this simulates out of stock

		//Act
		productSelectedState.SelectProduct(candy);

		//Assert
		_stateFactory.Received(1).CreateNoCoinState(_machine);
		_stateFactory.Received(1).CreateHasCoinState(_machine);
		_stateFactory.Received(1).CreateProductSelectedState(_machine);
		_stateFactory.Received(0).CreateProductDispensedState(_machine);

		Assert.Collection(_machine.MessageHistory,
			message => Assert.Equal("INSERT COIN", message),
			message => Assert.Equal("BALANCE: $0.25", message),
			message => Assert.Equal("SELECT PRODUCT", message),
			message => Assert.Equal("Coke - $1.00", message),
			message => Assert.Equal("BALANCE: $0.25", message),
			message => Assert.Equal("INSERT COIN", message),
			message => Assert.Equal("Candy OUT OF STOCK", message),
			message => Assert.Equal("SELECT PRODUCT", message));
		Assert.Equal("SELECT PRODUCT", _machine.CurrentMessage);
		Assert.Equal(0.25m, _machine.CurrentBalance);
		Assert.Equal(coke.Price, _machine.SelectedProduct.Price);
		Assert.Equal(coke.Name, _machine.SelectedProduct.Name);
	}
}
