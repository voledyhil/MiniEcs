using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BinarySerializer;
using BinarySerializer.Serializers.Baselines;
using MiniEcs.Core;
using Serializer = BinarySerializer.BinarySerializer;

namespace MiniEcs.Benchmark
{
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    public class SerializeTest
    {
        public SerializeTest()
        {
            EcsComponentType<MiniEcsComponentA>.Register();
            EcsComponentType<MiniEcsComponentB>.Register();
            EcsComponentType<MiniEcsComponentC>.Register();
            EcsComponentType<MiniEcsComponentD>.Register();
            
            _filter = new EcsFilter().AllOf<MiniEcsComponentA, MiniEcsComponentB>();
        }
        
        private class MiniEcsComponentA : IEcsComponent
        {
            [BinaryItem] public bool Bool;
            [BinaryItem] public byte Byte;
            [BinaryItem] public sbyte Sbyte;
            [BinaryItem] public short Short;
            [BinaryItem] public ushort UShort;
            [BinaryItem] public int Int;
            [BinaryItem] public uint UInt;
            [BinaryItem] public long Long;
            [BinaryItem] public ulong ULong;
            [BinaryItem] public double Double;
            [BinaryItem] public char Char;
            [BinaryItem] public float Float;
            [BinaryItem(true)] public float ShortFloat;
            [BinaryItem] public string String;
        }

        private class MiniEcsComponentB : IEcsComponent
        {
            [BinaryItem] public bool Bool;
            [BinaryItem] public byte Byte;
            [BinaryItem] public sbyte Sbyte;
            [BinaryItem] public short Short;
            [BinaryItem] public ushort UShort;
            [BinaryItem] public int Int;
            [BinaryItem] public uint UInt;
            [BinaryItem] public long Long;
            [BinaryItem] public ulong ULong;
            [BinaryItem] public double Double;
            [BinaryItem] public char Char;
            [BinaryItem] public float Float;
            [BinaryItem(true)] public float ShortFloat;
            [BinaryItem] public string String;
        }

        private class MiniEcsComponentC : IEcsComponent
        {
        }

        private class MiniEcsComponentD : IEcsComponent
        {
        }

        private EcsWorld _source;
        private Baseline<uint> _baseline;
        private byte[] _data;
        private readonly EcsFilter _filter;


        [GlobalSetup]
        public void Setup()
        {
            _source = new EcsWorld();

            for (int i = 0; i < byte.MaxValue; ++i)
            {
                _source.CreateEntity(InstantiateComponentA(), InstantiateComponentB(), new MiniEcsComponentD());
                _source.CreateEntity(InstantiateComponentA(), new MiniEcsComponentC());
                _source.CreateEntity(InstantiateComponentB(), new MiniEcsComponentD());
                _source.CreateEntity(InstantiateComponentB(), new MiniEcsComponentD());
                _source.CreateEntity(InstantiateComponentB(), new MiniEcsComponentC());
                _source.CreateEntity(InstantiateComponentA(), InstantiateComponentB());
                _source.CreateEntity(InstantiateComponentA(), new MiniEcsComponentD());
            }

            _baseline = new Baseline<uint>();
            _data = _source.Serialize(_filter, _baseline);
        }

        private static MiniEcsComponentB InstantiateComponentB()
        {
            return new MiniEcsComponentB
            {
                Bool = true,
                Byte = byte.MaxValue - 1,
                Double = double.MaxValue - 1,
                Int = int.MaxValue - 1,
                Long = long.MaxValue - 1,
                Sbyte = sbyte.MaxValue - 1,
                Short = short.MaxValue - 1,
                UInt = uint.MaxValue - 1,
                ULong = ulong.MaxValue - 1,
                UShort = ushort.MaxValue - 1,
                Char = char.MaxValue,
                Float = float.MaxValue - 1,
                ShortFloat = 1.5f,
                String = "DotNet"
            };
        }

        private static MiniEcsComponentA InstantiateComponentA()
        {
            return new MiniEcsComponentA
            {
                Bool = true,
                Byte = byte.MaxValue - 1,
                Double = double.MaxValue - 1,
                Int = int.MaxValue - 1,
                Long = long.MaxValue - 1,
                Sbyte = sbyte.MaxValue - 1,
                Short = short.MaxValue - 1,
                UInt = uint.MaxValue - 1,
                ULong = ulong.MaxValue - 1,
                UShort = ushort.MaxValue - 1,
                Char = char.MaxValue,
                Float = float.MaxValue - 1,
                ShortFloat = 1.5f,
                String = "DotNet"
            };
        }


        [Benchmark]
        public void SimpleSerializeTest()
        {
            byte[] data = _source.Serialize(_filter);
        }

        [Benchmark]
        public void SerializeEmptyBaselineTest()
        {
            Baseline<uint> baseline = new Baseline<uint>();
            byte[] data = _source.Serialize(_filter, baseline);
        }

        [Benchmark]
        public void SerializeFullBaselineTest()
        {
            byte[] data = _source.Serialize(_filter, _baseline);
        }

        [Benchmark]
        public void DeserializeTest()
        {
            EcsWorld target = new EcsWorld();
            target.Update(_data);
        }
    }
}