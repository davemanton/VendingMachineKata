using System.Collections.Generic;
using System.Linq;
using Application.Coins;
using Application.Products;
using Domain.Coins;
using Domain.Products;

namespace Application
{
    public class VendingMachine : IVendingMachine
    {
        private readonly ICoinDetector _coinDetector;
        private readonly IProductDispenser _productDispenser;

        private ICollection<Coin> _coins;
        private ICollection<string> _coinReturn;
        private string? _tempDisplayMessage;
        
        public VendingMachine(ICoinDetector coinDetector,
                              IProductDispenser productDispenser)
        {
            _coinDetector = coinDetector;
            _productDispenser = productDispenser;
        
            _coins = new List<Coin>();
            _coinReturn = new List<string>();
        }

        public ICollection<string> CheckCoinReturn()
        {
            var returnedCoins = _coinReturn.Select(x => x).ToList();
            _coinReturn.Clear();

            return returnedCoins;
        }

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
                _coinReturn.Add(pieceOfMetal);

                return;
            }

            _coins.Add(coin);
        }

        public void SelectProduct(string selection)
        {
            _coins = _productDispenser.TryDispense(selection, _coins, out var error);

            if (error == null)
            {
                _tempDisplayMessage = "THANK YOU";
                
                _coinReturn = _coins.Select(x => x.Name).ToList();
                _coins.Clear();

                return;
            }

            _tempDisplayMessage = $"PRICE {ConvertToDisplayMoney((int)error.Data["price"])}";
            
        }

        public ICollection<string> CheckDispenserHopper() => _productDispenser.DispenseBox.Select(x => x.Name).ToList();

        private static string ConvertToDisplayMoney(int value) => $"${decimal.Divide(value, 100):N2}";
    }
}