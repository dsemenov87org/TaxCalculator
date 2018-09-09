using System;
using System.Collections.Generic;
using TaxCalculator.BusinessLogic.Common;

namespace TaxCalculator.BusinessLogic
{
    /// <summary>
    /// Страховые взносы за себя
    /// </summary>
    public struct SelfInsuranceFee : IEquatable<SelfInsuranceFee>
    {
        public SelfInsuranceFee(RurMoney pfr, RurMoney ffoms)
        {
            if (pfr < RurMoney.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(pfr));
            }
            if (ffoms < RurMoney.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(ffoms));
            }

            Pension = pfr;
            Health = ffoms;
            Total = pfr + ffoms;
        }

        /// <summary>
        /// Взносы в пенсионный фонд
        /// </summary>
        public RurMoney Pension { get; }

        /// <summary>
        /// Взносы в фонд медицинского страхования
        /// </summary>
        public RurMoney Health { get; }
        
        public RurMoney Total { get; }

        public bool Equals(SelfInsuranceFee other)
        {
            return Pension == other.Pension && Health == other.Health;
        }

        public override bool Equals(object obj)
        {
            if (obj is SelfInsuranceFee)
            {
                return Equals((SelfInsuranceFee)obj);
            }
            else
            {
                return false;
            }
        }

        public static bool operator ==(SelfInsuranceFee l, SelfInsuranceFee r)
        {
            return l.Equals(r);
        }

        public static bool operator !=(SelfInsuranceFee l, SelfInsuranceFee r)
        {
            return !(l.Equals(r));
        }

        public override int GetHashCode()
        {
            var hashCode = 1577008601;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<RurMoney>.Default.GetHashCode(Pension);
            hashCode = hashCode * -1521134295 + EqualityComparer<RurMoney>.Default.GetHashCode(Health);
            return hashCode;
        }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
