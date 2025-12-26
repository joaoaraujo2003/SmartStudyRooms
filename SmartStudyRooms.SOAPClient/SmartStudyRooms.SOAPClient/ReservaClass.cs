using System;

namespace SmartStudyRooms.SOAPClient
{
    public class ReservaClass
    {
        public int ReservaId { get; set; }
        public int SalaId { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fim { get; set; }
        public bool Ativa { get; set; }

        public override string ToString()
        {
            return $"Reserva {ReservaId} | Sala {SalaId} | {Inicio:g} → {Fim:g} | Ativa: {Ativa}";
        }
    }
}
