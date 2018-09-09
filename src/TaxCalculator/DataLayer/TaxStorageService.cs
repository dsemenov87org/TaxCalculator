using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using TaxCalculator.BusinessLogic;
using TaxCalculator.BusinessLogic.Common;

namespace TaxCalculator.DataLayer
{
    public sealed class TaxStorageService : ITaxRateFetcher, ITaxAmountFetcher
    {
        private readonly IDistributedCache _cache;

        public TaxStorageService(TaxDatabaseSettings settings, IDistributedCache cache)
		{
            DataConnection.DefaultSettings = settings;

            _cache = cache;
        }

        public Task<RurMoney> FetchTaxAmount(ETaxAmount amount)
        {
            return UsingCache(amount, (raw) => new RurMoney(Decimal.Parse(raw)), (db, key) =>
                db.Amounts
                    .Where(a => a.Name == key.ToString())
                    .Select(a => new RurMoney(a.Value))
                    .FirstOrDefaultAsync()
            );
        }

        public Task<Rate> FetchTaxRate(ETaxRate rate)
        {
            return UsingCache(rate, (raw) => new Rate(Decimal.Parse(raw)), (db, key) =>
                db.Rates
                    .Where(a => a.Name == key.ToString())
                    .Select(a => new Rate(a.Value))
                    .FirstOrDefaultAsync()
            );
        }

        private async Task<U> UsingCache<T, U>(
            T key,
            Func<string, U> parse,
            Func<DataConnection, T, Task<U>> getFromStorage
            )
        {
            var cacheEntry = $"{GetType().Name}_{key}";

            var cached = await _cache.GetStringAsync(cacheEntry);

            if (!string.IsNullOrEmpty(cached))
            {
                return parse(cached);
            }

            using (var db = new DataConnection())
            {
                var stored = await getFromStorage(db, key);

                await _cache.SetStringAsync(cacheEntry, stored.ToString(),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                    });

                return stored;
            }
        }
    }

    class DataConnection : LinqToDB.Data.DataConnection
    {
        public ITable<AmountInDb> Amounts   => GetTable<AmountInDb>();
        public ITable<RateInDb> Rates       => GetTable<RateInDb>();
    }

    class ConnectionStringSettings : IConnectionStringSettings
    {
        public string ConnectionString { get; set; }
        public string Name { get; set; }
        public string ProviderName { get; set; }
        public bool IsGlobal => false;
    }

    public class TaxDatabaseSettings : ILinqToDBSettings
    {
        readonly ConnectionStringSettings _connectionString;

        public TaxDatabaseSettings(string host, int port, string user, string passwd, string dbname)
        {
            var passwordEntry =
                string.IsNullOrEmpty(passwd)
                    ? string.Empty
                    : $"password={passwd};";

            _connectionString =
                new ConnectionStringSettings
                {
                    Name = DefaultConfiguration,
                    ProviderName = DefaultDataProvider,
                    ConnectionString =
                        $"server={host};userid={user};port={port};{passwordEntry}database={dbname};"
                };
        }

        public IEnumerable<IDataProviderSettings> DataProviders =>
            Enumerable.Empty<IDataProviderSettings>();

        public string DefaultConfiguration => "Main";
        public string DefaultDataProvider => ProviderName.PostgreSQL;

        public IEnumerable<IConnectionStringSettings> ConnectionStrings
        {
            get
            {
                yield return _connectionString;
            }
        }
    }
}