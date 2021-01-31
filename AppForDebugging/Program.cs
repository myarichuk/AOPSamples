using SimpleAspects;
using System;

namespace AppForDebugging
{
    public class LoggingAspect : Aspect
    {
        public override void OnAfterExecute(MethodExecutionInfo mif)
        {
            Console.WriteLine($"After execution of '{mif.Method.Name}'");
        }

        public override void OnBeforeExecute(MethodExecutionInfo mif)
        {
            Console.WriteLine($"Before execution of '{mif.Method.Name}'");
        }

        public override void OnException(MethodExecutionInfo mif, Exception e)
        {
            Console.WriteLine($"During execution of '{mif.Method.Name}' an exception happened: {e}");
        }
    }

    class Program
    {
        [LoggingAspect]
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
