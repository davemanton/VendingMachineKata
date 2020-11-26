using Domain.Coins;

namespace Application.Coins
{
    public class UsaCoinDetector : ICoinDetector
    {
        public bool TryDetect(string pieceOfMetal, out Coin? coin)
        {
            switch (pieceOfMetal.ToLowerInvariant())
            {
                case "nickel":
                    coin = new Nickel();
                    return true;
                case "dime":
                    coin = new Dime();
                    return true;
                case "quarter":
                    coin = new Quarter();
                    return true;
                default:
                    coin = null;
                    return false;
            }
        }
    }
}
