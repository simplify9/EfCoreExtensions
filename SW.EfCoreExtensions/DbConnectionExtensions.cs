using System.Data.Common;

namespace SW.EfCoreExtensions
{
    internal static class DbConnectionExtensions
    {
        public static DbCommand CreateCommandObject(this DbConnection connection, DbTransaction transaction = null)
        {
            var command = connection.CreateCommand();
            if (transaction != null)
                command.Transaction = transaction;
            return command;
        }

        //private static string SqlProviderName(this DbConnection connection) => connection.GetType().FullName;


        //internal const string MYSQL = "MySql.Data.MySqlClient.MySqlConnection";
        //internal const string MSSQL = "System.Data.SqlClient.SqlConnection";
        //internal const string SQLITE = "Microsoft.Data.Sqlite.SqliteConnection";

        //private static string Column(KeyValuePair<string, SqlTypeInformation> map)
        //{
        //    string tmp = string.Empty;

        //    //TODO : implement provider specific UNIQUE implementation
        //    if (map.Value.IsUnique)
        //        tmp += string.Empty;

        //    tmp += $"\t{map.Key}\t{map.Value.SqlType},\n";

        //    return tmp;
        //}

        /// <summary>
        /// Creates table on connection object and returns the create statement used
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="sqlMaps"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        //public async static Task<string> CreateTable(this DbConnection connection, string tableName, IDictionary<string, SqlTypeInformation> sqlMaps, DbTransaction transaction = null)
        //{
        //    string sqlCreate = $"CREATE TABLE {tableName} (\n";
        //    foreach (var map in sqlMaps)
        //        sqlCreate += Column(map);

        //    sqlCreate = sqlCreate.Substring(0, sqlCreate.Length - 2) + "\n)\n";

        //    var command = connection.CreateCommandObject(transaction);
        //    command.CommandText = sqlCreate;
        //    await command.ExecuteNonQueryAsync();
        //    return sqlCreate;
        //}

        //public async static Task DropTable(this DbConnection connection, string tableName, DbTransaction transaction = null)
        //{
        //    var command = connection.CreateCommandObject(transaction);
        //    command.CommandText = $"DROP TABLE {tableName}";
        //    await command.ExecuteNonQueryAsync();
        //}


        /// <summary>
        /// Creates a table using provided fields
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <param name="fields">Field name, field type</param>
        /// <returns></returns>
        //async public static Task Add<TEntity>(this DbConnection connection, string tableName, TEntity entity, string identity = "Id", bool serverSideIdentity = true, DbTransaction transaction = null) 
        //{
        //    var entityType = typeof(TEntity);
        //    var properties = entityType.GetProperties();
        //    PropertyInfo idProperty = null;
        //    var fields = new StringBuilder();
        //    var parameters = new StringBuilder();
        //    var command = connection.CreateCommandObject(transaction);

        //    foreach (PropertyInfo property in properties)
        //    {
        //        bool isIdentity = property.Name.Equals(identity, StringComparison.OrdinalIgnoreCase); 
        //        if (!isIdentity || !serverSideIdentity)
        //        {
        //            var columnEscaped = GetColumnInfo(property).ColumnNameEscaped(connection.SqlProviderName());
        //            var paramaterName = $"@{GetColumnInfo(property).ColumnName}";
        //            fields.Append(columnEscaped + ", ");
        //            parameters.Append(paramaterName + ", ");

        //            if (connection.GetType().FullName == SQLITE) paramaterName = paramaterName.Substring(1); 

        //            command.AddCommandParameter(paramaterName, property.GetValue(entity));
        //        }
        //        else
        //            idProperty = property;
        //    }


        //    string insertStatement = $"INSERT INTO {tableName} ({fields.ToString().Remove(fields.ToString().Length - 2)}) VALUES ({parameters.ToString().Remove(parameters.ToString().Length - 2)}) {(serverSideIdentity && connection.GetType().FullName != SQLITE ? ";" + IdentityCommand : "")}";

