namespace Optum.VendingMachineApp.Models;

public readonly record struct Product
{
    internal static readonly Product Empty = new Product(null!, 0.00m);

    public String Name { get; }
    public Price Price { get; }

    private Product(String name, Price price)
    {
        Name = name;
        Price = price;
    }

    public static Product Create(String name, Price price)
    {
        if (name is null or "")
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (price <= 0m)
        {
            throw new InvalidOperationException($"Product price must be greater than zero");
        }

        return new Product(name, price);
    }

    public override String ToString() => $"{Name} - {Price:C}";

    public Boolean IsEmpty => Name == Empty.Name && Price == Empty.Price;
}
