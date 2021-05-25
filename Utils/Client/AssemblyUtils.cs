using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Utils.Client
{
    public static class AssemblyUtils
    {
        public static string GetAssemblyPath(Type type)
            => type.Assembly.Location;

        public static byte[] GetAssemblyBinary(string path)
            => File.ReadAllBytes(path);

        public static byte[] GetAssemblyBinary(Type type)
            => File.ReadAllBytes(type.Assembly.Location);

        public static byte[] GetAssemblyBinary(Assembly assembly)
            => File.ReadAllBytes(assembly.Location);

        public static string GetAssemblyName(Type type)
            => type.Assembly.FullName;


        public static string GetAssemblyName(string path)
            => Assembly.LoadFile(path)?.FullName;

        public static FileVersionInfo GetAssemblyVersion(Type type)
            => FileVersionInfo.GetVersionInfo(type.Assembly.Location);

        public static FileVersionInfo GetAssemblyVersion(Assembly assembly)
            => GetAssemblyVersion(assembly.Location);

        public static FileVersionInfo GetAssemblyVersion(string path)
            => FileVersionInfo.GetVersionInfo(path);

        public static bool IsSystemAssembly(Assembly a)
            => a.FullName.StartsWith("System.") && a.FullName.StartsWith("Microsoft.");

        public static bool IsSystemAssembly(AssemblyName a)
            => a.FullName.StartsWith("System.") && a.FullName.StartsWith("Microsoft.");

        public static bool IsSystemAssembly(string fullName)
            => fullName.StartsWith("System.") && fullName.StartsWith("Microsoft.");

        public static bool IsOskAssembly(AssemblyName a)
            => a.FullName.StartsWith("OrleansStatisticsKeeper.");

        public static bool IsOskAssembly(string fullName)
            => fullName.StartsWith("OrleansStatisticsKeeper.");

        public static IEnumerable<Assembly> GetNonSystemAssemblies(IEnumerable<Assembly> input)
            => input?.Where(a => !a.FullName.StartsWith("System.") && !a.FullName.StartsWith("Microsoft."));

        public static IEnumerable<AssemblyName> GetNonSystemAssemblies(IEnumerable<AssemblyName> input) 
            => input?.Where(a => !a.FullName.StartsWith("System.") && !a.FullName.StartsWith("Microsoft."));

        public static IEnumerable<AssemblyName> GetNonSystemReferencedAssemblies(AssemblyName input, int step = 0, int deepness = 2)
            => GetNonSystemReferencedAssemblies(input.FullName, step, deepness);

        public static IEnumerable<AssemblyName> GetNonSystemReferencedAssemblies(string input, int step = 0, int deepness = 2)
        {
            if (input == null)
                return null;
            if (step == deepness)
                return null;

            var asm = Assembly.Load(input);
            var references = new List<AssemblyName>(asm.GetReferencedAssemblies());

            for (int i= 0; i < references.Count; ++i)
            {
                var @ref = references[i];
                var refAsm = Assembly.Load(@ref);
                var refs = GetNonSystemReferencedAssemblies(refAsm.GetName(), ++step, deepness);
                if (refs == null)
                    break;

                references.AddRange(refs);
            }

            return GetNonSystemAssemblies(references);
        }
    }
}
