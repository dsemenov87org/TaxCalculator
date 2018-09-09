using System;
using TaxCalculator.BusinessLogic.Common;

namespace TaxCalculator.BusinessLogic
{
    /// <summary>
    /// параметры оплаты страховых взносов
    /// </summary>
    public sealed class InsuranceFeeParameters
    {
        public InsuranceFeeParameters(
            Rate pfrRate, 
            Rate ffomsRate,
            Rate fssRate,
            Rate additionalContribRate,
            RurMoney annualFreeIncomeBoundary)
        {
            if (annualFreeIncomeBoundary < RurMoney.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(annualFreeIncomeBoundary));
            }

            PfrRate = pfrRate;
            FfomsRate = ffomsRate;
            FssRate = fssRate;
            AdditionalContribRate = additionalContribRate;
            AnnualFreeIncomeBoundary = annualFreeIncomeBoundary;
        }

        /// <summary>
        /// процент взносов в Пенсионный фонд
        /// </summary>
        public Rate PfrRate { get; }

        /// <summary>
        /// процент взносов в Фонд медицинского страхования
        /// </summary>
        public Rate FfomsRate { get; }

        /// <summary>
        /// процент взносов в Фонд социального страхования
        /// </summary>
        public Rate FssRate { get; }

        /// <summary>
        ///  Если годовой доход превысил <see src="AnnualFreeIncomeBoundary" /> рублей, надо заплатить такой процент с этой суммы
        /// </summary>
        public Rate AdditionalContribRate { get; }

        /// <summary>
        /// Если годовой доход превысил эту сумму, надо заплатить <see src="AdditionalContribRate" />% с неё
        /// </summary>
        public RurMoney AnnualFreeIncomeBoundary { get; }
    }
}
