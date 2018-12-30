using System.Globalization;

namespace PatrickSachs.Serializer.Implementations
{
    public class LongSerializer : IBaseSerializer
    {
        public static readonly LongSerializer Instance = new LongSerializer();
        
        public int Order => int.MaxValue - 1;

        public bool IsHandled(SerializationContext.Ref reference)
        {
            return reference.Type == typeof(long);
        }

        public void Serialize(SerializationContext.Ref target, SerializationContext context)
        {
            long integer = (long) target.Object;
            target.Element.Value = integer.ToString(CultureInfo.InvariantCulture);
        }

        public object CreateInstance(SerializationContext.Ref source)
        {
            long value = long.Parse(source.Element.Value);
            return value;
        }

        public void Deserialize(SerializationContext.Ref source, SerializationContext context)
        {
        }
    }
}
