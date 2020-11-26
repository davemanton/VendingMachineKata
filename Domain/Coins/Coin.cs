namespace Domain.Coins
{
    public abstract record Coin
    {
        protected Coin(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}