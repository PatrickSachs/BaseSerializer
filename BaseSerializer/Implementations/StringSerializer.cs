namespace BaseSerializer.Implementations
{
    public class StringSerializer : IGameSerializer
    {
        public static readonly StringSerializer Instance = new StringSerializer();
        
        public int Order => int.MaxValue;

        public bool IsHandled(SerializationContext.Ref reference)
        {
            return reference.Type == typeof(string);
        }

        public void Serialize(SerializationContext.Ref target, SerializationContext context)
        {
            string text = (string) target.Object;
            target.Element.Value = text;
        }

        public object CreateInstance(SerializationContext.Ref source)
        {
            return source.Element.Value;
        }

        public void Deserialize(SerializationContext.Ref source, SerializationContext context)
        {
        }
    }
}
