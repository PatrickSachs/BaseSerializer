using System.Globalization;

namespace PatrickSachs.Serializer.Implementations
{
    public class UnsignedByteSerializer : IBaseSerializer
    {
        public static readonly UnsignedByteSerializer Instance = new UnsignedByteSerializer();
        
        public int Order => int.MaxValue - 1;

        public bool IsHandled(SerializationContext.Ref reference)
        {
            return reference.Type == typeof(byte);
        }

        public void Serialize(SerializationContext.Ref target, SerializationContext context)
        {
            byte integer = (byte) target.Object;
            target.Element.Value = integer.ToString(CultureInfo.InvariantCulture);
        }

        public object CreateInstance(SerializationContext.Ref source)
        {
            byte value = byte.Parse(source.Element.Value, CultureInfo.InvariantCulture);
            return value;
        }

        public void Deserialize(SerializationContext.Ref source, SerializationContext context)
        {
        }
    }
}
