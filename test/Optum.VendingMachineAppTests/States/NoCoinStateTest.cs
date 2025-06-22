namespace Optum.VendingMachineApp.UnitTest.States;

public class NoCoinStateTest
{
    private readonly ICoinValidator _coinValidator;
    private readonly IInventoryService _inventoryService;
    private readonly IStateFactory _stateFactory;
	private readonly VendingMachine _machine;

	public NoCoinStateTest()
	{
		_coinValidator = Substitute.For<ICoinValidator>();
		_inventoryService = Substitute.For<IInventoryService>();
		_stateFactory = Substitute.For<IStateFactory>();
		_stateFactory.CreateNoCoinState(Arg.Any<VendingMachine>()).Returns(m => new NoCoinState(m.Arg<VendingMachine>(), _coinValidator));
		_machine = new VendingMachine(_stateFactory);
	}

    [Fact]
    public void InitialState_ShouldBeNoCoinState()
	{
		//Arrange
		var noCoinState = new NoCoinState(_machine, _coinValidator);
		_stateFactory.CreateNoCoinState(_machine).Returns(noCoinState);

		//Act & Assert
		_stateFactory.Received(0).CreateHasCoinState(_machine);
		Assert.Collection(_machine.MessageHistory, message => Assert.Equal("INSERT COIN", message));
		Assert.Equal("INSERT COIN", _machine.CurrentMessage);
		Assert.Equal(0, _machine.CurrentBalance);
		Assert.Equal(0, _machine.SelectedProduct.Price);
        Assert.Null(_machine.SelectedProduct.Name);
	}

    [Fact]
    public void ReturnCoins_ShouldIndicateNoCoinsToReturn()
	{
		//Arrange
		var noCoinState = new NoCoinState(_machine, _coinValidator);
		_stateFactory.CreateNoCoinState(_machine).Returns(noCoinState);

		//Act
		noCoinState.ReturnCoins();

		//Assert
		_stateFactory.Received(0).CreateHasCoinState(_machine);
		Assert.Collection(_machine.MessageHistory,
            message => Assert.Equal("INSERT COIN", message),
            message => Assert.Equal("NO COINS TO RETURN", message));
		Assert.Equal("NO COINS TO RETURN", _machine.CurrentMessage);
		Assert.Equal(0, _machine.CurrentBalance);
		Assert.Equal(0, _machine.SelectedProduct.Price);
		Assert.Null(_machine.SelectedProduct.Name);
	}

    [Fact]
    public void SelectProduct_ShouldIndicateInsertCoinFirst()
    {
        //Arrange
        var coke = Product.Create("Coke", 1.00m);
		var noCoinState = new NoCoinState(_machine, _coinValidator);
        _stateFactory.CreateNoCoinState(_machine).Returns(noCoinState);

        //Act
        noCoinState.SelectProduct(coke);

		//Assert
		_stateFactory.Received(0).CreateHasCoinState(_machine);
		Assert.Collection(_machine.MessageHistory,
			message => Assert.Equal("INSERT COIN", message),
			message => Assert.Equal("INSERT COIN FIRST", message));
        Assert.Equal("INSERT COIN FIRST", _machine.CurrentMessage);
		Assert.Equal(0, _machine.CurrentBalance);
		Assert.Equal(0, _machine.SelectedProduct.Price);
		Assert.Null(_machine.SelectedProduct.Name);
	}

    [Fact]
    public void InsertCoin_ShouldIndicate_WhenInvalidCoinIsInserted()
    {
        //Arrange
        var invalidCoin = Coin.Create(25, 25);
        var validationResult = CoinValidationResult.CreateInvalidResult();
        var noCoinState = new NoCoinState(_machine, _coinValidator);
        _stateFactory.CreateNoCoinState(_machine).Returns(noCoinState);
        _coinValidator.Validate(invalidCoin).Returns(validationResult);

        //Act
        noCoinState.InsertCoin(invalidCoin);

		//Assert
		_stateFactory.Received(0).CreateHasCoinState(_machine);
		Assert.Collection(_machine.MessageHistory,
			message => Assert.Equal("INSERT COIN", message),
			message => Assert.Equal("INVALID COIN", message),
            message => Assert.Equal("RETURNING COIN...", message),
            message => Assert.Equal("INSERT COIN", message));
        Assert.Equal("INSERT COIN", _machine.CurrentMessage);
        Assert.Equal(0, _machine.CurrentBalance);
        Assert.Equal(0, _machine.SelectedProduct.Price);
        Assert.Null(_machine.SelectedProduct.Name);
	}

    [Fact]
    public void InsertCoin_ShouldIndicate_WhenValidCoinIsInserted()
    {
        //Arrange
        var validCoin = Coin.Create(25, 25);
        var validationResult = CoinValidationResult.CreateValidResult("Quarter", 0.25m);
        var noCoinState = new NoCoinState(_machine, _coinValidator);
        _stateFactory.CreateNoCoinState(_machine).Returns(noCoinState);
        _coinValidator.Validate(validCoin).Returns(validationResult);

        //Act
        noCoinState.InsertCoin(validCoin);

		//Assert
		_stateFactory.Received(1).CreateHasCoinState(_machine);
		Assert.Collection(_machine.MessageHistory,
			message => Assert.Equal("INSERT COIN", message),
			message => Assert.Equal("BALANCE: $0.25", message),
			message => Assert.Equal("SELECT PRODUCT", message));
        Assert.Equal("SELECT PRODUCT", _machine.CurrentMessage);
		Assert.Equal(0.25m, _machine.CurrentBalance);
		Assert.Equal(0, _machine.SelectedProduct.Price);
		Assert.Null(_machine.SelectedProduct.Name);
    }
}
