using System.Xml.Linq;
using PatrickSachs.Serializer;
using Xunit;
using Xunit.Abstractions;

namespace Test.Serializers
{
    public class FloatSerializer
    {
        private readonly ITestOutputHelper _output;

        public FloatSerializer(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void DoesSerializeWithoutCrashing()
        {
            float value = 42.3f;
            
            BaseSerializer serializer = new BaseSerializer();
            XDocument document = serializer.Serialize(value);
            
            _output.WriteLine(document.ToString());
        }

        [Fact]
        public void DoesSerializeAndDeserializeLargeFloatingNumbers()
        {
            float value = 42.34533f;
            
            BaseSerializer serializer = new BaseSerializer();
            XDocument document = serializer.Serialize(value);
            
            _output.WriteLine(document.ToString());

            float parsed = (float) serializer.Deserialize(document);
            
            _output.WriteLine("Got value: " + parsed + " / Value is: " + value);
            
            Assert.Equal(value, parsed);
            
        }

        [Fact]
        public void DoesSerializeAndDeserializeLargeNumbers()
        {
            float value = 64646242f;
            
            BaseSerializer serializer = new BaseSerializer();
            XDocument document = serializer.Serialize(value);
            
            _output.WriteLine(document.ToString());

            float parsed = (float) serializer.Deserialize(document);
            
            _output.WriteLine("Got value: " + parsed + " / Value is: " + value);
            
            Assert.Equal(value, parsed);
            
        }
    }
}
