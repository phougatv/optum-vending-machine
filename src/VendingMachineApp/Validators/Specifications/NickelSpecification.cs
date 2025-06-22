namespace Optum.VendingMachineApp.Validators.Specifications;

public sealed class NickelSpecification : ICoinSpecification
{
    public Decimal MonetaryValue => 0.05m;
    public String CoinType => "Nickel";
    public Boolean IsSatisfiedBy(Coin coin) =>
        coin.WeightInGrams is >= 4.980 and <= 5.020 &&
        coin.DiameterInMillimeters is >= 21.15 and <= 21.27;
}