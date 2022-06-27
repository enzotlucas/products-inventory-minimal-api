using ProductsInventory.API.Application.Contracts;

namespace ProductsInventory.Tests.Mocks
{
    public static class ProductsMock
    {
        public static Product GenerateValidProduct(string productName = null)
        {
            var name = productName ?? "Product Name";
            var quantity = 5;
            var price = 0.8;
            var cost = 0.2;
            var enabled = true;

            var validationResult = Substitute.For<ValidationResult>();
            validationResult.IsValid.Returns(true);

            var validator = Substitute.For<IValidator<Product>>();
            validator.Validate(Arg.Any<Product>()).Returns(validationResult);

            var product = Substitute.For<Product>(name, quantity, price, cost, enabled, validator);

            return product;
        }

        public static Product GenerateNotExistingProduct()
        {
            return new Product(false);
        }

        public static ProductRequest GenerateValidProductRequest()
        {
            return new ProductRequest
            {
                Name = "Product Name",
                Quantity = 5,
                Cost = .8,
                Price = 0.2,
                Enabled = true
            };
        }

        public static ProductRequest GenerateInvalidProductRequest()
        {
            return new ProductRequest
            {
                Name = "",
                Quantity = 0,
                Cost = 0.8,
                Price = 1,
                Enabled = true
            };
        }

        public static ProductResponse GenerateValidProductResponse(string name)
        {
            return new ProductResponse
            {
                Name = name ?? "ProductResponse",
                Quantity = 5,
                Cost = 0.2,
                Price = 0.4,
                Profit = 0.4 - 0.2,
                Enabled = true,
                CreatedAt = DateTime.Now
            };
        }

        public static IEnumerable<Product> GenerateProductList(int quantity)
        {
            if (quantity < 1)
                throw new Exception("Quantity must be higher than one");

            var products = new List<Product>();

            for(int i = 0; i < quantity; i++)
                products.Add(GenerateValidProduct($"Product {i}"));

            return products;
        }

        public static IEnumerable<ProductResponse> GenerateProductResponseList(int quantity)
        {
            if (quantity < 1)
                throw new Exception("Quantity must be higher than one");

            var products = new List<ProductResponse>();

            for (int i = 0; i < quantity; i++)
                products.Add(GenerateValidProductResponse($"Product {i}"));

            return products;
        }
    }
}
