using System.Globalization;

namespace PatrickSachs.Serializer.Implementations
{
    public class IntegerSerializer : IBaseSerializer
    {
        public static readonly IntegerSerializer Instance = new IntegerSerializer();
        
        public int Order => int.MaxValue - 1;

        public bool IsHandled(SerializationContext.Ref reference)
        {
            return reference.Type == typeof(int);
        }

        public void Serialize(SerializationContext.Ref target, SerializationContext context)
        {
            int integer = (int) target.Object;
            target.Element.Value = integer.ToString(CultureInfo.InvariantCulture);
        }

        public object CreateInstance(SerializationContext.Ref source)
        {
            int value = int.Parse(source.Element.Value, CultureInfo.InvariantCulture);
            return value;
        }

        public void Deserialize(SerializationContext.Ref source, SerializationContext context)
        {
        }
    }
}
