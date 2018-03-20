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
        private byte[] page;

        private string m_filename;

        private BasicFileHeader m_header;

        private bool m_isCompressed;

        private int m_memoryDataBlockStart;

        public byte[] Memory
        {
            get { return page; }
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
            //ClearMemory();
            //Decompress();

            return true;
        }

        private void Decompress()
        {
            int offset = 0; // Current offset into memory
            // TODO: It's 30 just now, but if this is v.2/3 of the file then it's not.. See docs.
            for (int i = m_memoryDataBlockStart; i < m_buffer.Length; i++)
            {
                if (m_buffer[i] == 0x00 && m_buffer[i + 1] == 0xED && m_buffer[i + 2] == 0xED && m_buffer[i + 3] == 0x00) break;

                if (i < m_buffer.Length - 4)
                {
                    if (m_buffer[i] == 0xED && m_buffer[i + 1] == 0xED && m_isCompressed)
                    {
                        i += 2;
                        int repeat = m_buffer[i++];
                        byte value = m_buffer[i];
                        for (int j = 0; j < repeat; j++)
                            page[offset++] = value;
                    }
                    else
                        page[offset++] = m_buffer[i];
                }
                else
                    page[offset++] = m_buffer[i];
            }
        }

        private byte[] DecodeBlock(byte[] block, int blockStartPos, int blockSize,  bool bCompression)
        {

            int offset = 0; 
            byte[] pageRet = new byte[0x4000];

            for (int i = blockStartPos; i < blockStartPos + blockSize; i++)
            {
                if (i < blockStartPos + blockSize - 4)
                {
                    if (block[i] == 0xED && block[i + 1] == 0xED && bCompression)
                    {
                        i += 2;
                        int repeat = block[i++];
                        byte value = block[i];
                        for (int j = 0; j < repeat; j++)
                            pageRet[offset++] = value;
                    }
                    else
                        pageRet[offset++] = block[i];
                }
                else
                    pageRet[offset++] = block[i];
            }

            return pageRet;
        }




        private void ClearMemory()
        {
            page = new byte[49152]; // FOR 48K Spectrum ONLY.
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
            //GCHandle handle = GCHandle.Alloc(m_buffer, GCHandleType.Pinned);
            //m_header = (BasicFileHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(BasicFileHeader));

            m_header.A = getByte(0);
            m_header.F = getByte(1);
            m_header.BC = getUshort(2);
            m_header.HL = getUshort(4);
            m_header.PC = getUshort(6);
            m_header.SP = getUshort(8);
            m_header.I = getByte(10);
            m_header.R = getByte(11);
            byte byte12 = getByte(12);
            m_header.R |=(byte)((byte12 & 0x01) << 7);
            m_header.border = (byte)((byte12 >> 1) & 0x07);
            m_isCompressed = ((byte12 & 0x20) != 0);

            m_header.DE = getUshort(13);
            m_header.BC_Dash = getUshort(15);
            m_header.DE_Dash = getUshort(17);
            m_header.HL_Dash = getUshort(19);
            m_header.A_Dash = getByte(21);
            m_header.F_Dash = getByte(22);
            m_header.IY = getUshort(23);
            m_header.IX = getUshort(25);

            m_header.IY = getUshort(23);
            m_header.IX = getUshort(25);
            m_header.IFF1 = getByte(27) > 0;
            m_header.IFF1 = getByte(28) > 0;

            byte byte29 = getByte(29);
            m_header.IM = (byte)(byte29 & 0x3);



            //Размер uiExtBlockSize == 23 для версии 2, и 54 для версии 3
            int uiExtBlockSize = m_buffer[31] << 8 | m_buffer[30];

            int offset;

            if (uiExtBlockSize == 23 || uiExtBlockSize == 54)
            {
                //Смещение 30 байт (разм. основ. блок + размер доп. блока + 2 байта (размер переменной uiExtBlockSize))
                offset = 30 + uiExtBlockSize + sizeof(ushort);
                //Программный счетчик
                m_header.PC = (ushort)(m_buffer[33] << 8 | m_buffer[32]);
                // Режим 
                int Mode = m_buffer[34];


                int pageBlockSize = m_buffer[offset + 1] << 8 | m_buffer[offset]; offset += 2;
                int pageBlockNum = m_buffer[offset]; offset++;
                byte[] page1 = DecodeBlock(m_buffer, offset, pageBlockSize, true); offset += pageBlockSize;

                pageBlockSize = m_buffer[offset + 1] << 8 | m_buffer[offset]; offset += 2;
                pageBlockNum = m_buffer[offset]; offset++;
                byte[] page2 = DecodeBlock(m_buffer, offset, pageBlockSize, true); offset += pageBlockSize;

                pageBlockSize = m_buffer[offset + 1] << 8 | m_buffer[offset]; offset += 2;
                pageBlockNum = m_buffer[offset]; offset++;
                byte[] page3 = DecodeBlock(m_buffer, offset, pageBlockSize, true); offset += pageBlockSize;


                page = new byte[0xC000]; // FOR 48K Spectrum ONLY.

                Buffer.BlockCopy(page1, 0, page, 0x4000, 0x4000);
                Buffer.BlockCopy(page2, 0, page, 0x8000, 0x4000);
                Buffer.BlockCopy(page3, 0, page, 0x0000, 0x4000);


            }
            else
            {
                offset = 30;
                //m_memoryDataBlockStart = 30;
                if (m_header.PC != 0)
                {
                    m_memoryDataBlockStart = 30;
                }

                ClearMemory();
                Decompress();

            }


        }

        private byte getByte(int offset)
        {
            return m_buffer[offset];
        }

        private ushort getUshort(int offset)
        {
            return (ushort)(m_buffer[offset + 1] << 8 | m_buffer[offset]);
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BasicFileHeader
    {
        public byte A;
        public byte F;
        public ushort BC;
        public ushort HL;
        public ushort PC;
        public ushort SP;
        public byte I;
        public byte R;

        /// <summary>
        /// Bit 0  : Bit 7 of the R-register
        /// Bit 1-3: Border colour
        /// Bit 4  : 1=Basic SamRom switched in
        /// Bit 5  : 1=Block of data is compressed
        /// Bit 6-7: No meaning
        /// </summary>
        //public byte Flags1;
        public ushort DE;
        public ushort BC_Dash;
        public ushort DE_Dash;
        public ushort HL_Dash;
        public byte A_Dash;
        public byte F_Dash;
        public ushort IY;
        public ushort IX;
        public bool IFF1;
        public bool IFF2;
        public byte IM;

        public byte border;

    }
}
