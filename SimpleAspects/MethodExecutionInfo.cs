using System;
using System.Reflection;

namespace SimpleAspects
{
    public class MethodExecutionInfo
    {
        public MethodInfo Method { get; set; }

        public object[] Params { get; set; }

        public object ReturnValue { get; set; }
    }
}
