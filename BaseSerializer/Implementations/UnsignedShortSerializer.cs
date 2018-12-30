using System.Globalization;

namespace PatrickSachs.Serializer.Implementations
{
    public class UnsignedShortSerializer : IBaseSerializer
    {
        public static readonly UnsignedShortSerializer Instance = new UnsignedShortSerializer();
        
        public int Order => int.MaxValue - 1;

        public bool IsHandled(SerializationContext.Ref reference)
        {
            return reference.Type == typeof(ushort);
        }

        public void Serialize(SerializationContext.Ref target, SerializationContext context)
        {
            ushort integer = (ushort) target.Object;
            target.Element.Value = integer.ToString(CultureInfo.InvariantCulture);
        }

        public object CreateInstance(SerializationContext.Ref source)
        {
            ushort value = ushort.Parse(source.Element.Value);
            return value;
        }

        public void Deserialize(SerializationContext.Ref source, SerializationContext context)
        {
        }
    }
}
