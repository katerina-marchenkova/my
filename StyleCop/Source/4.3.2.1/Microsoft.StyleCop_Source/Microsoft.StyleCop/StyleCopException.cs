namespace Microsoft.StyleCop
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public sealed class StyleCopException : Exception
    {
        public StyleCopException() : base(Strings.InternalError)
        {
        }

        public StyleCopException(string message) : base(message)
        {
        }

        private StyleCopException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }

        public StyleCopException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

