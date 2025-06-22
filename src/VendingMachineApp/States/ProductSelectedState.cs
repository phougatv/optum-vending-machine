namespace Optum.VendingMachineApp.States;

public sealed class ProductSelectedState : IState
{
	private readonly VendingMachine _machine;
	private readonly ICoinValidator _coinValidator;
	private readonly IInventoryService _inventoryService;

	public ProductSelectedState(
		VendingMachine machine,
		ICoinValidator coinValidator,
		IInventoryService inventoryService)
	{
		_machine = machine;
		_coinValidator = coinValidator;
		_inventoryService = inventoryService;
	}

	//Transition methods
	public void InsertCoin(Coin coin)
	{
		var result = _coinValidator.Validate(coin);
		if (!result.IsValid)
		{
			_machine.SetCurrentMessage("INVALID COIN");
			return;
		}

		_machine.AddToCurrentBalance(result.MonetaryValue);
		_machine.AddAndSetMessage($"BALANCE: {_machine.CurrentBalanceInUsd}");

		if (_machine.IsCurrentBalanceInsufficient())
		{
			_machine.AddAndSetMessage(Message.InsertCoin);
			return;
		}

		_inventoryService.DeductProduct(_machine.SelectedProduct);
		_machine.DeductFromCurrentBalance(_machine.SelectedProduct.Price);

		_machine.AddAndSetMessage($"DISPENSING {_machine.SelectedProduct.Name}...");
		_machine.AddAndSetMessage($"{_machine.SelectedProduct.Name} DISPENSED");
		_machine.AddAndSetMessage(Message.ThankYou);

		var nextState = _machine.StateFactory.CreateProductDispensedState(_machine);
		_machine.TransitionTo(nextState);
	}
	public void ReturnCoins()
	{
		_machine.AddAndSetMessage(Message.CoinsReturned);

		var nextState = _machine.StateFactory.CreateNoCoinState(_machine);
		_machine.TransitionTo(nextState);
		_machine.Reset();
	}

	public void SelectProduct(Product product)
	{
		if (_machine.SelectedProduct == product)
		{
			_machine.AddAndSetMessage($"{product.Name} ALREADY SELECTED");
			_machine.AddAndSetMessage(Message.InsertCoin);
			return;
		}

		if (!_inventoryService.IsAvailable(product))
		{
			_machine.AddAndSetMessage($"{product.Name} {Message.OutOfStock}");
			_machine.AddAndSetMessage(Message.SelectProduct);
			return;
		}

		_machine.SetSelectedProduct(product);
		_machine.AddAndSetMessage($"{product.Name} - ${product.Price}");
		if (_machine.IsCurrentBalanceInsufficient())
		{
			_machine.AddAndSetMessage($"BALANCE: {_machine.CurrentBalanceInUsd}");
			_machine.AddAndSetMessage(Message.InsertCoin);
			return;
		}

		_inventoryService.DeductProduct(product);
		_machine.DeductFromCurrentBalance(_machine.SelectedProduct.Price);

		_machine.AddAndSetMessage($"DISPENSING {_machine.SelectedProduct.Name}...");
		_machine.AddAndSetMessage($"{_machine.SelectedProduct.Name} DISPENSED");
		_machine.AddAndSetMessage(Message.ThankYou);

		var nextState = _machine.StateFactory.CreateProductDispensedState(_machine);
		_machine.TransitionTo(nextState);
	}
}
