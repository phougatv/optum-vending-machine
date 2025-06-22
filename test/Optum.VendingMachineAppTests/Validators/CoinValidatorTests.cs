namespace Optum.VendingMachineApp.UnitTest.Validators;

public class CoinValidatorTests
{
	[Fact]
	public void Validate_ShouldReturnTrue_WhenCoinIsValid()
	{
		// Arrange
		var coin = Coin.Create(10, 24);
		var specifications = new List<ICoinSpecification>
		{
			new CoinSpecification(0.10m, "Dime", 10, 24, 0.001),
			new CoinSpecification(0.25m, "Quarter", 11.34, 24.26, 0.001)
		};
		var validator = new CoinValidator(specifications);

		// Act
		var result = validator.Validate(coin);

		// Assert
		Assert.True(result.IsValid);
		Assert.Equal("Dime", result.CoinType);
		Assert.Equal(0.10m, result.MonetaryValue);
	}

	[Fact]
	public void Validate_ShouldReturnFalse_WhenCoinIsInvalid()
	{
		// Arrange
		var coin = Coin.Create(5, 20); // Invalid coin
		var specifications = new List<ICoinSpecification>
		{
			new CoinSpecification(0.10m, "Dime", 10, 24, 0.001),
			new CoinSpecification(0.25m, "Quarter", 11.34, 24.26, 0.001)
		};
		var validator = new CoinValidator(specifications);

		// Act
		var result = validator.Validate(coin);

		// Assert
		Assert.False(result.IsValid);
		Assert.Empty(result.CoinType);
		Assert.Equal(0, result.MonetaryValue);
	}

	[Fact]
	public void Constructor_ShouldThrowArgumentException_WhenSpecificationsAreEmpty()
	{
		// Arrange
		var emptySpecifications = new List<ICoinSpecification>();

		// Act & Assert
		Assert.Throws<ArgumentException>(() => new CoinValidator(emptySpecifications));
	}

	[Fact]
	public void Constructor_ShouldThrowArgumentNullException_WhenSpecificationsAreNull()
	{
		// Arrange
		ICollection<ICoinSpecification> nullSpecifications = null!;

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new CoinValidator(nullSpecifications));
	}
}
