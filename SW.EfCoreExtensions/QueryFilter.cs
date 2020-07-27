using System;
using System.Collections.Generic;
using System.Text;

namespace SW.EfCoreExtensions
{
    [Flags]
    public enum QueryFilter
    {
        None = 0,
        SoftDeletion = 1,
        Tenancy = 2
    }
}
