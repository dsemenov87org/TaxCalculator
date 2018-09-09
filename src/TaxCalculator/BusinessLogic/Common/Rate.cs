using System;

namespace TaxCalculator.BusinessLogic.Common
{
    /// <summary>
    /// Процент
    /// </summary>
    public struct Rate
    {
        public Rate(decimal value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            Value = value;
        }

        public decimal Value { get; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
