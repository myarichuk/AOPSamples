using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SimpleConsoleAppRewriter
{
    /*
     * Hello World example of low-level IL weaving by using Mono.Cecil
     */
    class Program
    {
        static void Main(string[] args)
        {
            using var moduleDefinition = ModuleDefinition.ReadModule(@"..\..\..\..\SimpleLib\bin\Debug\netcoreapp3.1\SimpleLib.dll");
            foreach(var type in moduleDefinition.Types)
            {
                if(type.Name.Contains("SimpleLib") && type.Namespace == "AOPSamples")
                {
                    foreach(var property in type.Properties)
                    {
                        if(property.Name == "CurrentDate")
                        {
                            var methodBody = property.GetMethod.Body;
                            var ilProcessor = methodBody.GetILProcessor();
                            var firstInstruction = methodBody.Instructions.First();

                            //since there are multiple ctors, we need to choose one based on parameter types
                            var dateTimeCtor = typeof(DateTime).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int) });
                            var ctorReference = moduleDefinition.ImportReference(dateTimeCtor);

                            //since there are multiple overloads of string.Format(), we need to choose one based on parameter types
                            var stringFormatMethod = moduleDefinition.ImportReference(typeof(string).GetMethod("Format", new Type[] { typeof(string), typeof(object) }));

                            //first argument of string.Format()
                            ilProcessor.InsertBefore(firstInstruction, ilProcessor.Create(OpCodes.Ldstr, "The time now is {0} (output of DateTime.Now)"));

                            //second argument of string.Format()
                            ilProcessor.InsertBefore(firstInstruction, ilProcessor.Create(OpCodes.Ldc_I4, 2020));
                            ilProcessor.InsertBefore(firstInstruction, ilProcessor.Create(OpCodes.Ldc_I4, 1));
                            ilProcessor.InsertBefore(firstInstruction, ilProcessor.Create(OpCodes.Ldc_I4, 1));
                            ilProcessor.InsertBefore(firstInstruction, ilProcessor.Create(OpCodes.Newobj, ctorReference));

                            //in order to cast datetime to string we need to box it
                            //if we do not add boxing, the compiler will call "object.ToString()" which may or may not work...
                            ilProcessor.InsertBefore(firstInstruction, ilProcessor.Create(OpCodes.Box, moduleDefinition.ImportReference(typeof(DateTime))));

                            //finally call string.Format() (string interpolation is syntax sugar for calling string.Format())
                            ilProcessor.InsertBefore(firstInstruction, ilProcessor.Create(OpCodes.Call, stringFormatMethod));
                            ilProcessor.InsertBefore(firstInstruction, ilProcessor.Create(OpCodes.Ret));
                            moduleDefinition.Assembly.Write(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName + "\\SimpleLib.dll");

                            break;
                        }

                    }

                    break;
                }
            }
        }
    }
}
