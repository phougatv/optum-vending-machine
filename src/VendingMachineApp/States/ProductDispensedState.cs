namespace Optum.VendingMachineApp.States;

public sealed class ProductDispensedState : IState
{
	private readonly VendingMachine _machine;

	public ProductDispensedState(VendingMachine machine)
	{
		ArgumentNullException.ThrowIfNull(machine, nameof(machine));
		_machine = machine;

		var nextState = _machine.StateFactory.CreateNoCoinState(_machine);
		_machine.TransitionTo(nextState);
		_machine.Reset();
	}

	//Transition methods
	public void InsertCoin(Coin coin) => ReturnCoins();
	public void ReturnCoins() => _machine.AddAndSetMessage(Message.ReturningCoin);
	public void SelectProduct(Product product) => _machine.AddAndSetMessage("WAIT FOR A FEW...");
}