using System;
using System.Collections;
using System.Collections.Generic;
using BinarySerializer.Data;
using BinarySerializer.Expressions;
using BinarySerializer.Serializers.Baselines;

namespace BinarySerializer.Serializers
{
    internal interface IBinaryObjectCollection : IDictionary
    {
        
    }
    
    internal interface IBinaryObjectCollection<TKey> : IBinaryObjectCollection where TKey : unmanaged
    {
        new IEnumerable<TKey> Keys { get; }
        bool TryGetValue(TKey key, out object value);
    }
    
    public class ByteBinaryObjectCollection<T> : Dictionary<byte, T>, IBinaryObjectCollection<byte> where T : class, new()
    {
        IEnumerable<byte> IBinaryObjectCollection<byte>.Keys => Keys;
        public bool TryGetValue(byte key, out object value)
        {
            if (TryGetValue(key, out T tValue))
            {
                value = tValue;
                return true;
            }
            value = null;    
            return false;
        }
    }

    public class ShortBinaryObjectCollection<T> : Dictionary<short, T>, IBinaryObjectCollection<short> where T : class, new()
    {
        IEnumerable<short> IBinaryObjectCollection<short>.Keys => Keys;
        public bool TryGetValue(short key, out object value)
        {
            if (TryGetValue(key, out T tValue))
            {
                value = tValue;
                return true;
            }
            value = null;    
            return false;
        }
    }
    
    public class UShortBinaryObjectCollection<T> : Dictionary<ushort, T>, IBinaryObjectCollection<ushort> where T : class, new()
    {
        IEnumerable<ushort> IBinaryObjectCollection<ushort>.Keys => Keys;
        public bool TryGetValue(ushort key, out object value)
        {
            if (TryGetValue(key, out T tValue))
            {
                value = tValue;
                return true;
            }
            value = null;    
            return false;
        }
    }
    
    public class IntBinaryObjectCollection<T> : Dictionary<int, T>, IBinaryObjectCollection<int> where T : class, new()
    {
        IEnumerable<int> IBinaryObjectCollection<int>.Keys => Keys;
        public bool TryGetValue(int key, out object value)
        {
            if (TryGetValue(key, out T tValue))
            {
                value = tValue;
                return true;
            }
            value = null;    
            return false;
        }
    }
    
    public class UIntBinaryObjectCollection<T> : Dictionary<uint, T>, IBinaryObjectCollection<uint> where T : class, new()
    {
        IEnumerable<uint> IBinaryObjectCollection<uint>.Keys => Keys;
        public bool TryGetValue(uint key, out object value)
        {
            if (TryGetValue(key, out T tValue))
            {
                value = tValue;
                return true;
            }
            value = null;    
            return false;
        }
    }
 
    public class LongBinaryObjectCollection<T> : Dictionary<long, T>, IBinaryObjectCollection<long> where T : class, new()
    {
        IEnumerable<long> IBinaryObjectCollection<long>.Keys => Keys;
        public bool TryGetValue(long key, out object value)
        {
            if (TryGetValue(key, out T tValue))
            {
                value = tValue;
                return true;
            }
            value = null;    
            return false;
        }
    }
    
    public class ULongBinaryObjectCollection<T> : Dictionary<ulong, T>, IBinaryObjectCollection<ulong> where T : class, new()
    {
        IEnumerable<ulong> IBinaryObjectCollection<ulong>.Keys => Keys;
        public bool TryGetValue(ulong key, out object value)
        {
            if (TryGetValue(key, out T tValue))
            {
                value = tValue;
                return true;
            }
            value = null;    
            return false;
        }
    }
    
    public abstract class DictionaryBinarySerializer<TKey, TWriter> : IBinarySerializer<TKey> where TKey : unmanaged where TWriter : IPrimitiveWriter<TKey>
    {
        private readonly int _keySize;
        private readonly TKey _reservedKey;
        private readonly ObjectActivator _itemCreator;
        private readonly CompositeBinarySerializer _itemSerializer;
        private readonly TWriter _writer;

        protected DictionaryBinarySerializer(int keySize, TKey reservedKey, ObjectActivator itemCreator,
            CompositeBinarySerializer itemSerializer, TWriter writer)
        {
            _keySize = keySize;
            _reservedKey = reservedKey;
            _itemCreator = itemCreator;
            _itemSerializer = itemSerializer;
            _writer = writer;
        }

        public void Update(object obj, BinaryDataReader reader)
        {
            IBinaryObjectCollection<TKey> collection = (IBinaryObjectCollection<TKey>) obj;

            while (reader.Position < reader.Length)
            {
                TKey key = _writer.Read(reader);
                if (_reservedKey.Equals(key))
                    break;

                using (BinaryDataReader itemReader = reader.ReadNode())
                {
                    if (!collection.TryGetValue(key, out object value))
                    {
                        value = _itemCreator();
                        collection.Add(key, value);
                    }
                    _itemSerializer.Update(value, itemReader);
                }
            }

            while (reader.Position < reader.Length)
            {
                collection.Remove(_writer.Read(reader));
            }
        }

