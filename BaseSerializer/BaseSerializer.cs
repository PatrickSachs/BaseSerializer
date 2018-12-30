using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using BaseSerializer.Implementations;

namespace BaseSerializer
{
    /// <summary>
    /// The base serializer is used to serialize objects to XML and back.
    /// </summary>
    public class BaseSerializer
    {
        private readonly IList<IBaseSerializer> _serializers;
        internal readonly BiDictionary<string, Assembly> _assemblyAliases;

        public BaseSerializer()
        {
            _assemblyAliases = new BiDictionary<string, Assembly>
            {
                {"mscorlib", typeof(Type).Assembly},
                {"serializer", typeof(BaseSerializer).Assembly}
            };
            List<IBaseSerializer> list = new List<IBaseSerializer>
            {
                FallbackSerializer.Instance,
                IntegerSerializer.Instance,
                StringSerializer.Instance,
                FloatSerializer.Instance
            };
            list.Sort(BaseSerializerComparer.Instance);
            _serializers = list;
        }

        /// <summary>
        /// All serializers used by this base serializer.
        /// </summary>
        public IList<IBaseSerializer> Serializers => _serializers;

        /// <summary>
        /// All possible aliases that can be used for assembly names instead of a fully qualified assembly name.
        /// </summary>
        public IDictionary<string, Assembly> AssemblyAliases => _assemblyAliases;
        
        /// <summary>
        /// Serializes the given object into a XML document.
        /// </summary>
        /// <param name="data">The object.</param>
        /// <returns>The XML document.</returns>
        public XDocument Serialize(object data)
        {
            SerializationContext ctx = new SerializationContext(this);
            SerializationContext.Ref root = SerializeReference(data, ctx);
            ctx.RootReference = root.Id;
            return ctx.Document;
        }

        /// <summary>
        /// Deserializes the given XML document to an object.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>The deserialized object.</returns>
        public object Deserialize(XDocument document)
        {
            SerializationContext ctx = new SerializationContext(this, document);
            var rootRef = DeserializeReference(ctx.RootReference, ctx);
            return rootRef.Object;
        }

        /// <summary>
        /// Deserializes the given XML document into an object.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="target">The already created object to deserialize into.</param>
        /// <exception cref="ArgumentNullException">The target is null.</exception>
        /// <exception cref="ArgumentException">Type mismatch - The target is of a different type than in XML.</exception>
        public void Deserialize(XDocument document, object target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            SerializationContext ctx = new SerializationContext(this, document);
            SerializationContext.Ref rootRef = ctx.FindReference(ctx.RootReference);
            if (rootRef.Type != target.GetType())
            {
                throw new ArgumentException("Invalid types - The object to deserialize into is of type " +
                                            target.GetType() + ", but the one in XML is " + rootRef.Type);
            }

            IBaseSerializer serializer = GetSerializer(rootRef);
            rootRef.Object = target;
            ctx.StoreReference(rootRef);
            serializer.Deserialize(rootRef, ctx);
        }

        /// <summary>
        /// Serializes the given object. 
        /// </summary>
        /// <param name="data">The object.</param>
        /// <param name="ctx">The context.</param>
        /// <returns>The reference.</returns>
        public SerializationContext.Ref SerializeReference(object data, SerializationContext ctx)
        {
            SerializationContext.Ref theRef = ctx.GetReference(data);
            if (theRef == null)
            {
                theRef = ctx.CreateReference(data);
                IBaseSerializer serializer = GetSerializer(theRef);
                serializer.Serialize(theRef, ctx);
            }

            return theRef;
        }

        /// <summary>
        /// Deserializes the given object. (If the object with this ID has already been deserialized it gets the
        /// deserialized instance)
        /// </summary>
        /// <param name="id">The ID of the reference.</param>
        /// <param name="ctx">The context.</param>
        /// <returns>The reference.</returns>
        public SerializationContext.Ref DeserializeReference(string id, SerializationContext ctx)
        {
            SerializationContext.Ref theRef = ctx.FindReference(id);
            if (theRef.Type != null && theRef.Object == null)
            {
                IBaseSerializer serializer = GetSerializer(theRef);
                theRef.Object = serializer.CreateInstance(theRef);
                ctx.StoreReference(theRef);
                serializer.Deserialize(theRef, ctx);
            }

            return theRef;
        }

        /// <summary>
        /// Gets the serializer that should be used for the given reference.
        /// </summary>
        /// <param name="theRef">The reference.</param>
        /// <returns>The serializer, or null.</returns>
        private IBaseSerializer GetSerializer(SerializationContext.Ref theRef)
        {
            foreach (var serializer in _serializers)
            {
                if (serializer.IsHandled(theRef))
                {
                    return serializer;
                }
            }

            return null;
        }
    }
}
