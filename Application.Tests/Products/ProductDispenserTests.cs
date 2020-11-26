using System;
using System.Collections.Generic;
using System.Linq;
using Application.Products;
using Domain.Coins;
using Domain.Products;
using Xunit;

namespace Application.Tests.Products
{
    public class ProductDispenserTests
    {
        private ICollection<Coin> _coins;

        public ProductDispenserTests()
        {
            _coins = new List<Coin>
            {
                new Quarter(),
                new Quarter(),
                new Quarter(),
                new Quarter()
            };
        }

        private IProductDispenser GetTarget()
        {
            return new ProductDispenser();
        }

        [Theory]
        [InlineData("a", typeof(Cola))]
        [InlineData("A", typeof(Cola))]
        [InlineData("b", typeof(Chips))]
        [InlineData("B", typeof(Chips))]
        [InlineData("c", typeof(Candy))]
        [InlineData("C", typeof(Candy))]
        public void TryDispense_PlacesAProductInDispenseBox(string productCode, Type expectedProductType)
        {
            var target = GetTarget();

            target.TryDispense(productCode, _coins, out _);

            Assert.IsType(expectedProductType, target.DispenseBox.Single());
        }

        [Fact]
        public void TryDispense_IncorrectCodeDoesntDispense_ReturnsFailure()
        {
            var target = GetTarget();

            target.TryDispense("badCode", _coins, out var error);

            Assert.Empty(target.DispenseBox);
            Assert.Equal("incorrect_code", error.ErrorCode);
        }

        [Theory]
        [InlineData("a")]
        [InlineData("A")]
        [InlineData("b")]
        [InlineData("B")]
        [InlineData("c")]
        [InlineData("C")]
        public void TryDispense_InsufficientFundsReturnsFailure_WithError(string productCode)
        {
            var target = GetTarget();

            var coins = new List<Coin>
            {
                new Nickel(),
                new Quarter()
            };

            target.TryDispense(productCode, coins, out var error);

            Assert.Empty(target.DispenseBox);
            Assert.Equal("insufficient_funds", error.ErrorCode);
        }

        [Theory]
        [InlineData("a")]
        [InlineData("A")]
        public void TryDispense_IfProductDispensed_ClearsCoins(string productCode)
        {
            var target = GetTarget();

            var result = target.TryDispense(productCode, _coins, out _);

            Assert.Empty(result);
            Assert.NotEmpty(target.DispenseBox);
        }

        [Theory]
        [InlineData("a", 25)]
        [InlineData("b", 75)]
        [InlineData("c", 60)]
        public void TryDispense_CalculatesAndReturnsChange(string productCode, int expectedChange)
        {
            var target = GetTarget();

            _coins.Add(new Quarter());

            var result = target.TryDispense(productCode, _coins, out _);

            Assert.Equal(expectedChange, result.Sum(x => x.Value));
        }
    }
}