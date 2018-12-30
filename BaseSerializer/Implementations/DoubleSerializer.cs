using System.Globalization;

namespace PatrickSachs.Serializer.Implementations
{
    public class DoubleSerializer : IBaseSerializer
    {
        public static readonly DoubleSerializer Instance = new DoubleSerializer();
        
        public int Order => int.MaxValue - 1;

        public bool IsHandled(SerializationContext.Ref reference)
        {
            return reference.Type == typeof(double);
        }

        public void Serialize(SerializationContext.Ref target, SerializationContext context)
        {
            double number = (double) target.Object;
            target.Element.Value = number.ToString(CultureInfo.InvariantCulture);
        }

        public object CreateInstance(SerializationContext.Ref source)
        {
            double value = double.Parse(source.Element.Value);
            return value;
        }

        public void Deserialize(SerializationContext.Ref source, SerializationContext context)
        {
        }
    }
}
