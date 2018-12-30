using System;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace PatrickSachs.Serializer.Implementations
{
    public class DictionarySerializer : IBaseSerializer
    {
        public static readonly DictionarySerializer Instance = new DictionarySerializer();

        public int Order => int.MaxValue - 1;

        public bool IsHandled(SerializationContext.Ref reference)
        {
            return typeof(IDictionary).IsAssignableFrom(reference.Type);
        }

        public void Serialize(SerializationContext.Ref target, SerializationContext context)
        {
            IDictionary dictionary = (IDictionary) target.Object;
            foreach (DictionaryEntry entry in dictionary)
            {
                XElement xmlEntry = new XElement("Entry");
                xmlEntry.SetAttributeValue("key", context.Serializer.SerializeReference(entry.Key, context).Id);
                xmlEntry.SetAttributeValue("value", context.Serializer.SerializeReference(entry.Value, context).Id);
                target.Element.Add(xmlEntry);
            }
        }

        public object CreateInstance(SerializationContext.Ref source)
        {
            // Dictionaries generally need their constructor to be properly initialized.
            var ctor = source.Type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                null, CallingConventions.HasThis, Array.Empty<Type>(), null);
            if (ctor != null)
            {
                return ctor.Invoke(Array.Empty<object>());
            }

            // Dangerous for dicts!
            return FormatterServices.GetUninitializedObject(source.Type);
        }

        public void Deserialize(SerializationContext.Ref source, SerializationContext context)
        {
            IDictionary dictionary = (IDictionary) source.Object;
            foreach (XElement element in source.Element.Elements())
            {
                string keyId = element.Attribute("key")?.Value ?? "null";
                string valueId = element.Attribute("value")?.Value ?? "null";
                object key = context.Serializer.DeserializeReference(keyId, context)?.Object;
                object value = context.Serializer.DeserializeReference(valueId, context)?.Object;
                dictionary.Add(key, value);
            }
        }
    }
}
