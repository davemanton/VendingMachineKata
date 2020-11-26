namespace Domain.Coins
{
    public record Dime : Coin
    {
        public Dime() : base("dime", 10) { }
    }
}