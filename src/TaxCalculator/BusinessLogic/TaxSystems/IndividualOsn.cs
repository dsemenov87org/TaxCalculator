using System.Collections.Generic;
using BusinessLogic.TaxSystem.Calculator;
using TaxCalculator.BusinessLogic.Common;
using static TaxCalculator.BusinessLogic.CalculatorHelper;

namespace TaxCalculator.BusinessLogic.TaxSystems
{
    public sealed class IndividualOsnTaxParameters : OsnTaxParameters
    {
        public IndividualOsnTaxParameters(
            Rate ndsRate,
            SelfInsuranceFee selfInsuranceContributions,
            TaxParametersBase baseParams)
            : base(ndsRate, baseParams)
        {
            SelfInsuranceFee = selfInsuranceContributions;
        }
        
        public SelfInsuranceFee SelfInsuranceFee { get; }

        public override ECompanyType CompanyType => ECompanyType.IP;
    }

    public static class IndividualOsnTaxCalculator
    {
        public static IndividualOsnTaxAggregate Calculate(
            TaxCalculationBase @base,
            IndividualOsnTaxParameters @params)
        {
            var (salesNds, buyNds, totalNds) = CalculateNds(@params);

            var individualNdfl = CalculateIndividualNdfl();

            var (total, burgen) =
                CalculateTotal(individualNdfl + totalNds, @base.TotalFee, @params);

            return new IndividualOsnTaxAggregate(
                individualNdfl,
                totalNds, 
                @params.SelfInsuranceFee,
                @base.AdditionalFee,
                @base.TotalFee,
                @base.EmployeeFee,
                @base.EmployeeNdfl,
                total,
                burgen
                );

            RurMoney CalculateIndividualNdfl()
            {
                var expense = @params.CustomerTaxParameters.Expense - buyNds + @params.SelfInsuranceFee.Total;

                var totalExpense = expense
                    + @base.EmployeeFee.Total
                    + @base.EmployeeNdfl 
                    + @params.CustomerTaxParameters.Salary;

                var profit = @params.CustomerTaxParameters.Income - salesNds - totalExpense;

                return RurMoney.Round(@params.NdflRate.Value * profit);
            }
        }
    }

    public sealed class IndividualOsnTaxAggregate : IndividualTaxAggregate
    {
        public IndividualOsnTaxAggregate(
            RurMoney pit,
            RurMoney nds,
            SelfInsuranceFee selfInsuranceFee,
            RurMoney additionalFee,
            RurMoney feeTotal,
            EmployeeInsuranceFee employeeFee,
            RurMoney employeeNdfl,
            RurMoney total,
            decimal burgen
            ) : base(selfInsuranceFee, additionalFee, feeTotal, employeeFee, employeeNdfl, total, burgen)
        {
            PIT = pit;
            VAT = nds;
        }

        /// <summary>
        /// Сумма НДФЛ
        /// </summary>
        public RurMoney PIT { get; }

        /// <summary>
        /// Сумма НДС
        /// </summary>
        public RurMoney VAT { get; }

        public override bool Equals(TaxAggregate other)
        {
            if (other is IndividualOsnTaxAggregate iud)
            {
                return
                    PIT == iud.PIT &&
                    VAT == iud.VAT &&
                    base.Equals(iud);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            var hashCode = 284677906;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<RurMoney>.Default.GetHashCode(PIT);
            hashCode = hashCode * -1521134295 + EqualityComparer<RurMoney>.Default.GetHashCode(VAT);
            return hashCode;
        }
    }
}
