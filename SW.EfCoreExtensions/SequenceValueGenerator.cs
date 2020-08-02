using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SW.EfCoreExtensions
{
    internal class SequenceValueGenerator : ValueGenerator
    {
        public override bool GeneratesTemporaryValues => false;

        async public override ValueTask<object> NextAsync(EntityEntry entry, CancellationToken cancellationToken = default)
        {
            var sequenceName = entry.Entity.GetType().Name;
            using var command = entry.Context.Database.GetDbConnection().CreateCommand();
            command.CommandText = $"UPDATE Sequences SET Value=LAST_INSERT_ID(Value) + 1 WHERE Entity = '{sequenceName}';SELECT LAST_INSERT_ID();";
            await entry.Context.Database.OpenConnectionAsync();
            using var reader = await command.ExecuteReaderAsync();
            await reader.ReadAsync();
            return reader.GetInt32(0);
        }

        protected override object NextValue(EntityEntry entry)
        {
            throw new NotImplementedException();
        }
    }
}
