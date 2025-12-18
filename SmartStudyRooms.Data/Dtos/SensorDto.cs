using System;
using System.Collections.Generic;
using System.Text;

namespace SmartStudyRooms.Data.Dtos
{
    public class SensorDto
    {
        public int SalaId { get; set; }
        public bool Ocupada { get; set; }
        public DateTime? UltimaAtualizacao { get; set; }
    }
}
