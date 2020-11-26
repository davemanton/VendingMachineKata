namespace Domain.Coins
{
    public record Quarter : Coin
    {
        public Quarter() : base("quarter", 25) { }
    }
}