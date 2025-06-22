namespace Optum.VendingMachineApp.Factories;

public interface IStateFactory
{
	IState CreateNoCoinState(VendingMachine machine);
	IState CreateHasCoinState(VendingMachine machine);
	IState CreateProductSelectedState(VendingMachine machine);
	IState CreateProductDispensedState(VendingMachine machine);
}
