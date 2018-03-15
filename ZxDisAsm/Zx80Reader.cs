using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZxDisAsm
{
    public class Z80Reader
    {
        private byte[] m_buffer;
        private byte[] m_memory;

        private string m_filename;

        private BasicFileHeader m_header;

        private bool m_isCompressed;

        private int m_memoryDataBlockStart;

        public byte[] Memory
        {
            get { return m_memory; }
        }

        public BasicFileHeader Header
        {
            get { return m_header; }
        }

        public bool Read(string _filename)
        {
            m_filename = _filename;

            LoadFile();
            GetHeader();
            ClearMemory();
            Decompress();

            return true;
        }

        private void Decompress()
        {
            int offset = 0; // Current offset into memory
            // TODO: It's 30 just now, but if this is v.2/3 of the file then it's not.. See docs.
            for (int i = m_memoryDataBlockStart; i < m_buffer.Length; i++)
            {
                if (m_buffer[i] == 0x00 && m_buffer[i + 1] == 0xED && m_buffer[i + 2] == 0xED && m_buffer[i + 3] == 0x00)
                {
                    break;
                }

                if (i < m_buffer.Length - 4)
                {
                    if (m_buffer[i] == 0xED && m_buffer[i + 1] == 0xED && m_isCompressed)
                    {
                        i += 2;
                        int repeat = m_buffer[i++];
                        byte value = m_buffer[i];
                        for (int j = 0; j < repeat; j++)
                        {
                            m_memory[offset] = value;
                            offset++;
                        }
                    }
                    else
                    {
                        m_memory[offset] = m_buffer[i];
                        offset++;
                    }
                }
                else
                {
                    m_memory[offset] = m_buffer[i];
                    offset++;
                }
            }
        }


        private void ClearMemory()
        {
            m_memory = new byte[49152]; // FOR 48K Spectrum ONLY.
        }

        private void LoadFile()
        {
            using (Stream stream = File.OpenRead(m_filename))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    m_buffer = reader.ReadBytes((int)stream.Length);
                }
            }
        }

        private void GetHeader()
        {
            // TODO: IF PC IS ZERO THEN THIS IS A VERSION 2/3 OF THE FILE AND AN ADDITIONAL HEADER MUST BE READ
            GCHandle handle = GCHandle.Alloc(m_buffer, GCHandleType.Pinned);
            m_header = (BasicFileHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(BasicFileHeader));
            m_isCompressed = (m_header.Flags1 & 0x20) == 0x20;



            //m_memoryDataBlockStart = ;

            if (m_header.PC != 0)
            {
                m_memoryDataBlockStart = 30;
            }

        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BasicFileHeader
    {
        public byte A;
        public byte F;
        public UInt16 BC;
        public UInt16 HL;
        public UInt16 PC;
        public UInt16 SP;
        public byte InterruptRegister;
        public byte RefreshRegister;

        /// <summary>
        /// Bit 0  : Bit 7 of the R-register
        /// Bit 1-3: Border colour
        /// Bit 4  : 1=Basic SamRom switched in
        /// Bit 5  : 1=Block of data is compressed
        /// Bit 6-7: No meaning
        /// </summary>
        public byte Flags1;
        public UInt16 DE;
        public UInt16 BC_Dash;
        public UInt16 DE_Dash;
        public UInt16 HL_Dash;
        public byte A_Dash;
        public byte F_Dash;
        public UInt16 IY;
        public UInt16 IX;
        public byte InterruptFlipFlop;
        public byte IFF2;
        public byte Flags2;
    }
}
