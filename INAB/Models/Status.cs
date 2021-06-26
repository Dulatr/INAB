using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INAB.Models
{
    public class Status
    {
        public string Message { get; private set; }
        public StatusType Type { get; private set; }
        public Status(string _msg = "",StatusType type = 0)
        {
            Message = _msg;
            Type = type;
        }
    }

    public enum StatusType
    {
        OK=0,
        BUSY=1,
        ERR=2,
    }
}
