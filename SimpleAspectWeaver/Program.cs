using Mono.Cecil;
using Mono.Cecil.Cil;
using SimpleAspects;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SimpleAspectWeaver
{
    public static class MonoCecilExtensions
    {
        public static string GetReflectionName(this TypeReference type)
        {
            if (type.IsGenericInstance)
            {
                var genericInstance = (GenericInstanceType)type;
                return $"{genericInstance.Namespace}.{type.Name}[{String.Join(",", genericInstance.GenericArguments.Select(p => p.GetReflectionName()).ToArray())}]";
            }
            return $"{type.FullName}";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1 || !File.Exists(args[0]) || !args[0].EndsWith(".dll"))
                throw new InvalidOperationException("Invalid command line arguments. Usage: SimpleAspectWeaver.exe [path to assembly that needs to be rewritten]");

            var targetDirectory = new FileInfo(args[0]).Directory.FullName;
            using var moduleDefinition = ModuleDefinition.ReadModule(args[0]);
            var targetAssembly = Assembly.LoadFile(new FileInfo(args[0]).FullName);
            foreach (var type in moduleDefinition.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (method.HasCustomAttributes)
                    {
                        foreach(var customAttribute in method.CustomAttributes)
                        {
                            var typeName = customAttribute.AttributeType.GetReflectionName();

                            //find the current custom attribute type in target assembly
                            var attrType = targetAssembly.GetExportedTypes().FirstOrDefault(x => x.FullName.Contains(typeName));
                            //if true then we need to weave the aspects in :)
                            if (typeof(Aspect).IsAssignableFrom(attrType))
                            {
                                var ilProcessor = method.Body.GetILProcessor();
                                var firstInstruction = method.Body.Instructions.First();
                                //var retInstruction = method.Body.Instructions.First(x => x.OpCode == OpCodes.Ret);

                                var getAspectsPerInstance = moduleDefinition.ImportReference(
                                    typeof(AspectEnvironment).GetProperty(nameof(AspectEnvironment.AspectsPerInstance)).GetGetMethod());


                                //var local = AspectEnvironment.AspectsPerInstance;
                                ilProcessor.InsertBefore(firstInstruction, ilProcessor.Create(OpCodes.Call, getAspectsPerInstance));
                                ilProcessor.InsertBefore(firstInstruction, ilProcessor.Create(OpCodes.Stloc_0));

                                var concurrentDictionaryGetOrAdd = moduleDefinition.ImportReference(
                                    typeof(ConcurrentDictionary<object, Aspect>)
                                        .GetMethod("GetOrAdd", new Type[] { typeof(object), typeof(Func<object, Aspect>) }));

                                var aspectFactory = moduleDefinition.ImportReference(
                                    typeof(AspectUtils)
                                        .GetMethod(nameof(AspectUtils.CreateAspect), 1, new Type[] { attrType }));
                                //var exceptionHandler = new ExceptionHandler(ExceptionHandlerType.Catch)
                                //{
                                //    TryStart = method.Body.Instructions.First(),
                                //    TryEnd = lastInstruction,
                                //    HandlerStart = write,
                                //    HandlerEnd = retInstruction,
                                //    CatchType = module.Import(typeof(Exception)),
                                //};


                                //ilProcessor.InsertAfter(firstInstruction, ilProcessor.Create(OpCodes.Callvirt,))
                            }
                        }
                    }
                }
            }
        }
    }
}
