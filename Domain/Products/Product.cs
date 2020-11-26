namespace Domain.Products
{
    public abstract record Product
    {
        protected Product(string name,
                          int price)
        {
            Name = name;
            Price = price;
        }

        public string Name { get; }
        public int Price { get; }
    }

    public record Cola : Product
    {
        public Cola() : base("Cola", 100) {}
    }

    public record Chips : Product
    {
        public Chips() : base("Chips", 50) { }
    }

    public record Candy : Product
    {
        public Candy() : base("Candy", 65) { }
    }
}