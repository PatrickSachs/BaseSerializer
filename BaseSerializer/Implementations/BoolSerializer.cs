namespace PatrickSachs.Serializer.Implementations
{
    public class BoolSerializer : IBaseSerializer
    {
        private const string True = "true";
        private const string False = "false";

        public static readonly BoolSerializer Instance = new BoolSerializer();

        public int Order => int.MaxValue - 1;

        public bool IsHandled(SerializationContext.Ref reference)
        {
            return reference.Type == typeof(bool);
        }

        public void Serialize(SerializationContext.Ref target, SerializationContext context)
        {
            bool boolean = (bool) target.Object;
            target.Element.Value = boolean ? True : False;
        }

        public object CreateInstance(SerializationContext.Ref source)
        {
            return source.Element.Value.Equals(True);
        }

        public void Deserialize(SerializationContext.Ref source, SerializationContext context)
        {
        }
    }
}
