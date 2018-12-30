using System.Globalization;

namespace PatrickSachs.Serializer.Implementations
{
    public class DecimalSerializer : IBaseSerializer
    {
        public static readonly DecimalSerializer Instance = new DecimalSerializer();
        
        public int Order => int.MaxValue - 1;

        public bool IsHandled(SerializationContext.Ref reference)
        {
            return reference.Type == typeof(decimal);
        }

        public void Serialize(SerializationContext.Ref target, SerializationContext context)
        {
            decimal number = (decimal) target.Object;
            target.Element.Value = number.ToString(CultureInfo.InvariantCulture);
        }

        public object CreateInstance(SerializationContext.Ref source)
        {
            decimal value = decimal.Parse(source.Element.Value);
            return value;
        }

        public void Deserialize(SerializationContext.Ref source, SerializationContext context)
        {
        }
    }
}
