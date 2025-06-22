namespace Optum.VendingMachineApp.Services.Initialization;

public class CoinSpecificationInitializationService
{
	private readonly List<CoinSpecificationOptions> _coinOptions;

	public CoinSpecificationInitializationService(List<CoinSpecificationOptions> coinOptions)
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