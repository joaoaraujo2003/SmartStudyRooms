using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartStudyRooms.Data.Models;
using SmartStudyRooms.Data.Repositories;
using System.Collections.Generic;

namespace SmartStudyRooms.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalasController : ControllerBase
    {
        private readonly SalaRepository _repo;

        public SalasController(SalaRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Sala>> GetAll()
        {
            var salas = _repo.GetAll();
            return Ok(salas);
        }

        [HttpGet("disponiveis")]
        public ActionResult<IEnumerable<Sala>> GetDisponiveis()
        {
            var salas = _repo.GetDisponiveis();
            return Ok(salas);
        }

        [HttpGet("{id}")]
        public ActionResult<IEnumerable<Sala>> Get(int id)
        {
            var sala = _repo.GetById(id);
            if (sala == null) return NotFound();
            return Ok(sala);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Sala sala)
        {
            if (sala == null)
                return BadRequest("Dados da sala inválidos.");

            if (string.IsNullOrWhiteSpace(sala.Nome))
                return BadRequest("O nome da sala é obrigatório.");

            if (sala.Capacidade <= 0)
                return BadRequest("A capacidade tem de ser maior que 0.");

            sala.Ocupada = false;
            sala.ReservadaAte = null;

            int id = _repo.Create(sala);

            sala.SalaId = id;

            return CreatedAtAction(nameof(Get), new { id = sala.SalaId }, sala);
        }

        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] Sala sala)
        {
            sala.SalaId = id;
            var ok = _repo.Update(sala);
            if (!ok) return NotFound();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var ok = _repo.Delete(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
