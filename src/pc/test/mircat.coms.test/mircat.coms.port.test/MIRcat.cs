using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace psc.mircat.coms.port.test
{
    public class MIRcat : SerialObject
    {
        protected CRC16 m_crc = new CRC16();

        public enum Command : ushort
        {
            // Information Commands
            GetLightInformation = 0x0001,
            GetQclInformation = 0x0002,
            GetStatusMask = 0x0003,
            GetSystemInformation = 0x0004,
            ClearFault = 0x0005,

            // Scan Commands
            ScanMode = 0x0032,
            SingleTuneModeParameters = 0x0033,
            SweepModeParameters = 0x0034,
            StartScan = 0x0035,
            GetScanProgress = 0x0036,
            StepMeasureModeParameters = 0x0037,
            MultiSpectralModeParameters = 0x0038,

            // Laser Commands
            LaserParameters = 0x0064,
            LaserQclParameters = 0x0065,
            LaserEmit = 0x0066,
            LaserArm = 0x0067,

            // System Commands
            DisplayUnits = 0x00C8,
            SaveState = 0x00C9,
            RestoreState = 0x00CA,
            EraseState = 0x00CB,
            GetSavedStateName = 0x00CC,
            GetNumberOfSavedStates = 0x00CD,
            AudioNotificationPrefs = 0x00CF,

            // Administrative Commands
            ShutdownLaser = 0x0099
        };



        public void GetLightInformation()
        {
            var cmdWord = Command.GetLightInformation;
            Action cmdAction = Action.Read;

            SpinCmd(cmdWord, cmdAction, 0);
        }
        public void GetQclInformation()
        {
            var cmdWord = Command.GetQclInformation;
            Action cmdAction = Action.Read;

            SpinCmd(cmdWord, cmdAction, 1);
        }
        public void GetStatusMask()
        {
            var cmdWord = Command.GetStatusMask;
            Action cmdAction = Action.Read;

            SpinCmd(cmdWord, cmdAction, 2);
        }
        public void GetSystemInformation()
        {
            var cmdWord = Command.GetSystemInformation;
            Action cmdAction = Action.Read;

            SpinCmd(cmdWord, cmdAction, 3);
        }
        public void ClearFault()
        {
            var cmdWord = Command.ClearFault;
            Action cmdAction = Action.Execute;

            SpinCmd(cmdWord, cmdAction, 0);
        }

        private void SpinCmd(Command cmdWord, Action cmdAction, ushort seqWord)
        {
            Console.WriteLine("attempting '{0}' command: '{1}'...", cmdAction, cmdWord);

            byte[] cmd = FormatCommand((ushort)cmdWord, cmdAction, seqWord);

            Console.WriteLine($"sending: 0x {string.Join(" ", cmd.Select(b=>b.ToString("x2")))}");

            //Console.Write("command bytes: \n0x");
            //foreach (byte b in cmd) { Console.Write(" {0:x2}", b); }
            //Console.WriteLine("");

            SendData(cmd);

            GetResponse();
        }

        // Note: response is a duplicate packet of the command, with the ack byte |'d with 0x80 if rejected!, 
        //       followed by a data packet.  The data packet has the ack byte == 0x04.

        protected void GetResponse()
        {
            //byte ack = 0;
            //List<byte> resp = new List<byte>();


            Console.WriteLine("reading response...");


            int nTries = 5;
            m_serPort.ReadTimeout = 100;

            byte[] buf = new byte[65536];
            bool bRun = true;
            int nGet;

            nGet = 0;
            var bytesReported = 0;
            while (bRun)
            {
                try
                {
                    nGet += m_serPort.Read(buf, nGet, buf.Length - nGet);
                }
                catch (TimeoutException) { }

                if (nGet > bytesReported)
                {
                    if (0 == bytesReported)
                    {
                        Console.Write("received: 0x");
                    }

                    Console.Write($" {(string.Join(" ", buf.Skip(bytesReported).Take(nGet - bytesReported).Select(b => b.ToString("x2"))))}");

                    bytesReported = nGet;
                }

                if (nGet >= 12)    // we have enough bytes for it to be a packet... begin analysis
                {
                    // first pass... assume our first byte is the packet first byte...

                    Console.WriteLine("\n\n** TEST PASSED **\n");


                    // ver
                    int ver = buf[0];

                    // command
                    UInt16 commandID = (UInt16)System.Net.IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buf, 1));

                    // ack
                    byte ack = buf[3];

                    // sequence
                    UInt16 sequence = (UInt16)System.Net.IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buf, 4));

                    //// status (ignore?)
                    //UInt16 status = (UInt16)System.Net.IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buf, 6));

                    // data length
                    UInt16 dataLength = (UInt16)System.Net.IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buf, 8));

                    return;

                    if (nGet >= (12 + dataLength))  // do we have all the data yet?
                    {
                        Console.WriteLine("");
                        Console.WriteLine("Received packet");

                        // check crc and return
                        UInt16 crc = (UInt16)System.Net.IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buf, 10 + dataLength));


                        Console.WriteLine("packet:");
                        Console.WriteLine(" version : {0}", ver);
                        Console.WriteLine(" command : {0}", commandID);
                        Console.WriteLine(" ack     : {0}", ack);
                        Console.WriteLine(" sequence: {0}", sequence);
                        Console.WriteLine(" data len: {0}", dataLength);
                        Console.WriteLine(" crc     : 0x{0:x4}", crc);
                        Console.Write(" data    : ");
                        for (int i = 10; i < dataLength; i++)
                        {
                            Console.Write(" {0:x2}", buf[i]);
                        }
                        Console.WriteLine("");
                        //Console.Write("         : ");
                        //for (int i = 0; i < dataLength; i++)
                        //{
                        //    Console.Write(" {0}", (char)buf[10 + i]);
                        //}

                        List<byte> bart = new List<byte>();
                        for (int i = 0; i < 10 + dataLength; i++)
                        {
                            bart.Add(buf[i]);
                        }
                        Console.WriteLine("calculated crc: 0x{0:x4}", m_crc.Calculate(bart.ToArray()));


                        return;
                    }

                    // don't have all the data... keep reading

                    System.Threading.Thread.Sleep(10);
                }

                if (0 >= nTries--)
                {
                    // give up
                    bRun = false;

                    Console.WriteLine("\n\n!! TEST FAILED !!\n");
                }
            }
        }

        public enum Action
        {
            Read = 1,
            Write = 2,
            Execute = 3
        }

        protected byte[] FormatCommand(UInt16 commandID, Action action, UInt16 sequence) // this version assumes no data
        {
            List<byte> cmd = new List<byte>();
            byte[] bits;

            // ver
            cmd.Add(0);

            // command
            bits = BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder((short)commandID));
            cmd.AddRange(bits);

            // act/ack
            cmd.Add((byte)action);

            // sequence
            bits = BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder((short)sequence));
            cmd.AddRange(bits);

            // status
            cmd.Add(0);
            cmd.Add(0);

            // bytes...
            ushort bytelen = (ushort)(cmd.Count + 2);
            bits = BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder((short)bytelen));
            cmd.AddRange(bits);

            // CRC
            bits = BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder((short)CRC_16_ANSI(cmd.ToArray())));
            //bits = BitConverter.GetBytes(CRC_16_ANSI(cmd.ToArray()));   // <-- use this if CRC byte order is backwards
            cmd.AddRange(bits);

            return cmd.ToArray();
        }

        protected UInt16 CRC_16_ANSI(byte[] packet)
        {
            return m_crc.Calculate(packet);
        }
    }
}
