namespace Optum.VendingMachineApp.States;

public class HasCoinState : IState
{
	private readonly VendingMachine _machine;
	private readonly ICoinValidator _coinValidator;
	private readonly IInventoryService _inventoryService;

	public HasCoinState(
		VendingMachine vendingMachine,
		ICoinValidator coinValidator,
		IInventoryService inventoryService)
	{
		ArgumentNullException.ThrowIfNull(vendingMachine, nameof(vendingMachine));
		ArgumentNullException.ThrowIfNull(coinValidator, nameof(coinValidator));
		_machine = vendingMachine;
		_coinValidator = coinValidator;
		_inventoryService = inventoryService;
	}

	//Transition methods
	public void InsertCoin(Coin coin)
	{
		var result = _coinValidator.Validate(coin);
		if (!result.IsValid)
		{
			_machine.AddAndSetMessage(Message.InvalidCoin);
			_machine.AddAndSetMessage(Message.CoinsReturned);
			_machine.AddAndSetMessage(Message.InsertCoin);
			return;
		}

		_machine.AddToCurrentBalance(result.MonetaryValue);
		_machine.AddAndSetMessage($"BALANCE: {_machine.CurrentBalanceInUsd}");
		_machine.AddAndSetMessage(Message.SelectProduct);
	}
	public void ReturnCoins()
	{
		_machine.AddAndSetMessage(Message.CoinsReturned);
		_machine.AddAndSetMessage(Message.InsertCoin);

		var nextState = _machine.StateFactory.CreateNoCoinState(_machine);
		_machine.TransitionTo(nextState);
		_machine.Reset();
	}
	public void SelectProduct(Product product)
	{
		var isProductAvailable = _inventoryService.IsAvailable(product);
		if (!isProductAvailable)
		{
			_machine.AddAndSetMessage($"{product.Name} {Message.OutOfStock}");
			_machine.AddAndSetMessage(Message.SelectProduct);
			return;
		}

		_machine.SetSelectedProduct(product);
		_machine.AddAndSetMessage($"{product.Name} - ${product.Price}");
		_machine.AddAndSetMessage($"BALANCE: {_machine.CurrentBalanceInUsd}");
		_machine.AddAndSetMessage(Message.InsertCoin);
		var nextState = _machine.StateFactory.CreateProductSelectedState(_machine);
		_machine.TransitionTo(nextState);
	}
}
