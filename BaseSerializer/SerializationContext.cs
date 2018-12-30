using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;

namespace PatrickSachs.Serializer
{
    public class SerializationContext
    {
        private readonly BaseSerializer _serializer;

        private readonly Dictionary<string, Ref> _idToRefs = new Dictionary<string, Ref>();
        private readonly Dictionary<object, Ref> _objToRefs = new Dictionary<object, Ref>();
        private readonly XElement _refs;
        private ulong _lastId;

        /// <summary>
        /// Creates a new serialization context during serialization.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        internal SerializationContext(BaseSerializer serializer)
        {
            _serializer = serializer;
            _refs = new XElement("Data");
            Document = new XDocument(_refs);
        }

        /// <summary>
        /// Creates a new serialization context during deserialization.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="document">The XML document being deserialized.</param>
        internal SerializationContext(BaseSerializer serializer, XDocument document)
        {
            if (document.Root == null)
            {
                throw new ArgumentException("Cannot deserialize an uninitialized document!");
            }

            _serializer = serializer;
            Document = document;
            _refs = document.Root;

            foreach (var element in _refs.Elements())
            {
                Ref aRef = new Ref(this, element);
                _idToRefs.Add(aRef.Id, aRef);
            }
        }

        /// <summary>
        /// Gets the serializer this context is associated with.
        /// </summary>
        public BaseSerializer Serializer => _serializer;

        /// <summary>
        /// The root reference of this context.
        /// </summary>
        internal string RootReference
        {
            get => _refs.Attribute("root")?.Value ?? "null";
            set => _refs.SetAttributeValue("root", value);
        }

        /// <summary>
        /// The XML document used by this context.
        /// </summary>
        internal XDocument Document { get; }

        /// <summary>
        /// Finds a reference with the given ID.
        /// </summary>
        /// <param name="id">The ID to look for.</param>
        /// <returns>The Ref, or null if no ref for the ID could be found.</returns>
        internal Ref FindReference(string id)
        {
            if (id == "null")
            {
                return Ref.Null;
            }

            if (!_idToRefs.TryGetValue(id, out Ref theRef))
            {
                return null;
            }

            return theRef;
        }

        /// <summary>
        /// Finds a reference for the given object.
        /// </summary>
        /// <param name="obj">The object to look for.</param>
        /// <returns>The Ref, or null if no ref for the object could be found.</returns>
        internal Ref GetReference(object obj)
        {
            if (obj == null)
            {
                return Ref.Null;
            }

            if (!_objToRefs.TryGetValue(obj, out Ref theRef))
            {
                return null;
            }

            return theRef;
        }

        /// <summary>
        /// Saves the reference to the internal lookup, allowing us to retrieve it again using its ID and object reference.
        /// </summary>
        /// <param name="theRef">The reference to store.</param>
        internal void StoreReference(Ref theRef)
        {
            if (theRef.Object != null)
            {
                _objToRefs[theRef.Object] = theRef;
                _idToRefs[theRef.Id] = theRef;
            }
        }

        /// <summary>
        /// Creates a reference for the given object. Does NOT check for existence prior to creation..
        /// </summary>
        /// <param name="obj">The object to create the reference for.</param>
        /// <returns>The newly created reference.</returns>
        internal Ref CreateReference(object obj)
        {
            Ref theRef = new Ref(this, obj);
            _objToRefs.Add(obj, theRef);
            _idToRefs.Add(theRef.Id, theRef);

            _refs.Add(theRef.Element);

            return theRef;
        }

        private static string SerializerTypeName(BaseSerializer serializer, Type of)
        {
            if (of == null)
            {
                return "null/null";
            }

            if (serializer._assemblyAliases.TryGetValue(of.Assembly, out string prefix))
            {
                return prefix + "/" + of.FullName;
            }

            return "/" + of.AssemblyQualifiedName;
        }

        private static Type SerializerType(BaseSerializer serializer, string name)
        {
            if (name == "null/null")
            {
                return null;
            }

            string[] split = name.Split('/');
            if (split.Length != 2)
            {
                throw new ArgumentException("Invalid type prefix: '" + name +
                                            "', expected to be delimited by a single slash.");
            }

            if (serializer._assemblyAliases.TryGetValue(split[1], out Assembly assembly))
            {
                return assembly.GetType(split[1]);
            }

            return Type.GetType(split[1]);
        }

        private string CreateId()
        {
            _lastId++;
            return "ref-" + _lastId;
        }

        /// <summary>
        /// A reference during serialization.
        /// </summary>
        public class Ref
        {
            /// <summary>
            /// A null reference singleton.
            /// </summary>
            public static readonly Ref Null = new Ref(null, "null", null);

            private Ref(SerializationContext context, string id, object obj)
            {
                Context = context;
                Id = id;
                Object = obj;
                Element = new XElement(Id);
                Type = obj?.GetType();
                Element.SetAttributeValue("type", SerializerTypeName(context.Serializer, Type));
            }

            /// <summary>
            /// Creates a new reference during serialization.
            /// </summary>
            /// <param name="context">The serializer context.</param>
            /// <param name="obj">The object being serialized.</param>
            internal Ref(SerializationContext context, object obj) : this(context, context.CreateId(), obj)
            {
            }

            /// <summary>
            /// Creates a new reference during deserialization.
            /// </summary>
            /// <param name="context">The serializer context.</param>
            /// <param name="el">The XML element being deserialized.</param>
            internal Ref(SerializationContext context, XElement el)
            {
                Context = context;
                Element = el;
                Id = el.Name.LocalName;
                var typeAttribute = el.Attribute("type");
                Type = typeAttribute != null ? SerializerType(context.Serializer, typeAttribute.Value) : null;
                Object = null;
            }

            /// <summary>
            /// The type this reference refers to. (Can be null!)
            /// </summary>
            public Type Type { get; }

            /// <summary>
            /// The serializer context of this reference. (Can be null!)
            /// </summary>
            public SerializationContext Context { get; }

            /// <summary>
            /// The ID of this reference.
            /// </summary>
            public string Id { get; }

            /// <summary>
            /// The object this reference is pointing to. This value should not be set manually.
            /// </summary>
            public object Object { get; internal set; }

            /// <summary>
            /// The XML Element of the data.
            /// </summary>
            public XElement Element { get; }
        }
    }
}
