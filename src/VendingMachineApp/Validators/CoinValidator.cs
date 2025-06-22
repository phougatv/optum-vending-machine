namespace Optum.VendingMachineApp.Validators;

public sealed class CoinValidator : ICoinValidator
{
    private readonly ICollection<ICoinSpecification> _specifications;

    public CoinValidator(ICollection<ICoinSpecification> coinSpecifications)
    {
        ArgumentNullException.ThrowIfNull(coinSpecifications, nameof(coinSpecifications));
        if (coinSpecifications.Count == 0)
        {
            throw new ArgumentException("Coin specifications cannot be empty.", nameof(coinSpecifications));
        }

        _specifications = coinSpecifications;
    }

    public CoinValidationResult Validate(Coin coin)
    {
        foreach (var specification in _specifications)
        {
            if (specification.IsSatisfiedBy(coin))
            {
                return CoinValidationResult.CreateValidResult(specification.CoinType, specification.MonetaryValue);
            }
        }

        return CoinValidationResult.CreateInvalidResult();
    }
}
