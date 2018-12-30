using System.Globalization;

namespace PatrickSachs.Serializer.Implementations
{
    public class ByteSerializer : IBaseSerializer
    {
        public static readonly ByteSerializer Instance = new ByteSerializer();
        
        public int Order => int.MaxValue - 1;

        public bool IsHandled(SerializationContext.Ref reference)
        {
            return reference.Type == typeof(sbyte);
        }

        public void Serialize(SerializationContext.Ref target, SerializationContext context)
        {
            sbyte integer = (sbyte) target.Object;
            target.Element.Value = integer.ToString(CultureInfo.InvariantCulture);
        }

        public object CreateInstance(SerializationContext.Ref source)
        {
            sbyte value = sbyte.Parse(source.Element.Value, CultureInfo.InvariantCulture);
            return value;
        }

        public void Deserialize(SerializationContext.Ref source, SerializationContext context)
        {
        }
    }
}
