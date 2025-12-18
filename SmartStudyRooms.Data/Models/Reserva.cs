using System;

namespace SmartStudyRooms.Data.Models
{
    public class Reserva
    {
        public int ReservaId { get; set; }
        public int SalaId { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fim { get; set; }
        public bool Ativa { get; set; }
    }
}
