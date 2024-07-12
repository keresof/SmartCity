namespace Shared.Common.Abstract
{
    public interface IHasDomainEvent
    {
        public List<DomainEvent> DomainEvents { get; set; }
    }


    public abstract class DomainEvent
    {
        protected DomainEvent()
        {
            DateOccured = DateTimeOffset.UtcNow;
        }
        public DateTimeOffset DateOccured { get; protected set; } = DateTime.UtcNow;
    }
}