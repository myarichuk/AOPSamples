using System;

namespace SimpleAspects
{
    //marker "interface" for weaving aspects into code
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class Aspect : Attribute
    {
        //some terminology:
        //1) code "injected" into certain points is called "advise"
        // --> the following methods allow to inject advise before, after and during exceptions of method execution
        //2) the definition of WHERE the advise is applied (before, after execution, etc) is called "poincut"
        // --> the following methods define three pointcuts

        public virtual void OnBeforeExecute(MethodExecutionInfo mif) { }

        public virtual void OnAfterExecute(MethodExecutionInfo mif) { }

        public virtual void OnException(MethodExecutionInfo mif, Exception e) { }
    }
}
