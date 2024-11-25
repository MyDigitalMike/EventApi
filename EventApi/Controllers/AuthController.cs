using EventApi.Models;
using EventApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EventApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] EmailRequest emailRequest)
        {
            // Validar que el campo Email no esté vacío
            if (string.IsNullOrEmpty(emailRequest.Email))
            {
                return BadRequest(new { Message = "El campo email es requerido." });
            }

            // Buscar usuario por correo
            var user = await _unitOfWork.Repository<User>().FindAsync(u => u.Email == emailRequest.Email);

            if (user.Any())
            {
                return Ok(new { UserId = user.First().Id, UserName = user.First().UserName });
            }

            return Unauthorized(new { Message = "Correo no registrado" });
        }
    }
}
