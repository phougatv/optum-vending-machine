namespace Optum.VendingMachineApp.States;

public sealed class NoCoinState : IState
{
	private readonly VendingMachine _machine;
	private readonly ICoinValidator _coinValidator;

	public NoCoinState(VendingMachine vendingMachine, ICoinValidator coinValidator)
	{
		ArgumentNullException.ThrowIfNull(vendingMachine, nameof(vendingMachine));
		ArgumentNullException.ThrowIfNull(coinValidator, nameof(coinValidator));
		_machine = vendingMachine;
		_coinValidator = coinValidator;
	}

	//Transition methods
	public void InsertCoin(Coin coin)
	{
		var result = _coinValidator.Validate(coin);
		if (!result.IsValid)
		{
			_machine.AddAndSetMessage(Message.InvalidCoin);
			_machine.AddAndSetMessage(Message.ReturningCoin);
			_machine.AddAndSetMessage(Message.InsertCoin);
			return;
		}

		_machine.AddToCurrentBalance(result.MonetaryValue);
		_machine.AddAndSetMessage($"BALANCE: {_machine.CurrentBalanceInUsd}");
		_machine.AddAndSetMessage(Message.SelectProduct);

		var nextState = _machine.StateFactory.CreateHasCoinState(_machine);
		_machine.TransitionTo(nextState);
	}
	public void ReturnCoins() => _machine.AddAndSetMessage(Message.NoCoinsToReturn);
	public void SelectProduct(Product product) => _machine.AddAndSetMessage(Message.InsertCoinFirst);
}
