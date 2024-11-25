namespace EventApi.Models
{
    public class Event: BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; }
        public int MaxCapacity { get; set; }
        public int CreatedBy { get; set; } 
        public User? EventCreator { get; set; }
        public List<EventRegistration> Registrations { get; set; } = new List<EventRegistration>();
        public int RegisteredCount => Registrations?.Count ?? 0;
    }
}
