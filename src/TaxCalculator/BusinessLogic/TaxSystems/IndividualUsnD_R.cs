using System.Collections.Generic;
using BusinessLogic.TaxSystem.Calculator;
using TaxCalculator.BusinessLogic.Common;
using static TaxCalculator.BusinessLogic.CalculatorHelper;

namespace TaxCalculator.BusinessLogic.TaxSystems
{
    public sealed class IndividualUsnD_RTaxParameters : IndividualUsnTaxParameters
    {
        public IndividualUsnD_RTaxParameters(
            Rate minTaxRate,
            SelfInsuranceFee selfInsuranceContributions,
            Rate usnRate,
            TaxParametersBase baseParams)
            : base(selfInsuranceContributions, usnRate, baseParams)
        {
            MinTaxRate = minTaxRate;
        }

        public Rate MinTaxRate { get; }

        public override EUsnType UsnType => EUsnType.IncomeExpense;
    }

    public static class IndividualUsnD_RTaxCalculator
    {
        public static IndividualUsnD_RTaxAggregate Calculate(
            TaxCalculationBase @base,
            IndividualUsnD_RTaxParameters @params)
        {
            var totalExpenses = @params.SelfInsuranceFee.Total
                + @base.EmployeeFee.Total
                + @base.EmployeeNdfl
                + @params.CustomerTaxParameters.Expense
                + @params.CustomerTaxParameters.Salary;

            var chargedTax = CalculateUsnDRChargedTax(@params, totalExpenses);

            var taxTotal = RurMoney.Max(chargedTax, @params.MinTaxRate.Value * @params.CustomerTaxParameters.Income);

            var (total, burgen) = CalculateTotal(taxTotal + @base.EmployeeNdfl, @base.TotalFee, @params);

            return new IndividualUsnD_RTaxAggregate(
                totalExpenses,
                taxTotal,
                @params.SelfInsuranceFee,
                @base.EmployeeFee,
                @base.AdditionalFee,
                @base.EmployeeNdfl,
                @base.TotalFee,
                total,
                burgen
                );
        }
    }

    /// <summary>
    /// результат расчета калькулятора для ИП УСН Д-Р
    /// </summary>
    public sealed class IndividualUsnD_RTaxAggregate : IndividualUsnTaxAggregate
    {
        public IndividualUsnD_RTaxAggregate(
            RurMoney taxExpenses,
            RurMoney sts,
            SelfInsuranceFee selfInsuranceFee,
            EmployeeInsuranceFee employeeFee,
            RurMoney additionalFee,
            RurMoney employeeNdfl,
            RurMoney feeTotal,
            RurMoney total,
            decimal burgen
            ) : base(sts, selfInsuranceFee, additionalFee, feeTotal, employeeFee, employeeNdfl, total, burgen)
        {
            TaxableExpenses = taxExpenses;
        }

        /// <summary>
        /// Расходы, принимаемые для целей налогообложения
        /// </summary>
        public RurMoney TaxableExpenses { get; }
        
        public override bool Equals(TaxAggregate other)
        {
            if (other is IndividualUsnD_RTaxAggregate iudr)
            {
                return
                    TaxableExpenses == iudr.TaxableExpenses &&
                    base.Equals(iudr);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            var hashCode = 208538877;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<RurMoney>.Default.GetHashCode(TaxableExpenses);
            return hashCode;
        }
    }
}
