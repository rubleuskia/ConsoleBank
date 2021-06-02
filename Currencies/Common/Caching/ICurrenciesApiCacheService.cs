using System.Threading.Tasks;

namespace Currencies.Common.Caching
{
    public interface ICurrenciesApiCacheService : ICurrenciesApi
    {
        Task Initialize();
    }
}
