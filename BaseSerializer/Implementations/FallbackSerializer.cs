using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace BaseSerializer.Implementations
{
    /// <summary>
    ///     Fallback serializes. Uses reflection to get all desired fields and then serializes them using other serializers.
    /// </summary>
    public class FallbackSerializer : IGameSerializer
    {
        /// <summary>
        /// What should be serialized? Flagged enum.
        /// </summary>
        public SerializeType Type { get; }

        /// <summary>
        /// If not null this predicate must pass for the given member to be serialized.
        /// </summary>
        public Predicate<MemberInfo> Checker { get; }

        /// <summary>
        /// What should be serialized? Flagged enum.
        /// </summary>
        [Flags]
        public enum SerializeType
        {
            /// <summary>
            /// Fields should be serialized.
            /// </summary>
            Fields,

            /// <summary>
            /// Properties should be serialized.
            /// </summary>
            Properties
        }

        /// <summary>
        /// The default serializer. Serializes only fields. No attribute required.
        /// </summary>
        public static readonly FallbackSerializer Instance = new FallbackSerializer(SerializeType.Fields, null);

        /// <summary>
        /// Creates a new fallback serializer.
        /// </summary>
        /// <param name="type">What should be serialized? Flagged enum.</param>
        /// <param name="checker">If not null this predicate must pass for the given member to be serialized.</param>
        public FallbackSerializer(SerializeType type, Predicate<MemberInfo> checker)
        {
            Type = type;
            Checker = checker;
        }

        /// <inheritdoc />
        /// <summary>
        /// The fallback serializer should be the last serializer to be considered. +1 was added in case a user created
        /// one needs to be even later for some reason.
        /// </summary>
        public int Order => int.MinValue + 1;

        /// <inheritdoc />
        public bool IsHandled(SerializationContext.Ref reference)
        {
            return true;
        }

        /// <inheritdoc />
        public void Serialize(SerializationContext.Ref target, SerializationContext context)
        {
            foreach (FieldInfo field in GetFields(target.Type))
            {
                object value = field.GetValue(target.Object);
                SerializationContext.Ref theRef = context.Serializer.SerializeReference(value, context);
                // todo: Conflicts if a class has two fields with the same name!
                XElement element = new XElement(field.Name);
                element.Value = theRef.Id;
                target.Element.Add(element);
            }

            foreach (PropertyInfo property in GetProperties(target.Type))
            {
                object value = property.GetValue(target.Object);
                SerializationContext.Ref theRef = context.Serializer.SerializeReference(value, context);
                // todo: Conflicts if a class has two fields with the same name!
                XElement element = new XElement(property.Name);
                element.Value = theRef.Id;
                target.Element.Add(element);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// The fallback serializer creates classes WITHOUT calling their constructor.
        /// </summary>
        public object CreateInstance(SerializationContext.Ref source)
        {
            object obj = FormatterServices.GetUninitializedObject(source.Type);
            return obj;
        }

        /// <inheritdoc />
        public void Deserialize(SerializationContext.Ref source, SerializationContext context)
        {
            foreach (FieldInfo field in GetFields(source.Type))
            {
                // todo: Conflicts if a class has two fields with the same name!
                string name = field.Name;
                XElement element = source.Element.Element(name);
                if (element != null)
                {
                    string id = element.Value;
                    var value = context.Serializer.DeserializeReference(id, context);
                    field.SetValue(source.Object, value.Object);
                }
            }

            foreach (PropertyInfo property in GetProperties(source.Type))
            {
                // todo: Conflicts if a class has two fields with the same name!
                string name = property.Name;
                XElement element = source.Element.Element(name);
                if (element != null)
                {
                    string id = element.Value;
                    var value = context.Serializer.DeserializeReference(id, context);
                    property.SetValue(source.Object, value.Object);
                }
            }
        }

        private IEnumerable<FieldInfo> GetFields(Type type)
        {
            if ((Type & SerializeType.Fields) != 0)
            {
                FieldInfo[] fields = type
                    .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                foreach (FieldInfo field in fields)
                {
                    if (Checker == null || Checker(field))
                    {
                        yield return field;
                    }
                }
            }
        }

        private IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            if ((Type & SerializeType.Properties) != 0)
            {
                PropertyInfo[] properties = type
                    .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                foreach (PropertyInfo property in properties)
                {
                    if (Checker == null || Checker(property))
                    {
                        yield return property;
                    }
                }
            }
        }
    }
}
