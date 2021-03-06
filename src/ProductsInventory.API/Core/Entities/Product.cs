namespace ProductsInventory.API.Core.Entities
{
    public class Product
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; private set; }
        public int Quantity { get; private set; }
        public double Price { get; private set; }
        public double Cost { get; private set; }
        public double Profit
        {
            get
            {
                return Price - Cost;
            }
        }
        public bool Enabled { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public Product(string name, int quantity, double price, double cost, bool enabled, IValidator<Product> validator)
        {
            Name = name;
            Quantity = quantity;
            Price = price;
            Cost = cost;
            Enabled = enabled;

            Validate(validator);
        }

        protected Product() { }

        public Product(bool valid)
        {
            if (valid)
                throw new BusinessException("Project constructor with no parameters are not alowed");

            Id = Guid.Empty;
        }

        private void Validate(IValidator<Product> validator)
        {
            var validationResult = validator.Validate(this);

            if (validationResult.IsValid)
                return;

            throw new InvalidProductException(validationResult.ToDictionary());
        }

        public void Enable()
             => Enabled = true;

        public void Disable() 
            => Enabled = false;

        public void WithdrawFromStock(int quantity)
        {
            if (quantity < 1 || Quantity - quantity < 0 || Quantity == 0)
                throw new InvalidQuantityException();

            Quantity -= quantity;
        }

        public void AddStock(int quantity)
        {
            if (quantity < 1)
                throw new InvalidQuantityException();

            Quantity += quantity;
        }

        public void Update(ProductRequest dto)
        {
            UpdateName(dto.Name);

            UpdateQuantity(dto.Quantity);

            UpdatePrice(dto.Price);

            UpdateCost(dto.Cost);

            UpdateEnable(dto.Enabled);
        }

        private void UpdateName(string name)
        {
            if (name is null || name.Equals(Name))
                return;

            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidNameException();

            Name = name;
        }

        private void UpdateQuantity(int? quantity)
        {
            if (quantity is null)
                return;

            if (quantity < 0)
                throw new InvalidQuantityException();

            Quantity = quantity.Value;
        }

        private void UpdatePrice(double? price)
        {
            if (price is null)
                return;

            if (price < 0 || price < Cost)
                throw new InvalidPriceException();

            Price = price.Value;
        }

        private void UpdateCost(double? cost)
        {
            if (cost is null)
                return;

            if (cost < 0 || cost > Price)
                throw new InvalidCostException();

            Cost = cost.Value;
        }

        private void UpdateEnable(bool? enable)
        {
            if (enable is null)
                return;

            if (enable.Value)
                Enable();
            else
                Disable();
        }

        public bool Valid() 
            => Id != Guid.Empty;

        public override string ToString()
        {
            return $"Id:{Id};" +
                   $"Name:{Name};" +
                   $"Quantity:{Quantity};" +
                   $"Price:{Price};" +
                   $"Cost:{Cost};" +
                   $"Profit:{Profit};" +
                   $"Enabled:{Enabled};" +
                   $"CreatedAt:{CreatedAt};" +
                   $"UpdatedAt:{UpdatedAt}";
        }
    }
}
