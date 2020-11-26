using System.Collections.Generic;
using System.Linq;
using Application.CoinDetectors;
using Domain.Coins;

namespace Application
{
    public class VendingMachine : IVendingMachine
    {
        private readonly ICoinDetector _coinDetector;

        private readonly ICollection<Coin> _coins;
        private readonly ICollection<string> _rejectionBox;
        
        public VendingMachine(ICoinDetector coinDetector)
        {
            _coinDetector = coinDetector;
            _coins = new List<Coin>();
            _rejectionBox = new List<string>();
        }

        public string Display()
        {
            return _coins.Any()
                ? $"${decimal.Divide(_coins.Sum(x => x.Value), 100):N2}"
                : "INSERT COIN";
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

        public ICollection<string> CheckRejectionBox() => _rejectionBox;
    }
}