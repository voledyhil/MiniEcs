using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MiniEcs.Core
{
    /// <summary>
    /// Manager of all archetypes.
    /// Carries out the creation of new archetypes,
    /// and searches for existing archetypes
    /// </summary>
    public class EcsArchetypeManager
    {
        /// <summary>
        /// Number of existing archetypes
        /// </summary>
        public int ArchetypeCount => _archetypes.Count;

        /// <summary>
        /// Returns the first empty archetype
        /// </summary>
        public EcsArchetype Empty => _emptyArchetype;

        /// <summary>
        /// Archetype unique identifier generator
        /// </summary>
        private int _archetypeIdCounter;

        /// <summary>
        /// First empty archetype
        /// </summary>
        private readonly EcsArchetype _emptyArchetype;

        /// <summary>
        /// List of all archetypes created
        /// </summary>
        private readonly List<EcsArchetype> _archetypes;

        /// <summary>
        /// List of all archetypes corresponding to each type of component
        /// </summary>
        private readonly List<EcsArchetype>[] _archetypeIndices;

        /// <summary>
        /// Creates an archetype manager
        /// </summary>
        public EcsArchetypeManager()
        {
            _emptyArchetype = new EcsArchetype(_archetypeIdCounter++, new byte[] { });
            _archetypes = new List<EcsArchetype> {_emptyArchetype};
            _archetypeIndices = new List<EcsArchetype>[byte.MaxValue];

            for (int i = 0; i < _archetypeIndices.Length; i++)
            {
                _archetypeIndices[i] = new List<EcsArchetype>();
            }
        }

        /// <summary>
        /// Gets the enumerator of all archetypes whose identifier
        /// is greater than or equal to startId
        /// </summary>
        /// <param name="startId">Archetype start id</param>
        /// <returns>Archetype enumerator</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<EcsArchetype> GetArchetypes(int startId)
        {
            for (int i = startId; i < _archetypes.Count; i++)
            {
                yield return _archetypes[i];
            }
        }

        /// <summary>
        /// Gets an enumerator of all archetypes that have the specified type
        /// of component whose identifier is greater than or equal to startId
        /// </summary>
        /// <param name="index">Unique type of component</param>
        /// <param name="startId">Archetype start id</param>
        /// <returns>Archetype enumerator</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<EcsArchetype> GetArchetypes(byte index, int startId)
        {
            List<EcsArchetype> archetypes = _archetypeIndices[index];

            for (int i = archetypes.Count - 1; i >= 0; i--)
            {
                EcsArchetype archetype = archetypes[i];
                if (archetype.Id <= startId)
                    break;

                yield return archetype;
            }
        }

        /// <summary>
        /// Finds an existing archetype or creates a new one based on
        /// a set of unique types of components.
        /// </summary>
        /// <param name="indices">A set of unique component types.</param>
        /// <returns></returns>
        public EcsArchetype FindOrCreateArchetype(params byte[] indices)
        {
            Array.Sort(indices);

            return InnerFindOrCreateArchetype(indices);
        }

        /// <summary>
        /// Finds an existing archetype or creates a new one based on a set
        /// of unique types of components without pre-sorting
        /// to improve performance.
        /// </summary>
        /// <param name="indices">A set of unique component types.</param>
        /// <returns>Existing or new archetype</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private EcsArchetype InnerFindOrCreateArchetype(byte[] indices)
        {
            EcsArchetype curArchetype = _emptyArchetype;
            for (int i = 0; i < indices.Length; i++)
            {
                byte index = indices[i];
                if (!curArchetype.Next.TryGetValue(index, out EcsArchetype nextArchetype))
                {
                    byte[] archetypeIndices = new byte[i + 1];
                    for (int j = 0; j < archetypeIndices.Length; j++)
                        archetypeIndices[j] = indices[j];

                    nextArchetype = new EcsArchetype(_archetypeIdCounter++, archetypeIndices);
                    nextArchetype.Prior[index] = curArchetype;
                    foreach (ushort componentType in nextArchetype.Indices)
                    {
                        _archetypeIndices[componentType].Add(nextArchetype);
                    }

                    curArchetype.Next[index] = nextArchetype;

                    _archetypes.Add(nextArchetype);
                }

                curArchetype = nextArchetype;
            }

            return curArchetype;
        }

        /// <summary>
        /// Finds or creates the next archetype based on an existing archetype
        /// after adding a new component type
        /// </summary>
        /// <param name="archetype">base archetype</param>
        /// <param name="addIndex">new component type</param>
        /// <returns>Existing or new archetype</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EcsArchetype FindOrCreateNextArchetype(EcsArchetype archetype, byte addIndex)
        {
            if (archetype.Next.TryGetValue(addIndex, out EcsArchetype nextArchetype))
                return nextArchetype;

            bool added = false;
            int length = 0;
            byte[] indices = new byte[archetype.IndicesCount + 1];
            foreach (byte index in archetype.Indices)
            {
                if (addIndex < index && !added)
                {
                    indices[length++] = addIndex;
                    added = true;
                }

                indices[length++] = index;
            }

            if (!added)
                indices[length] = addIndex;

            return InnerFindOrCreateArchetype(indices);
        }

        /// <summary>
        /// Finds or creates a previous archetype based on an existing archetype
        /// after deleting an existing component type
        /// </summary>
        /// <param name="archetype">base archetype</param>
        /// <param name="removeIndex">component type</param>
        /// <returns>Existing or new archetype</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EcsArchetype FindOrCreatePriorArchetype(EcsArchetype archetype, byte removeIndex)
        {
            if (archetype.Prior.TryGetValue(removeIndex, out EcsArchetype priorArchetype))
                return priorArchetype;

            int length = 0;
            byte[] indices = new byte[archetype.IndicesCount - 1];
            foreach (byte index in archetype.Indices)
            {
                if (index != removeIndex)
                    indices[length++] = index;
            }

            return InnerFindOrCreateArchetype(indices);
        }
    }
}