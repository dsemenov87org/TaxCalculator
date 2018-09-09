using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TaxCalculator.BusinessLogic.Common;

namespace TaxCalculator.BusinessLogic
{
    /// <summary>
    /// результат расчета калькулятора
    /// </summary>
    public abstract class TaxAggregate : IEquatable<TaxAggregate>
    {
        protected TaxAggregate(
            RurMoney insuranceContributionsTotal,
            EmployeeInsuranceFee employeeFee,
            RurMoney ndfl,
            RurMoney total,
            decimal taxBurden)
        {
            EmployeeFee = employeeFee;
            EmployeeNdfl = ndfl;
            InsuranceContributions = insuranceContributionsTotal;
            Total = total;
            TaxBurden = taxBurden;
        }

        public EmployeeInsuranceFee EmployeeFee { get; }
        public RurMoney EmployeeNdfl { get; }
        public RurMoney InsuranceContributions { get; }
        public RurMoney Total { get; }

        /// <summary>
        /// Налоговая нагрузка, %
        /// </summary>
        public decimal TaxBurden { get; }

        public virtual bool Equals(TaxAggregate other)
        {
            return 
                EmployeeFee == other.EmployeeFee &&
                InsuranceContributions == other.InsuranceContributions &&
                EmployeeFee == other.EmployeeFee &&
                EmployeeNdfl == other.EmployeeNdfl &&
                TaxBurden == other.TaxBurden &&
                Total == other.Total;
        }

        public override bool Equals(object other)
        {
            return Equals(other as TaxAggregate);
        }

        public override int GetHashCode()
        {
            var hashCode = 1717186281;
            hashCode = hashCode * -1521134295 + EqualityComparer<EmployeeInsuranceFee>.Default.GetHashCode(EmployeeFee);
            hashCode = hashCode * -1521134295 + EqualityComparer<RurMoney>.Default.GetHashCode(EmployeeNdfl);
            hashCode = hashCode * -1521134295 + EqualityComparer<RurMoney>.Default.GetHashCode(InsuranceContributions);
            hashCode = hashCode * -1521134295 + EqualityComparer<decimal>.Default.GetHashCode(TaxBurden);
            hashCode = hashCode * -1521134295 + EqualityComparer<RurMoney>.Default.GetHashCode(Total);
            return hashCode;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
