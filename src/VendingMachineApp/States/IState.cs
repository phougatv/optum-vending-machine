namespace Optum.VendingMachineApp.States;

public interface IState
{
	void InsertCoin(Coin coin);
	void SelectProduct(Product product);
	void ReturnCoins();
}
