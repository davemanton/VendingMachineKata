﻿using System.Collections.Generic;
using System.Linq;
using Application;
using Application.CoinDetectors;
using Domain.Coins;
using Moq;
using Xunit;

namespace Application.Tests
{
    public class VendingMachineTests
    {

        private readonly Mock<ICoinDetector> _coinDetector;

        private delegate void MockCoinAction(string pieceOfMetal, out Coin? outVal);
        private Coin? _validCoin;

        public VendingMachineTests()
        {
            _coinDetector = new Mock<ICoinDetector>();
            _coinDetector.Setup(x => x.TryDetect(It.Is<string>(arg => new[] {"nickel", "dime", "quarter"}.Contains(arg)), out _validCoin))
                .Returns(true)
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
        }

        private IVendingMachine GetTarget()
        {
            return new VendingMachine(_coinDetector.Object);
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

            var result = target.CheckRejectionBox();

            foreach (var piece in piecesOfMetal)
                Assert.Contains(piece, result);
        }

        [Theory]
        [InlineData("nickel")]
        [InlineData("dime")]
        [InlineData("quarter")]
        public void InsertCoins_PlacesCoinInRejectionBox_IfCoinNotDetectedProperly(string coin)
        {
            _validCoin = null;
            _coinDetector.Setup(x => x.TryDetect(It.IsAny<string>(), out _validCoin))
                .Returns(true);

            var target = GetTarget();

            target.InsertCoin(coin);

            var result = target.CheckRejectionBox();

            Assert.Contains(coin, result);
        }
    }
}