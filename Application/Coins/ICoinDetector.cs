using Domain.Coins;

namespace Application.Coins
{
    public interface ICoinDetector
    {
        bool TryDetect(string pieceOfMetal, out Coin? coin);
    }
}