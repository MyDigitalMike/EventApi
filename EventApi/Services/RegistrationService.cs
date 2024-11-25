using EventApi.Models;
using EventApi.Repositories;

namespace EventApi.Services
{
    public class RegistrationService
    {
        private readonly IUnitOfWork _unitOfWork;
        public RegistrationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<string> RegisterUserToEvent(int userId, int eventId)
        {
            // Verificar si el evento existe
            var eventEntity = await _unitOfWork.Repository<Event>().GetByIdAsync(eventId);
            if (eventEntity == null)
            {
                return "El evento no existe.";
            }

            // Verificar que el usuario no sea el creador del evento
            if (eventEntity.CreatedBy == userId)
            {
                return "No puedes inscribirte en un evento que tú mismo creaste.";
            }

            // Verificar si la capacidad máxima ya ha sido alcanzada
            if ((eventEntity.Registrations?.Count ?? 0) >= eventEntity.MaxCapacity)
            {
                return "La capacidad máxima del evento ya ha sido alcanzada.";
            }

            // Verificar si el usuario ya está inscrito en este evento
            var existingRegistration = await _unitOfWork.Repository<EventRegistration>()
                .FindAsync(r => r.UserId == userId && r.EventId == eventId);
            if (existingRegistration.Any())
            {
                return "Ya estás inscrito en este evento.";
            }

            // Verificar si el usuario ya está inscrito en 3 eventos diferentes
            var userRegistrationsCount = await _unitOfWork.Repository<EventRegistration>()
                .CountAsync(r => r.UserId == userId);
            if (userRegistrationsCount >= 3)
            {
                return "Solo puedes inscribirte en un máximo de 3 eventos diferentes.";
            }

            // Registrar al usuario en el evento
            var registration = new EventRegistration
            {
                UserId = userId,
                EventId = eventId
            };

            await _unitOfWork.Repository<EventRegistration>().AddAsync(registration);
            await _unitOfWork.SaveChangesAsync();

            return "Inscripción completada con éxito.";
        }
    }
}
