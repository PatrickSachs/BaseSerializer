using System.Xml.Linq;
using PatrickSachs.Serializer;
using Xunit;
using Xunit.Abstractions;

namespace Test.Serializers
{
    public class NullValue
    {
        /// <summary>
        /// A test data structure. The equality members are required for Equals assertion checking.
        /// </summary>
        private class Data
        {
            public string String;
            public int Integer;
            public Data Nested;
            
            protected bool Equals(Data other)
            {
                return string.Equals(String, other.String) && Integer == other.Integer && Equals(Nested, other.Nested);
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
                    var hashCode = (String != null ? String.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ Integer;
                    hashCode = (hashCode * 397) ^ (Nested != null ? Nested.GetHashCode() : 0);
                    return hashCode;
                }
            }
        }

        private readonly ITestOutputHelper _output;

        public NullValue(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void DoesSerializeWithoutCrashing()
        {
            BaseSerializer serializer = new BaseSerializer();
            XDocument document = serializer.Serialize(null);

            _output.WriteLine(document.ToString());
        }

        [Fact]
        public void DoesSerializeAndDeserialize()
        {
            BaseSerializer serializer = new BaseSerializer();
            XDocument document = serializer.Serialize(null);

            _output.WriteLine(document.ToString());

            var parsed = serializer.Deserialize(document);

            Assert.Null(parsed);
        }

        [Fact]
        public void DoesSerializeAndDeserializeInDataStructure()
        {
            Data value = new Data {Nested = new Data {String = "Not null"}};

            BaseSerializer serializer = new BaseSerializer();
            XDocument document = serializer.Serialize(value);

            _output.WriteLine(document.ToString());

            var parsed = serializer.Deserialize(document);

            Assert.Equal(value, parsed);
        }
    }
}
