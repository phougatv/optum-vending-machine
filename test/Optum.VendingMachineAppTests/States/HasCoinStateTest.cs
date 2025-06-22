namespace Optum.VendingMachineApp.UnitTest.States;

public class HasCoinStateTest
{
	private readonly ICoinValidator _coinValidator;
	private readonly IInventoryService _inventoryService;
	private readonly IStateFactory _stateFactory;
	private readonly VendingMachine _machine;
	private readonly Coin _initialCoin;
	private readonly CoinValidationResult _initialCoinValidationResult;

	public HasCoinStateTest()
	{
		_coinValidator = Substitute.For<ICoinValidator>();
		_initialCoin = Coin.Create(10, 10); // this coin will be inserted during the NoCoinState
		_initialCoinValidationResult = CoinValidationResult.CreateValidResult("Quarter", 0.25m);
		_coinValidator.Validate(_initialCoin).Returns(_initialCoinValidationResult);

		_inventoryService = Substitute.For<IInventoryService>();
		_stateFactory = Substitute.For<IStateFactory>();
		_stateFactory.CreateNoCoinState(Arg.Any<VendingMachine>()).Returns(m => new NoCoinState(m.Arg<VendingMachine>(), _coinValidator));
		_machine = new VendingMachine(_stateFactory);
	}

	[Fact]
	public void InitialState_ShouldBeHasCoinState()
	{
		//Arrange
		var hasCoinState = new HasCoinState(_machine, _coinValidator, _inventoryService);
		_stateFactory.CreateHasCoinState(_machine).Returns(hasCoinState);
		_machine.InsertCoin(_initialCoin);	//this simulates the transition from NoCoinState to HasCoinState

		//Act & Assert
		_stateFactory.Received(1).CreateNoCoinState(_machine);
		_stateFactory.Received(0).CreateProductSelectedState(_machine);
		_stateFactory.Received(0).CreateProductSelectedState(_machine);

		Assert.Collection(_machine.MessageHistory,
			message => Assert.Equal("INSERT COIN", message),
			message => Assert.Equal("BALANCE: $0.25", message),
			message => Assert.Equal("SELECT PRODUCT", message));
		Assert.Equal("SELECT PRODUCT", _machine.CurrentMessage);
		Assert.Equal(0.25m, _machine.CurrentBalance);
		Assert.Equal(0, _machine.SelectedProduct.Price);
		Assert.Null(_machine.SelectedProduct.Name);
	}

	[Fact]
	public void ReturnCoins_ShouldReturnCoins()
	{
		//Arrange
		var hasCoinState = new HasCoinState(_machine, _coinValidator, _inventoryService);
		_stateFactory.CreateHasCoinState(_machine).Returns(hasCoinState);
		_machine.InsertCoin(_initialCoin); // this simulates the transition from NoCoinState to HasCoinState

		//Act
		hasCoinState.ReturnCoins();

		//Assert
		_stateFactory.Received(2).CreateNoCoinState(_machine);
		_stateFactory.Received(0).CreateProductSelectedState(_machine);
		_stateFactory.Received(0).CreateProductDispensedState(_machine);

		Assert.Collection(_machine.MessageHistory, message => Assert.Equal("INSERT COIN", message));
		Assert.Equal("INSERT COIN", _machine.CurrentMessage);
		Assert.Equal(0, _machine.CurrentBalance);
		Assert.Equal(0, _machine.SelectedProduct.Price);
		Assert.Null(_machine.SelectedProduct.Name);
	}

	[Fact]
	public void InsertCoin_ShouldIndicate_WhenInvalidCoinIsInserted()
	{
		//Arrange
		var invalidCoin = Coin.Create(100, 100);
		var invalidResult = CoinValidationResult.CreateInvalidResult();
		var hasCoinState = new HasCoinState(_machine, _coinValidator, _inventoryService);
		_stateFactory.CreateHasCoinState(_machine).Returns(hasCoinState);
		_coinValidator.Validate(invalidCoin).Returns(invalidResult);
		_machine.InsertCoin(_initialCoin); // this simulates the transition from NoCoinState to HasCoinState

		//Act
		hasCoinState.InsertCoin(invalidCoin);

		//Assert
		_stateFactory.Received(1).CreateNoCoinState(_machine);
		_stateFactory.Received(0).CreateProductSelectedState(_machine);
		_stateFactory.Received(0).CreateProductDispensedState(_machine);

		Assert.Collection(_machine.MessageHistory,
			message => Assert.Equal("INSERT COIN", message),
			message => Assert.Equal("BALANCE: $0.25", message),
			message => Assert.Equal("SELECT PRODUCT", message),
			message => Assert.Equal("INVALID COIN", message),
			message => Assert.Equal("COIN(S) RETURNED", message),
			message => Assert.Equal("INSERT COIN", message));
		Assert.Equal("INSERT COIN", _machine.CurrentMessage);
		Assert.Equal(0.25m, _machine.CurrentBalance);
		Assert.Equal(0, _machine.SelectedProduct.Price);
		Assert.Null(_machine.SelectedProduct.Name);
	}

