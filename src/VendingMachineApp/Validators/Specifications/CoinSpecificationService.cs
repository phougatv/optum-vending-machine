namespace Optum.VendingMachineApp.Validators.Specifications;

public sealed class CoinSpecificationService
{
	private readonly List<CoinSpecificationOptions> _coinOptions;

	public CoinSpecificationService(List<CoinSpecificationOptions> coinOptions)
	{
		_coinOptions = coinOptions;
	}

	public IEnumerable<ICoinSpecification> GetSpecifications()
	{
		foreach (var option in _coinOptions)
		{
			yield return new CoinSpecification(
				option.MonetaryValue,
				option.CoinType,
				option.WeightInGrams,
				option.DiameterInMillimeters,
				option.Tolerance
			);
		}
	}
}
