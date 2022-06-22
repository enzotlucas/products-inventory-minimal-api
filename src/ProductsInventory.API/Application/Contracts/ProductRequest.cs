namespace ProductsInventory.API.Application.Contracts
{
    public class ProductRequest
    {
        public string Name { get; set; }
        public int? Quantity { get; set; }
        public double? Price { get; set; }
        public double? Cost { get; set; }
        public bool? Enabled { get; set; }

        public override string ToString()
        {
            return $"Name: {Name};" +
                   $"Quantity: {Quantity};" +
                   $"Price: {Price};" +
                   $"Cost: {Cost};" +
                   $"Enabled: {Enabled};";
        }
    }
}
