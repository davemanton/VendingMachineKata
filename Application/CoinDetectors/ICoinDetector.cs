using Domain.Coins;

namespace Application.CoinDetectors
{
    public interface ICoinDetector
    {
        bool TryDetect(string pieceOfMetal, out Coin? coin);
    }
}