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
        private TaxStorageService _taxStorageService;

        [SetUp]
        public void Init()
        {
            var services = new ServiceCollection();

            InitServices(services);
            
            var serviceProvider = services.BuildServiceProvider();

            _taxStorageService =
                new TaxStorageService(
                    new TaxDatabaseSettings(Env("PG_HOST"), Env("TAX_CALCULATOR_DB_NAME")),
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
                options.Configuration = Env("REDIS_HOST");
                options.InstanceName = "SampleInstance";
            });
        }
        private static string Env(string name)
        {
            return Environment.GetEnvironmentVariable(name)
                ?? throw new ArgumentNullException($"Environment variable '{name}' is not set.");
        }
    }
}