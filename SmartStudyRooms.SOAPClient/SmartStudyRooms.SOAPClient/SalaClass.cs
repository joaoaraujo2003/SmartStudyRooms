using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartStudyRooms.SOAPClient
{
    public class SalaClass
    {
        public int SalaId { get; set; }
        public string Texto { get; set; }
        public string Estado { get; set; }

        public override string ToString()
        {
            return Texto;
        }

    }
}
