namespace ProductsInventory.API.Application.Contracts
{
    public class ProductResponse
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double Cost { get; set; }
        public double Profit { get; set; }
        public bool Enabled { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
