using System;

namespace Currencies
{
    public class CurrencyNotAvailableException : Exception
    {
        public CurrencyNotAvailableException(string message) : base(message)
        {
        }
    }
}