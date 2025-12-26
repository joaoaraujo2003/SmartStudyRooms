using SmartStudyRooms.Data.Repositories;
using SmartStudyRooms.Data.Models;
using System.Collections.Generic;

public class SalaSoapService : ISalaSoapService
{
    private readonly SalaRepository _repo;

    public SalaSoapService(SalaRepository repo)
    {
        _repo = repo;
    }

    public IEnumerable<Sala> ListarSalas()
    {
        return _repo.GetAll();
    }

    public Sala ObterSala(int id)
    {
        return _repo.GetById(id);
    }

    public int CriarSala(Sala sala)
    {
        return _repo.Create(sala);
    }
}
