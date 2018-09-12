using System;
using System.Linq;
using System.Threading.Tasks;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace TaxCalculator.DataLayer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var services = new ServiceCollection();

            MigrateUp(
                services,
                Environment.GetEnvironmentVariable("PG_HOST"),
                Environment.GetEnvironmentVariable("TAX_CALCULATOR_DB_NAME")
                ).Wait();
        }

        public static async Task MigrateUp(IServiceCollection services, string host, string dbname)
        {
            var connectionStr = $"server={host};userid=postgres;port=5432";

            Console.WriteLine(connectionStr);

            using (var conn = new NpgsqlConnection(connectionStr))
            using (var cmd = conn.CreateCommand())
            {
                await conn.OpenAsync();

                cmd.CommandText = $"SELECT count(*) FROM pg_database WHERE datname = '{dbname}'";
                var d = await cmd.ExecuteScalarAsync();
                if ((long)d == 0)
                {
                    cmd.CommandText = $@"CREATE DATABASE {dbname} WITH OWNER postgres";

                    await cmd.ExecuteNonQueryAsync();
                }
            }

            var serviceProvider =
                services
                    .AddFluentMigratorCore()
                    .ConfigureRunner(rb =>
                        rb.AddPostgres()
                            .WithGlobalConnectionString(
                                new TaxDatabaseSettings(host, dbname)
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
    }
}