        //    command.CommandText = insertStatement;

        //    if (serverSideIdentity)
        //    {
        //        var newId = await command.ExecuteScalarAsync();
        //        //idProperty.SetValue(entity, Convert.ChangeType(newId, idProperty.PropertyType));
        //    }
        //    else
        //        await command.ExecuteNonQueryAsync();
        //}
        //async public static Task Add<TEntity>(this DbConnection connection, TEntity entity, DbTransaction transaction = null)
        //{
        //    var entityType = typeof(TEntity);
        //    var tableInfo = GetTableInfo(entityType);
        //    await connection.Add(tableInfo.TableName, entity, tableInfo.IdentityColumn, tableInfo.ServerSideIdentity, transaction);
        //}

        //async public static Task Update<TEntity>(this DbConnection connection, string tableName, TEntity entity, string identity = "Id",  DbTransaction transaction = null)
        //{
        //    var entityType = typeof(TEntity);
        //    var properties = entityType.GetProperties();

        //    var idColumn = string.Empty;
        //    var idColumnParameter = string.Empty;
        //    var fields = new StringBuilder();
        //    var command = connection.CreateCommandObject(transaction);

        //    foreach (PropertyInfo property in properties)
        //    {
        //        var parameterName = $"{GetColumnInfo(property).ColumnName}";
        //        bool isIdentity = property.Name.Equals(identity, StringComparison.OrdinalIgnoreCase); 
        //        if (!isIdentity)
        //        {
        //            string column = GetColumnInfo(property).ColumnNameEscaped(connection.SqlProviderName());
        //            fields.Append(column + "= " + parameterName + ", ");

        //        }
        //        else
        //        {
        //            idColumnParameter = parameterName;
        //            idColumn = GetColumnInfo(property).ColumnNameEscaped(connection.SqlProviderName());

        //        }

        //        command.AddCommandParameter(parameterName, property.GetValue(entity));
        //    }

        //    string updateStatement = $"UPDATE {tableName} SET {fields.ToString().Remove(fields.ToString().Length - 2)} WHERE {idColumn}={idColumnParameter}";
        //    command.CommandText = updateStatement;

        //    await command.ExecuteNonQueryAsync();
        //}
        //async public static Task Update<TEntity>(this DbConnection connection, TEntity entity, DbTransaction transaction = null)
        //{
        //    Type entityType = typeof(TEntity);
        //    var tableInfo = GetTableInfo(entityType);
        //    await connection.Update(tableInfo.TableName, entity, tableInfo.IdentityColumn, transaction);
        //}

        //static public Task<IEnumerable<TEntity>> All<TEntity>(this DbConnection connection, SearchyCondition condition) where TEntity : new()
        //{
        //    return connection.All<TEntity>(new SearchyCondition[] { condition });
        //}
        //static public Task<IEnumerable<TEntity>> All<TEntity>(this DbConnection connection, string tableName, SearchyCondition condition) where TEntity : new()
        //{
        //    return connection.All<TEntity>(tableName, new SearchyCondition[] { condition });
        //}
        //async static public Task<IEnumerable<TEntity>> All<TEntity>(this DbConnection connection, string tableName, IEnumerable<SearchyCondition> conditions = null, IEnumerable<SearchySort> sorts = null, int pageSize = 0, int pageIndex = 0) where TEntity : new()
        //{
        //    Type entityType = typeof(TEntity);
        //    var command = connection.CreateCommandObject();
        //    string where = "";
        //    string orderBy = "";

        //    where = FilterCondition<TEntity>(command, conditions);

        //    if (sorts?.Count() > 0)
        //    {
        //        orderBy = " ORDER BY ";
        //        foreach (var sort in sorts)
        //            if (sort.Sort == SearchySortOrder.DEC)
        //                orderBy += string.Format(" {0} {1},", sort.Field, "DESC ");
        //            else
        //                orderBy += string.Format(" {0} {1},", sort.Field, "ASC");
        //        orderBy = orderBy.Remove(orderBy.Length - 1);
        //    }

