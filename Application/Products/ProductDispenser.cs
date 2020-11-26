using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.ComTypes;
using Domain.Coins;
using Domain.Products;

namespace Application.Products
{
    public interface IProductDispenser
    {
        void TryDispense(string productCode, ICollection<Coin> coins, out DispenserError? error);
        ICollection<Product> DispenseBox { get; }
    }

    public class ProductDispenser : IProductDispenser
    {
        public ProductDispenser()
        {
            DispenseBox = new List<Product>();
        }

        public ICollection<Product> DispenseBox { get; private set; }

        public void TryDispense(string productCode, ICollection<Coin> coins, out DispenserError? error)
        {
            error = null;
            var product = GetProductByCode(productCode);

            if (product == null)
            {
                error = new DispenserError("incorrect_code");
                return;
            }

            if (coins.Sum(x => x.Value) < product.Price)
            {
                error = new DispenserError(
                    "insufficient_funds",
                    new Dictionary<string, object>
                    {
                        {"price", product.Price}
                    });

                return;
            }

            DispenseBox.Add(product);
            coins.Clear();
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

    public record DispenserError
    {
        public DispenserError(string errorCode, IDictionary<string, object>? data = null)
        {
            ErrorCode = errorCode;
            Data = data ?? new Dictionary<string, object>();
        }

        public string ErrorCode { get; }
        public IDictionary<string, object> Data { get; }
    }
}