using System.Collections.Generic;
using SmartStudyRooms.Data.Repositories;
using SmartStudyRooms.Data.Models;

namespace SmartStudyRooms.API.Services.SOAP
{
    public class ReservaSoapService : IReservaSoapService
    {
        private readonly ReservaRepository _repo;
        public ReservaSoapService(ReservaRepository repo)
        {
            _repo = repo;
        }

        public IEnumerable<Reserva> ListarReservas()
        {
            return _repo.ListarReservas();
        }

        public Reserva ObterReserva(int id)
        {
            return _repo.ObterReserva(id);
        }
        public void CriarReserva(Reserva reserva)
        {
            _repo.CriarReserva(reserva);
        }
        public void CancelarReserva(int reservaId)
        {
            _repo.CancelarReserva(reservaId);
        }
    }
}
