using Microsoft.AspNetCore.Mvc;
using SmartStudyRooms.Data.Dtos;
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

        [HttpPost("movimento")]
        public IActionResult PostMovimento([FromBody] SensorDto dto)
        {
            _sensorRepo.AtualizarSensor(dto.SalaId, dto.Movimento);

            if (dto.Movimento)
            {
                // Movimento detectado → sala ocupada
                _salaRepo.AtualizarOcupacao(dto.SalaId, true);
            }

            return Ok(new { mensagem = "Estado do sensor atualizado" });
        }
    }
}
