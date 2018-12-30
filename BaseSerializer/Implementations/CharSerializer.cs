using System.Globalization;

namespace PatrickSachs.Serializer.Implementations
{
    public class CharSerializer : IBaseSerializer
    {
        public static readonly CharSerializer Instance = new CharSerializer();
        
        public int Order => int.MaxValue - 1;

        public bool IsHandled(SerializationContext.Ref reference)
        {
            return reference.Type == typeof(char);
        }

        public void Serialize(SerializationContext.Ref target, SerializationContext context)
        {
            char character = (char) target.Object;
            target.Element.Value = character.ToString(CultureInfo.InvariantCulture);
        }

        public object CreateInstance(SerializationContext.Ref source)
        {
            char value = char.Parse(source.Element.Value);
            return value;
        }

        public void Deserialize(SerializationContext.Ref source, SerializationContext context)
        {
        }
    }
}
