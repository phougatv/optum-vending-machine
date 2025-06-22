namespace Optum.VendingMachineApp;

public sealed class VendingMachine
{
	private IState _currentState;
	private readonly List<String> _messageHistory;

	internal IStateFactory StateFactory { get; }

	public Product SelectedProduct { get; private set; }
	public Decimal CurrentBalance { get; private set; }
	public String CurrentMessage { get; private set; }
	public IReadOnlyList<String> MessageHistory => _messageHistory.AsReadOnly();

	internal String CurrentBalanceInUsd => CurrentBalance.ToString("C", CultureInfo.GetCultureInfo("en-US"));
	internal String SelectedProductPriceInUsd => SelectedProduct.Price.Value.ToString("C", CultureInfo.GetCultureInfo("en-US"));

	public VendingMachine(IStateFactory stateFactory)
	{
		ArgumentNullException.ThrowIfNull(stateFactory, nameof(stateFactory));
		StateFactory = stateFactory;
		CurrentMessage = String.Empty;
		CurrentBalance = 0m;

		_currentState = StateFactory.CreateNoCoinState(this);
		_messageHistory = [];

		AddAndSetMessage(Message.InsertCoin);
	}

	// Transition methods
	public void InsertCoin(Coin coin) => _currentState.InsertCoin(coin);
	public void ReturnCoins() => _currentState.ReturnCoins();
	public void SelectProduct(Product product) => _currentState.SelectProduct(product);

	// State management methods
	internal Boolean IsCurrentBalanceInsufficient() => SelectedProduct.Price > CurrentBalance;
	internal void TransitionTo(IState newState)
	{
		ArgumentNullException.ThrowIfNull(newState, nameof(newState));
		_currentState = newState;
	}
	internal void SetCurrentMessage(String message) => CurrentMessage = message;
	internal void SetSelectedProduct(Product product) => SelectedProduct = product;
	internal void AddToCurrentBalance(Decimal amount) => CurrentBalance += amount;
	internal void DeductFromCurrentBalance(Decimal amount) => AddToCurrentBalance(-1 * amount);
	internal void Reset()
	{
		if (CurrentBalance > 0.00m)
		{
			AddAndSetMessage(Message.ReturningCoin);
		}

		CurrentBalance = 0m;
		SelectedProduct = Product.Empty;
		CurrentMessage = Message.InsertCoin;

		_messageHistory.Clear();
		AddAndSetMessage(CurrentMessage);
	}
	internal void AddAndSetMessage(String message)
	{
		Console.WriteLine(message);
		CurrentMessage = message;
		_messageHistory.Add(message);
	}
}
