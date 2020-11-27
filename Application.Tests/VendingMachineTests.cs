using System.Collections.Generic;
using System.Linq;
using Application.Coins;
using Application.Products;
using Domain.Coins;
using Domain.Products;
using Moq;
using Xunit;

namespace Application.Tests
{
    public class VendingMachineTests
    {

        private readonly Mock<ICoinDetector> _coinDetector;
        private readonly Mock<IProductDispenser> _productDispenser;

        private delegate void MockCoinAction(string pieceOfMetal, out Coin? outVal);
        private delegate void MockDispenseAction(string productCode, ICollection<Coin> coins, out DispenserError? error);
        private Coin? _validCoin;
        private bool _isValidCoin;
        private DispenserError? _dispenserError;
        private ICollection<Coin> _dispenserCoinReturn;
        private ICollection<Product> _dispensedProducts; 

        public VendingMachineTests()
        {
            _coinDetector = new Mock<ICoinDetector>();
            _isValidCoin = true;
            
            _productDispenser = new Mock<IProductDispenser>();
            _dispenserCoinReturn = new List<Coin>();
            _dispensedProducts = new List<Product>();
        }

        private IVendingMachine GetTarget()
        {
            _productDispenser.Setup(x => x.DispenseBox).Returns(_dispensedProducts);

            _coinDetector.Setup(x => x.TryDetect(It.IsAny<string>(), out _validCoin))
                .Returns(_isValidCoin)
                .Callback(new MockCoinAction((string coinName, out Coin? coin) =>
                {
                    coin = coinName switch
                    {
                        "nickel" => new Nickel(),
                        "dime" => new Dime(),
                        "quarter" => new Quarter(),
                        _ => null
                    };
                }));

            _productDispenser.Setup(x => x.TryDispense(It.IsAny<string>(), It.IsAny<ICollection<Coin>>(), out _dispenserError))
                .Returns(_dispenserCoinReturn)
                .Callback(new MockDispenseAction((string productCode, ICollection<Coin> coins, out DispenserError? error) =>
                {
                    error = _dispenserError;
                }));

            return new VendingMachine(_coinDetector.Object, _productDispenser.Object);
        }

        [Fact]
        public void Display_ShowsInsertCoins()
        {
            var target = GetTarget();

            var result = target.Display();

            Assert.Equal("INSERT COIN", result);
        }

        [Theory]
        [InlineData("nickel", "$0.05")]
        [InlineData("dime", "$0.10")]
        [InlineData("quarter", "$0.25")]
        public void Display_ShowsValueWhenValidCoin(string pieceOfMetal, string expectedDisplay)
        {
            var target = GetTarget();

            target.InsertCoin(pieceOfMetal);
            var result = target.Display();

            Assert.Equal(expectedDisplay, result);
        }

        [Theory]
        [InlineData("bottlecap")]
        [InlineData("pog")]
        [InlineData("washer")]
        public void Display_ShowsInsertCoin_WhenInvalidCoin(string pieceOfMetal)
        {
            var target = GetTarget();

            target.InsertCoin(pieceOfMetal);
            var result = target.Display();

            Assert.Equal("INSERT COIN", result);
        }

        [Theory]
        [InlineData("$0.10", "nickel", "nickel")]
        [InlineData("$0.15", "dime", "nickel")]
        [InlineData("$0.40", "nickel", "dime", "quarter")]
        [InlineData("$0.40", "quarter", "nickel", "dime")]
        [InlineData("$0.55", "nickel", "nickel", "dime", "dime", "quarter")]
        [InlineData("$0.30", "nickel", "dime", "nickel", "dime")]
        [InlineData("$1.25", "quarter", "quarter", "quarter", "quarter", "quarter")]
        public void Display_ShowsTotal_WhenMultipleCoinsInserted(string expectedDisplay, params string[] piecesOfMetal)
        {
            var target = GetTarget();

            foreach(var piece in piecesOfMetal)
                target.InsertCoin(piece);
            
            var result = target.Display();

            Assert.Equal(expectedDisplay, result);
        }

