using System;

namespace OrleansStatisticsKeeper.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IndexedAttribute : Attribute
    {
        public bool IsAscending { get; }
        public IndexedAttribute(bool isAscending = true) => IsAscending = isAscending;
    }
}
