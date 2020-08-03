using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using System.Dynamic;
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

            switch (entry.Context.GetDbType())
            {
                case RelationalDbType.MySql:
                    command.CommandText = $"UPDATE Sequences SET Value=LAST_INSERT_ID(Value) + 1 WHERE Entity = '{sequenceName}';SELECT LAST_INSERT_ID();";
                    break;

                case RelationalDbType.Sqlite:
                    command.CommandText = $"UPDATE Sequences SET Value=Value+1 WHERE Entity='{sequenceName}'; SELECT Value-1 FROM Sequences WHERE Entity='{sequenceName}';";
                    break;

                case RelationalDbType.Postgre:
                case RelationalDbType.Oracle:
                case RelationalDbType.MsSql:
                default:
                    throw new NotImplementedException();
            }

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
