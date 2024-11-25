using EventApi.Models;
using EventApi.Repositories;
using EventApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly EventService _eventService;
        private readonly RegistrationService _registrationService;
        public EventsController(IUnitOfWork unitOfWork, EventService eventService, RegistrationService registrationService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
            _registrationService = registrationService ?? throw new ArgumentNullException(nameof(registrationService));
        }
        [HttpPost("{eventId}/register")]
        public async Task<IActionResult> RegisterUserToEvent(int eventId, [FromQuery] int userId)
        {
            var result = await _registrationService.RegisterUserToEvent(userId, eventId);

            if (result == "Inscripción completada con éxito.")
            {
                return Ok(new { message = result });
            }

            return BadRequest(new { error = result });
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateEvent([FromBody] Event newEvent, [FromQuery] int userId)
        {
            if (userId <= 0)
            {
                return BadRequest(new { Message = "El ID del usuario no es válido." });
            }

            if (newEvent == null)
            {
                return BadRequest(new { Message = "El evento no puede ser nulo." });
            }

            try
            {
                newEvent.EventCreator = null;
                newEvent.CreatedBy = userId;
                newEvent.CreatedAt = DateTime.UtcNow;
                newEvent.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Repository<Event>().AddAsync(newEvent);
                await _unitOfWork.SaveChangesAsync();

                return Ok(new { Message = "Evento creado exitosamente.", Data = newEvent });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Ocurrió un error al crear el evento.", Error = ex.Message });
            }
        }
        [HttpPut("{eventId}")]
        public async Task<IActionResult> UpdateEvent(int eventId, [FromQuery] int userId, [FromBody] Event updatedEvent)
        {
            var result = await _eventService.UpdateEvent(eventId, userId, updatedEvent);

            if (result == "Evento actualizado exitosamente.")
            {
                return Ok(new { message = result });
            }

            return BadRequest(new { error = result });
        }
        [HttpDelete("{eventId}")]
        public async Task<IActionResult> DeleteEvent(int eventId, [FromQuery] int userId)
        {
            var result = await _eventService.DeleteEvent(eventId, userId);

            if (result == "Evento eliminado exitosamente.")
            {
                return Ok(new { message = result });
            }

            return BadRequest(new { error = result });
        }
        [HttpGet("list")]
        public async Task<IActionResult> GetAllEvents()
        {
            try
            {
                // Obtener todos los eventos desde el repositorio
                var events = await _unitOfWork.Repository<Event>().GetAllAsync();

                // Si no hay eventos, devolver una lista vacía con mensaje
                if (events == null || !events.Any())
                {
                    return Ok(new { Message = "No se encontraron eventos.", Data = new List<Event>() });
                }

                return Ok(new { Message = "Eventos encontrados.", Data = events });
            }
            catch (Exception ex)
            {
                // Loguear el error (puedes usar ILogger aquí)
                return StatusCode(500, new { Message = "Ocurrió un error al procesar la solicitud.", Error = ex.Message });
            }
        }
    
    [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetEventsByUserId(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest(new { Message = "El ID del usuario no es válido." });
            }

            try
            {
                var events = await _unitOfWork.Repository<Event>().FindAsync(e => e.CreatedBy == userId);

                if (!events.Any()) // Si no hay eventos
                {
                    return Ok(new { Message = $"No se encontraron eventos para el usuario con ID {userId}.", Data = new List<Event>() });
                }

                return Ok(new { Message = "Eventos encontrados.", Data = events });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Ocurrió un error al procesar la solicitud.", Error = ex.Message });
            }
        }
        [HttpGet("list-with-registrations")]
        public async Task<IActionResult> GetAllEventsWithRegistrationCount()
        {
            var events = await _eventService.GetAllEventsWithRegistrationCount();
            return Ok(new { Message = "Eventos encontrados.", Data = events });
        }
        [HttpGet("list-for-user/{userId}")]
        public async Task<IActionResult> GetAllEventsForUser(int userId)
        {
            var events = await _eventService.GetAllEventsForUser(userId);
            return Ok(new { Message = "Eventos encontrados.", Data = events });
        }
    }
    }
