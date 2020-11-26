namespace Domain.Coins
{
    public record Nickel : Coin
    {
        public Nickel() : base("nickel", 5) { }
    }
}