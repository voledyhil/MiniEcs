using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MiniEcs.Core.Systems
{
    /// <inheritdoc />
    /// <summary>
    /// Occurs when the list of nodes cannot be sorted.
    /// For example, the system "A" has the attribute EcsUpdateAfterAttribute(typeof(B)),
    /// and the system "B" has the attribute EcsUpdateAfterAttribute(typeof(A))
    /// </summary>
    public class SystemSorterException : Exception
    {
        public SystemSorterException(string message) : base(message)
        {
            
        }
    }

    /// <summary>
    /// Sorts the list of systems based on attributes
    /// <see cref="EcsUpdateBeforeAttribute"/> and <see cref="EcsUpdateAfterAttribute"/>
    /// </summary>
    public static class EcsSystemSorter
    {
        /// <summary>
        /// Sorts the list of systems based on attributes
        /// <see cref="EcsUpdateBeforeAttribute"/> and <see cref="EcsUpdateAfterAttribute"/>
        /// </summary>
        /// <param name="systems">List of systems</param>
        /// <exception cref="SystemSorterException">
        /// Occurs when the list of nodes cannot be sorted
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<T>(List<T> systems)
        {
            SystemDependencies<T>[] dependencies = new SystemDependencies<T>[systems.Count];
            Dictionary<Type, int> systemsDict = new Dictionary<Type, int>();

            for (int i = 0; i < systems.Count; i++)
            {
                T system = systems[i];
                dependencies[i] = new SystemDependencies<T>
                {
                    System = system,
                    Before = new List<Type>()
                };
                systemsDict[system.GetType()] = i;
            }

            for (int i = 0; i < dependencies.Length; i++)
            {
                Type systemType = dependencies[i].System.GetType();
                Attribute[] before = Attribute.GetCustomAttributes(systemType, typeof(EcsUpdateBeforeAttribute));
                Attribute[] after = Attribute.GetCustomAttributes(systemType, typeof(EcsUpdateAfterAttribute));

                foreach (Attribute attr in before)
                {
                    EcsUpdateBeforeAttribute dep = (EcsUpdateBeforeAttribute) attr;
                    if (dep.Type == systemType)
                        continue;

                    if (!systemsDict.TryGetValue(dep.Type, out int index))
                        continue;

                    dependencies[i].Before.Add(dep.Type);
                    dependencies[index].After++;
                }

                foreach (Attribute attr in after)
                {
                    EcsUpdateAfterAttribute dep = (EcsUpdateAfterAttribute) attr;
                    if (dep.Type == systemType)
                        continue;

                    if (!systemsDict.TryGetValue(dep.Type, out int index))
                        continue;

                    dependencies[index].Before.Add(systemType);
                    dependencies[i].After++;
                }
            }

            int size = 0;
            int[] indices = new int[systems.Count];

            systems.Clear();

            for (int i = 0; i < dependencies.Length; i++)
            {
                if (dependencies[i].After == 0)
                    indices[size++] = i;
            }

            while (size > 0)
            {
                int index = indices[0];
                indices[0] = indices[--size];
                SystemDependencies<T> sd = dependencies[index];

                systems.Add(sd.System);
                foreach (Type type in sd.Before)
                {
                    index = systemsDict[type];
                    dependencies[index].After--;
                    if (dependencies[index].After == 0)
                        indices[size++] = index;
                }
            }

            if (systemsDict.Count != systems.Count)
                throw new SystemSorterException("unable to sort systems");
        }

        private struct SystemDependencies<T>
        {
            public T System;
            public List<Type> Before;
            public int After;
        }

    }
}