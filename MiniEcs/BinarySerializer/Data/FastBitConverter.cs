using System.Runtime.InteropServices;

namespace BinarySerializer.Data
{
    public static class FastBitConverter
    {
        [StructLayout(LayoutKind.Explicit)]
        private struct ConverterHelperDouble
        {
            [FieldOffset(0)]
            public ulong Along;

            [FieldOffset(0)]
            public double Adouble;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct ConverterHelperFloat
        {
            [FieldOffset(0)]
            public int Aint;

            [FieldOffset(0)]
            public float Afloat;
        }
        
        public static void Write(byte[] buffer, int offset, ulong data)
        {
            buffer[offset] = (byte)data;
            buffer[offset + 1] = (byte)(data >> 8);
            buffer[offset + 2] = (byte)(data >> 16);
            buffer[offset + 3] = (byte)(data >> 24);
            buffer[offset + 4] = (byte)(data >> 32);
            buffer[offset + 5] = (byte)(data >> 40);
            buffer[offset + 6] = (byte)(data >> 48);
            buffer[offset + 7] = (byte)(data >> 56);
        }

        public static void Write(byte[] buffer, int offset, int data)
        {
            buffer[offset] = (byte)data;
            buffer[offset + 1] = (byte)(data >> 8);
            buffer[offset + 2] = (byte)(data >> 16);
            buffer[offset + 3] = (byte)(data >> 24);
        }

        public static void Write(byte[] buffer, int offset, short data)
        {
            buffer[offset] = (byte)data;
            buffer[offset + 1] = (byte)(data >> 8);
        }
        
        public static void Write(byte[] bytes, int startIndex, double value)
        {
            ConverterHelperDouble ch = new ConverterHelperDouble { Adouble = value };
            Write(bytes, startIndex, ch.Along);
        }

        public static void Write(byte[] bytes, int startIndex, float value)
        {
            ConverterHelperFloat ch = new ConverterHelperFloat { Afloat = value };
            Write(bytes, startIndex, ch.Aint);
        }

        public static void Write(byte[] bytes, int startIndex, ushort value)
        {
            Write(bytes, startIndex, (short)value);
        }

        public static void Write(byte[] bytes, int startIndex, uint value)
        {
            Write(bytes, startIndex, (int)value);
        }

        public static void Write(byte[] bytes, int startIndex, long value)
        {
            Write(bytes, startIndex, (ulong)value);
        }
    }
}
