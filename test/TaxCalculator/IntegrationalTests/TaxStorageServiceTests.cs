using System;
using System.Threading.Tasks;
using FluentMigrator.Runner;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TaxCalculator.BusinessLogic;
using TaxCalculator.DataLayer;

namespace IntegrationalTests
{
    [TestFixture]
    public class TaxStorageServiceTests
    {
        private string pghost = Env("PG_HOST", "db");
        private int pgport = Int32.Parse(Env("PG_PORT", "5432"));
        private string pguser = Env("PG_USER", "postgres");
        private string pgpasswd = Env("PG_PASSWD", string.Empty);
        private string dbname = Env("TAX_CALCULATOR_DB_NAME", "taxcalculator");
        private string redisHost = Env("REDIS_HOST", "cache");
        private readonly string redisInst = Env("REDIS_INSTANCE", "SampleInstance");

        private TaxStorageService _taxStorageService;

        [SetUp]
        public void Init()
        {
            var services = new ServiceCollection();

            InitServices(services);
            
            var serviceProvider = services.BuildServiceProvider();
            
            RunMigrations(serviceProvider);

            _taxStorageService =
                new TaxStorageService(
                    new TaxDatabaseSettings(pghost, pgport, pguser, pgpasswd, dbname),
                    serviceProvider.GetService<IDistributedCache>()
                );
        }

        [Test]
        public async Task TestFetchTaxAmount()
        {
            var amount = await _taxStorageService.FetchTaxAmount(ETaxAmount.PfrSelfAmount);
        }

        private void InitServices (IServiceCollection services)
        {
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = redisHost;
                options.InstanceName = redisInst;
            });

            services
                .AddFluentMigratorCore()
                .ConfigureRunner(rb =>
                    rb.AddPostgres()
                        .WithGlobalConnectionString(
                            $"server={pghost};userid={pguser};port={pgport};{PgPasswdEntry(pgpasswd)}database={dbname};")
                        .ScanIn(typeof(TaxStorageService).Assembly)
                        .For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                .BuildServiceProvider(false);
        }

        private static void RunMigrations (IServiceProvider serviceProvider)
        {
            // Put the database update into a scope to ensure
            // that all resources will be disposed.
            using (var scope = serviceProvider.CreateScope())
            {
                var runner =
                    scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

                runner.MigrateUp();
            }
        }

        private static string PgPasswdEntry(string passwd) =>
            string.IsNullOrEmpty(passwd)
                ? string.Empty
                : $"password={passwd};";

        private static string Env(string name, string orElse = null)
        {
            return Environment.GetEnvironmentVariable(name) ?? orElse
                ?? throw new ArgumentNullException($"Environment variable '{name}' is not set.");
        }
    }
}