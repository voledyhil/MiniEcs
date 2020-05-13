using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MiniEcs.Core
{
    public delegate void ForEachEHandler(IEcsEntity entity);

    public delegate void ForEachArchetypeHandler(IEcsArchetype archetype);

    public delegate void ForEachEcHandler<in TC0>(IEcsEntity entity, TC0 comp0) where TC0 : IEcsComponent;

    public delegate void ForEachEccHandler<in TC0, in TC1>(IEcsEntity entity, TC0 comp0, TC1 comp1)
        where TC0 : IEcsComponent where TC1 : IEcsComponent;

    public delegate void ForEachEcccHandler<in TC0, in TC1, in TC2>(IEcsEntity entity, TC0 comp0, TC1 comp1, TC2 comp2)
        where TC0 : IEcsComponent where TC1 : IEcsComponent where TC2 : IEcsComponent;

    public delegate void ForEachEccccHandler<in TC0, in TC1, in TC2, in TC3>(IEcsEntity entity, TC0 comp0, TC1 comp1,
        TC2 comp2, TC3 comp3) where TC0 : IEcsComponent
        where TC1 : IEcsComponent
        where TC2 : IEcsComponent
        where TC3 : IEcsComponent;

    /// <summary>
    /// collection of archetypes matching filter criteria
    /// </summary>
    public interface IEcsGroup
    {
        int CalculateCount();

        void ForEach(ForEachArchetypeHandler handler);
        void ForEach(ForEachEHandler handler);
        void ForEach<TC0>(ForEachEcHandler<TC0> handler) where TC0 : IEcsComponent;
        void ForEach<TC0, TC1>(ForEachEccHandler<TC0, TC1> handler) where TC0 : IEcsComponent where TC1 : IEcsComponent;

        void ForEach<TC0, TC1, TC2>(ForEachEcccHandler<TC0, TC1, TC2> handler) where TC0 : IEcsComponent
            where TC1 : IEcsComponent
            where TC2 : IEcsComponent;

        void ForEach<TC0, TC1, TC2, TC3>(ForEachEccccHandler<TC0, TC1, TC2, TC3> handler) where TC0 : IEcsComponent
            where TC1 : IEcsComponent
            where TC2 : IEcsComponent
            where TC3 : IEcsComponent;

        IEcsEntity[] ToEntityArray();
    }

    /// <inheritdoc />
    /// <summary>
    /// Сollection of archetypes matching filter criteria
    /// </summary>
    public class EcsGroup : IEcsGroup
    {
        private delegate void ForEachArchetypeHandler(EcsArchetype archetype);

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
                count += archetype.EntitiesCount;
            }

            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ForEach(ForEachArchetypeHandler handler)
        {
            foreach (EcsArchetype archetype in _archetypes)
            {
                if (archetype.EntitiesCount <= 0)
                    continue;

                handler(archetype);
            }
        }

        public IEcsEntity[] ToEntityArray()
        {
            int index = 0;
            IEcsEntity[] totalEntities = new IEcsEntity[CalculateCount()];

            ForEach(archetype =>
            {
                EcsEntity[] entities = archetype.GetEntities(out int length);
                for (int j = 0; j < length; j++)
                {
                    totalEntities[index++] = entities[j];
                }
            });

            return totalEntities;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ForEach(Core.ForEachArchetypeHandler handler)
        {
            foreach (EcsArchetype archetype in _archetypes)
            {
                if (archetype.EntitiesCount <= 0)
                    continue;

                handler(archetype);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ForEach(ForEachEHandler handler)
        {
            ForEach(archetype =>
            {
                EcsEntity[] entities = archetype.GetEntities(out int length);
                for (int j = 0; j < length; j++)
                {
                    handler(entities[j]);
                }
            });
        }

        void IEcsGroup.ForEach<TC0>(ForEachEcHandler<TC0> handler)
        {
            ForEach(archetype =>
            {
                EcsComponentPool<TC0> comps0 = archetype.GetComponentPool<TC0>();

                EcsEntity[] entities = archetype.GetEntities(out int length);
                for (int j = 0; j < length; j++)
                {
                    handler(entities[j], comps0.GetTyped(j));
                }
            });
        }


        void IEcsGroup.ForEach<TC0, TC1>(ForEachEccHandler<TC0, TC1> handler)
        {
            ForEach(archetype =>
            {
                EcsComponentPool<TC0> comps0 = archetype.GetComponentPool<TC0>();
                EcsComponentPool<TC1> comps1 = archetype.GetComponentPool<TC1>();

                EcsEntity[] entities = archetype.GetEntities(out int length);
                for (int j = 0; j < length; j++)
                {
                    handler(entities[j], comps0.GetTyped(j), comps1.GetTyped(j));
                }
            });
        }

        void IEcsGroup.ForEach<TC0, TC1, TC2>(ForEachEcccHandler<TC0, TC1, TC2> handler)
        {
            ForEach(archetype =>
            {
                EcsComponentPool<TC0> comps0 = archetype.GetComponentPool<TC0>();
                EcsComponentPool<TC1> comps1 = archetype.GetComponentPool<TC1>();
                EcsComponentPool<TC2> comps2 = archetype.GetComponentPool<TC2>();

                EcsEntity[] entities = archetype.GetEntities(out int length);
                for (int j = 0; j < length; j++)
                {
                    handler(entities[j], comps0.GetTyped(j), comps1.GetTyped(j), comps2.GetTyped(j));
                }
            });
        }

        void IEcsGroup.ForEach<TC0, TC1, TC2, TC3>(ForEachEccccHandler<TC0, TC1, TC2, TC3> handler)
        {
            ForEach(archetype =>
            {
                EcsComponentPool<TC0> comps0 = archetype.GetComponentPool<TC0>();
                EcsComponentPool<TC1> comps1 = archetype.GetComponentPool<TC1>();
                EcsComponentPool<TC2> comps2 = archetype.GetComponentPool<TC2>();
                EcsComponentPool<TC3> comps3 = archetype.GetComponentPool<TC3>();

                EcsEntity[] entities = archetype.GetEntities(out int length);
                for (int j = 0; j < length; j++)
                {
                    handler(entities[j], comps0.GetTyped(j), comps1.GetTyped(j), comps2.GetTyped(j),
                        comps3.GetTyped(j));
                }
            });
        }
    }
}