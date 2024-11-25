namespace EventApi.Models
{
    public class EventRegistration : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }

    }
}
