using System;
using System.Threading.Tasks;
using TaxCalculator.BusinessLogic;
using TaxCalculator.BusinessLogic.Common;

namespace TaxCalculator.Tests.BusinessLogic
{
    public sealed class MockTaxRateFetcher : ITaxRateFetcher
    {
        public Task<Rate> FetchTaxRate(ETaxRate rate)
        {
            return Task.FromResult(GetRate(rate));
        }

        public Rate GetRate(ETaxRate rateType)
        {
            switch (rateType)
            {
                case ETaxRate.AdditionalFeeRate:
                    return new Rate(0.01m);

                case ETaxRate.FomsRate:
                    return new Rate(0.051m);

                case ETaxRate.FssRate:
                    return new Rate(0.029m);

                case ETaxRate.MinTaxRate:
                    return new Rate(0.01m);

                case ETaxRate.NdflRate:
                    return new Rate(0.13m);

                case ETaxRate.NdsRate:
                    return new Rate(0.18m);

                case ETaxRate.PfrRate:
                    return new Rate(0.22m);

                case ETaxRate.ProfitRate:
                    return new Rate(0.2m);

                case ETaxRate.UsnDRate:
                    return new Rate(0.06m);

                case ETaxRate.UsnDRRate:
                    return new Rate(0.15m);

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
