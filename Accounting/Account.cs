using System;

namespace Accounting
{
    public class Account
    {
        public Guid Id => Guid.NewGuid();
        private decimal _amount;
        private string _currencyCode;

        public string CurrencyCode => _currencyCode;

        public void Withdraw(decimal amount)
        {
            _amount -= amount;
        }

        public void Acquire(decimal amount)
        {
            _amount += amount;
        }
    }
}
