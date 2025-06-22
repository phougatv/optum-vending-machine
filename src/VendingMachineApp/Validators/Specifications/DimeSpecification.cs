namespace Optum.VendingMachineApp.Validators.Specifications;

public sealed class DimeSpecification : ICoinSpecification
{
    public Decimal MonetaryValue => 0.10m;
    public String CoinType => "Dime";
    public Boolean IsSatisfiedBy(Coin coin) =>
        coin.WeightInGrams is >= 2.250 and <= 2.290 &&
        coin.DiameterInMillimeters is >= 17.85 and <= 17.99;
}
