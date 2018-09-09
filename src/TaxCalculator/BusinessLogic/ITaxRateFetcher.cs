using System.Threading.Tasks;
using TaxCalculator.BusinessLogic.Common;

namespace TaxCalculator.BusinessLogic
{
    public interface ITaxRateFetcher
    {
        Task<Rate> FetchTaxRate(ETaxRate rate);
    }
}