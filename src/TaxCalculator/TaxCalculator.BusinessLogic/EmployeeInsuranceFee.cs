using System;
using System.Collections.Generic;
using TaxCalculator.BusinessLogic.Common;

namespace TaxCalculator.BusinessLogic
{
    /// <summary>
    /// Страховые взносы за сотрудников
    /// </summary>
    public struct EmployeeInsuranceFee : IEquatable<EmployeeInsuranceFee>
    {
        public EmployeeInsuranceFee(RurMoney pfr, RurMoney ffoms, RurMoney fss)
        {
            if (fss < RurMoney.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(pfr));
            }
            if (pfr < RurMoney.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(pfr));
            }
            if (ffoms < RurMoney.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(ffoms));
            }

            PFR = pfr;
            FFOMS = ffoms;
            FSS = fss;
            
            Total = PFR + FFOMS + FSS;
        }

        public RurMoney PFR { get; }
        public RurMoney FFOMS { get; }
        public RurMoney FSS { get; }

        public RurMoney Total { get; }

        public bool Equals(EmployeeInsuranceFee other)
        {
            return PFR == other.PFR && FFOMS == other.FFOMS && FSS == other.FSS;
        }

        public override bool Equals(object obj)
        {
            if (obj is EmployeeInsuranceFee eic)
            {
                return Equals(eic);
            }
            else
            {
                return false;
            }
        }

        public static bool operator ==(EmployeeInsuranceFee l, EmployeeInsuranceFee r)
        {
            return l.Equals(r);
        }

        public static bool operator !=(EmployeeInsuranceFee l, EmployeeInsuranceFee r)
        {
            return !(l.Equals(r));
        }

        public override int GetHashCode()
        {
            var hashCode = 1009164728;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<RurMoney>.Default.GetHashCode(PFR);
            hashCode = hashCode * -1521134295 + EqualityComparer<RurMoney>.Default.GetHashCode(FFOMS);
            hashCode = hashCode * -1521134295 + EqualityComparer<RurMoney>.Default.GetHashCode(FSS);
            return hashCode;
        }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
