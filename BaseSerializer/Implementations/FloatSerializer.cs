using System.Globalization;

namespace PatrickSachs.Serializer.Implementations
{
    public class FloatSerializer : IBaseSerializer
    {
        public static readonly FloatSerializer Instance = new FloatSerializer();
        
        public int Order => int.MaxValue - 1;

        public bool IsHandled(SerializationContext.Ref reference)
        {
            return reference.Type == typeof(float);
        }

        public void Serialize(SerializationContext.Ref target, SerializationContext context)
        {
            float number = (float) target.Object;
            target.Element.Value = number.ToString(CultureInfo.InvariantCulture);
        }

        public object CreateInstance(SerializationContext.Ref source)
        {
            float value = float.Parse(source.Element.Value);
            return value;
        }

        public void Deserialize(SerializationContext.Ref source, SerializationContext context)
        {
        }
    }
}
