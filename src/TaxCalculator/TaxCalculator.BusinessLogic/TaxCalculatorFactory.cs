using System;
using System.Threading.Tasks;
using TaxCalculator.BusinessLogic.Common;
using TaxCalculator.BusinessLogic.TaxSystems;
using static TaxCalculator.BusinessLogic.CalculatorHelper;

namespace TaxCalculator.BusinessLogic
{
    public sealed class TaxCalculatorFactory
    {
        private readonly ITaxAmountFetcher _taxAmountFetcher;
        private readonly ITaxRateFetcher _taxRateFetcher;

        public TaxCalculatorFactory(ITaxAmountFetcher taxAmountFetcher, ITaxRateFetcher taxRateFetcher)
        {
            _taxAmountFetcher = taxAmountFetcher;
            _taxRateFetcher = taxRateFetcher;
        }

        public async Task<Func<CustomerTaxParameters, Task<TaxAggregate>>> CreateCalculator(
            ECompanyType companyType,
            EAccountTaxationSystem taxSystem)
        {
            var selfContribs = 
                new SelfInsuranceFee(
                    await _taxAmountFetcher.FetchTaxAmount(ETaxAmount.PfrSelfAmount),
                    await _taxAmountFetcher.FetchTaxAmount(ETaxAmount.FomsSelfAmount));

            var usnDRate =
                await _taxRateFetcher.FetchTaxRate(ETaxRate.UsnDRate);

            var usnDRRate =
                await _taxRateFetcher.FetchTaxRate(ETaxRate.UsnDRRate);

            var ndsRate =
                await _taxRateFetcher.FetchTaxRate(ETaxRate.NdsRate);

            var minTaxRate =
                await _taxRateFetcher.FetchTaxRate(ETaxRate.MinTaxRate);

            var profitRate =
                await _taxRateFetcher.FetchTaxRate(ETaxRate.ProfitRate);

            switch (companyType)
            {
                case ECompanyType.IP:
                    return CreateIndividualCalculator();

                case ECompanyType.OOO:
                    return CreateOrganizationCalculator();

                default:
                    throw new NotSupportedException();
            }

            async Task<TaxParametersBase> BaseParams(CustomerTaxParameters customerTaxParameters)
            {
                return new TaxParametersBase(
                    customerTaxParameters,
                    new InsuranceFeeParameters(
                        await _taxRateFetcher.FetchTaxRate(ETaxRate.PfrRate),
                        await _taxRateFetcher.FetchTaxRate(ETaxRate.FomsRate),
                        await _taxRateFetcher.FetchTaxRate(ETaxRate.FssRate),
                        await _taxRateFetcher.FetchTaxRate(ETaxRate.AdditionalFeeRate),
                        await _taxAmountFetcher.FetchTaxAmount(ETaxAmount.AdditionalFeeLimit)
                    ),
                    await _taxRateFetcher.FetchTaxRate(ETaxRate.NdflRate));
            }

            TaxCalculationBase BaseComputations(TaxParameters @params, SelfInsuranceFee selfFee = default(SelfInsuranceFee))
            {
                var employeeFee = CalculateEmployeeFee(@params);
                var employeeNdfl = CalculateEmployeeNdfl(@params);
                var additionalFee = CalculateAdditionalInsuranceFee(@params);

                return new TaxCalculationBase(
                    employeeFee,
                    employeeNdfl,
                    additionalFee,
                    CalculateTotalFee(employeeFee, additionalFee, selfFee, @params)
                    );
            }

            Func<CustomerTaxParameters, Task<TaxAggregate>> CreateIndividualCalculator()
            {
                switch (taxSystem)
                {
                    case EAccountTaxationSystem.Usn6:
                        return async (customerParams) => {
                            var @params = new IndividualUsnDTaxParameters(
                                selfContribs,
                                usnDRate,
                                await BaseParams(customerParams)
                            );

                            return IndividualUsnDTaxCalculator.Calculate(
                                BaseComputations(@params, selfContribs),
                                @params);
                        };

                    case EAccountTaxationSystem.Usn15:
                        return async (customerParams) =>
                        {
                            var @params = new IndividualUsnD_RTaxParameters(
                                minTaxRate,
                                selfContribs,
                                usnDRRate,
                                await BaseParams(customerParams)
                            );

                            return IndividualUsnD_RTaxCalculator.Calculate(
                                BaseComputations(@params, selfContribs),
                                @params);
                        };

                    case EAccountTaxationSystem.Osn:
                        return async (customerParams) =>
                        {
                            var @params = new IndividualOsnTaxParameters(
                                ndsRate,
                                selfContribs,
                                await BaseParams(customerParams)
                            );

                            return IndividualOsnTaxCalculator.Calculate(
                                BaseComputations(@params, selfContribs),
                                @params);
                        };

                    default:
                        throw new NotSupportedException();
                }
            }

            Func<CustomerTaxParameters, Task<TaxAggregate>> CreateOrganizationCalculator()
            {
                switch (taxSystem)
                {
                    case EAccountTaxationSystem.Usn6:
                        return async (customerParams) =>
                        {
                            var @params = new OrganizationUsnDTaxParameters(
                                usnDRate,
                                await BaseParams(customerParams)
                            );

                            return OrgUsnDTaxCalculator.Calculate(
                                BaseComputations(@params),
                                @params);
                        };

                    case EAccountTaxationSystem.Usn15:
                        return async (customerParams) => {
                            var @params = new OrganizationUsnD_RTaxParameters(
                                minTaxRate,
                                usnDRRate,
                                await BaseParams(customerParams)
                            );

                            return OrgUsnD_RTaxCalculator.Calculate(
                                BaseComputations(@params),
                                @params);
                        };

                    case EAccountTaxationSystem.Osn:
                        return async (customerParams) => {
                            var @params = new OrganizationOsnTaxParameters(
                                profitRate,
                                ndsRate,
                                await BaseParams(customerParams)
                            );

                            return OrgOsnTaxCalculator.Calculate(
                                BaseComputations(@params),
                                @params);
                        };

                    default:
                        throw new NotSupportedException();
                }
            }
        }
    }
}
