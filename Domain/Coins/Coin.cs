namespace Domain.Coins
{
    public abstract record Coin
    {
        protected Coin(string name, int value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public int Value { get; }
    }
}