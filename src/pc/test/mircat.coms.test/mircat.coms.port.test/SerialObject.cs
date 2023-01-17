using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace psc.mircat.coms.port.test
{
    public abstract class SerialObject : ISerial, IDisposable
    {
        protected System.IO.Ports.SerialPort m_serPort = new System.IO.Ports.SerialPort();
        protected object m_mutex = new object();

        public string PortName
        {
            get { return m_serPort.PortName; }
            set { m_serPort.PortName = value; }
        }
        public int BaudRate
        {
            get { return m_serPort.BaudRate; }
            set { m_serPort.BaudRate = value; }
        }
        public int DataBits
        {
            get { return m_serPort.DataBits; }
            set { m_serPort.DataBits = value; }
        }
        public System.IO.Ports.Parity Parity
        {
            get { return m_serPort.Parity; }
            set { m_serPort.Parity = value; }
        }
        public System.IO.Ports.StopBits StopBits
        {
            get { return m_serPort.StopBits; }
            set { m_serPort.StopBits = value; }
        }
        public System.IO.Ports.Handshake Handshake
        {
            get { return m_serPort.Handshake; }
            set { m_serPort.Handshake = value; }
        }

        virtual public void Connect()
        {
            lock (m_mutex)
            {
                m_serPort.Open();
                m_serPort.DiscardInBuffer();
            }
        }
        virtual public void Disconnect()
        {
            lock (m_mutex)
            {
                m_serPort.Close();
            }
        }
        virtual public bool IsConnected
        {
            get
            {
                lock (m_mutex)
                {
                    return m_serPort.IsOpen;
                }
            }
        }
        virtual public void Dispose()
        {
            m_serPort.Dispose();
        }

        protected void SendData(byte[] data)
        {
            lock (m_mutex)
            {
                m_serPort.Write(data, 0, data.Length);
            }
        }
    }
}
