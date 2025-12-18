using Microsoft.AspNetCore.Mvc;
using SmartStudyRooms.Data.Models;
using SmartStudyRooms.Data.Repositories;

namespace SmartStudyRooms.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservasController : ControllerBase
    {
        private readonly ReservaRepository _repo;

        public ReservasController(ReservaRepository repo)
        {
            _repo = repo;
        }

        [HttpPost]
        public IActionResult CriarReserva([FromBody] Reserva reserva)
        {
            if (reserva.Fim <= reserva.Inicio)
                return BadRequest("Data fim inválida.");

            if (!_repo.SalaDisponivel(reserva.SalaId, reserva.Inicio, reserva.Fim))
                return BadRequest("Sala já reservada nesse período.");

            int id = _repo.CriarReserva(reserva);
            reserva.ReservaId = id;

            return Ok(reserva);
        }
    }

}
