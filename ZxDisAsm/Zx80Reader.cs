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

            GetSNA();

            //GetHeader();
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
                            page[offset] = value;
                            offset++;
                        }
                    }
                    else
                    {
                        page[offset] = m_buffer[i];
                        offset++;
                    }
                }
                else
                {
                    page[offset] = m_buffer[i];
                    offset++;
                }
            }
        }

        private byte[] DecodeBlock(byte[] block, int blockStartPos, int blockSize,  bool bCompression)
        {

            int offset = 0; 
            byte[] pageRet = new byte[0x4000];

            for (int i = blockStartPos; i < blockSize; i++)
            {
                //if (block[i] == 0x00 && block[i + 1] == 0xED && block[i + 2] == 0xED && block[i + 3] == 0x00)
                //{
                //    break;
                //}

                //if (i < block.Length - 4)
                //{
                    if (block[i] == 0xED && block[i + 1] == 0xED && bCompression)
                    {
                        i += 2;
                        int repeat = block[i++];
                        byte value = block[i];
                        for (int j = 0; j < repeat; j++)
                        {
                            pageRet[offset] = value;
                            offset++;
                        }
                    }
                    else
                    {
                        pageRet[offset] = block[i];
                        offset++;
                    }
                //}
                //else
                //{
                //    pageRet[offset] = block[i];
                //    offset++;
                //}
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
            GCHandle handle = GCHandle.Alloc(m_buffer, GCHandleType.Pinned);
            m_header = (BasicFileHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(BasicFileHeader));
            m_isCompressed = (m_header.Flags1 & 0x20) == 0x20;

            //Размер uiExtBlockSize == 23 для версии 2, и 54 для версии 3
            int uiExtBlockSize = m_buffer[31] << 8 | m_buffer[30];

            int offset;

            if (uiExtBlockSize == 23 || uiExtBlockSize == 54)
            {
                //Смещение 30 байт (разм. основ. блок + размер доп. блока + 2 байта (размер переменной uiExtBlockSize))
                offset = 30 + uiExtBlockSize + sizeof(ushort);

                int uiPC = m_buffer[33] << 8 | m_buffer[32];

                //Программный счетчик
                m_header.PC = (ushort)uiPC;

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


                page = new byte[49152]; // FOR 48K Spectrum ONLY.

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


        public void GetSNA()
        {

            int offset = 0x00;

            m_header.InterruptRegister = m_buffer[offset++];
            m_header.HL_Dash = (ushort)(m_buffer[offset + 1] << 8 | m_buffer[offset]); offset += 2;
            m_header.DE_Dash = (ushort)(m_buffer[offset + 1] << 8 | m_buffer[offset]); offset += 2;
            m_header.BC_Dash = (ushort)(m_buffer[offset + 1] << 8 | m_buffer[offset]); offset += 2;
            m_header.A_Dash = m_buffer[offset++];
            m_header.F_Dash = m_buffer[offset++];
            m_header.HL = (ushort)(m_buffer[offset + 1] << 8 | m_buffer[offset]); offset += 2;
            m_header.DE = (ushort)(m_buffer[offset + 1] << 8 | m_buffer[offset]); offset += 2;
            m_header.BC = (ushort)(m_buffer[offset + 1] << 8 | m_buffer[offset]); offset += 2;
            m_header.IY = (ushort)(m_buffer[offset + 1] << 8 | m_buffer[offset]); offset += 2;
            m_header.IX = (ushort)(m_buffer[offset + 1] << 8 | m_buffer[offset]); offset += 2;
            m_header.InterruptFlipFlop = m_buffer[offset++];
            m_header.RefreshRegister = m_buffer[offset++];
            m_header.A = m_buffer[offset++];
            m_header.F = m_buffer[offset++];
            m_header.SP = (ushort)(m_buffer[offset + 1] << 8 | m_buffer[offset]); offset += 2;
            m_header.Flags1 = m_buffer[offset++];
            m_header.Flags2 = m_buffer[offset++];

            page = new byte[0xC000];

            Buffer.BlockCopy(m_buffer, offset, page, 0x0000, 0xC000);
 
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
        public ushort DE;
        public ushort BC_Dash;
        public ushort DE_Dash;
        public ushort HL_Dash;
        public byte A_Dash;
        public byte F_Dash;
        public ushort IY;
        public ushort IX;
        public byte InterruptFlipFlop;
        public byte IFF2;
        public byte Flags2;
    }
}
