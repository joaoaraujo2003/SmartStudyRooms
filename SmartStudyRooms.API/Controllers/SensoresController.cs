using Microsoft.AspNetCore.Mvc;
using SmartStudyRooms.Data.Models;
using SmartStudyRooms.Data.Repositories;

namespace SmartStudyRooms.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensoresController : Controller
    {
        private readonly SensorRepository _sensorRepo;
        private readonly SalaRepository _salaRepo;

        public SensoresController(SensorRepository sensorRepo,
        SalaRepository salaRepo)
        {
            _sensorRepo = sensorRepo;
            _salaRepo = salaRepo;
        }

        [HttpPost("presenca")]
        public IActionResult AtualizarEstado([FromBody] SensorState dto)
        {
            if (dto == null)
                return BadRequest();

            _sensorRepo.AtualizarSensor(dto.SalaId, dto.Ocupada);   

            _salaRepo.AtualizarOcupacao(dto.SalaId, dto.Ocupada);

            return Ok("Estado atualizado");
        }
    }
}
