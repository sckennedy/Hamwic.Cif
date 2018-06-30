using System;

namespace Hamwic.Cif.Core.CoreEntities
{
    public class ValidationError
    {
        public ValidationError(Type entityType, string propertyName, string message)
        {
            EntityType = entityType;
            PropertyName = propertyName;
            Message = message;
        }

        public Type EntityType { get; }
        public string PropertyName { get; }
        public string Message { get; }
    }
}