        //    string selectStatement = $"{BuildSelect<TEntity>(tableName,connection)} {where} {orderBy}";

        //    if (pageSize > 0)
        //        selectStatement = selectStatement.AddSqlLimit(pageSize, connection.GetType(), pageIndex * pageSize);

        //    command.CommandText = selectStatement;

        //    return await (await command.ExecuteReaderAsync()).BindReader<TEntity>();
        //}
        //async static public Task<IEnumerable<TEntity>> All<TEntity>(this DbConnection connection, IEnumerable<SearchyCondition> conditions = null, IEnumerable<SearchySort> sorts = null, int pageSize = 0, int pageIndex = 0) where TEntity : new()
        //{
        //    Type entityType = typeof(TEntity);
        //    string tableName = GetTableInfo(entityType).TableName;
        //    return await connection.All<TEntity>(tableName, conditions, sorts, pageSize, pageIndex);
        //}
        //async public static Task<IEnumerable<TEntity>> All<TEntity>(this DbConnection connection, string queryText) where TEntity : new()
        //{
        //    var command = connection.CreateCommandObject();

        //    command.CommandText = queryText;

        //    return await BindReader<TEntity>(await command.ExecuteReaderAsync());
        //}
        //public static Task<IEnumerable<TEntity>> All<TEntity>(this DbConnection connection, string field, object value, SearchyRule rule = SearchyRule.EqualsTo) where TEntity : new()
        //{
        //    return connection.All<TEntity>(new SearchyCondition[] { new SearchyCondition(field, rule, value) });
        //}
        //public static Task<IEnumerable<TEntity>> All<TEntity>(this DbConnection connection, string tableName, string field, object value, SearchyRule rule = SearchyRule.EqualsTo) where TEntity : new()
        //{
        //    return connection.All<TEntity>(tableName, new SearchyCondition[] { new SearchyCondition(field, rule, value) });
        //}


        //async static public Task<int> Delete<TEntity>(this DbConnection connection, string tableName, IEnumerable<SearchyCondition> conditions = null, DbTransaction transaction = null) where TEntity : new()
        //{
        //    var command = connection.CreateCommandObject(transaction);
        //    string where = FilterCondition<TEntity>(command, conditions);

        //    var deleteStatement = $"delete FROM {tableName} {where} ";

        //    command.CommandText = deleteStatement;

        //    return await command.ExecuteNonQueryAsync();
        //}

        //async static public Task<int> Delete<TEntity>(this DbConnection connection, IEnumerable<SearchyCondition> conditions = null, DbTransaction transaction = null) where TEntity : new()
        //{
        //    string tableName = GetTableInfo(typeof(TEntity)).TableName;
        //    return await connection.Delete<TEntity>(tableName, conditions, transaction);
        //}

        //async static public Task<int> Count<TEntity>(this DbConnection connection, IEnumerable<SearchyCondition> conditions = null) where TEntity : new()
        //{
        //    string tableName = GetTableInfo(typeof(TEntity)).TableName;
        //    return await connection.Count<TEntity>(tableName, conditions);
        //}
        //async static public Task<int> Count<TEntity>(this DbConnection connection, string tableName, IEnumerable<SearchyCondition> conditions = null) where TEntity : new()
        //{
        //    var command = connection.CreateCommandObject();
        //    string where = "";
        //    string orderBy = "";

        //    where = FilterCondition<TEntity>(command, conditions);


        //    string selectStatement = $"SELECT COUNT(*) FROM {tableName} {where} {orderBy}";


        //    command.CommandText = selectStatement;

        //    var result = await command.ExecuteScalarAsync();

