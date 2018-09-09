using FluentMigrator;
using TaxCalculator.BusinessLogic;
using TaxCalculator.DataLayer;

namespace TaxCalculator.DataLayer.Migrations
{
    [Migration(1, "initial")]
    public class M0001 : Migration
    {
        const string RatesTable = "rates";
        const string AmountTable = "amounts";

        public override void Up()
        {
            Create.Table(RatesTable)
                .WithColumn("Name")
                    .AsString()
                    .PrimaryKey()
                .WithColumn("Value")
                    .AsDecimal()
                    .NotNullable()
                ;

            var rates = new[]
            {
                (ETaxRate.UsnDRRate,            0.15m),
                (ETaxRate.UsnDRate,             0.06m),
                (ETaxRate.AdditionalFeeRate,    0.01m),
                (ETaxRate.PfrRate,              0.22m),
                (ETaxRate.FomsRate,             0.051m),
                (ETaxRate.FssRate,              0.029m),
                (ETaxRate.ProfitRate,           0.02m),
                (ETaxRate.NdsRate,              0.18m)
            };

            foreach (var (rate, value) in rates)
            {
                Insert.IntoTable(RatesTable)
                    .Row(new RateInDb{ Name = rate.ToString(), Value = value});
            }

            Create.Table(AmountTable)
                .WithColumn("Name")
                    .AsString()
                    .PrimaryKey()
                .WithColumn("Value")
                    .AsDecimal()
                    .NotNullable()
                ;

            var amounts = new[]
            {
                (ETaxAmount.PfrSelfAmount,       26545m),
                (ETaxAmount.FomsSelfAmount,      5840m),
                (ETaxAmount.AdditionalFeeLimit,  300000m),
            };

            foreach (var (amount, value) in amounts)
            {
                Insert.IntoTable(AmountTable)
                    .Row(new AmountInDb{Name = amount.ToString(), Value = value});
            }
        }

        public override void Down()
        {
            Delete.Table(RatesTable);
            Delete.Table(AmountTable);
        }
    }
}