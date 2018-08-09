using System;

namespace Dimensioner.Components
{
    public class ComponentReaderException : Exception
    {
        public Type ComponentType { get; }

        public ComponentReaderException(Type componentType, Exception innerException)
            : base(innerException.Message, innerException)
        {
            ComponentType = componentType;
        }
    }
}