	[Fact]
	public void InsertCoin_ShouldIndicate_WhenValidCoinIsInserted()
	{
		//Arrange
		var validCoin = Coin.Create(25, 25);
		var validationResult = CoinValidationResult.CreateValidResult("Quarter", 0.25m);
		var hasCoinState = new HasCoinState(_machine, _coinValidator, _inventoryService);
		_stateFactory.CreateHasCoinState(_machine).Returns(hasCoinState);
		_coinValidator.Validate(validCoin).Returns(validationResult);
		_machine.InsertCoin(_initialCoin); // this simulates the transition from NoCoinState to HasCoinState

		//Act
		hasCoinState.InsertCoin(validCoin);

		//Assert
		_stateFactory.Received(1).CreateNoCoinState(_machine);
		_stateFactory.Received(0).CreateProductSelectedState(_machine);
		_stateFactory.Received(0).CreateProductDispensedState(_machine);

		Assert.Collection(_machine.MessageHistory,
			message => Assert.Equal("INSERT COIN", message),
			message => Assert.Equal("BALANCE: $0.25", message),
			message => Assert.Equal("SELECT PRODUCT", message),
			message => Assert.Equal("BALANCE: $0.50", message),
			message => Assert.Equal("SELECT PRODUCT", message));
		Assert.Equal("SELECT PRODUCT", _machine.CurrentMessage);
		Assert.Equal(0.50m, _machine.CurrentBalance);
		Assert.Equal(0, _machine.SelectedProduct.Price);
		Assert.Null(_machine.SelectedProduct.Name);
	}

	[Fact]
	public void SelectProduct_ShouldIndicate_WhenSelectedProductIsOutOfStock()
	{
		//Arrange
		var hasCoinState = new HasCoinState(_machine, _coinValidator, _inventoryService);
		var coke = Product.Create("Coke", 1.00m);
		_stateFactory.CreateHasCoinState(_machine).Returns(hasCoinState);
		_inventoryService.IsAvailable(coke).Returns(false);
		_machine.InsertCoin(_initialCoin); // this simulates the transition from NoCoinState to HasCoinState

		//Act
		hasCoinState.SelectProduct(coke);

		//Assert
		_stateFactory.Received(1).CreateNoCoinState(_machine);
		_stateFactory.Received(0).CreateProductSelectedState(_machine);
		_stateFactory.Received(0).CreateProductDispensedState(_machine);

		Assert.Collection(_machine.MessageHistory,
			message => Assert.Equal("INSERT COIN", message),
			message => Assert.Equal("BALANCE: $0.25", message),
			message => Assert.Equal("SELECT PRODUCT", message),
			message => Assert.Equal("Coke OUT OF STOCK", message),
			message => Assert.Equal("SELECT PRODUCT", message));
		Assert.Equal("SELECT PRODUCT", _machine.CurrentMessage);
		Assert.Equal(0.25m, _machine.CurrentBalance);
		Assert.Equal(0, _machine.SelectedProduct.Price);
		Assert.Null(_machine.SelectedProduct.Name);
	}

	[Fact]
	public void SelectProduct_ShouldTransitionToProductSelectedState_WhenProductIsSelected()
	{
		//Arrange
		var hasCoinState = new HasCoinState(_machine, _coinValidator, _inventoryService);
		var productSelectedState = new ProductSelectedState(_machine, _coinValidator, _inventoryService);
		var coke = Product.Create("Coke", 1.00m);
		_stateFactory.CreateHasCoinState(_machine).Returns(hasCoinState);
		_stateFactory.CreateProductSelectedState(_machine).Returns(productSelectedState);
		_inventoryService.IsAvailable(coke).Returns(true);
		_machine.InsertCoin(_initialCoin); // this simulates the transition from NoCoinState to HasCoinState

		//Act
		hasCoinState.SelectProduct(coke);

		//Assert
		_stateFactory.Received(1).CreateNoCoinState(_machine);
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
		Assert.Equal(1, _machine.SelectedProduct.Price);
		Assert.Equal("Coke", _machine.SelectedProduct.Name);
	}
}
