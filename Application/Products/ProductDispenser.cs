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
        }

        public ICollection<Product> DispenseBox { get; private init; }

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
            return CalculateChange(product.Price, coins.Sum(x => x.Value));
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

        private static ICollection<Coin> CalculateChange(int price,
                                                         int coinValue)
        {
            var change = coinValue - price;
            var changeCoins = new List<Coin>();

            var tempChange = changeCoins.Sum(x => x.Value);
            while (tempChange < change)
            {
                switch (change - tempChange)
                {
                    case >= 25:
                        changeCoins.Add(new Quarter());
                        break;
                    case >= 10:
                        changeCoins.Add(new Dime());
                        break;
                    case >= 5:
                        changeCoins.Add(new Nickel());
                        break;
                }

                tempChange = changeCoins.Sum(x => x.Value);
            }

            return changeCoins;
        }
    }
}