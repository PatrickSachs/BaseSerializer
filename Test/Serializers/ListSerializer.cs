using System.Collections.Generic;
using System.Xml.Linq;
using PatrickSachs.Serializer;
using Xunit;
using Xunit.Abstractions;

namespace Test.Serializers
{
    public class ListSerializer
    {
        private readonly ITestOutputHelper _output;

        public ListSerializer(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void DoesSerializeWithoutCrashing()
        {
            List<string> value = new List<string>
            {
                "value-1", 
                "value-2"
            };

            BaseSerializer serializer = new BaseSerializer();
            XDocument document = serializer.Serialize(value);

            _output.WriteLine(document.ToString());
        }

        [Fact]
        public void DoesSerializeAndDeserialize()
        {
            List<string> value = new List<string>
            {
                "value-1", 
                "value-2"
            };

            BaseSerializer serializer = new BaseSerializer();
            XDocument document = serializer.Serialize(value);

            _output.WriteLine(document.ToString());

            var parsed = serializer.Deserialize(document);

            Assert.Equal(value, parsed);
        }
    }
}
