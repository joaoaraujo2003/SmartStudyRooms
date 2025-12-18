using System;
using System.Collections.Generic;
using System.Text;

namespace SmartStudyRooms.Data.Models
{
    public class SensorState
    {
        public int SensorStateId { get; set; }
        public int SalaId { get; set; }
        public bool Ocupada { get; set; }
        public DateTime UltimaAtualizacao { get; set; }
    }
}
