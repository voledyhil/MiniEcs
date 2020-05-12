using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MiniEcs.Core
{
    /// <inheritdoc />
    /// <summary>
    /// collection of archetypes matching filter criteria
    /// </summary>
    public interface IEcsGroup : IEnumerable<EcsEntity>
    {
        /// <summary>
        /// Calculate the number of entities in a group
        /// </summary>
        /// <returns>Number of entities</returns>
        int CalculateCount();
    }

    /// <inheritdoc />
    /// <summary>
    /// Сollection of archetypes matching filter criteria
    /// </summary>
    public class EcsGroup : IEcsGroup
    {
        /// <summary>
        /// Сurrent group version
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// List of current archetypes
        /// </summary>
        private readonly List<EcsArchetype> _archetypes;

        /// <summary>
        /// Creates a new archetype group corresponding to the specified version.
        /// </summary>
        /// <param name="version">Group version</param>
        /// <param name="archetypes">Archetype collection</param>
        public EcsGroup(int version, IEnumerable<EcsArchetype> archetypes)
        {
            Version = version;
            _archetypes = new List<EcsArchetype>(archetypes);
        }

        /// <summary>
        /// Adds new archetypes to the group, raises the version of the group
        /// </summary>
        /// <param name="newVersion">New Version</param>
        /// <param name="newArchetypes">New Archetypes</param>
        public void Update(int newVersion, IEnumerable<EcsArchetype> newArchetypes)
        {
            Version = newVersion;
            _archetypes.AddRange(newArchetypes);
        }

        /// <inheritdoc />
        /// <summary>
        /// Calculate the number of entities in a group
        /// </summary>
        /// <returns>Number of entities</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CalculateCount()
        {
            int count = 0;
            foreach (EcsArchetype archetype in _archetypes)
            {
                count += archetype.Entities.Count;
            }
            return count;
        }

        /// <inheritdoc />
        /// <summary>
        /// Returns an enumerator of all entities in the group
        /// </summary>
        /// <returns>Enumerator of entities</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<EcsEntity> GetEnumerator()
        {
            for (int i = 0; i < _archetypes.Count; i++)
            {
                EcsArchetype archetype = _archetypes[i];
                if (archetype.Entities.Count <= 0)
                    continue;

                HashSet<EcsEntity>.Enumerator enumerator = archetype.GetEnumerator();
                while (enumerator.MoveNext())
                    yield return enumerator.Current;

                enumerator.Dispose();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}