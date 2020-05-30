using System;
using System.Collections.Generic;
using BinarySerializer.Data;
using BinarySerializer.Serializers;
using BinarySerializer.Serializers.Baselines;
using Serializer = BinarySerializer.BinarySerializer;

namespace MiniEcs.Core
{
    public partial class EcsWorld
    {
        private EcsEntityExtended CreateEntity(uint id)
        {
            EcsEntityExtended entity = _entitiesPool.Count <= 0
                ? new EcsEntityExtended(_entitiesPool, _entities, _archetypeManager)
                : _entitiesPool.Dequeue();
            
            entity.Initialize(id);
            
            _entities.Add(id, entity);
            _entityCounter = Math.Max(_entityCounter, ++id);
            
            return entity;
        }

        /// <summary>
        /// Reads data from DataReader, and updates the world of entities.
        /// </summary>
        /// <param name="reader">Data Reader</param>
        public void Update(BinaryDataReader reader)
        {   
            while (reader.Position < reader.Length)
            {
                uint key = reader.ReadUInt();
                if (key == uint.MaxValue)
                    break;

                if (!_entities.TryGetValue(key, out EcsEntityExtended entity))
                    entity = CreateEntity(key); 
                
                using (BinaryDataReader entityReader = reader.ReadNode())
                {
                    while (entityReader.Position < entityReader.Length)
                    {
                        byte index = entityReader.ReadByte();
                        if (index == byte.MaxValue)
                            break;
                        
                        using (BinaryDataReader componentReader = entityReader.ReadNode())
                        {
                            IEcsComponent component;
                            if (entity.HasComponent(index))
                            {
                                component = entity.GetComponent(index);
                            }
                            else
                            {
                                component = EcsTypeManager.CreateComponent(index);
                                entity.AddComponent(index, component);
                            }
                            
                            Serializer.GetSerializer(EcsTypeManager.Types[index]).Update(component, componentReader);
                        }
                    }
                    
                    while (entityReader.Position < entityReader.Length)
                    {
                        entity.RemoveComponent(entityReader.ReadByte());
                    }
                }
            }

            while (reader.Position < reader.Length)
            {
                _entities[reader.ReadUInt()].Destroy();
            }
        }

        /// <summary>
        /// Serializes all entities matching the specified filter
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="writer">Data writer</param>
        public void Serialize(EcsFilter filter, BinaryDataWriter writer)
        {
            IEcsGroup group = Filter(filter);
            
            foreach (IEcsArchetype archetype in group)
            {
                byte[] indices = archetype.Indices;

                for (int i = 0; i < archetype.EntitiesCount; i++)
                {
                    IEcsEntity entity = archetype[i];
         
                    BinaryDataWriter entityWriter = writer.TryWriteNode(sizeof(uint));                   
                    foreach (byte index in indices)
                    {
                        CompositeBinarySerializer ser = Serializer.GetSerializer(EcsTypeManager.Types[index]);
                        BinaryDataWriter componentWriter = entityWriter.TryWriteNode(sizeof(byte));
                        ser.Serialize(archetype.GetComponentPool(index).Get(i), componentWriter);
                        entityWriter.WriteByte(index);
                        componentWriter.PushNode();
                    }

                    writer.WriteUInt(entity.Id);
                    entityWriter.PushNode();
                }   
            }
        }
        
        /// <summary>
        /// Serializes all objects matching the specified filter. Regarding baseline
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="writer">Data Writer</param>
        /// <param name="baseline">Baseline</param>
        public void Serialize(EcsFilter filter, BinaryDataWriter writer, Baseline<uint> baseline)
        {
            IEcsGroup group = Filter(filter);

            List<uint> entitiesBaseKeys = new List<uint>(baseline.BaselineKeys);
            foreach (IEcsArchetype archetype in group)
            {
                byte[] indices = archetype.Indices;
                
                for (int i = 0; i < archetype.EntitiesCount; i++)
                {
                    IEcsEntity entity = archetype[i];

                    uint entityId = entity.Id;
                    
                    BinaryDataWriter entityWriter = writer.TryWriteNode(sizeof(uint));
                    Baseline<byte> entityBaseline = baseline.GetOrCreateBaseline<Baseline<byte>>(entityId, 0, out bool entIsNew);
                    List<byte> entityBaseKeys = new List<byte>(entityBaseline.BaselineKeys);

                    foreach (byte index in indices)
                    {
                        CompositeBinarySerializer ser = Serializer.GetSerializer(EcsTypeManager.Types[index]);
                        BinaryDataWriter compWriter = entityWriter.TryWriteNode(sizeof(byte));
                        Baseline<byte> compBaseline = entityBaseline.GetOrCreateBaseline<Baseline<byte>>(index, ser.Count, out bool compIsNew);
                        ser.Serialize(archetype.GetComponentPool(index).Get(i), compWriter, compBaseline);

                        if (compWriter.Length > 0 || compIsNew)
                        {
                            entityWriter.WriteByte(index);
                            compWriter.PushNode();
                        }

                        entityBaseKeys.Remove(index);
                    }

                    if (entityBaseKeys.Count > 0)
                    {
                        entityWriter.WriteByte(byte.MaxValue);

                        foreach (byte key in entityBaseKeys)
                        {
                            entityWriter.WriteByte(key);
                            entityBaseline.DestroyBaseline(key);
                        }
                    }

                    if (entityWriter.Length > 0 || entIsNew)
                    {
                        writer.WriteUInt(entityId);
                        entityWriter.PushNode();
                    }

                    entitiesBaseKeys.Remove(entityId);
                }
            }

            if (entitiesBaseKeys.Count <= 0)
                return;
            
            writer.WriteUInt(uint.MaxValue);

            foreach (uint key in entitiesBaseKeys)
            {
                writer.WriteUInt(key);
                baseline.DestroyBaseline(key);
            }
        }
    }
}