using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace ProPatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
                return;

            string assemblyPath = args[0];

            AssemblyDefinition assembly = Mono.Cecil.AssemblyDefinition.ReadAssembly(assemblyPath);

            var type = assembly.MainModule.Types.Single(t => t.Name.Equals("RVOAuthPCO", StringComparison.InvariantCultureIgnoreCase));

            var method = type.Methods.Single(x => x.Name.Equals("APIWebRequest", StringComparison.InvariantCultureIgnoreCase));

            if (method.IsPublic)
                return;

            method.IsPublic = true;

            assembly.Write(assemblyPath);
        }
    }
}
