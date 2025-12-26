using CoreWCF;
using SmartStudyRooms.Data.Models;
using System.Collections.Generic;

namespace SmartStudyRooms.API.Services.SOAP
{
    [ServiceContract]
    public interface IReservaSoapService
    {
        [OperationContract]
        IEnumerable<Reserva> ListarReservas();

        [OperationContract]
        Reserva ObterReserva(int id);

        [OperationContract]
        void CriarReserva(Reserva reserva);

        [OperationContract]
        void CancelarReserva(int reservaId);
    }
}
