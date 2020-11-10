using System;
using System.Collections.Generic;
using System.Text;

namespace SW.EfCoreExtensions
{
    public enum RelationalDbType
    {
        Unknown = 0,
        MsSql = 1,
        MySql = 2,
        Sqlite = 3,
        PgSql = 4,
        Oracle = 5
    }
}
