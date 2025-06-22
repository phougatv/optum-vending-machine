namespace Optum.VendingMachineApp.Validators.Specifications;

public sealed class QuarterSpecification : ICoinSpecification
{
    public Decimal MonetaryValue => 0.25m;
    public String CoinType => "Quarter";
    public Boolean IsSatisfiedBy(Coin coin) =>
        coin.WeightInGrams is >= 5.650 and <= 5.700 &&
        coin.DiameterInMillimeters is >= 24.20 and <= 24.30;
}
