using System;
using System.Collections.Generic;
using System.Text;

namespace SmartStudyRooms.Data.Models
{
    public class Sala
    {
        public int SalaId { get; set; }
        public string Nome { get; set; }
        public int Capacidade { get; set; }
        public bool Ocupada { get; set; }
        public DateTime? ReservadaAte { get; set; }
    }
}
