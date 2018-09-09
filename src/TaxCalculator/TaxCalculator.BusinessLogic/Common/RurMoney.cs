using System;

namespace TaxCalculator.BusinessLogic.Common
{
    /// <summary>
    /// Деньги в рублях
    /// </summary>
    public struct RurMoney : IEquatable<RurMoney>
    {
        public RurMoney(decimal amount) : this()
        {
            Amount = Math.Round(amount * 100) / 100;
        }

        public decimal Amount { get; }

        public static RurMoney operator +(RurMoney r1, RurMoney r2)
        {
            return new RurMoney(r1.Amount + r2.Amount);
        }

        public static RurMoney operator -(RurMoney r1, RurMoney r2)
        {
            return new RurMoney(r1.Amount - r2.Amount);
        }

        public static RurMoney operator *(decimal koeff, RurMoney r)
        {
            return new RurMoney(r.Amount * koeff);
        }

        public static bool operator ==(RurMoney left, RurMoney right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RurMoney left, RurMoney right)
        {
            return !(left.Equals(right));
        }

        public static bool operator >(RurMoney left, RurMoney right)
        {
            return left.Amount > right.Amount;
        }

        public static bool operator <(RurMoney left, RurMoney right)
        {
            return left.Amount < right.Amount;
        }

        public static bool operator >=(RurMoney left, RurMoney right)
        {
            return left == right || left > right;
        }

        public static bool operator <=(RurMoney left, RurMoney right)
        {
            return left == right || left < right;
        }

        public static RurMoney Min(RurMoney left, RurMoney right)
        {
            return left <= right ? left : right;
        }

        public static RurMoney Max(RurMoney left, RurMoney right)
        {
            return left <= right ? right : left;
        }

        public static RurMoney Round(RurMoney rur)
        {
            return new RurMoney(Math.Round(rur.Amount));
        }

        public static RurMoney Zero => new RurMoney(0m);

        public override bool Equals(object obj)
        {
            if ((obj is RurMoney))
            {
                return Equals((RurMoney)obj);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(RurMoney money)
        {
            return Amount == money.Amount;
        }

        public override int GetHashCode()
        {
            var hashCode = 608998249;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + Amount.GetHashCode();
            return hashCode;
        }
 
        public override string ToString()
        {
            return Amount.ToString();
        }
    }
}
