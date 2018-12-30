namespace BaseSerializer.Implementations
{
    /// <summary>
    ///     Implement this interface when creating custom serializers for the base serializer.<br />All non-abstract classes
    ///     implementing this class will automatically be registered. These classes must provide a default public constructor.
    /// </summary>
    public interface IGameSerializer
    {
        /// <summary>
        ///     The order of this serializer. Serializers with a higher order will be considered first.
        /// </summary>
        int Order { get; }

        /// <summary>
        ///     Checks if the given type can be handled by this serializer.
        /// </summary>
        /// <param name="reference">The reference to check.</param>
        /// <returns>true if the serializer can deserialize and serializer the given type.</returns>
        bool IsHandled(SerializationContext.Ref reference);

        /// <summary>
        ///     Serializes the given value.
        /// </summary>
        /// <param name="target">The serializer block to serialize to.</param>
        /// <param name="context">The context for additional data and references.</param>
        void Serialize(SerializationContext.Ref target, SerializationContext context);

        /// <summary>
        ///     Creates an instance from deserialized data. The instance must not be populated right away, but an instance must be
        ///     created and used later on.
        /// </summary>
        /// <param name="source">The serializer block.</param>
        /// <returns>The instance.</returns>
        object CreateInstance(SerializationContext.Ref source);

        /// <summary>
        ///     Deserializes the entire data.
        /// </summary>
        /// <param name="source">The block containing the data.</param>
        /// <param name="context">The context to used for additional data and references.</param>
        void Deserialize(SerializationContext.Ref source, SerializationContext context);
    }
}
