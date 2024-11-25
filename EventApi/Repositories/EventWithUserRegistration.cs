namespace EventApi.Repositories
{
    public class EventWithUserRegistration
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; }
        public int MaxCapacity { get; set; }
        public int RegisteredCount { get; set; }
        public bool IsUserRegistered { get; set; }
    }
}
