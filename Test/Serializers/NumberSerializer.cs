using System.Xml.Linq;
using PatrickSachs.Serializer;
using Xunit;
using Xunit.Abstractions;

namespace Test.Serializers
{
    public class NumberSerializer
    {
        private readonly ITestOutputHelper _output;

        public NumberSerializer(ITestOutputHelper output)
        {
            _output = output;
        }

        private class Data
        {
            public sbyte Byte;
            public byte UByte;
            public short Short;
            public ushort UShort;
            public int Integer;
            public uint UInteger;
            public long Long;
            public ulong ULong;
            public float Float;
            public double Double;
            public decimal Decimal;

            protected bool Equals(Data other)
            {
                return Byte == other.Byte && UByte == other.UByte && Short == other.Short && UShort == other.UShort &&
                       Integer == other.Integer && UInteger == other.UInteger && Long == other.Long &&
                       ULong == other.ULong && Float.Equals(other.Float) && Double.Equals(other.Double) &&
                       Decimal.Equals(other.Decimal);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Data) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = Byte.GetHashCode();
                    hashCode = (hashCode * 397) ^ UByte.GetHashCode();
                    hashCode = (hashCode * 397) ^ Short.GetHashCode();
                    hashCode = (hashCode * 397) ^ UShort.GetHashCode();
                    hashCode = (hashCode * 397) ^ Integer;
                    hashCode = (hashCode * 397) ^ (int) UInteger;
                    hashCode = (hashCode * 397) ^ Long.GetHashCode();
                    hashCode = (hashCode * 397) ^ ULong.GetHashCode();
                    hashCode = (hashCode * 397) ^ Float.GetHashCode();
                    hashCode = (hashCode * 397) ^ Double.GetHashCode();
                    hashCode = (hashCode * 397) ^ Decimal.GetHashCode();
                    return hashCode;
                }
            }

            public Data(sbyte b, byte uByte, short s, ushort uShort, int integer, uint uInteger, long l, ulong uLong,
                float f, double d, decimal dec)
            {
                Byte = b;
                UByte = uByte;
                Short = s;
                UShort = uShort;
                Integer = integer;
                UInteger = uInteger;
                Long = l;
                ULong = uLong;
                Float = f;
                Double = d;
                Decimal = dec;
            }

            public override string ToString()
            {
                return
                    $"{nameof(Byte)}: {Byte}, {nameof(UByte)}: {UByte}, {nameof(Short)}: {Short}, {nameof(UShort)}: {UShort}, {nameof(Integer)}: {Integer}, {nameof(UInteger)}: {UInteger}, {nameof(Long)}: {Long}, {nameof(ULong)}: {ULong}, {nameof(Float)}: {Float}, {nameof(Double)}: {Double}, {nameof(Decimal)}: {Decimal}";
            }
        }

        [Fact]
        public void DoesSerializeWithoutCrashing()
        {
            Data value = new Data(42, 42, 42, 42, 42, 42, 42, 42, 42, 42, 42);

            BaseSerializer serializer = new BaseSerializer();
            XDocument document = serializer.Serialize(value);

            _output.WriteLine(document.ToString());
        }

        [Fact]
        public void DoesSerializeAndDeserialize()
        {
            Data value = new Data(42, 42, 42, 42, 42, 42, 42, 42, 42, 42, 42);

            BaseSerializer serializer = new BaseSerializer();
            XDocument document = serializer.Serialize(value);

            _output.WriteLine(document.ToString());

            Data parsed = (Data) serializer.Deserialize(document);

            _output.WriteLine("Got value: " + parsed + " / Value is: " + value);

            Assert.Equal(value, parsed);
        }

        [Fact]
        public void DoesSerializeAndDeserializeLargeFloatingNumbers()
        {
            Data value = new Data(sbyte.MaxValue, byte.MaxValue, short.MaxValue, ushort.MaxValue, int.MaxValue,
                uint.MaxValue, long.MaxValue, ulong.MaxValue, 42.34533f, 42.345324233f, new decimal());

            BaseSerializer serializer = new BaseSerializer();
            XDocument document = serializer.Serialize(value);

            _output.WriteLine(document.ToString());

            Data parsed = (Data) serializer.Deserialize(document);

            _output.WriteLine("Got value: " + parsed + " / Value is: " + value);

            Assert.Equal(value, parsed);
        }

        [Fact]
        public void DoesSerializeAndDeserializeLargeNumbers()
        {
            Data value = new Data(sbyte.MaxValue, byte.MaxValue, short.MaxValue, ushort.MaxValue, int.MaxValue,
                uint.MaxValue, long.MaxValue, ulong.MaxValue, float.MaxValue, double.MaxValue, decimal.MaxValue);

            BaseSerializer serializer = new BaseSerializer();
            XDocument document = serializer.Serialize(value);

            _output.WriteLine(document.ToString());

            Data parsed = (Data) serializer.Deserialize(document);

            _output.WriteLine("Got value: " + parsed + " / Value is: " + value);

            Assert.Equal(value, parsed);
        }
    }
}
