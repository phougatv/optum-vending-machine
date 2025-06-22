namespace Optum.VendingMachineApp.Validators;

public sealed class CoinValidationResult
{
	public Boolean IsValid { get; }
	public String CoinType { get; }
	public Decimal MonetaryValue { get; }

	private CoinValidationResult(Boolean isValid, String coinType, Decimal monetaryValue)
	{
		IsValid = isValid;
		CoinType = coinType;
		MonetaryValue = monetaryValue;
	}

	public static CoinValidationResult CreateValidResult(String coinType, Decimal monetaryValue)
		=> new CoinValidationResult(true, coinType, monetaryValue);
	public static CoinValidationResult CreateInvalidResult()
		=> new CoinValidationResult(false, String.Empty, 0m);
}
