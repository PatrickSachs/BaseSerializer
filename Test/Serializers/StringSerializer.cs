using System.Xml.Linq;
using PatrickSachs.Serializer;
using Xunit;
using Xunit.Abstractions;

namespace Test.Serializers
{
    public class StringSerializer
    {
        private readonly ITestOutputHelper _output;

        public StringSerializer(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void DoesSerializeWithoutCrashing()
        {
            string value = "Hello World!";

            BaseSerializer serializer = new BaseSerializer();
            XDocument document = serializer.Serialize(value);

            _output.WriteLine(document.ToString());
        }

        [Fact]
        public void DoesSerializeAndDeserialize()
        {
            string value = "Hello World!";

            BaseSerializer serializer = new BaseSerializer();
            XDocument document = serializer.Serialize(value);

            _output.WriteLine(document.ToString());

            string parsed = (string) serializer.Deserialize(document);

            _output.WriteLine("Got value: " + parsed + " / Value is: " + value);

            Assert.Equal(value, parsed);
        }

        [Fact]
        public void DoesSerializeAndDeserializeDangerousCharacters()
        {
            string value = "</ref-1>'\\ \"<><ref-2>&\r\n\t</ref-2>";

            BaseSerializer serializer = new BaseSerializer();
            XDocument document = serializer.Serialize(value);

            _output.WriteLine(document.ToString());

            string parsed = (string) serializer.Deserialize(document);

            _output.WriteLine("Got value: " + parsed + " / Value is: " + value);

            Assert.Equal(value, parsed);
        }

        [Fact]
        public void DoesSerializeAndDeserializeLongText()
        {
            string value = Lorem + "\n" + Lorem;

            BaseSerializer serializer = new BaseSerializer();
            XDocument document = serializer.Serialize(value);

            _output.WriteLine(document.ToString());

            string parsed = (string) serializer.Deserialize(document);

            _output.WriteLine("Got value: " + parsed + " / Value is: " + value);

            Assert.Equal(value, parsed);
        }


        private const string Lorem =
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin vel libero in velit tempor iaculis. Vestibulum at ipsum non mi porta varius. Donec tempus arcu ut pellentesque facilisis. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Curabitur eget mauris et mauris porta ultricies at a leo. Nam sit amet nibh et diam aliquet ultricies eget eu nibh. Donec elementum, lacus et tristique posuere, urna eros ullamcorper risus, quis gravida ante lorem eget tortor. Aliquam erat volutpat. Ut lacinia sagittis leo, sed scelerisque mi aliquam id. Vestibulum a nisi enim. Mauris a ultrices lorem. Curabitur interdum leo at odio eleifend imperdiet. Fusce est nunc, aliquam sit amet volutpat in, luctus ut sapien. Donec quis finibus elit. Aenean dignissim eleifend augue, in rhoncus turpis gravida rhoncus. Maecenas leo sapien, egestas ac elit vel, aliquet sodales odio.\n" +
            "Ut aliquam, orci a fringilla placerat, nisi magna tempor enim, ac placerat augue mi ac enim. Aliquam consequat vulputate arcu id elementum. Mauris tellus quam, rutrum ac semper ac, fringilla vel nulla. Cras lacinia, nisi non pharetra eleifend, leo justo tempus augue, ut iaculis enim nisi non ante. Donec id condimentum quam. Morbi at egestas est, non porttitor lectus. Mauris maximus nibh eget lacus bibendum vehicula. Quisque fermentum aliquet porta. Suspendisse pretium orci eu risus fringilla, sed molestie est facilisis.\n" +
            "Quisque in porttitor mauris. Donec sollicitudin enim viverra, tristique mi vel, tristique urna. Nullam volutpat, justo ut feugiat interdum, purus sapien euismod metus, a efficitur arcu erat a dui. Pellentesque maximus nulla sit amet molestie semper. Vivamus mattis, tellus ac viverra auctor, dolor urna faucibus orci, vitae blandit velit lorem sit amet tellus. Nulla fermentum dolor lorem, ac fermentum ipsum facilisis sit amet. Nam lacus leo, ullamcorper sed scelerisque et, tempus vel sem. Vivamus vel velit facilisis, varius lorem ac, lobortis dui.\n" +
            "Sed auctor erat at eros mattis commodo. Duis nec porttitor justo. Mauris eu massa nulla. Quisque fermentum blandit sem ac finibus. Ut finibus felis ac augue feugiat, eu dignissim ipsum lobortis. Pellentesque tempus lobortis eros, at porttitor magna dictum eu. Pellentesque elit libero, luctus id efficitur sit amet, cursus sed orci.\n" +
            "Vivamus gravida lacus ac iaculis facilisis. Sed sit amet turpis tincidunt, fermentum tellus et, ultrices magna. Sed maximus eros id dolor cursus posuere. In posuere dictum turpis vel tempor. Praesent ante velit, lobortis convallis arcu non, imperdiet pulvinar velit. Donec efficitur nisl sem, vitae suscipit sem cursus varius. Aliquam convallis pellentesque consectetur. Sed tortor justo, elementum ut magna et, vestibulum malesuada mi. Sed nec sagittis quam, non ornare est. Pellentesque malesuada nunc ullamcorper risus ultricies, a gravida erat ullamcorper. Vivamus et dapibus massa. Sed lacinia congue mattis. Sed porta accumsan arcu, iaculis ultrices eros vestibulum sed.\n" +
            "Vivamus et pulvinar massa, in ultricies urna. Fusce metus ligula, congue id nisi non, pulvinar vestibulum ipsum. Donec ultrices nec sapien vel sodales. Proin facilisis quis augue vel iaculis. Nam dignissim arcu nec ultrices venenatis. Nullam venenatis lacinia ornare. Proin et sodales diam. Integer congue dignissim ex, a viverra ligula semper vitae. Integer orci quam, rhoncus placerat nulla eget, vehicula ultrices turpis. Nam commodo in dolor ut sodales.\n" +
            "Maecenas eget sapien libero. Ut ut lorem ut nibh viverra faucibus a ac dui. Sed non nisi placerat, laoreet magna quis, viverra massa. Praesent facilisis massa vitae tellus molestie luctus. Donec pretium vulputate diam nec placerat. Sed malesuada vestibulum justo vel dictum. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Proin commodo pulvinar magna, eget luctus dui. Nulla facilisi. Donec dictum orci nec dictum gravida.\n" +
            "Vestibulum sit amet hendrerit sem, sit amet ornare enim. Aenean vehicula, ligula sit amet posuere interdum, orci ipsum sagittis lacus, mattis dapibus est nunc eu erat. Ut mollis felis nunc, a aliquet tellus rutrum vel. Nunc ullamcorper eleifend tempor. Sed pharetra consectetur faucibus. Suspendisse potenti. Proin tellus mi, ultricies eu consectetur sed, fermentum quis diam. Nunc non sapien pulvinar, scelerisque sem quis, convallis libero.\n" +
            "Ut vel diam leo. Sed lectus massa, volutpat et odio nec, commodo placerat tellus. Mauris condimentum convallis imperdiet. Integer sodales ullamcorper elit. Nullam laoreet dictum metus ut convallis. Suspendisse purus tortor, tincidunt ac enim vitae, gravida facilisis massa. Praesent eget tortor id nisl pulvinar rutrum.\n" +
            "Sed euismod laoreet mi, quis auctor urna faucibus sit amet. Nulla facilisi. Aenean pharetra enim a purus tristique, vitae porta est commodo. In sed ullamcorper elit, sed cursus lacus. Sed pretium, eros ut vehicula pharetra, ante mi consectetur neque, nec varius enim erat eget orci. Donec eget diam orci. Proin molestie magna sit amet malesuada eleifend. Curabitur eleifend suscipit lobortis. Nulla aliquam efficitur lectus, id sollicitudin sapien vestibulum vitae.";
    }
}
