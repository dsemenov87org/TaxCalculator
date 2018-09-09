using System.Collections.Generic;
using TaxCalculator.BusinessLogic.Common;
using static TaxCalculator.BusinessLogic.CalculatorHelper;

namespace TaxCalculator.BusinessLogic.TaxSystems
{
    public sealed class OrganizationUsnD_RTaxParameters : UsnTaxParameters
    {
        public OrganizationUsnD_RTaxParameters(
            Rate minTaxRate,
            Rate usnRate,
            TaxParametersBase baseParams)
            : base(usnRate, baseParams)
        {
            MinTaxRate = minTaxRate;
        }

        public override ECompanyType CompanyType => ECompanyType.OOO;

        public override EUsnType UsnType => EUsnType.IncomeExpense;

        public Rate MinTaxRate { get; }
    }

    public sealed class OrgUsnD_RTaxCalculator
    {
        public static OrgUsnD_RTaxAggregate Calculate(
            TaxCalculationBase @base,
            OrganizationUsnD_RTaxParameters @params)
        {
            var totalExpenses = @base.EmployeeFee.Total
                + @base.EmployeeNdfl
                + @params.CustomerTaxParameters.Expense
                + @params.CustomerTaxParameters.Salary;

            var chargedTax = CalculateUsnDRChargedTax(@params, totalExpenses);

            var sts = RurMoney.Max(chargedTax, @params.MinTaxRate.Value * @params.CustomerTaxParameters.Income);

            var (total, burgen) = CalculateTotal(sts + @base.EmployeeNdfl, @base.TotalFee, @params);

            return new OrgUsnD_RTaxAggregate(
                totalExpenses,
                sts,
                @base.EmployeeFee,
                @base.EmployeeNdfl,
                @base.TotalFee,
                total,
                burgen
                );
        }
    }

    /// <summary>
    /// результат расчета калькулятора для ООО УСН Д-Р
    /// </summary>
    public sealed class OrgUsnD_RTaxAggregate : TaxAggregate
    {
        public OrgUsnD_RTaxAggregate(
            RurMoney taxExpenses,
            RurMoney sts,
            EmployeeInsuranceFee employeeFee,
            RurMoney ndfl,
            RurMoney feeTotal,
            RurMoney total,
            decimal burgen
            ) : base(feeTotal, employeeFee, ndfl, total, burgen)
        {
            TaxableExpenses = taxExpenses;
            STS = sts;
        }

        public RurMoney STS { get; }

        /// <summary>
        /// Расходы, принимаемые для целей налогообложения
        /// </summary>
        public RurMoney TaxableExpenses { get; }

        public override bool Equals(TaxAggregate other)
        {
            if (other is OrgUsnD_RTaxAggregate oudr)
            {
                return
                    base.Equals(oudr) &&
                    STS == oudr.STS &&
                    TaxableExpenses == oudr.TaxableExpenses;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            var hashCode = 1470217011;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<RurMoney>.Default.GetHashCode(STS);
            hashCode = hashCode * -1521134295 + EqualityComparer<RurMoney>.Default.GetHashCode(TaxableExpenses);
            return hashCode;
        }
    }
}
