using System.Collections.Generic;
using Domain.Coins;
using Domain.Products;

namespace Application.Products
{
    public interface IProductDispenser
    {
        void TryDispense(string productCode, ICollection<Coin> coins, out DispenserError? error);
        ICollection<Product> DispenseBox { get; }
    }
}