        public void Serialize(object obj, BinaryDataWriter writer)
        {
            IBinaryObjectCollection<TKey> collection = (IBinaryObjectCollection<TKey>) obj;

            foreach (TKey key in collection.Keys)
            {
                if (_reservedKey.Equals(key))
                    throw new ArgumentException();
                
                BinaryDataWriter itemWriter = writer.TryWriteNode(_keySize);
                _itemSerializer.Serialize(collection[key], itemWriter);
                _writer.Write(writer, key);
                itemWriter.PushNode();
            }
        }

        void IBinarySerializer.Serialize(object obj, BinaryDataWriter writer, IBaseline baseline)
        {
            Serialize(obj, writer, (IBaseline<TKey>) baseline);
        }

        public void Serialize(object obj, BinaryDataWriter writer, IBaseline<TKey> baseline)
        {
            IBinaryObjectCollection<TKey> collections = (IBinaryObjectCollection<TKey>) obj;

            List<TKey> baseKeys = new List<TKey>(baseline.BaselineKeys);
            foreach (TKey key in collections.Keys)
            {
                BinaryDataWriter itemWriter = writer.TryWriteNode(_keySize);
                Baseline<byte> itemBaseline = baseline.GetOrCreateBaseline<Baseline<byte>>(key, _itemSerializer.Count, out bool isNew);
                
                _itemSerializer.Serialize(collections[key], itemWriter, itemBaseline);
                if (itemWriter.Length > 0 || isNew)
                {
                    _writer.Write(writer, key);
                    itemWriter.PushNode();
                }
                
                baseKeys.Remove(key);
            }

            if (baseKeys.Count <= 0)
                return;

            _writer.Write(writer, _reservedKey);
           
            foreach (TKey key in baseKeys)
            {
                _writer.Write(writer, key);
                baseline.DestroyBaseline(key);
            }
        }
    }
    
    public class DictionaryByteKeyBinarySerializer : DictionaryBinarySerializer<byte, ByteWriter>
    {
        public DictionaryByteKeyBinarySerializer(ObjectActivator itemCreator, CompositeBinarySerializer itemSerializer, ByteWriter writer) : base(
            sizeof(byte), byte.MaxValue, itemCreator, itemSerializer, writer)
        {
        }
    }

    public class DictionaryShortKeyBinarySerializer : DictionaryBinarySerializer<short, ShortWriter>
    {
        public DictionaryShortKeyBinarySerializer(ObjectActivator itemCreator, CompositeBinarySerializer itemSerializer, ShortWriter writer) : base(
            sizeof(ushort), short.MaxValue, itemCreator, itemSerializer, writer)
        {
        }
    }
    
    public class DictionaryUShortKeyBinarySerializer : DictionaryBinarySerializer<ushort, UShortWriter>
    {
        public DictionaryUShortKeyBinarySerializer(ObjectActivator itemCreator, CompositeBinarySerializer itemSerializer, UShortWriter writer) : base(
            sizeof(ushort), ushort.MaxValue, itemCreator, itemSerializer, writer)
        {
        }
    }
    
    public class DictionaryIntKeyBinarySerializer : DictionaryBinarySerializer<int, IntWriter>
    {
        public DictionaryIntKeyBinarySerializer(ObjectActivator itemCreator, CompositeBinarySerializer itemSerializer, IntWriter writer) : base(
            sizeof(int), int.MaxValue, itemCreator, itemSerializer, writer)
        {
        }
    }
    
    public class DictionaryUIntKeyBinarySerializer : DictionaryBinarySerializer<uint, UIntWriter>
    {
        public DictionaryUIntKeyBinarySerializer(ObjectActivator itemCreator, CompositeBinarySerializer itemSerializer, UIntWriter writer) : base(
            sizeof(int), uint.MaxValue, itemCreator, itemSerializer, writer)
        {
        }
    }
    
    public class DictionaryLongKeyBinarySerializer : DictionaryBinarySerializer<long, LongWriter>
    {
        public DictionaryLongKeyBinarySerializer(ObjectActivator itemCreator, CompositeBinarySerializer itemSerializer, LongWriter writer) : base(
            sizeof(long), long.MaxValue, itemCreator, itemSerializer, writer)
        {
        }
    }
    
    public class DictionaryULongKeyBinarySerializer : DictionaryBinarySerializer<ulong, ULongWriter>
    {
        public DictionaryULongKeyBinarySerializer(ObjectActivator itemCreator, CompositeBinarySerializer itemSerializer, ULongWriter writer) : base(
            sizeof(long), long.MaxValue, itemCreator, itemSerializer, writer)
        {
        }
    }
}