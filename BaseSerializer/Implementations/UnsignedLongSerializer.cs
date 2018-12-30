using System.Globalization;

namespace PatrickSachs.Serializer.Implementations
{
    public class UnsignedLongSerializer : IBaseSerializer
    {
        public static readonly UnsignedLongSerializer Instance = new UnsignedLongSerializer();
        
        public int Order => int.MaxValue - 1;

        public bool IsHandled(SerializationContext.Ref reference)
        {
            return reference.Type == typeof(ulong);
        }

        public void Serialize(SerializationContext.Ref target, SerializationContext context)
        {
            ulong integer = (ulong) target.Object;
            target.Element.Value = integer.ToString(CultureInfo.InvariantCulture);
        }

        public object CreateInstance(SerializationContext.Ref source)
        {
            ulong value = ulong.Parse(source.Element.Value);
            return value;
        }

        public void Deserialize(SerializationContext.Ref source, SerializationContext context)
        {
        }
    }
}
