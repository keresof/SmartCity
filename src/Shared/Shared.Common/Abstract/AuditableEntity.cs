// Shared.Common.Abstract.AuditableEntity.cs

using System;

namespace Shared.Common.Abstract
{
    public abstract class AuditableEntity
    {
        public int Id { get; set; }
        public DateTime Created { get; protected set; }
        public string CreatedBy { get; protected set; }
        public DateTime? LastModified { get; protected set; }
        public string LastModifiedBy { get; protected set; }

        public void SetCreatedBy(string createdBy)
        {
            if (string.IsNullOrEmpty(CreatedBy))
            {
                CreatedBy = createdBy;
                Created = DateTime.UtcNow;
            }
        }

        public void SetLastModifiedBy(string lastModifiedBy)
        {
            LastModifiedBy = lastModifiedBy;
            LastModified = DateTime.UtcNow;
        }
    }
}