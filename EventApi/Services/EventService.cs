using EventApi.Models;
using EventApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EventApi.Services
{
    public class EventService
    {
        private readonly IUnitOfWork _unitOfWork;
        public EventService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<string> DeleteEvent(int eventId, int userId)
        {
            // Buscar el evento
            var existingEvent = await _unitOfWork.Repository<Event>().GetByIdAsync(eventId);
            if (existingEvent == null)
            {
                return "El evento no existe.";
            }

            // Verificar que el usuario sea el creador del evento
            if (existingEvent.CreatedBy != userId)
            {
                return "No tienes permiso para eliminar este evento.";
            }

            // Verificar si hay asistentes inscritos en el evento
            var registrationsCount = await _unitOfWork.Repository<EventRegistration>()
                .CountAsync(r => r.EventId == eventId);

            if (registrationsCount > 0)
            {
                return "No se puede eliminar un evento con asistentes registrados.";
            }

            // Eliminar el evento
            _unitOfWork.Repository<Event>().Delete(existingEvent);
            await _unitOfWork.SaveChangesAsync();

            return "Evento eliminado exitosamente.";
        }
        public async Task<string> UpdateEvent(int eventId, int userId, Event updatedEvent)
        {
            // Buscar el evento
            var existingEvent = await _unitOfWork.Repository<Event>().GetByIdAsync(eventId);
            if (existingEvent == null)
            {
                return "El evento no existe.";
            }

            // Verificar que el usuario sea el creador del evento
            if (existingEvent.CreatedBy != userId)
            {
                return "No tienes permiso para editar este evento.";
            }

            // Actualizar solo los campos permitidos
            existingEvent.MaxCapacity = updatedEvent.MaxCapacity;
            existingEvent.EventDate = updatedEvent.EventDate;
            existingEvent.Location = updatedEvent.Location;
            existingEvent.UpdatedAt = DateTime.UtcNow;

            // Guardar los cambios
            _unitOfWork.Repository<Event>().Update(existingEvent);
            await _unitOfWork.SaveChangesAsync();

            return "Evento actualizado exitosamente.";
        }
        public async Task<List<Event>> GetAllEvents()
        {
            var events = await _unitOfWork.Repository<Event>().GetAllAsync();
            return events?.ToList() ?? new List<Event>();
        }

        public async Task<List<Event>> GetEventsByUserId(int userId)
        {
            // Obtener eventos filtrados por el id del usuario usando Unit of Work
            var events = await _unitOfWork.Repository<Event>().FindAsync(e => e.CreatedBy == userId);

            return events?.ToList() ?? new List<Event>();
        }
        public async Task<List<EventWithRegistrationCount>> GetAllEventsWithRegistrationCount()
        {
            var events = await _unitOfWork.Repository<Event>()
    .FindAsync(null, e => e.Registrations);

            return events.Select(e => new EventWithRegistrationCount
            {
                EventId = e.Id,
                Name = e.Name,
                Description = e.Description,
                EventDate = e.EventDate,
                Location = e.Location,
                MaxCapacity = e.MaxCapacity,
                RegisteredCount = e.Registrations.Count
            }).ToList();
        }
        public async Task<List<EventWithUserRegistration>> GetAllEventsForUser(int userId)
        {
            var events = await _unitOfWork.Repository<Event>()
            .FindAsync(null, e => e.Registrations);

            return events.Select(e => new EventWithUserRegistration
            {
                EventId = e.Id,
                Name = e.Name,
                Description = e.Description,
                EventDate = e.EventDate,
                Location = e.Location,
                MaxCapacity = e.MaxCapacity,
                RegisteredCount = e.Registrations.Count,
                IsUserRegistered = e.Registrations.Any(r => r.UserId == userId)
            }).ToList();
        }
    }
}
