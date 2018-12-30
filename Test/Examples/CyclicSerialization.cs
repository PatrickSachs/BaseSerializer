using System.Collections.Generic;
using System.Xml.Linq;
using PatrickSachs.Serializer;
using Xunit;
using Xunit.Abstractions;

namespace Test.Examples
{
    public class CyclicSerialization
    {
        private readonly ITestOutputHelper _output;

        public CyclicSerialization(ITestOutputHelper output)
        {
            _output = output;
        }
        
        public class UserAccount
        {
            public string Name;
            public string Gender;
            public int Age;
            public IList<UserAccount> Friends = new List<UserAccount>();

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
            // User accounts reference each in a cyclic manner
            UserAccount account1 = new UserAccount("Mike Johnson", "Male", 36);
            UserAccount account2 = new UserAccount("John Harper", "Male", 57);
            account1.Friends.Add(account2);
            account2.Friends.Add(account1);

            BaseSerializer serializer = new BaseSerializer();
            XDocument document = serializer.Serialize(account1);
            // You should only see one instance of "Male" in the XML document.
            _output.WriteLine(document.ToString());

            return document;
        }
    }
}
