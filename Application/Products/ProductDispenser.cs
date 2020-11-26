using System.Collections.Generic;
using System.Linq;
using Domain.Coins;
using Domain.Products;

namespace Application.Products
{
    public class ProductDispenser : IProductDispenser
    {
        public ProductDispenser()
        {
            DispenseBox = new List<Product>();
            Change = new List<Coin>();
        }

        public ICollection<Product> DispenseBox { get; private init; }
        public ICollection<Coin> Change { get; private init; }

        public ICollection<Coin> TryDispense(string productCode, ICollection<Coin> coins, out DispenserError? error)
        {
            error = null;
            var product = GetProductByCode(productCode);

            if (product == null)
            {
                error = new DispenserError("incorrect_code");
                return coins;
            }

            if (coins.Sum(x => x.Value) < product.Price)
            {
                error = new DispenserError(
                    "insufficient_funds",
                    new Dictionary<string, object>
                    {
                        {"price", product.Price}
                    });

                return coins;
            }

            DispenseBox.Add(product);
            return new List<Coin>();
        }

        private static Product? GetProductByCode(string code)
        {
            return code.ToLowerInvariant() switch
            {
                "a" => new Cola(),
                "b" => new Chips(),
                "c" => new Candy(),
                _ => null
            };
        }
        
    }
}