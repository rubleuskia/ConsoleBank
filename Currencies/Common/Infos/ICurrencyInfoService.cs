using System;
using System.Threading.Tasks;

namespace Currencies.Common.Infos
{
    public interface ICurrencyInfoService
    {
        Task<string[]> GetAvailableCurrencies();

        Task<double> GetCurrencyRate(string charCode, DateTime? onDate = null);

        Task<decimal> ConvertToLocal(decimal amount, string charCode);

        Task<decimal> ConvertFromLocal(decimal amount, string charCode);

        Task<double> GetMinRate(string abbreviation, DateTime start, DateTime end);

        Task<double> GetMaxRate(string abbreviation, DateTime start, DateTime end);

        Task<double> GetAvgRate(string abbreviation, DateTime start, DateTime end);
    }
}
