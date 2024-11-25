namespace EventApi.Models
{
    public class User: BaseEntity
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public ICollection<EventRegistration> Registrations { get; set; }
    }
}
