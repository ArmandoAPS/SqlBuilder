using System;
using System.Collections.Generic;

using System.Reflection;

namespace ANMappings
{
    public class MappingFactory
    {
        private static readonly object _padlock = new object();
        private static readonly IDictionary<Type, IMapping> _cache = new Dictionary<Type, IMapping>();

        public static IMapping GetMapping(Type mapping_type)
        {
            _cache.TryGetValue(mapping_type, out IMapping mapping);
            if (mapping == null)
            {
                lock (_padlock)
                {
                    mapping = (IMapping)Activator.CreateInstance(mapping_type);
                    _cache[mapping_type] = mapping;
                }
            }
            return mapping;
        }

        public static IMapping GetMapping<T>() where T: IMapping, new()
        {
           return GetMapping(typeof(T));
        }
    }
}
