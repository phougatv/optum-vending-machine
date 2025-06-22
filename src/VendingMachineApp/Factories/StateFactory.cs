namespace Optum.VendingMachineApp.Factories;

public sealed class StateFactory : IStateFactory
{
	private readonly ICoinValidator _coinValidator;
	private readonly IInventoryService _inventoryService;

	public StateFactory(ICoinValidator coinValidator, IInventoryService inventoryService)
	{
		ArgumentNullException.ThrowIfNull(coinValidator, nameof(coinValidator));
		ArgumentNullException.ThrowIfNull(inventoryService, nameof(inventoryService));
		_coinValidator = coinValidator;
		_inventoryService = inventoryService;
	}

	public IState CreateHasCoinState(VendingMachine machine)
		=> new HasCoinState(machine, _coinValidator, _inventoryService);
	public IState CreateNoCoinState(VendingMachine machine)
		=> new NoCoinState(machine, _coinValidator);
	public IState CreateProductDispensedState(VendingMachine machine)
		=> new ProductDispensedState(machine);
	public IState CreateProductSelectedState(VendingMachine machine)
		=> new ProductSelectedState(machine, _coinValidator, _inventoryService);
}
