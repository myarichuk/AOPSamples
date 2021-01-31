using System;

namespace SimpleAspects
{
    public static class AspectUtils
    {
        public static TAspect CreateAspect<TAspect>(object key) where TAspect : Aspect =>
            Activator.CreateInstance<TAspect>();
    }
}
