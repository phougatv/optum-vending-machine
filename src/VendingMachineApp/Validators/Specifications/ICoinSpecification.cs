namespace Optum.VendingMachineApp.Validators.Specifications;

public interface ICoinSpecification
{
	Boolean IsSatisfiedBy(Coin coin);
	Decimal MonetaryValue { get; }
	String CoinType { get; }
}
