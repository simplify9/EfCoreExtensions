using System;

namespace SW.EfCoreExtensions.UnitTests
{
    public class Bag
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public string Entity { get; set; }
        public bool  Closed { get; set; }
        public DateTime? TS { get; set; }
    }
}