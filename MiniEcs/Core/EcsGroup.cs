using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MiniEcs.Core
{
    public delegate void ForEachArchetypeHandler(EcsArchetype archetype);
    public delegate void ForEachEHandler(IEcsEntity entity);

    public delegate void ForEachECHandler<in C0>(IEcsEntity entity, C0 comp0) where C0 : IEcsComponent;
    public delegate void ForEachECCHandler<in C0, in C1>(IEcsEntity entity, C0 comp0, C1 comp1) where C0 : IEcsComponent where C1 : IEcsComponent;
    public delegate void ForEachECCCHandler<in C0, in C1, in C2>(IEcsEntity entity, C0 comp0, C1 comp1, C2 comp2) where C0 : IEcsComponent where C1 : IEcsComponent where C2 : IEcsComponent;
    public delegate void ForEachECCCCHandler<in C0, in C1, in C2, in C3>(IEcsEntity entity, C0 comp0, C1 comp1, C2 comp2, C3 comp3) where C0 : IEcsComponent where C1 : IEcsComponent where C2 : IEcsComponent where C3 : IEcsComponent;

    /// <summary>
    /// collection of archetypes matching filter criteria
    /// </summary>
    public interface IEcsGroup
    {
        /// <summary>
        /// Calculate the number of entities in a group
        /// </summary>
        /// <returns>Number of entities</returns>
        int CalculateCount();

        void ForEach(ForEachArchetypeHandler handler);
        void ForEach(ForEachEHandler handler);
        void ForEach<C0>(ForEachECHandler<C0> handler) 
            where C0 : IEcsComponent;
        
        void ForEach<C0, C1>(ForEachECCHandler<C0, C1> handler) 
            where C0 : IEcsComponent 
            where C1 : IEcsComponent;

        void ForEach<C0, C1, C2>(ForEachECCCHandler<C0, C1, C2> handler) where C0 : IEcsComponent
            where C1 : IEcsComponent
            where C2 : IEcsComponent;

        void ForEach<C0, C1, C2, C3>(ForEachECCCCHandler<C0, C1, C2, C3> handler) where C0 : IEcsComponent
            where C1 : IEcsComponent
            where C2 : IEcsComponent
            where C3 : IEcsComponent;

        IEcsEntity[] ToEntityArray();
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
                count += archetype.EntitiesCount;
            }
            return count;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ForEach(ForEachArchetypeHandler handler)
        {
            for (int i = 0; i < _archetypes.Count; i++)
            {
                EcsArchetype archetype = _archetypes[i];
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ForEach<C0>(ForEachECHandler<C0> handler) where C0 : IEcsComponent
        {
            ForEach(archetype =>
            {
                EcsComponentPool<C0> comps0 = archetype.GetComponentPool<C0>();

                EcsEntity[] entities = archetype.GetEntities(out int length);
                for (int j = 0; j < length; j++)
                {
                    handler(entities[j], comps0.GetTyped(j));
                }   
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ForEach<C0, C1>(ForEachECCHandler<C0, C1> handler) where C0 : IEcsComponent where C1 : IEcsComponent
        {
            ForEach(archetype =>
            {
                EcsComponentPool<C0> comps0 = archetype.GetComponentPool<C0>();
                EcsComponentPool<C1> comps1 = archetype.GetComponentPool<C1>();

                EcsEntity[] entities = archetype.GetEntities(out int length);
                for (int j = 0; j < length; j++)
                {
                    handler(entities[j], comps0.GetTyped(j), comps1.GetTyped(j));
                }    
            });
        }

        public void ForEach<C0, C1, C2>(ForEachECCCHandler<C0, C1, C2> handler) where C0 : IEcsComponent where C1 : IEcsComponent where C2 : IEcsComponent
        {
            ForEach(archetype =>
            {
                EcsComponentPool<C0> comps0 = archetype.GetComponentPool<C0>();
                EcsComponentPool<C1> comps1 = archetype.GetComponentPool<C1>();
                EcsComponentPool<C2> comps2 = archetype.GetComponentPool<C2>();

                EcsEntity[] entities = archetype.GetEntities(out int length);
                for (int j = 0; j < length; j++)
                {
                    handler(entities[j], comps0.GetTyped(j), comps1.GetTyped(j), comps2.GetTyped(j));
                }    
            });
        }

        public void ForEach<C0, C1, C2, C3>(ForEachECCCCHandler<C0, C1, C2, C3> handler) where C0 : IEcsComponent where C1 : IEcsComponent where C2 : IEcsComponent where C3 : IEcsComponent
        {
            ForEach(archetype =>
            {
                EcsComponentPool<C0> comps0 = archetype.GetComponentPool<C0>();
                EcsComponentPool<C1> comps1 = archetype.GetComponentPool<C1>();
                EcsComponentPool<C2> comps2 = archetype.GetComponentPool<C2>();
                EcsComponentPool<C3> comps3 = archetype.GetComponentPool<C3>();

                EcsEntity[] entities = archetype.GetEntities(out int length);
                for (int j = 0; j < length; j++)
                {
                    handler(entities[j], comps0.GetTyped(j), comps1.GetTyped(j), comps2.GetTyped(j), comps3.GetTyped(j));
                }    
            });
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
    }
}