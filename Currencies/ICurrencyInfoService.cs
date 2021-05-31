
using System;
using System.Threading.Tasks;

namespace Currencies
{
    // class CurrencyParameters
    // {
    //     public string Abbr { get; set; }
    //     public DateTime? OnDate { get; set; }
    // }

    public interface ICurrencyInfoService
    {
        Task<string[]> GetAvailableCurrencies();

        Task<double> GetCurrencyRate(string abbreviation, DateTime? onDate = null);

        Task<double> ConvertTo(double amount, string abbreviation);

        Task<double> ConvertFrom(double amount, string abbreviation);

        Task<double> GetMinRate(string abbreviation, DateTime start, DateTime end);

        Task<double> GetManRate(string abbreviation, DateTime start, DateTime end);

        Task<double> GetAvgRate(string abbreviation, DateTime start, DateTime end);
    }
}
