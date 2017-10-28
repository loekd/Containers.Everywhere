using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Web.Repositories
{
    public class CounterStateDbContext : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;
        public const string ConnectionStringSettingName = "CounterStateStoreConnectionString";
        private readonly string _connectionString;


        /// <summary>
        /// Creates a new instance using the provided configuration.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="loggerFactory">Optional</param>
        public CounterStateDbContext(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            string connectionString = configuration.GetConnectionString(ConnectionStringSettingName);
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException($"Configuration value '{ConnectionStringSettingName}' cannot be null or whitespace.", nameof(configuration));

            _connectionString = connectionString;
        }

        /// <summary>
        /// Creates a new instance using the provided connection string.
        /// </summary>
        /// <param name="connectionString"></param>
        public CounterStateDbContext(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));

            _connectionString = connectionString;
        }

        /// <summary>
        /// Creates a new instance using the provided options.
        /// </summary>
        /// <param name="options"></param>
        internal CounterStateDbContext(DbContextOptions options)
            : base(options)
        {
        }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrWhiteSpace(_connectionString))
            {
                optionsBuilder.UseSqlServer(_connectionString, options =>
                {
                    options.EnableRetryOnFailure();
                });

            }
            if (_loggerFactory != null)
            {
                optionsBuilder.UseLoggerFactory(_loggerFactory);
            }
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<CounterState>(b =>
        //    {
        //        b.HasKey(e => e.Id);
        //        b.Property(e => e.Id).UseSqlServerIdentityColumn();
        //    });
        //}

        /// <summary>
        /// Holds CounterState
        /// </summary>
        public DbSet<CounterState> CounterStates { get; set; }
    }
}