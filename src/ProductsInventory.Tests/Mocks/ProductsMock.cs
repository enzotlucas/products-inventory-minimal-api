﻿using ProductsInventory.API.Application.Contracts;

namespace ProductsInventory.Tests.Mocks
{
    public static class ProductsMock
    {
        public static Product GenerateValidProduct()
        {
            var name = "Product Name";
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
    }
}
