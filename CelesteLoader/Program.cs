using CelesteModAPI;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CelesteLoader
{
    class PatchResolver : IAssemblyResolver
    {
        private AssemblyDefinition assembly;
        private static IAssemblyResolver baseResolver = new DefaultAssemblyResolver();

        public PatchResolver(AssemblyDefinition originalCeleste)
        {
            assembly = originalCeleste;
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            if (name.Name == "CelestePlaceholder") return assembly;
            return baseResolver.Resolve(name);
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            if (name.Name == "CelestePlaceholder") return assembly;
            return baseResolver.Resolve(name, parameters);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

    }

    class Program
    {
        static void Main(string[] args)
        {
            AssemblyDefinition celesteOriginal = AssemblyDefinition.ReadAssembly("CelestePlaceholder.exe", new ReaderParameters { InMemory = true });
            AssemblyDefinition celestePatched = AssemblyDefinition.ReadAssembly("CelestePlaceholder.exe", new ReaderParameters { InMemory = true });
            var patchResolver = new PatchResolver(celesteOriginal);
            AssemblyDefinition celesteMod = AssemblyDefinition.ReadAssembly("CelesteExampleMod.dll", new ReaderParameters { InMemory = true, AssemblyResolver = patchResolver});

            celestePatched.Name = new AssemblyNameDefinition("PatchedCeleste", celesteOriginal.Name.Version);

            Console.WriteLine(celestePatched.FullName);

            var patches = FindPatches(celesteMod);

            foreach (TypeDefinition patch in patches)
            {
                var originalPatchedClass = patch.BaseType.Resolve();

                var patchedClass = celestePatched.MainModule.GetType(originalPatchedClass.FullName);

                patch.BaseType = patchedClass.Module.ImportReference(originalPatchedClass);
                patch.GenericParameters.ElementAt(0).Constraints.Clear();
                patch.GenericParameters.ElementAt(0).Constraints.Add(patchedClass.Module.ImportReference(typeof(IPatch)));
                patchedClass.BaseType = patchedClass.Module.ImportReference(patch);
                //patchedClass.Attributes = new Mono.Cecil.TypeAttributes { };
                patchedClass.Methods.Clear();
                //patchedClass.NestedTypes.Clear();
                //patchedClass.SecurityDeclarations.Clear();
                //patchedClass.Events.Clear();
                //patchedClass.Fields.Clear();
            }

            var stream = new MemoryStream();

            celestePatched.Write(stream, new WriterParameters() { });

            var patchedCeleste = Assembly.Load(stream.ToArray());

            patchedCeleste.GetType("Program").GetMethod("main").Invoke(null, null);
        }

        static List<TypeDefinition> FindPatches(AssemblyDefinition mod)
        {
            var patches = new List<TypeDefinition>();

            foreach (TypeDefinition type in mod.MainModule.Types)
            {
                patches.AddRange(ProcessTypeForPatches(type));
            }

            return patches;
        }

        static List<TypeDefinition> ProcessTypeForPatches(TypeDefinition type)
        {
            var patches = new List<TypeDefinition>();

            foreach (TypeDefinition subtype in type.NestedTypes)
            {
                patches.AddRange(ProcessTypeForPatches(subtype));
            }

            var genericParameters = type.GenericParameters;

            if (genericParameters.Count == 1 && genericParameters.ElementAt(0).Constraints.ElementAt(0).FullName == "CelesteModAPI.IPatch")
            {
                patches.Add(type);
            }

            return patches;
        }
    }
}