        [Theory]
        [InlineData("bottlecap")]
        [InlineData("pog")]
        [InlineData("washer")]
        [InlineData("bottlecap", "pog")]
        [InlineData("pog", "washer")]
        [InlineData("washer", "pog", "bottlecap")]
        public void InsertCoins_PlacesMetal_InRejectionBoxWhenInvalid(params string[] piecesOfMetal)
        {
            var target = GetTarget();

            foreach (var piece in piecesOfMetal)
                target.InsertCoin(piece);

            var result = target.CheckCoinReturn();

            foreach (var piece in piecesOfMetal)
                Assert.Contains(piece, result);
        }

        [Theory]
        [InlineData("nickel")]
        [InlineData("dime")]
        [InlineData("quarter")]
        public void InsertCoins_PlacesCoinInRejectionBox_IfCoinNotDetectedProperly(string coin)
        {
            var target = GetTarget();

            _validCoin = null;
            _coinDetector.Setup(x => x.TryDetect(It.IsAny<string>(), out _validCoin))
                .Returns(true);

            target.InsertCoin(coin);

            var result = target.CheckCoinReturn();

            Assert.Contains(coin, result);
        }

        [Theory]
        [InlineData("a")]
        [InlineData("b")]
        [InlineData("c")]
        public void SelectProduct_IfMachineVends_DisplaysThankYou(string selection)
        {
            var target = GetTarget();

            for(var i = 0; i < 4; i++)
                target.InsertCoin("quarter");

            target.SelectProduct(selection);

            var response = target.Display();

            Assert.Equal("THANK YOU", response);
        }

        [Theory]
        [InlineData("a")]
        [InlineData("b")]
        [InlineData("c")]
        public void SelectProduct_DisplaysInsertCoins_AfterThanks(string selection)
        {
            var target = GetTarget();

            target.SelectProduct(selection);

            target.Display();
            var response = target.Display();

            Assert.Equal("INSERT COIN", response);
        }

        [Theory]
        [InlineData("a", 100)]
        [InlineData("b", 50)]
        [InlineData("c", 65)]
        public void SelectProduct_InsufficientFunds_DisplaysPrice(string selection, int price)
        {
            _dispenserError = new DispenserError("insufficient_funds", new Dictionary<string, object>(){ { "price", price } });

            var target = GetTarget();

            target.SelectProduct(selection);

            var response = target.Display();

            Assert.Equal($"PRICE ${decimal.Divide(price, 100):N2}", response);
        }

        [Fact]
        public void SelectProduct_InsufficientFunds_SecondDisplayShowsInsertCoins_IfNoCoins()
        {
            _dispenserError = new DispenserError("insufficient_funds", new Dictionary<string, object>() { { "price", 100 } });

            var target = GetTarget();

            target.SelectProduct("a");

            target.Display();
            var response = target.Display();

            Assert.Equal($"INSERT COIN", response);
        }

        [Fact]
        public void SelectProduct_InsufficientFunds_SecondDisplayShowsAmount()
        {
            _dispenserError = new DispenserError("insufficient_funds", new Dictionary<string, object>() { { "price", 100 } });
            _dispenserCoinReturn = new List<Coin>
            {
                new Nickel()
            };

            var target = GetTarget();

            target.InsertCoin("nickel");

            target.SelectProduct("a");

            target.Display();
            var response = target.Display();

            Assert.Equal($"$0.05", response);
        }

        [Fact]
        public void SelectProduct_PutsChangeInCoinReturn()
        {
            _dispenserCoinReturn = new List<Coin>
            {
                new Nickel(),
                new Dime()
            };

            var target = GetTarget();

            target.SelectProduct("a");

            var result = target.CheckCoinReturn();

            foreach (var coin in _dispenserCoinReturn)
                Assert.Contains(coin.Name, result);
        }

        [Fact]
        public void CheckCoinReturn_ShouldClearCoinReturn()
        {
            var target = GetTarget();

            target.InsertCoin("button");

            var result = target.CheckCoinReturn();
            Assert.Contains("button", result);

            result = target.CheckCoinReturn();
            Assert.Empty(result);
        }

        [Fact]
        public void CheckDispenserHopper_ShouldReturnDispenserValues()
        {
            _dispensedProducts = new List<Product>()
            {
                new Candy(),
                new Chips(),
                new Cola()
            };

            var target = GetTarget();

            var result = target.CheckDispenserHopper();
            
            foreach(var product in _dispensedProducts)
                Assert.Contains(product.Name, result);
        }

    }
}