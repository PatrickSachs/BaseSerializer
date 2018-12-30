using System.Xml.Linq;
using PatrickSachs.Serializer;
using Xunit;
using Xunit.Abstractions;

namespace Test.Examples
{
    /// <summary>
    /// This example demonstrates on how to quickly serialize data with the BaseSerializer.
    /// </summary>
    public class SimpleSerialization
    {
        private readonly ITestOutputHelper _output;

        public SimpleSerialization(ITestOutputHelper output)
        {
            _output = output;
        }

        /// <summary>
        /// A simple user account. By default the base serializer only serializes fields so we'll use fields.
        /// If you are looking for property serialization take a look at "PropertySerialization.cs".
        /// </summary>
        public class UserAccount
        {
            public string Name;
            public string Gender;
            public int Age;

            public UserAccount(string name, string gender, int age)
            {
                Name = name;
                Gender = gender;
                Age = age;
            }
        }

        [Fact]
        public XDocument SaveData()
        {
            // Create the user account & serializer
            UserAccount account = new UserAccount("Mike Johnson", "Male", 36);
            BaseSerializer serializer = new BaseSerializer();
            // Serialize it to an XML document
            XDocument document = serializer.Serialize(account);
            // We'll print the document to our unit test output, but you could also save it to a file. 
            _output.WriteLine(document.ToString());
            //document.Save("~/Documents/Data.xml"); <- Save it

            return document;
        }
    }
}
