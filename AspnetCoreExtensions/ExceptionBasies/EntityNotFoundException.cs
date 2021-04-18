using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace AspnetCoreExtensions.ExceptionBasies
{
    /// <summary>
    /// Represents errors that occurs during retrieving single business entity from Repository.
    /// </summary>
    [Serializable]
    public class EntityNotFoundException : Exception
    {
        /// <summary>
        /// Gets Business Entity name that doesn't found.
        /// </summary>
        public string EntityName { get; private set; }

        /// <summary>
        /// Gets the Business Entity Id that is requested from repository and doesn't found.
        /// </summary>
        public object EntityId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
        /// </summary>
        public EntityNotFoundException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class with a specified business entity name, and its identity number.
        /// </summary>
        /// <param name="entityName">business entity name that doesn't found.</param>
        /// <param name="entityId">business entity identity number that used to query from repository.</param>
        public EntityNotFoundException(string entityName, object entityId)
            : base($"{entityName} object with {entityId} not exists in repository.")
        {
            this.EntityName = entityName;
            this.EntityId = entityId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class with specified business entity name, identity number and error message.
        /// </summary>
        /// <param name="entityName">business entity name that doesn't found.</param>
        /// <param name="entityId">business entity identity number that used to query from repository.</param>
        /// <param name="message">custom message that describes the error.</param>
        public EntityNotFoundException(string entityName, object entityId, string message)
            : base(message)
        {
            this.EntityId = entityId;
            this.EntityName = entityName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class with specified business entity name, identity number, error message, and a reference to the inner exception that is the cuase of this exception.
        /// </summary>
        /// <param name="entityName">business entity name that doesn't found.</param>
        /// <param name="entityId">business entity identity number that used to query from repository.</param>
        /// <param name="message">custom message that describes the error.</param>
        /// <param name="inner">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public EntityNotFoundException(string entityName, object entityId, string message, Exception inner)
            : base(message, inner)
        {
            this.EntityName = entityName;
            this.EntityId = entityId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected EntityNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            EntityName = info.GetString(nameof(EntityName));
            EntityId = info.GetString(nameof(EntityId));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(EntityName), EntityName);
            info.AddValue(nameof(EntityId), EntityId);
        }
    }
}
