using System.Collections.Generic;
using System.Linq;
using Application.CoinDetectors;
using Application.Products;
using Domain.Coins;

namespace Application
{
    public class VendingMachine : IVendingMachine
    {
        private readonly ICoinDetector _coinDetector;
        private readonly IProductDispenser _productDispenser;

        private readonly ICollection<Coin> _coins;
        private readonly ICollection<string> _rejectionBox;

        private string? _tempDisplayMessage;
        
        public VendingMachine(ICoinDetector coinDetector,
                              IProductDispenser productDispenser)
        {
            _coinDetector = coinDetector;
            _productDispenser = productDispenser;
        
            _coins = new List<Coin>();
            _rejectionBox = new List<string>();
        }

        public ICollection<string> CheckRejectionBox() => _rejectionBox;

        public string Display()
        {
            if (string.IsNullOrWhiteSpace(_tempDisplayMessage))
                return _coins.Any()
                    ? ConvertToDisplayMoney(_coins.Sum(x => x.Value))
                    : "INSERT COIN";

            var message = _tempDisplayMessage;
            _tempDisplayMessage = null;

            return message;
        }

        public void InsertCoin(string pieceOfMetal)
        {
            var success = _coinDetector.TryDetect(pieceOfMetal, out var coin);

            if (!success || coin == null)
            {
                _rejectionBox.Add(pieceOfMetal);

                return;
            }

            _coins.Add(coin);
        }

        public void SelectProduct(string selection)
        {
            _productDispenser.TryDispense(selection, _coins, out var error);

            if (error == null)
            {
                _tempDisplayMessage = "THANK YOU";

                return;
            }

            _tempDisplayMessage = $"PRICE {ConvertToDisplayMoney((int)error.Data["price"])}";
        }

        private static string ConvertToDisplayMoney(int value) => $"${decimal.Divide(value, 100):N2}";
    }
}