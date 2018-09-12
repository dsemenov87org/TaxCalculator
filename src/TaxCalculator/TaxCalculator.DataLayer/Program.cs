using System;
using System.Linq;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace TaxCalculator.DataLayer
{
    class Program
    {
        public static void Main(string[] args)
        {
            if (args == null || args.Length < 1)
            {
                throw new ArgumentNullException("Database server hostname");
            }

            var services = new ServiceCollection();

            var serviceProvider =
                services
                    .AddFluentMigratorCore()
                    .ConfigureRunner(rb =>
                        rb.AddPostgres()
                            .WithGlobalConnectionString(
                                new TaxDatabaseSettings( 
                                    Environment.GetEnvironmentVariable("PG_HOST"),
                                    Environment.GetEnvironmentVariable("TAX_CALCULATOR_DB_NAME")
                                )
                                    .ConnectionStrings
                                    .Select(x => x.ConnectionString)
                                    .First())
                            .ScanIn(typeof(TaxStorageService).Assembly)
                            .For.Migrations())
                    .AddLogging(lb => lb.AddFluentMigratorConsole())
                    .BuildServiceProvider(false);

            var runner =
                serviceProvider.GetRequiredService<IMigrationRunner>();

            runner.MigrateUp();
        }

        public async Task 
    }
}