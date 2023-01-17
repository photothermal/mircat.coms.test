using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace psc.mircat.coms.port.test
{
    public interface ISerial
    {
        bool IsConnected { get; }

        string PortName { get; set; }

        int BaudRate { get; set; }

        int DataBits { get; set; }

        System.IO.Ports.Parity Parity { get; set; }

        System.IO.Ports.StopBits StopBits { get; set; }

        System.IO.Ports.Handshake Handshake { get; set; }

        void Connect();

        void Disconnect();
    }
}
