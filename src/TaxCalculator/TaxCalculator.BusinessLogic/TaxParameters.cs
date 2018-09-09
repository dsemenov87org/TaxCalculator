using System;
using TaxCalculator.BusinessLogic.Common;

namespace TaxCalculator.BusinessLogic
{
    public sealed class CustomerTaxParameters
    {
        /// <param name="income">доход</param>
        /// <param name="expense">расход</param>
        /// <param name="salary">зарплата сотрудников</param>
        public CustomerTaxParameters(RurMoney income, RurMoney expense, RurMoney salary)
        {
            if (income < RurMoney.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(income));
            }
            if (expense < RurMoney.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(expense));
            }
            if (salary < RurMoney.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(salary));
            }

            Income = income;
            Expense = expense;
            Salary = salary;
        }

        /// <summary>
        /// доход
        /// </summary>
        public RurMoney Income { get; }

        /// <summary>
        /// расход
        /// </summary>
        public RurMoney Expense { get; }

        /// <summary>
        /// зарплата сотрудников
        /// </summary>
        public RurMoney Salary { get; }
    }

    public sealed class TaxParametersBase
    {
        public TaxParametersBase(
            CustomerTaxParameters customerTaxParameters,
            InsuranceFeeParameters insuranceFeeRates,
            Rate ndflRate)
        {
            CustomerTaxParameters = customerTaxParameters
                ?? throw new ArgumentNullException(nameof(customerTaxParameters));

            InsuranceFeeParams = insuranceFeeRates
                ?? throw new ArgumentNullException(nameof(insuranceFeeRates));

            NdflRate = ndflRate;
        }

        public CustomerTaxParameters CustomerTaxParameters { get; }

        public InsuranceFeeParameters InsuranceFeeParams { get; }

        /// <summary>
        /// процент НДФЛ
        /// </summary>
        public Rate NdflRate { get; }
    }

    /// <summary>
    /// Параметры для калькулятора налогов
    /// </summary>
    public abstract class TaxParameters
    {
        public TaxParameters(TaxParametersBase @base)
        {
            if (@base == null)
            {
                throw new ArgumentNullException(nameof(@base));
            }

            CustomerTaxParameters = @base.CustomerTaxParameters;

            InsuranceFeeParams = @base.InsuranceFeeParams;

            NdflRate = @base.NdflRate;
        }

        public abstract ECompanyType CompanyType { get; }

        public CustomerTaxParameters CustomerTaxParameters { get; }

        public InsuranceFeeParameters InsuranceFeeParams { get; }

        /// <summary>
        /// процент НДФЛ
        /// </summary>
        public Rate NdflRate { get; }
    }

    public enum EUsnType
    {
        Income,
        IncomeExpense
    }

    public abstract class UsnTaxParameters : TaxParameters
    {
        public UsnTaxParameters(Rate usnRate, TaxParametersBase baseParams)
            : base(baseParams)
        {
            UsnRate = usnRate;
        }

        public abstract EUsnType UsnType { get; }

        /// <summary>
        /// процент УСН
        /// </summary>
        public Rate UsnRate { get; }
    }

    public abstract class OsnTaxParameters : TaxParameters
    {
        public OsnTaxParameters(Rate ndsRate, TaxParametersBase baseParams)
            : base(baseParams)
        {
            NdsRate = ndsRate;
        }

        /// <summary>
        /// процент НДС
        /// </summary>
        public Rate NdsRate { get; }
    }

    public abstract class IndividualUsnTaxParameters : UsnTaxParameters
    {
        public IndividualUsnTaxParameters(
            SelfInsuranceFee selfInsuranceFees,
            Rate usnRate,
            TaxParametersBase baseParams)
            : base(usnRate, baseParams)
        {
            SelfInsuranceFee = selfInsuranceFees;
        }

        public override ECompanyType CompanyType => ECompanyType.IP;

        /// <summary>
        /// страховые взносы за себя
        /// </summary>
        public SelfInsuranceFee SelfInsuranceFee { get; }
    }
}
