using System;
using LinqToDB.Mapping;
using TaxCalculator.BusinessLogic;

namespace TaxCalculator.DataLayer
{
    [Table(Name = "rates")]
    public sealed class RateInDb
    {
        [PrimaryKey]
        public string Name { get; set; }

        [Column]
        public decimal Value { get; set; }
    }
}