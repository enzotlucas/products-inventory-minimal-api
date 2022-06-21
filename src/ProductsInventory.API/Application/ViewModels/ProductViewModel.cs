namespace ProductsInventory.API.Application.ViewModels
{
    public class ProductViewModel
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public int? Quantity { get; set; }
        public double? Price { get; set; }
        public double? Cost { get; set; }
        public double? Profit { get; set; }
        public bool? Enabled { get; set; }
        public DateTime CreatedAt { get; set; }

        public Product ToEntity()
        {            
            if (Name is null ||
                Quantity is null ||
                Price is null ||
                Cost is null ||
                Enabled is null)
                throw new InvalidProductException();

            return new Product(Name, Quantity.Value, Price.Value, Cost.Value, Enabled.Value);
        }

        public override string ToString()
        {
            return $"Id: {Id};" +
                   $"Name: {Name};" +
                   $"Quantity: {Quantity};" +
                   $"Price: {Price};" +
                   $"Cost: {Cost};" +
                   $"Enabled: {Enabled};";
        }

        public ProductViewModel(Product product)
        {
            Id = product.Id;
            Name = product.Name;
            Quantity = product.Quantity;
            Price = product.Price;
            Cost = product.Cost;
            Profit = product.Profit;
            Enabled = product.Enabled;
            CreatedAt = product.CreatedAt;
        }
    }
}
