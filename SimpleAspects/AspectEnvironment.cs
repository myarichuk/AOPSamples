using System.Collections.Concurrent;

namespace SimpleAspects
{
    public static class AspectEnvironment
    {
        public static ConcurrentDictionary<object, Aspect> AspectsPerInstance { get; } = new ConcurrentDictionary<object, Aspect>();
    }
}
