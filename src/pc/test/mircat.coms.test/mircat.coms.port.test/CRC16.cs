using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace psc.mircat.coms.port.test
{

    public class CRC16
    {
        protected const UInt16 KERNEL = 0x8005;

        protected UInt16[] m_table = new UInt16[256];
        private const UInt16 M_WIDTH = 16;
        private const UInt16 M_MSBIT = (1 << (M_WIDTH - 1));

        public CRC16()
        {
            int table_size = m_table.Length;
            UInt16 remainder;
            UInt16 bit;

            for (int div = 0; div < table_size; div++)
            {
                remainder = (UInt16)(div << (M_WIDTH - 8));

                for (bit = 0; bit < 8; bit++)
                {
                    if (0 != (remainder & M_MSBIT))
                    {
                        remainder = (UInt16)((remainder << 1) ^ KERNEL);
                    }
                    else
                    {
                        remainder = (UInt16)(remainder << 1);
                    }
                }

                m_table[div] = remainder;
            }
        }

        public UInt16 Calculate(byte[] input)
        {
            UInt16 remainder = 0;
            byte data;

            foreach (byte b in input)
            {
                data = (byte)(b ^ (remainder >> (M_WIDTH - 8)));
                remainder = (UInt16)(m_table[data] ^ (remainder << 8));
            }
            return remainder;
        }
    }
}
