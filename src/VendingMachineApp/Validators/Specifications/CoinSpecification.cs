namespace Optum.VendingMachineApp.Validators.Specifications;

public sealed class CoinSpecification : ICoinSpecification
{
	private readonly Double _minWeight;
	private readonly Double _maxWeight;
	private readonly Double _minDiameter;
	private readonly Double _maxDiameter;

	public Decimal MonetaryValue { get; }
	public String CoinType { get; }

	public CoinSpecification(
		Decimal monetaryValue,
		String coinType,
		Double weightInGrams,
		Double diameterInMillimeters,
		Double tolerance)
	{
		MonetaryValue = monetaryValue;
		CoinType = coinType;

		_minWeight = weightInGrams - tolerance;
		_maxWeight = weightInGrams + tolerance;
		_minDiameter = diameterInMillimeters - tolerance;
		_maxDiameter = diameterInMillimeters + tolerance;
	}

	public Boolean IsSatisfiedBy(Coin coin)
	{
		return coin.WeightInGrams >= _minWeight &&
			   coin.WeightInGrams <= _maxWeight &&
			   coin.DiameterInMillimeters >= _minDiameter &&
			   coin.DiameterInMillimeters <= _maxDiameter;
	}
}