        //    return Convert.ToInt32(result);
        //}
        //async public static Task<TEntity> One<TEntity>(this DbConnection connection, object key) where TEntity : new()
        //{
        //    var tableName = GetTableInfo(typeof(TEntity)).TableName;
        //    return await connection.One<TEntity>(tableName, key);
        //}
        //async public static Task<TEntity> One<TEntity>(this DbConnection connection, string tableName,  object key) where TEntity : new()
        //{
        //    var identityColumn = GetTableInfo(typeof(TEntity)).IdentityColumn;
        //    string selectStatement = $"{BuildSelect<TEntity>(tableName,connection)} WHERE {identityColumn}=@{identityColumn}";
        //    var command = connection.CreateCommandObject();
        //    command.CommandText = selectStatement;
        //    command.AddCommandParameter(identityColumn, key);
        //    return (await BindReader<TEntity>(await command.ExecuteReaderAsync())).SingleOrDefault();
        //}

        //private static string BuildSelect<TEntity>(string tableName , DbConnection connection)
        //{
        //    var entityType = typeof(TEntity);
        //    string fields = "";

        //    foreach (var property in entityType.GetProperties())

        //        fields = @$"{fields}{GetColumnInfo(property).ColumnNameEscaped(connection.SqlProviderName())},";

        //    return $"SELECT {fields.Remove(fields.Length - 1)} FROM {tableName} ";
        //}
        //private static string AddSqlLimit(this string sqlStatement, int pageSize, Type sqlProvider, int paging = 0)
        //{

        //    if (sqlProvider.FullName == MYSQL || sqlProvider.FullName == SQLITE)
        //        sqlStatement += paging == 0 ? $" LIMIT {pageSize}" : $" LIMIT {paging}, {pageSize}";
        //    else if (sqlProvider.FullName == MSSQL)
        //        sqlStatement = sqlStatement.Insert(7, $"TOP ({pageSize}) ");
        //    return sqlStatement;
        //}




        //private static IDbDataParameter AddCommandParameter(this IDbCommand command, string parameterName, object parameterValue = null)
        //{
        //    IDbDataParameter parameter = command.CreateParameter();

        //    parameter.Direction = ParameterDirection.Input;
        //    parameter.ParameterName = "@" + parameterName;
        //    parameter.Value = parameterValue ?? DBNull.Value;

        //    command.Parameters.Add(parameter);

        //    return parameter;
        //}

        //private static IDataParameter CreateParameterObject(IDbCommand command)
        //{
        //    return command.CreateParameter();
        //}

        //async private static Task<IEnumerable<TEntity>> BindReader<TEntity>(this DbDataReader reader) where TEntity : new()
        //{
        //    var properties = typeof(TEntity).GetProperties();
        //    var list = new List<TEntity>();

        //    var propertyMapper = new Dictionary<int, int>();

        //    for (var fieldIndex = 0; fieldIndex < reader.FieldCount; fieldIndex++)

        //        for (var propertyIndex = 0; propertyIndex < properties.Length; propertyIndex++)
        //        {
        //            string columnName = GetColumnInfo(properties[propertyIndex]).ColumnName;

        //            if (columnName.Equals(reader.GetName(fieldIndex), StringComparison.OrdinalIgnoreCase))
        //            {
        //                propertyMapper.Add(fieldIndex, propertyIndex);
        //                break;
        //            }
        //        }


        //    while (await reader.ReadAsync())
        //    {
        //        var entity = new TEntity();

        //        for (var index = 0; index < reader.FieldCount; index++)
        //        {

        //            if (!reader.IsDBNull(index) && propertyMapper.TryGetValue(index, out int propertyIndex))
        //                properties[propertyIndex].SetValue(
        //                    entity, 
        //                    reader[index].ConvertValueToType(properties[propertyIndex].PropertyType), 
        //                    null
        //                );
        //        }

        //        list.Add(entity);
        //    }

        //    reader.Close();
        //    return list;
        //}


        //public static string FilterCondition<TEntity>(DbCommand command, IEnumerable<SearchyCondition> conditions = null)
        //{


        //    var entityType = typeof(TEntity);

        //    var where = string.Empty;

