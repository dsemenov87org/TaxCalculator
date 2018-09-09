using LinqToDB.Mapping;
using TaxCalculator.BusinessLogic;

namespace TaxCalculator.DataLayer
{
    [Table(Name = "amounts")]
    public sealed class AmountInDb
    {
        [PrimaryKey]
        public string Name { get; set; }
        
        [Column]
        public decimal Value { get; set; }
    }
}