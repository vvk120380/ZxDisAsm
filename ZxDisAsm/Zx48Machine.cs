using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZxDisAsm
{
    partial class Zx48Machine : Z80Core
    {
        byte opcode;

        public Zx48Machine()
        {

        }
        public void Reset()
        {
            this.PC = 0x0000;
        }

        byte peek8(ushort addr)
        {
            if (addr <= ROMEnd) return ROM[addr];
            if (addr <= RAMDispEnd) return RAMDisp[addr - ROMEnd + 1];
            if (addr <= RAMAttrEnd) return RAMAttr[addr - RAMDispEnd + 1];
            return RAM[addr - RAMAttrEnd + 1];
        }

        void poke8(ushort addr, byte val)
        {
            if (addr <= ROMEnd) ROM[addr] = val;
            if (addr <= RAMDispEnd) RAMDisp[addr - ROMEnd + 1] = val;
            if (addr <= RAMAttrEnd) RAMAttr[addr - RAMDispEnd + 1] = val;
            RAM[addr - RAMAttrEnd + 1] = val;
        }

        ushort peek16(ushort addr)
        {
            return (ushort)(peek8((ushort)(addr + 1)) << 8 | peek8(addr));
        }

        void poke16(ushort addr, ushort val)
        {
            poke8(addr, (byte)val);
            poke8((ushort)(addr + 1), (byte)(val >> 8));
        }

        byte next8()
        {
            return peek8(PC++);
        }

        ushort next16() {
            ushort tmp = peek8(PC);
            PC++;
            return tmp;
        }

        bool isBIT(byte F, byte mask)
        {
            return (((F & mask) == mask) ? true : false);
        }

        public void Execute()
        {
            opcode = next8();

            switch (opcode)
            {
                //Команды загрузки 8-битного регистра непосредственным 8-битным значением
                case 0x3E: { A = next8(); break; }
                case 0x06: { B = next8(); break; }
                case 0x0E: { C = next8(); break; }
                case 0x16: { D = next8(); break; }
                case 0x1E: { E = next8(); break; }
                case 0x26: { H = next8(); break; }
                case 0x2E: { L = next8(); break; }
                //Команды загрузки 16-битного регистра непосредственным 16-битным значением
                case 0x01: { BC = next16(); break; }
                case 0x11: { DE = next16(); break; }
                case 0x21: { HL = next16(); break; }
                case 0x31: { SP = next16(); break; }
                //Команды загрузки 8-битного регистра значением 8-битного регистра
                case 0x7F: { A = A; break; }
                case 0x78: { A = B; break; }
                case 0x79: { A = C; break; }
                case 0x7A: { A = D; break; }
                case 0x7B: { A = E; break; }
                case 0x7C: { A = H; break; }
                case 0x7D: { A = L; break; }
                case 0x47: { B = A; break; }
                case 0x40: { B = B; break; }
                case 0x41: { B = C; break; }
                case 0x42: { B = D; break; }
                case 0x43: { B = E; break; }
                case 0x44: { B = H; break; }
                case 0x45: { B = L; break; }
                case 0x4F: { C = A; break; }
                case 0x48: { C = B; break; }
                case 0x49: { C = C; break; }
                case 0x4A: { C = D; break; }
                case 0x4B: { C = E; break; }
                case 0x4C: { C = H; break; }
                case 0x4D: { C = L; break; }
                case 0x57: { D = A; break; }
                case 0x50: { D = B; break; }
                case 0x51: { D = C; break; }
                case 0x52: { D = D; break; }
                case 0x53: { D = E; break; }
                case 0x54: { D = H; break; }
                case 0x55: { D = L; break; }
                case 0x5F: { E = A; break; }
                case 0x58: { E = B; break; }
                case 0x59: { E = C; break; }
                case 0x5A: { E = D; break; }
                case 0x5B: { E = E; break; }
                case 0x5C: { E = H; break; }
                case 0x5D: { E = L; break; }
                case 0x67: { H = A; break; }
                case 0x60: { H = B; break; }
                case 0x61: { H = C; break; }
                case 0x62: { H = D; break; }
                case 0x63: { H = E; break; }
                case 0x64: { H = H; break; }
                case 0x65: { H = L; break; }
                case 0x6F: { L = A; break; }
                case 0x68: { L = B; break; }
                case 0x69: { L = C; break; }
                case 0x6A: { L = D; break; }
                case 0x6B: { L = E; break; }
                case 0x6C: { L = H; break; }
                case 0x6D: { L = L; break; }
                //Команды загрузки 16-битного регистра значением 16-битного регистра
                case 0xF9: { SP = HL; break; }
                //Команды загрузки 8 - битного регистра значением в памяти по абсолютному адресу
                case 0x3A: { A = peek8(next16()); break; }
                case 0x2A: { HL = peek16(next16()); break; }
                //Команды загрузки 8-битного регистра значением в памяти по адресу в 16-битной регистровой паре
                case 0x0A: { A = peek8(BC); break; }
                case 0x1A: { A = peek8(DE); break; }
                case 0x7E: { A = peek8(HL); break; }
                case 0x46: { B = peek8(HL); break; }
                case 0x4E: { C = peek8(HL); break; }
                case 0x56: { D = peek8(HL); break; }
                case 0x5E: { E = peek8(HL); break; }
                case 0x66: { H = peek8(HL); break; }
                case 0x6E: { L = peek8(HL); break; }
                //Команды помещения значения регистра в память по абсолютному адресу
                case 0x32: { poke8(next16(),  A); break; }
                case 0x22: { poke16(next16(), HL); break; }


                case 0xDD:
                    {
                        opcode = next8();
                        switch (opcode)
                        {
                            //Команды загрузки 8-битного регистра непосредственным 8-битным значением
                            case 0x26: { IXH = next8(); break; }
                            case 0x2E: { IXL = next8(); break; }
                            //Команды загрузки 16-битного регистра непосредственным 16-битным значением
                            case 0x21: { IX  = next16(); break; }
                            //Команды загрузки 8-битного регистра значением 8-битного регистра
                            case 0x7C: { A = IXH; break; }
                            case 0x7D: { A = IXL; break; }
                            case 0x44: { B = IXH; break; }
                            case 0x45: { B = IXL; break; }
                            case 0x4C: { C = IXH; break; }
                            case 0x4D: { C = IXL; break; }
                            case 0x54: { D = IXH; break; }
                            case 0x55: { D = IXL; break; }
                            case 0x5C: { E = IXH; break; }
                            case 0x5D: { E = IXL; break; }
                            case 0x67: { IYH = A; break; }
                            case 0x60: { IYH = B; break; }
                            case 0x61: { IYH = C; break; }
                            case 0x62: { IYH = D; break; }
                            case 0x63: { IYH = E; break; }
                            case 0x64: { IYH = IYH; break; }
                            case 0x65: { IYH = IYL; break; }
                            case 0x6F: { IYL = A; break; }
                            case 0x68: { IYL = B; break; }
                            case 0x69: { IYL = C; break; }
                            case 0x6A: { IYL = D; break; }
                            case 0x6B: { IYL = E; break; }
                            case 0x6C: { IYL = IYH; break; }
                            case 0x6D: { IYL = IYL; break; }
                            //Команды загрузки 16-битного регистра значением 16-битного регистра
                            case 0xF9: { SP = IX; break; }
                            //Команды загрузки 8-битного регистра значением в памяти по абсолютному адресу
                            case 0x2A: { IX = peek16(next16()); break; }
                            //Команды загрузки 8-битного регистра значением в памяти по адресу в индексном регистре (со смещением)
                            case 0x7E: { A = peek8((ushort)(IX + next8())); break; }
                            case 0x46: { B = peek8((ushort)(IX + next8())); break; }
                            case 0x4E: { C = peek8((ushort)(IX + next8())); break; }
                            case 0x56: { D = peek8((ushort)(IX + next8())); break; }
                            case 0x5E: { E = peek8((ushort)(IX + next8())); break; }
                            case 0x66: { H = peek8((ushort)(IX + next8())); break; }
                            case 0x6E: { L = peek8((ushort)(IX + next8())); break; }
                            //Команды помещения значения регистра в память по абсолютному адресу
                            case 0x22: { poke16(next16(), IX); break; }


                        }
                        break;
                    }

                case 0xFD:
                    {
                        opcode = next8();
                        switch (opcode)
                        {
                            //Команды загрузки 8-битного регистра непосредственным 8-битным значением
                            case 0x26: { IYH = next8(); break; }
                            case 0x2E: { IYL = next8(); break; }
                            //Команды загрузки 16-битного регистра непосредственным 16-битным значением
                            case 0x21: { IY  = next16(); break; }
                            //Команды загрузки 8-битного регистра значением 8-битного регистра
                            case 0x7C: { A = IYH; break; }
                            case 0x7D: { A = IYL; break; }
                            case 0x44: { B = IYH; break; }
                            case 0x45: { B = IYL; break; }
                            case 0x4C: { C = IYH; break; }
                            case 0x4D: { C = IYL; break; }
                            case 0x54: { D = IYH; break; }
                            case 0x55: { D = IYL; break; }
                            case 0x5C: { E = IYH; break; }
                            case 0x5D: { E = IYL; break; }
                            case 0x67: { IXH = A; break; }
                            case 0x60: { IXH = B; break; }
                            case 0x61: { IXH = C; break; }
                            case 0x62: { IXH = D; break; }
                            case 0x63: { IXH = E; break; }
                            case 0x64: { IXH = IXH; break; }
                            case 0x65: { IXH = IXL; break; }
                            case 0x6F: { IXL = A; break; }
                            case 0x68: { IXL = B; break; }
                            case 0x69: { IXL = C; break; }
                            case 0x6A: { IXL = D; break; }
                            case 0x6B: { IXL = E; break; }
                            case 0x6C: { IXL = IXH; break; }
                            case 0x6D: { IXL = IXL; break; }
                            //Команды загрузки 16-битного регистра значением 16-битного регистра
                            case 0xF9: { SP = IY; break; }
                            //Команды загрузки 8-битного регистра значением в памяти по абсолютному адресу
                            case 0x2A: { IY = peek16(next16()); break; }
                            //Команды загрузки 8-битного регистра значением в памяти по адресу в индексном регистре (со смещением)
                            case 0x7E: { A = peek8((ushort)(IY + next8())); break; }
                            case 0x46: { B = peek8((ushort)(IY + next8())); break; }
                            case 0x4E: { C = peek8((ushort)(IY + next8())); break; }
                            case 0x56: { D = peek8((ushort)(IY + next8())); break; }
                            case 0x5E: { E = peek8((ushort)(IY + next8())); break; }
                            case 0x66: { H = peek8((ushort)(IY + next8())); break; }
                            case 0x6E: { L = peek8((ushort)(IY + next8())); break; }
                            //Команды помещения значения регистра в память по абсолютному адресу
                            case 0x22: { poke16(next16(), IY); break; }
                        }
                        break;
                    }

                case 0xED:
                    {
                        opcode = next8();
                        switch (opcode)
                        {
                            //Команды загрузки, использующие служебный 8-битный регистр в качестве одного из операндов
                            case 0x47: { I = A; break; }
                            case 0x57: {
                                    A = I;
                                    F_PARITY = IFF2;
                                    F_NEG = false;
                                    F_HALF = false;
                                    F_3 = isBIT(I, (byte)flags.n3);
                                    F_5 = isBIT(I, (byte)flags.n5);
                                    F_SIGN = isBIT(I, (byte)flags.S);
                                    F_ZERO = (I == 0) ? true : false;
                                    break;
                                }
                            case 0x4F: { R = A; break; }
                            case 0x5F:
                                {
                                    A = R;
                                    F_PARITY = IFF2;
                                    F_NEG = false;
                                    F_HALF = false;
                                    F_3 = isBIT(R, (byte)flags.n3);
                                    F_5 = isBIT(R, (byte)flags.n5);
                                    F_SIGN = isBIT(R, (byte)flags.S);
                                    F_ZERO = (R == 0) ? true : false;
                                    break;
                                }

                            //Команды загрузки 8 - битного регистра значением в памяти по абсолютному адресу
                            case 0x4B: { BC = peek16(next16()); break; }
                            case 0x5B: { DE = peek16(next16()); break; }
                            case 0x6B: { HL = peek16(next16()); break; }
                            case 0x7B: { SP = peek16(next16()); break; }
                            //Команды помещения значения регистра в память по абсолютному адресу
                            case 0x43: { poke16(next16(), BC); break; }
                            case 0x53: { poke16(next16(), DE); break; }
                            case 0x63: { poke16(next16(), HL); break; }
                            case 0x73: { poke16(next16(), SP); break; }

                        }
                        break;
                    }

                        default: break;
            }
        }



    }
}
