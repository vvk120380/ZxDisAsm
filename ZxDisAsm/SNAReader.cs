using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZxDisAsm
{
    class SNAReader
    {
        private const int HeaderSize = 0x1B;
        private const int DataSize = 0xC000;

        private byte[] m_filedata;
        private string m_filename;
        private byte[] m_memory;
        private SNAFileHeader m_header;

        public SNAFileHeader GetHeader()
        {
            return m_header; 
        }

        public byte[] GetMemory()
        {
            return m_memory;
        }

        public bool Load(string fn)
        {
            m_filename = fn;
            if (!LoadFile()) return false;
            if (!LoadHeader()) return false;
            if (!LoadData()) return false;
            return true;
        }

        private bool LoadFile()
        {
            using (Stream stream = File.OpenRead(m_filename))
            {
                if ((int)stream.Length != (HeaderSize + DataSize)) return false;
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    m_filedata = reader.ReadBytes((int)stream.Length);
                }
            }
            return true;
        }

        private bool LoadHeader()
        {
            m_header.I = getByte(0);
            m_header.HL_Dash = getUshort(1);
            m_header.DE_Dash = getUshort(3);
            m_header.BC_Dash = getUshort(5);
            m_header.A_Dash = getByte(7);
            m_header.F_Dash = getByte(8);
            m_header.HL = getUshort(9);
            m_header.DE = getUshort(11);
            m_header.BC = getUshort(13);
            m_header.IY = getUshort(15);
            m_header.IX = getUshort(17);
            m_header.InterruptFlag = getByte(19);
            m_header.R = getByte(20);
            m_header.A = getByte(21);
            m_header.F = getByte(22);
            m_header.SP = getUshort(23);
            m_header.IM = getByte(25);
            m_header.Border = getByte(26);
            return true;
        }

        private bool LoadData()
        {
            m_memory = new byte[0xC000];
            Buffer.BlockCopy(m_filedata, 27, m_memory, 0x0000, 0xC000);
            return true;
        }

        private byte getByte(int offset)
        {
            return m_filedata[offset];
        }

        private ushort getUshort(int offset)
        {
            return (ushort)(m_filedata[offset + 1] << 8 | m_filedata[offset]);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SNAFileHeader
    {
        public byte A;
        public byte F;
        public ushort BC;
        public ushort DE;
        public ushort HL;

        public byte A_Dash;
        public byte F_Dash;
        public ushort BC_Dash;
        public ushort DE_Dash;
        public ushort HL_Dash;

        public ushort PC;
        public ushort SP;

        public byte I;
        public byte R;

        public byte InterruptFlag;

        public byte IM;
        public byte Border;

        public ushort IY;
        public ushort IX;



        /// <summary>
        /// Bit 0  : Bit 7 of the R-register
        /// Bit 1-3: Border colour
        /// Bit 4  : 1=Basic SamRom switched in
        /// Bit 5  : 1=Block of data is compressed
        /// Bit 6-7: No meaning
        /// </summary>
        /// 
        //public byte Flags1;
        //public byte InterruptFlipFlop;
        //public byte IFF2;
        //public byte Flags2;
    }

}
