using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.EfCoreExtensions.UnitTests.Domain
{
    class MetaData : BaseEntity, IHasOptionalTenant
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public int? TenantId { get; private set; }
    }
}
