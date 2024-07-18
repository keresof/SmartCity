// Shared.Common.Abstract.AuditableEntity.cs

using System;

namespace Shared.Common.Abstract
{
    public abstract class AuditableEntity : BaseEntity
    {
        public DateTime Created { get; protected set; }
        public string? CreatedBy { get; set;}
        public DateTime LastModified { get; protected set; }
        public string? LastModifiedBy { get; protected set; }

        public void Updated(string updatedBy, DateTime updatedOn = default)
        {
            if (updatedOn == default)
            {
                updatedOn = DateTime.UtcNow;
            }
            LastModified = updatedOn;
            LastModifiedBy = updatedBy;
        }

    }
}