using System.Collections.Generic;
using System.Xml.Linq;
using PatrickSachs.Serializer;
using Xunit;
using Xunit.Abstractions;

namespace Test.Serializers
{
    public class DictionarySerializer
    {
        private readonly ITestOutputHelper _output;

        public DictionarySerializer(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void DoesSerializeWithoutCrashing()
        {
            Dictionary<string, string> value = new Dictionary<string, string>
            {
                {"key-1", "value-1"},
                {"key-2", "value-2"}
            };

            BaseSerializer serializer = new BaseSerializer();
            XDocument document = serializer.Serialize(value);

            _output.WriteLine(document.ToString());
        }

        [Fact]
        public void DoesSerializeAndDeserialize()
        {
            Dictionary<string, string> value = new Dictionary<string, string>
            {
                {"key-1", "value-1"},
                {"key-2", "value-2"}
            };

            BaseSerializer serializer = new BaseSerializer();
            XDocument document = serializer.Serialize(value);

            _output.WriteLine(document.ToString());

            var parsed = serializer.Deserialize(document);

            Assert.Equal(value, parsed);
        }
    }
}
