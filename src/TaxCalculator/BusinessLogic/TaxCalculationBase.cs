using System;
using TaxCalculator.BusinessLogic.Common;

namespace TaxCalculator.BusinessLogic
{
    public sealed class TaxCalculationBase
    {
        public TaxCalculationBase(
            EmployeeInsuranceFee employeeFee,
            RurMoney employeeNdfl,
            RurMoney additionalFee,
            RurMoney totalFee
            )
        {
            if (employeeNdfl < RurMoney.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(employeeNdfl));
            }
            if (additionalFee < RurMoney.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(additionalFee));
            }
            if (totalFee < RurMoney.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(totalFee));
            }

            EmployeeFee = employeeFee;
            EmployeeNdfl = employeeNdfl;
            AdditionalFee = additionalFee;
            TotalFee = totalFee;
        }

        public EmployeeInsuranceFee EmployeeFee { get; }
        public RurMoney EmployeeNdfl { get; }
        public RurMoney AdditionalFee { get; }
        public RurMoney TotalFee { get; }
    }
}
