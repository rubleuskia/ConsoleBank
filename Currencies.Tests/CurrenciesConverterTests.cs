using System;
using Currencies.Common;
using Currencies.Common.Conversion;
using FluentAssertions;
using Xunit;

namespace Currencies.Tests
{
    public class CurrenciesConverterTests
    {
        [Fact]
        public void ConvertToLocal_OneToOne_ReturnsInitialAmount()
        {
            // arrange
            var amount = 100;
            var rateMode = new CurrencyRateModel
            {
                Nominal = 1,
                Rate = 1
            };

            // act
            var result = CurrenciesConverter.ConvertToLocal(amount, rateMode);

            // assert
            result.Should().Be(amount);
        }

        [Fact]
        public void ConvertToLocal_NegativeRate_ReturnsNegativeResult()
        {
            // arrange
            var amount = 100;
            var rateMode = new CurrencyRateModel
            {
                Nominal = 1,
                Rate = -100,
            };

            // act
            var result = CurrenciesConverter.ConvertToLocal(amount, rateMode);

            // assert
            result.Should().Be(-10000m);
        }

        [Fact]
        public void ConvertToLocal_ZeroAmount_ReturnsZero()
        {
            // arrange
            var amount = 0;
            var rateMode = new CurrencyRateModel
            {
                Nominal = 1,
                Rate = 1,
            };

            // act
            var result = CurrenciesConverter.ConvertToLocal(amount, rateMode);

            // assert
            result.Should().Be(0m);
        }

        [Fact]
        public void ConvertToLocal_ZeroNominal_ThrowsException()
        {
            // arrange
            var amount = 100;
            var rateMode = new CurrencyRateModel
            {
                Nominal = 0,
                Rate = 1,
            };

            // act
            Action action = () => CurrenciesConverter.ConvertToLocal(amount, rateMode);

            // assert
            action.Should().Throw<DivideByZeroException>();
        }

        [Theory]
        [InlineData(0, 100, 100, 0)]
        [InlineData(100, 1, 1, 100)]
        [InlineData(100, -1, 1, -100)]
        public void ConvertToLocal_Always_ReturnsResult(
            decimal amount, double rate, int nominal, decimal expectedResult)
        {
            // arrange
            var rateMode = new CurrencyRateModel
            {
                Nominal = nominal,
                Rate = rate,
            };

            // act
            var result = CurrenciesConverter.ConvertToLocal(amount, rateMode);

            // assert
            result.Should().Be(expectedResult);
        }
    }
}
