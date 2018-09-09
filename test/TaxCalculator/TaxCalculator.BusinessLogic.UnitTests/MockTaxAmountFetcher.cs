using System;
using System.Threading.Tasks;
using TaxCalculator.BusinessLogic;
using TaxCalculator.BusinessLogic.Common;

namespace TaxCalculator.Tests.BusinessLogic
{
    public sealed class MockTaxAmountFetcher : ITaxAmountFetcher
    {
        public Task<RurMoney> FetchTaxAmount(ETaxAmount taxAmountType)
        {
            return Task.FromResult(new RurMoney(GetDecimal(taxAmountType)));
        }

        private static decimal GetDecimal(ETaxAmount taxAmountType)
        {
            switch (taxAmountType)
            {
                case ETaxAmount.AdditionalFeeLimit  : return 300000m;
                case ETaxAmount.FomsSelfAmount      : return 5840m;
                case ETaxAmount.PfrSelfAmount       : return 26545m;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
