using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.EfCoreExtensions.UnitTests.Domain
{
    class Ticket : BaseEntity, ISoftDelete, IHasTenant
    {
        public int Number { get; set; }
        public bool Deleted { get; private set; }
        public int TenantId { get; private set; }
    }
}
