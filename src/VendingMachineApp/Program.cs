namespace Optum.VendingMachineApp;

internal class Program
{
	static void Main(String[] args)
	{
		// Build configuration
		var configuration = new ConfigurationBuilder()
			.SetBasePath(AppContext.BaseDirectory)
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			.Build();

		var coinSpecificationOptions = configuration.GetSection("CoinSpecifications").Get<List<CoinSpecificationOptions>>();
		if (coinSpecificationOptions is null)
		{
			Console.WriteLine("Coin specifications not found in configuration.");
			return;
		}

		var productOptions = configuration.GetSection("Products").Get<List<ProductOptions>>();
		if (productOptions is null)
		{
			Console.WriteLine("Product specifications not found in configuration.");
			return;
		}

		//initialize coin specification service with options and coin validator with coin specifications
		var coinSpecificationService = new CoinSpecificationService(coinSpecificationOptions);
		var coinSpecifications = coinSpecificationService.GetSpecifications().ToList();
		var coinValidator = (ICoinValidator)new CoinValidator(coinSpecifications);

		//initialize inventory service with products
		var inventoryService = (IInventoryService)new ProductInventoryService();
		var productInitializationService = new ProductInitializationService(productOptions);
		productInitializationService.InitializeInventory(inventoryService);

		var nickel = Coin.Create(5, 21.21);        //5.000g and 21.21mm. Monetary value is 0.05m
		var dime = Coin.Create(2.268, 17.91);      //2.268g and 17.91mm. Monetary value is 0.10m
		var quarter = Coin.Create(5.670, 24.26);   //5.670g and 24.26mm. Monetary value is 0.25m

		var cola = Product.Create("Cola", new Price(1m));
		var chips = Product.Create("Chips", new Price(0.5m));
		var candy = Product.Create("Candy", new Price(0.65m));

		var stateFactory = (IStateFactory)new StateFactory(coinValidator, inventoryService);

		Console.WriteLine("---- Vending Machine Simulation ----");

		var vendingMachine = new VendingMachine(stateFactory);
		vendingMachine.InsertCoin(quarter);
		vendingMachine.SelectProduct(cola);
		vendingMachine.InsertCoin(quarter);
		vendingMachine.InsertCoin(quarter);
		vendingMachine.InsertCoin(quarter);
	}
}
