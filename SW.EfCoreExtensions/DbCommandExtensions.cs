using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace SW.EfCoreExtensions
{
    internal static class DbCommandExtensions
    {
        public static IDbDataParameter AddParameter(this DbCommand command, string parameterName, object parameterValue = null)
        {
            IDbDataParameter parameter = command.CreateParameter();
            
            parameter.Direction = ParameterDirection.Input;
            parameter.ParameterName = "@" + parameterName;
            parameter.Value = parameterValue ?? DBNull.Value;

            command.Parameters.Add(parameter);

            return parameter;
        }

        public static string FilterCondition<TEntity>(this DbCommand command, RelationalDbType relationalDbType, IEnumerable<SearchyCondition> conditions = null)
        {
            //var entityType = typeof(TEntity);

            var where = string.Empty;

            if (conditions == null || conditions.Count() == 0)
                return "";

            int index = 0;

            foreach (var condition in conditions.Where(x => x.Filters != null && x.Filters.Count != 0))
            {
                if (where == string.Empty)
                    where = " WHERE (";
                else
                    where = $"{where} or ( ";
                foreach (var filter in condition.Filters)
                {
                    index += 1;
                    var filterColName = ColumnAttribute.Get<TEntity>(filter.Field).ColumnNameEscaped(relationalDbType);
                    var filterColumnParameter = ColumnAttribute.Get<TEntity>(filter.Field).ColumnName;
                    var parameter = command.AddParameter(filterColumnParameter + index.ToString());

                    switch (filter.Rule)
                    {
                        case SearchyRule.EqualsTo:
                            where = $"{where} ({filterColName}={parameter.ParameterName}) AND ";
                            parameter.Value = filter.Value;
                            break;

                        case SearchyRule.LessThan:
                            where = $"{where} ({filterColName}<{parameter.ParameterName}) AND ";
                            parameter.Value = filter.Value;
                            break;

                        case SearchyRule.LessThanOrEquals:
                            where = $"{where} ({filterColName}<={parameter.ParameterName}) AND ";
                            parameter.Value = filter.Value;
                            break;

                        case SearchyRule.GreaterThan:
                            where = $"{where} ({filterColName}>{parameter.ParameterName}) AND ";
                            parameter.Value = filter.Value;
                            break;

                        case SearchyRule.GreaterThanOrEquals:
                            where = $"{where} ({filterColName}>={parameter.ParameterName}) AND ";
                            parameter.Value = filter.Value;
                            break;

                        case SearchyRule.NotEqualsTo:
                            where = $"{where} ({filterColName}<>{parameter.ParameterName}) AND ";
                            parameter.Value = filter.Value;
                            break;

                        case SearchyRule.StartsWith:
                            where = $"{where} ({filterColName} like {parameter.ParameterName}) AND ";
                            parameter.Value = string.Concat(filter.Value, "%");
                            break;

                        case SearchyRule.Contains:
                            where = $"{where} ({filterColName} like {parameter.ParameterName}) AND ";
                            parameter.Value = string.Concat("%", filter.Value, "%");
                            break;

                            //case SearchyRule.EqualsToList:
                            //    {
                            //        var _ListType = _e.Value.GetType();
                            //        var _ItemType = _ListType.GetGenericArguments();
                            //        var _GenericListType = typeof(List<>);
                            //        var _GenericList = _GenericListType.MakeGenericType(_ItemType);

                            //        if (_GenericList != _ListType)
                            //            throw new Exception(string.Format("The value for the filter {0} is not a generic list", _filtercolname));

                            //        var _Values = new StringBuilder();
                            //        if (_ItemType.Contains(Type.GetType("System.String")) || _ItemType.Contains(Type.GetType("System.Guid")))
                            //        {
                            //            foreach (var _Value in _e.Value)
                            //                _Values.Append(string.Concat("'", _Value.ToString().Replace("'", "''"), "'", ","));
                            //        }
                            //        else
                            //            foreach (var _Value in _e.Value)
                            //                _Values.Append(string.Concat(_Value, ","));

                            //        _whereclause = string.Format(_whereclause + " ({0} IN ({1})) AND ", _filtercolname, _Values.ToString().TrimEnd(new char[] { ',' }));
                            //        break;
                            //    }
                    }
                }

                where = where.Remove(where.Length - 5);

                where = $"{where})";
            }
            return where;
        }
    }
}
