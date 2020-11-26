using System;
using Application.CoinDetectors;
using Domain.Coins;
using Xunit;

namespace Application.Tests
{
    public class UsaCoinDetectorTests
    {
        private ICoinDetector GetTarget()
        {
            return new UsaCoinDetector();
        }

        [Theory]
        [InlineData("nickel")]
        [InlineData("Nickel")]
        [InlineData("dime")]
        [InlineData("Dime")]
        [InlineData("quarter")]
        [InlineData("Quarter")]
        public void Insert_AcceptsValidMetalPiece_AndReturnsValidTrue(string metal)
        {
            var target = GetTarget();

            var result = target.TryDetect(metal, out _);

            Assert.True(result);
        }

        [Theory]
        [InlineData("bottlecap")]
        [InlineData("pog")]
        [InlineData("tiddlywink")]
        public void Insert_AcceptsInvalidMetalPiece_AndReturnsValidFalse(string metal)
        {
            var target = GetTarget();

            var result = target.TryDetect(metal, out _);

            Assert.False(result);
        }

        [Theory]
        [InlineData("nickel", typeof(Nickel))]
        [InlineData("Nickel", typeof(Nickel))]
        [InlineData("dime", typeof(Dime))]
        [InlineData("Dime", typeof(Dime))]
        [InlineData("quarter", typeof(Quarter))]
        [InlineData("Quarter", typeof(Quarter))]
        public void Insert_AcceptsValidMetalPiece_AndOutsCoin(string metal, Type expectedCoinType)
        {
            var target = GetTarget();

            var result = target.TryDetect(metal, out var coin);

            Assert.True(result);
            Assert.IsType(expectedCoinType, coin);
        }

        [Theory]
        [InlineData("bottlecap")]
        [InlineData("pog")]
        [InlineData("tiddlywink")]
        public void Insert_AcceptsInvalidMetalPiece_AndReturnsNullCoin(string metal)
        {
            var target = GetTarget();

            var result = target.TryDetect(metal, out var coin);

            Assert.Null(coin);
        }
    }
}
