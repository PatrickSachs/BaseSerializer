using System.Xml.Linq;
using PatrickSachs.Serializer;
using Xunit;
using Xunit.Abstractions;

namespace Test
{
    /// <summary>
    /// This unit test ensures that value types(structs) are compatible with the base serializer.
    /// </summary>
    public class ValueType
    {
        private readonly ITestOutputHelper _output;

        public ValueType(ITestOutputHelper output)
        {
            _output = output;
        }

        public struct NestedData
        {
            public float Float;
            public int Integer;

            public NestedData(float f, int integer)
            {
                Float = f;
                Integer = integer;
            }

            public bool Equals(NestedData other)
            {
                return Float.Equals(other.Float) && Integer == other.Integer;
            }

            public override string ToString()
            {
                return $"{nameof(Float)}: {Float}, {nameof(Integer)}: {Integer}";
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is NestedData other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Float.GetHashCode() * 397) ^ Integer;
                }
            }
        }

        public struct Data
        {
            public string String;
            public int Integer;
            public NestedData NestedData;

            public Data(string s, int integer, NestedData nestedData)
            {
                String = s;
                Integer = integer;
                NestedData = nestedData;
            }

            public bool Equals(Data other)
            {
                return string.Equals(String, other.String) && Integer == other.Integer &&
                       NestedData.Equals(other.NestedData);
            }

            public override string ToString()
            {
                return $"{nameof(String)}: {String}, {nameof(Integer)}: {Integer}, {nameof(NestedData)}: {NestedData}";
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is Data other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = (String != null ? String.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ Integer;
                    hashCode = (hashCode * 397) ^ NestedData.GetHashCode();
                    return hashCode;
                }
            }
        }

        [Fact]
        public void DoesSerializeAndDeserialize()
        {
            Data value = new Data("Hello World", 42, new NestedData(6.9f, 42));

            BaseSerializer serializer = new BaseSerializer();
            XDocument document = serializer.Serialize(value);

            _output.WriteLine(document.ToString());

            Data parsed = (Data) serializer.Deserialize(document);

            _output.WriteLine("Got value: " + parsed + " / Value is: " + value);

            Assert.Equal(value, parsed);
        }
    }
}
