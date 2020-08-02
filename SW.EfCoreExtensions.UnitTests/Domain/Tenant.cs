using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.EfCoreExtensions.UnitTests.Domain
{
    class Tenant : BaseEntity, ISoftDelete
    {
        public string Name { get; set; }
        public bool Deleted { get; private set; }
    }
}
