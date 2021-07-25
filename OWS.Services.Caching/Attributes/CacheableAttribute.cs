using System;

namespace OWS.Services.Caching.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CacheableAttribute : Attribute
    {
        /// <summary>
        /// Should we cache this request?
        /// </summary>
        public bool Cache { get; set; }
    }
}
