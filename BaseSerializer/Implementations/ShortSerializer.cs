using System.Globalization;

namespace PatrickSachs.Serializer.Implementations
{
    public class ShortSerializer : IBaseSerializer
    {
        public static readonly ShortSerializer Instance = new ShortSerializer();
        
        public int Order => int.MaxValue - 1;

        public bool IsHandled(SerializationContext.Ref reference)
        {
            return reference.Type == typeof(short);
        }

        public void Serialize(SerializationContext.Ref target, SerializationContext context)
        {
            short integer = (short) target.Object;
            target.Element.Value = integer.ToString(CultureInfo.InvariantCulture);
        }

        public object CreateInstance(SerializationContext.Ref source)
        {
            short value = short.Parse(source.Element.Value, CultureInfo.InvariantCulture);
            return value;
        }

        public void Deserialize(SerializationContext.Ref source, SerializationContext context)
        {
        }
    }
}
