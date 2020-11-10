using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SW.EfCoreExtensions
{
    public static class DatabaseFacadeExtensions
    {
        private static string identityCommand = "SELECT LAST_INSERT_ID();";

        /// <summary>
        /// Creates table on connection object and returns the create statement used
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="sqlMaps"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async static Task<string> CreateTable(this DatabaseFacade databaseFacade, string tableName, IDictionary<string, SqlTypeInformation> sqlMaps, DbTransaction transaction = null)
        {
            string sqlCreate = $"CREATE TABLE {tableName} (\n";
            foreach (var map in sqlMaps)
                sqlCreate += Column(map);

            sqlCreate = sqlCreate.Substring(0, sqlCreate.Length - 2) + "\n)\n";

            var command = databaseFacade.GetDbConnection().CreateCommandObject(transaction);
            command.CommandText = sqlCreate;
            await databaseFacade.ExecuteNonQueryAsync(command);
            return sqlCreate;
        }

        public async static Task DropTable(this DatabaseFacade databaseFacade, string tableName, DbTransaction transaction = null)
        {
            var command = databaseFacade.GetDbConnection().CreateCommandObject(transaction);
            command.CommandText = $"DROP TABLE {tableName}";
            await databaseFacade.ExecuteNonQueryAsync(command);
        }

        /// <summary>
        /// Creates a table using provided fields
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <param name="fields">Field name, field type</param>
        /// <returns></returns>
        async public static Task Add<TEntity>(this DatabaseFacade databaseFacade, string tableName, TEntity entity, string identity = "Id", bool serverSideIdentity = true, DbTransaction transaction = null)
        {
            var entityType = typeof(TEntity);
            var properties = entityType.GetProperties();
            var fields = new StringBuilder();
            var parameters = new StringBuilder();
            var command = databaseFacade.GetDbConnection().CreateCommandObject(transaction);

            foreach (PropertyInfo property in properties)
            {
                bool isIdentity = property.Name.Equals(identity, StringComparison.OrdinalIgnoreCase);
                //PropertyInfo idProperty;
                if (!isIdentity || !serverSideIdentity)
                {
                    var columnEscaped = ColumnAttribute.Get(property).ColumnNameEscaped(databaseFacade.GetDbType());
                    var paramaterName = $"@{ColumnAttribute.Get(property).ColumnName}";
                    fields.Append(columnEscaped + ", ");
                    parameters.Append(paramaterName + ", ");

                    if (databaseFacade.GetDbType() == RelationalDbType.Sqlite) paramaterName = paramaterName.Substring(1);

                    command.AddParameter(paramaterName, property.GetValue(entity));
                }
                //else
                //idProperty = property;
            }

            command.CommandText = @$"INSERT INTO {tableName} 
                ({fields.ToString().Remove(fields.ToString().Length - 2)}) 
                VALUES ({parameters.ToString().Remove(parameters.ToString().Length - 2)}) 
                {(serverSideIdentity && databaseFacade.GetDbType() != RelationalDbType.Sqlite ? ";" + identityCommand : "")}";

            if (serverSideIdentity)
            {
                var newId = await databaseFacade.ExecuteScalarAsync(command);
                //idProperty.SetValue(entity, Convert.ChangeType(newId, idProperty.PropertyType));
            }
            else
                await databaseFacade.ExecuteNonQueryAsync(command);
        }
        async public static Task Add<TEntity>(this DatabaseFacade databaseFacade, TEntity entity, DbTransaction transaction = null)
        {
            var entityType = typeof(TEntity);
            var tableInfo = TableAttribute.Get(entityType);
            await databaseFacade.Add(tableInfo.TableName, entity, tableInfo.IdentityColumn, tableInfo.ServerSideIdentity, transaction);
        }

        async public static Task<int> Update<TEntity>(this DatabaseFacade databaseFacade, string tableName, TEntity entity, string identity = "Id", DbTransaction transaction = null)
        {
            var entityType = typeof(TEntity);
            var properties = entityType.GetProperties();
            var idColumn = string.Empty;
            var idColumnParameter = string.Empty;
            var fields = new StringBuilder();
            var command = databaseFacade.GetDbConnection().CreateCommandObject(transaction);

            foreach (PropertyInfo property in properties)
            {
                var parameterName = $"{ColumnAttribute.Get(property).ColumnName}";
                bool isIdentity = property.Name.Equals(identity, StringComparison.OrdinalIgnoreCase);
                if (!isIdentity)
                {
                    string column = ColumnAttribute.Get(property).ColumnNameEscaped(databaseFacade.GetDbType());
                    fields.Append(column + "= @" + parameterName + ", ");

                }
                else
                {
                    idColumnParameter = '@' + parameterName;
                    idColumn = ColumnAttribute.Get(property).ColumnNameEscaped(databaseFacade.GetDbType());

                }

                command.AddParameter(parameterName, property.GetValue(entity));
            }

            command.CommandText = $"UPDATE {tableName} SET {fields.ToString().Remove(fields.ToString().Length - 2)} WHERE {idColumn}={idColumnParameter}";
            return await databaseFacade.ExecuteNonQueryAsync(command);
        }
        async public static Task<int> Update<TEntity>(this DatabaseFacade databaseFacade, TEntity entity, DbTransaction transaction = null)
        {
            Type entityType = typeof(TEntity);
            var tableInfo = TableAttribute.Get(entityType);
            return await databaseFacade.Update(tableInfo.TableName, entity, tableInfo.IdentityColumn, transaction);
        }

        async static public Task<int> Delete<TEntity>(this DatabaseFacade databaseFacade, string tableName, IEnumerable<SearchyCondition> conditions = null, DbTransaction transaction = null) where TEntity : new()
        {
            var command = databaseFacade.GetDbConnection().CreateCommandObject(transaction);
            string where = command.FilterCondition<TEntity>(databaseFacade.GetDbType(), conditions);
            command.CommandText = $"delete FROM {tableName} {where} ";
            return await databaseFacade.ExecuteNonQueryAsync(command);
        }
        async static public Task<int> Delete<TEntity>(this DatabaseFacade databaseFacade, IEnumerable<SearchyCondition> conditions = null, DbTransaction transaction = null) where TEntity : new()
        {
            string tableName = TableAttribute.Get<TEntity>().TableName;
            return await databaseFacade.Delete<TEntity>(tableName, conditions, transaction);
        }

        async static public Task<int> Count<TEntity>(this DatabaseFacade databaseFacade, IEnumerable<SearchyCondition> conditions = null) where TEntity : new()
        {
            string tableName = TableAttribute.Get<TEntity>().TableName;
            return await databaseFacade.Count<TEntity>(tableName, conditions);
        }
        async static public Task<int> Count<TEntity>(this DatabaseFacade databaseFacade, string tableName, IEnumerable<SearchyCondition> conditions = null) where TEntity : new()
        {
            var command = databaseFacade.GetDbConnection().CreateCommandObject();
            string where = command.FilterCondition<TEntity>(databaseFacade.GetDbType(), conditions);
            command.CommandText = $"SELECT COUNT(*) FROM {tableName} {where}";
            var result = await databaseFacade.ExecuteScalarAsync(command);
            return Convert.ToInt32(result);
        }

        async public static Task<TEntity> One<TEntity>(this DatabaseFacade databaseFacade, object key) where TEntity : new()
        {
            var tableName = TableAttribute.Get<TEntity>().TableName;
            return await databaseFacade.One<TEntity>(tableName, key);
        }
        async public static Task<TEntity> One<TEntity>(this DatabaseFacade databaseFacade, string tableName, object key) where TEntity : new()
        {
            var identityColumn = TableAttribute.Get<TEntity>().IdentityColumn;
            var command = databaseFacade.GetDbConnection().CreateCommandObject();
            command.CommandText = $"{BuildSelect<TEntity>(databaseFacade.GetDbType(), tableName)} WHERE {identityColumn}=@{identityColumn}"; ;
            command.AddParameter(identityColumn, key);
            return (await databaseFacade.ExecuteAndBindReaderAsync<TEntity>(command)).SingleOrDefault();
        }

        static public Task<IEnumerable<TEntity>> All<TEntity>(this DatabaseFacade databaseFacade, SearchyCondition condition) where TEntity : new()
        {
            return databaseFacade.All<TEntity>(new SearchyCondition[] { condition });
        }
        static public Task<IEnumerable<TEntity>> All<TEntity>(this DatabaseFacade databaseFacade, string tableName, SearchyCondition condition) where TEntity : new()
        {
            return databaseFacade.All<TEntity>(tableName, new SearchyCondition[] { condition });
        }
        async static public Task<IEnumerable<TEntity>> All<TEntity>(this DatabaseFacade databaseFacade, string tableName, IEnumerable<SearchyCondition> conditions = null, IEnumerable<SearchySort> sorts = null, int pageSize = 0, int pageIndex = 0) where TEntity : new()
        {
            var relationalDbType = databaseFacade.GetDbType();
            var command = databaseFacade.GetDbConnection().CreateCommandObject();
            string orderBy = "";
            string where = command.FilterCondition<TEntity>(relationalDbType, conditions);

            if (sorts?.Count() > 0)
            {
                orderBy = " ORDER BY ";
                foreach (var sort in sorts)
                    if (sort.Sort == SearchySortOrder.DEC)
                        orderBy += string.Format(" {0} {1},", sort.Field, "DESC ");
                    else
                        orderBy += string.Format(" {0} {1},", sort.Field, "ASC");
                orderBy = orderBy.Remove(orderBy.Length - 1);
            }

            string selectStatement = $"{BuildSelect<TEntity>(relationalDbType, tableName)} {where} {orderBy}";

            if (pageSize > 0)
                selectStatement = AddSqlLimit(relationalDbType, selectStatement, pageSize, pageIndex * pageSize);

            command.CommandText = selectStatement;
            return await databaseFacade.ExecuteAndBindReaderAsync<TEntity>(command);
        }
        async static public Task<IEnumerable<TEntity>> All<TEntity>(this DatabaseFacade databaseFacade, IEnumerable<SearchyCondition> conditions = null, IEnumerable<SearchySort> sorts = null, int pageSize = 0, int pageIndex = 0) where TEntity : new()
        {
            string tableName = TableAttribute.Get<TEntity>().TableName;
            return await databaseFacade.All<TEntity>(tableName, conditions, sorts, pageSize, pageIndex);
        }
        async public static Task<IEnumerable<TEntity>> All<TEntity>(this DatabaseFacade databaseFacade, string queryText) where TEntity : new()
        {
            var command = databaseFacade.GetDbConnection().CreateCommandObject();
            command.CommandText = queryText;
            return await databaseFacade.ExecuteAndBindReaderAsync<TEntity>(command);
        }
        public static Task<IEnumerable<TEntity>> All<TEntity>(this DatabaseFacade databaseFacade, string field, object value, SearchyRule rule = SearchyRule.EqualsTo) where TEntity : new()
        {
            return databaseFacade.All<TEntity>(new SearchyCondition[] { new SearchyCondition(field, rule, value) });
        }
        public static Task<IEnumerable<TEntity>> All<TEntity>(this DatabaseFacade databaseFacade, string tableName, string field, object value, SearchyRule rule = SearchyRule.EqualsTo) where TEntity : new()
        {
            return databaseFacade.All<TEntity>(tableName, new SearchyCondition[] { new SearchyCondition(field, rule, value) });
        }

        public static RelationalDbType GetDbType(this DatabaseFacade databaseFacade)
        {
            switch (databaseFacade.ProviderName.ToLower())
            {
                case "microsoft.entityframeworkcore.sqlserver":
                    return RelationalDbType.MsSql;

                case "pomelo.entityframeworkcore.mysql":
                    return RelationalDbType.MySql;

                case "microsoft.entityframeworkcore.sqlite":
                    return RelationalDbType.Sqlite;

                //Npgsql.EntityFrameworkCore.PostgreSQL
                case "npgsql.entityframeworkcore.postgresql":
                    return RelationalDbType.PgSql;

                case "oracle.entityframeworkcore":
                    return RelationalDbType.Oracle;

                default:
                    return RelationalDbType.Unknown;
            }
        }


        async private static Task<IEnumerable<TEntity>> ExecuteAndBindReaderAsync<TEntity>(this DatabaseFacade databaseFacade, DbCommand command) where TEntity : new()
        {
            await databaseFacade.OpenConnectionAsync();
            try
            {
                var reader = await command.ExecuteReaderAsync();
                return await reader.Bind<TEntity>();
            }
            finally
            {
                await databaseFacade.CloseConnectionAsync();
            }
        }

        async private static Task<int> ExecuteNonQueryAsync(this DatabaseFacade databaseFacade, DbCommand command)
        {
            await databaseFacade.OpenConnectionAsync();
            try
            {
                return await command.ExecuteNonQueryAsync();
            }
            finally
            {
                await databaseFacade.CloseConnectionAsync();
            }
        }
        async private static Task<object> ExecuteScalarAsync(this DatabaseFacade databaseFacade, DbCommand command)
        {
            await databaseFacade.OpenConnectionAsync();
            try
            {
                return await command.ExecuteScalarAsync();
            }
            finally
            {
                await databaseFacade.CloseConnectionAsync();
            }
        }

        private static string BuildSelect<TEntity>(RelationalDbType relationalDbType, string tableName)
        {
            var entityType = typeof(TEntity);
            string fields = "";

            foreach (var property in entityType.GetProperties())
                fields = @$"{fields}{ColumnAttribute.Get(property).ColumnNameEscaped(relationalDbType)},";

            return $"SELECT {fields.Remove(fields.Length - 1)} FROM {tableName} ";
        }
        private static string AddSqlLimit(RelationalDbType relationalDbType, string sqlStatement, int pageSize, int paging = 0)
        {
            if (relationalDbType == RelationalDbType.MySql || relationalDbType == RelationalDbType.Sqlite)
                sqlStatement += paging == 0 ? $" LIMIT {pageSize}" : $" LIMIT {paging}, {pageSize}";
            else if (relationalDbType == RelationalDbType.MsSql)
                sqlStatement = sqlStatement.Insert(7, $"TOP ({pageSize}) ");
            return sqlStatement;
        }
        private static string Column(KeyValuePair<string, SqlTypeInformation> map)
        {
            string tmp = string.Empty;

            //TODO : implement provider specific UNIQUE implementation
            if (map.Value.IsUnique)
                tmp += string.Empty;

            tmp += $"\t{map.Key}\t{map.Value.SqlType},\n";

            return tmp;
        }

    }
}




