using System;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace PatrickSachs.Serializer.Implementations
{
    /// <summary>
    /// This serializer handles the serialization of IList and List`1
    /// </summary>
    public class ListSerializer : IBaseSerializer
    {
        public static readonly ListSerializer Instance = new ListSerializer();

        public int Order => int.MinValue + 100;

        public bool IsHandled(SerializationContext.Ref reference)
        {
            return typeof(IList).IsAssignableFrom(reference.Type);
        }

        public void Serialize(SerializationContext.Ref target, SerializationContext context)
        {
            IList list = (IList) target.Object;
            XElement element = target.Element;
            foreach (object value in list)
            {
                SerializationContext.Ref valueRef = context.Serializer.SerializeReference(value, context);
                element.Add(new XElement("Entry", valueRef.Id));
            }
        }

        public object CreateInstance(SerializationContext.Ref source)
        {
            // Lists generally need their constructor to be properly initialized.
            var ctor = source.Type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                null, CallingConventions.HasThis, Array.Empty<Type>(), null);
            if (ctor != null)
            {
                return ctor.Invoke(Array.Empty<object>());
            }

            // Dangerous for lists!
            return FormatterServices.GetUninitializedObject(source.Type);
        }

        public void Deserialize(SerializationContext.Ref source, SerializationContext context)
        {
            IList list = (IList) source.Object;
            foreach (XElement element in source.Element.Elements())
            {
                string id = element.Value;
                object value = context.Serializer.DeserializeReference(id, context)?.Object;
                list.Add(value);
            }
        }
    }
}
