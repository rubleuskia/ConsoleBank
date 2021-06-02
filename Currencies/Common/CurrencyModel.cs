namespace Currencies.Common
{
    // TODO: check if generic type CurrencyModel<T> is possible to use here
    public class CurrencyModel
    {
        public string Id { get; set; }
        public string CharCode { get; set; }
        public string Name { get; set; }
    }
}