        //    if (conditions == null || conditions.Count() == 0)
        //        return "";





        //    int index = 0;

        //    foreach (var condition in conditions.Where(x => x.Filters != null && x.Filters.Count != 0))
        //    {
        //        if (where == string.Empty)
        //            where = " WHERE (";
        //        else
        //            where = $"{where} or ( ";
        //        foreach (var filter in condition.Filters)
        //        {
        //            index += 1;
        //            var filterColName = GetColumnInfo(entityType, filter.Field).ColumnNameEscaped(command.Connection.SqlProviderName());
        //            var filterColumnParameter = GetColumnInfo(entityType, filter.Field).ColumnName;
        //            var parameter = command.AddCommandParameter(filterColumnParameter + index.ToString());

        //            switch (filter.Rule)
        //            {
        //                case SearchyRule.EqualsTo:
        //                    where = $"{where} ({filterColName}={parameter.ParameterName}) AND ";
        //                    parameter.Value = filter.Value;
        //                    break;

        //                case SearchyRule.LessThan:
        //                    where = $"{where} ({filterColName}<{parameter.ParameterName}) AND ";
        //                    parameter.Value = filter.Value;
        //                    break;

        //                case SearchyRule.LessThanOrEquals:
        //                    where = $"{where} ({filterColName}<={parameter.ParameterName}) AND ";
        //                    parameter.Value = filter.Value;
        //                    break;

        //                case SearchyRule.GreaterThan:
        //                    where = $"{where} ({filterColName}>{parameter.ParameterName}) AND ";
        //                    parameter.Value = filter.Value;
        //                    break;

        //                case SearchyRule.GreaterThanOrEquals:
        //                    where = $"{where} ({filterColName}>={parameter.ParameterName}) AND ";
        //                    parameter.Value = filter.Value;
        //                    break;

        //                case SearchyRule.NotEqualsTo:
        //                    where = $"{where} ({filterColName}<>{parameter.ParameterName}) AND ";
        //                    parameter.Value = filter.Value;
        //                    break;

        //                case SearchyRule.StartsWith:
        //                    where = $"{where} ({filterColName} like {parameter.ParameterName}) AND ";
        //                    parameter.Value = string.Concat(filter.Value, "%");
        //                    break;

        //                case SearchyRule.Contains:
        //                    where = $"{where} ({filterColName} like {parameter.ParameterName}) AND ";
        //                    parameter.Value = string.Concat("%", filter.Value, "%");
        //                    break;


        //                    //case SearchyRule.EqualsToList:
        //                    //    {
        //                    //        var _ListType = _e.Value.GetType();
        //                    //        var _ItemType = _ListType.GetGenericArguments();
        //                    //        var _GenericListType = typeof(List<>);
        //                    //        var _GenericList = _GenericListType.MakeGenericType(_ItemType);

        //                    //        if (_GenericList != _ListType)
        //                    //            throw new Exception(string.Format("The value for the filter {0} is not a generic list", _filtercolname));

        //                    //        var _Values = new StringBuilder();
        //                    //        if (_ItemType.Contains(Type.GetType("System.String")) || _ItemType.Contains(Type.GetType("System.Guid")))
        //                    //        {
        //                    //            foreach (var _Value in _e.Value)
        //                    //                _Values.Append(string.Concat("'", _Value.ToString().Replace("'", "''"), "'", ","));
        //                    //        }
        //                    //        else
        //                    //            foreach (var _Value in _e.Value)
        //                    //                _Values.Append(string.Concat(_Value, ","));

        //                    //        _whereclause = string.Format(_whereclause + " ({0} IN ({1})) AND ", _filtercolname, _Values.ToString().TrimEnd(new char[] { ',' }));
        //                    //        break;
        //                    //    }
        //            }
        //        }

        //        where = where.Remove(where.Length - 5);

        //        where = $"{where})";
        //    }






        //    return where;


        //}

        //public static string IdentityCommand => "SELECT LAST_INSERT_ID();";
    }
}




