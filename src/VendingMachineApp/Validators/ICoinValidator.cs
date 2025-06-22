namespace Optum.VendingMachineApp.Validators;

public interface ICoinValidator
{
    CoinValidationResult Validate(Coin coin);
}
