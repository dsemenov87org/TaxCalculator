using System.Collections.Generic;
using TaxCalculator.BusinessLogic;
using TaxCalculator.BusinessLogic.Common;

namespace BusinessLogic.TaxSystem.Calculator
{
    public abstract class IndividualTaxAggregate : TaxAggregate
    {
        protected IndividualTaxAggregate(
            SelfInsuranceFee selfInsuranceFee,
            RurMoney additionalFee,
            RurMoney feeTotal,
            EmployeeInsuranceFee employeeInsuranceFee,
            RurMoney ndfl,
            RurMoney total,
            decimal taxBurden
            ) : base(feeTotal, employeeInsuranceFee, ndfl, total, taxBurden)
        {
            InsuranceOneself = selfInsuranceFee;
            AdditionalInsuranceFee = additionalFee;
        }

        /// <summary>
        /// Взносы за себя
        /// </summary>
        public SelfInsuranceFee InsuranceOneself { get; }

        /// <summary>
        /// Взносы за доход свыше определенного
        /// </summary>
        public RurMoney AdditionalInsuranceFee { get; }

        public override bool Equals(TaxAggregate other)
        {
            if (other is IndividualTaxAggregate iudr)
            {
                return
                    AdditionalInsuranceFee == iudr.AdditionalInsuranceFee &&
                    InsuranceOneself == iudr.InsuranceOneself &&
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
            hashCode = hashCode * -1521134295 + EqualityComparer<SelfInsuranceFee>.Default.GetHashCode(InsuranceOneself);
            hashCode = hashCode * -1521134295 + EqualityComparer<RurMoney>.Default.GetHashCode(AdditionalInsuranceFee);
            return hashCode;
        }
    }

    public abstract class IndividualUsnTaxAggregate : IndividualTaxAggregate
    {
        protected IndividualUsnTaxAggregate(
            RurMoney sts,
            SelfInsuranceFee selfInsuranceFee,
            RurMoney additionalFee,
            RurMoney feeTotal,
            EmployeeInsuranceFee employeeFee,
            RurMoney employeeNdfl,
            RurMoney total,
            decimal burgen
            ) : base(selfInsuranceFee, additionalFee, feeTotal, employeeFee, employeeNdfl, total, burgen)
        {
            STS = sts;
        }

        /// <summary>
        /// Сумма УСН
        /// </summary>
        public RurMoney STS { get; }

        public override bool Equals(TaxAggregate other)
        {
            if (other is IndividualUsnTaxAggregate iudr)
            {
                return
                    STS == iudr.STS &&
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
            hashCode = hashCode * -1521134295 + EqualityComparer<RurMoney>.Default.GetHashCode(STS);
            return hashCode;
        }
    }
}
