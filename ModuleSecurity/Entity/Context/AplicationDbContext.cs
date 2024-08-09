﻿using Dapper;
using Entity.Model.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;


namespace Entity.Context
{
    public class AplicationDBContext : DbContext
    {
        protected readonly IConfiguration _configuration;
        public AplicationDBContext(DbContextOptions<AplicationDBContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();

        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
        }
        public override int SaveChanges()
        {
            EnsureAudit();
            return base.SaveChanges();

        }
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSucess, CancellationToken cancellationToken = default)
        {
            EnsureAudit();
            return base.SaveChangesAsync(acceptAllChangesOnSucess,
                                        cancellationToken);
        }
        public async Task<IEnumerable<T>> QueryAsync<T>(string text, object parameters = null, int? timeout = null, CommandType? type = null)
        {
            using var command = new DapperEFCoreCommand(this,
                                    text,
                                    parameters,
                                    timeout,
                                    type,
                                    CancellationToken.None);
            var connection = Database.GetDbConnection();
            return await connection.QueryAsync<T>(command.Definition);
        }
        public async Task<T> QueryFirstOrDefaultAsync<T>(string text, object parameters = null, int? timeout = null, CommandType? type = null)
        {
            using var command = new DapperEFCoreCommand(this,
                                    text,
                                    parameters,
                                    timeout,
                                    type,
                                    CancellationToken.None);
            var connection = Database.GetDbConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(command.Definition);
        }

    private void EnsureAudit()
    {
        ChangeTracker.DetectChanges();
    }
//security
    public DbSet<Role> Roles => Set<Role>();

public readonly struct DapperEFCoreCommand : IDisposable
{
    public DapperEFCoreCommand(DbContext context, String text, Object parameters, int? timeout, CommandType? type, CancellationToken ct)
    {
        var transaction = context.Database.CurrentTransaction?.GetDbTransaction();
        var commandType = type ?? CommandType.Text;
        var commandTimeout = timeout ?? context.Database.GetCommandTimeout() ?? 30;

        Definition = new CommandDefinition(
            text,
            parameters,
            transaction,
            commandTimeout,
            commandType,
            cancellationToken: ct

            );
    } 
    public CommandDefinition Definition { get; }
    public void Dispose() { }
    }
  }
}
