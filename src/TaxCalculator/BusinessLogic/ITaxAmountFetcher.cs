using System.Threading.Tasks;
using TaxCalculator.BusinessLogic.Common;

namespace TaxCalculator.BusinessLogic
{
    public interface ITaxAmountFetcher
    {
        Task<RurMoney> FetchTaxAmount(ETaxAmount amount);
    }
}