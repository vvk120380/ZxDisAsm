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
        byte tmp8;
        ushort tmp16;

        public Zx48Machine()
        {

        }

        public void Reset()
        {
            PC = 0x0000;
            SP = 0xffff;
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

        public void PushStack(int val)
        {
            SP = (ushort)((SP - 2) & 0xffff);
            poke8((ushort)((SP + 1) & 0xffff), (byte)(val >> 8));
            poke8((ushort)(SP & 0xffff), (byte)(val & 0xff));
        }

        public ushort PopStack()
        {
            int val = (peek8(SP)) | (peek8((ushort)(SP + 1)) << 8);
            SP = (ushort)((SP + 2) & 0xffff);
            return (ushort)val;
        }

        public void Execute()
        {
            opcode = next8();

            switch (opcode)
            {
                //Команды загрузки 8-битного регистра непосредственным 8-битным значением
                case 0x3E: { A = next8(); break; } //LD A,N
                case 0x06: { B = next8(); break; } //LD B,N
                case 0x0E: { C = next8(); break; } //LD C,N
                case 0x16: { D = next8(); break; } //LD D,N
                case 0x1E: { E = next8(); break; } //LD E,N
                case 0x26: { H = next8(); break; } //LD H,N
                case 0x2E: { L = next8(); break; } //LD L,N
                //Команды загрузки 16-битного регистра непосредственным 16-битным значением
                case 0x01: { BC = next16(); break; } //LD BC,NN
                case 0x11: { DE = next16(); break; } //LD DE,NN
                case 0x21: { HL = next16(); break; } //LD HL,NN
                case 0x31: { SP = next16(); break; } //LD SP,NN
                //Команды загрузки 8-битного регистра значением 8-битного регистра
                case 0x7F: { A = A; break; } //LD A,A
                case 0x78: { A = B; break; } //LD A,B
                case 0x79: { A = C; break; } //LD A,C
                case 0x7A: { A = D; break; } //LD A,D
                case 0x7B: { A = E; break; } //LD A,E
                case 0x7C: { A = H; break; } //LD A,H
                case 0x7D: { A = L; break; } //LD A,L
                case 0x47: { B = A; break; } //LD B,A
                case 0x40: { B = B; break; } //LD B,B
                case 0x41: { B = C; break; } //LD B,C
                case 0x42: { B = D; break; } //LD B,D
                case 0x43: { B = E; break; } //LD B,E
                case 0x44: { B = H; break; } //LD B,H
                case 0x45: { B = L; break; } //LD B,L
                case 0x4F: { C = A; break; } //LD C,A
                case 0x48: { C = B; break; } //LD C,B
                case 0x49: { C = C; break; } //LD C,C
                case 0x4A: { C = D; break; } //LD C,D
                case 0x4B: { C = E; break; } //LD C,E
                case 0x4C: { C = H; break; } //LD C,H
                case 0x4D: { C = L; break; } //LD C,L
                case 0x57: { D = A; break; } //LD D,A
                case 0x50: { D = B; break; } //LD D,B
                case 0x51: { D = C; break; } //LD D,C
                case 0x52: { D = D; break; } //LD D,D
                case 0x53: { D = E; break; } //LD D,E
                case 0x54: { D = H; break; } //LD D,H
                case 0x55: { D = L; break; } //LD D,L
                case 0x5F: { E = A; break; } //LD E,A
                case 0x58: { E = B; break; } //LD E,B
                case 0x59: { E = C; break; } //LD E,C
                case 0x5A: { E = D; break; } //LD E,D
                case 0x5B: { E = E; break; } //LD E,E
                case 0x5C: { E = H; break; } //LD E,H
                case 0x5D: { E = L; break; } //LD E,L
                case 0x67: { H = A; break; } //LD H,A
                case 0x60: { H = B; break; } //LD H,B
                case 0x61: { H = C; break; } //LD H,C
                case 0x62: { H = D; break; } //LD H,D
                case 0x63: { H = E; break; } //LD H,E
                case 0x64: { H = H; break; } //LD H,H
                case 0x65: { H = L; break; } //LD H,L
                case 0x6F: { L = A; break; } //LD L,A
                case 0x68: { L = B; break; } //LD L,B
                case 0x69: { L = C; break; } //LD L,C
                case 0x6A: { L = D; break; } //LD L,D
                case 0x6B: { L = E; break; } //LD L,E
                case 0x6C: { L = H; break; } //LD L,H
                case 0x6D: { L = L; break; } //LD L,L
                //Команды загрузки 16-битного регистра значением 16-битного регистра
                case 0xF9: { SP = HL; break; } //LD SP,HL
                //Команды загрузки 8 - битного регистра значением в памяти по абсолютному адресу
                case 0x3A: { A = peek8(next16()); break; }   //LD A,(NN)
                case 0x2A: { HL = peek16(next16()); break; } //LD HL,(NN)
                //Команды загрузки 8-битного регистра значением в памяти по адресу в 16-битной регистровой паре
                case 0x0A: { A = peek8(BC); break; } //LD A,(BC)
                case 0x1A: { A = peek8(DE); break; } //LD A,(DE)
                case 0x7E: { A = peek8(HL); break; } //LD A,(HL)
                case 0x46: { B = peek8(HL); break; } //LD B,(HL)
                case 0x4E: { C = peek8(HL); break; } //LD C,(HL)
                case 0x56: { D = peek8(HL); break; } //LD D,(HL)
                case 0x5E: { E = peek8(HL); break; } //LD E,(HL)
                case 0x66: { H = peek8(HL); break; } //LD H,(HL)
                case 0x6E: { L = peek8(HL); break; } //LD L,(HL)
                //Команды помещения значения регистра в память по абсолютному адресу
                case 0x32: { poke8(next16(),  A); break; } //LD (NN),A
                case 0x22: { poke16(next16(), HL); break; } //LD (NN),HL
                //Команды помещения значения 8-битного регистра в память по адресу в 16-битной регистровой паре
                case 0x02: { poke8(BC, A); break; } //LD (BC),A
                case 0x12: { poke8(DE, A); break; } //LD (DE),A
                case 0x77: { poke8(HL, A); break; } //LD (HL),A
                case 0x70: { poke8(HL, B); break; } //LD (HL),B
                case 0x71: { poke8(HL, C); break; } //LD (HL),C
                case 0x72: { poke8(HL, D); break; } //LD (HL),D
                case 0x73: { poke8(HL, E); break; } //LD (HL),E
                case 0x74: { poke8(HL, H); break; } //LD (HL),H
                case 0x75: { poke8(HL, L); break; } //LD (HL),L
                //Команда помещения непосредственного 8 - битного значения в память по адресу в регистре HL
                case 0x36: { poke8(HL, next8()); break; }  //LD (HL),N
                //Команды обмена значений 16 - битных регистровых пар
                case 0xD9: { tmp16 = BC; BC = BC_; BC_ = tmp16; tmp16 = DE; DE = DE_; DE_ = tmp16; tmp16 = HL; HL = HL_; HL_ = tmp16; break; } //EXX
                case 0x08: { tmp16 = AF; AF = AF_; AF_ = tmp16; break; } //EX AF,AF'
                case 0xEB: { tmp16 = DE; DE = HL; HL = tmp16; break; } //EX DE,HL
                //Команды обмена значений 16-битных регистровых пар и памяти
                case 0xE3: { tmp16 = peek16(SP); poke16(SP, HL); HL = tmp16; break; } //EX(SP),HL
                //Команды сложения значения аккумулятора со значением 8-битного регистра
                case 0x87: { Add_R(A); break; } //ADD A,A
                case 0x80: { Add_R(B); break; } //ADD A,B
                case 0x81: { Add_R(C); break; } //ADD A,C
                case 0x82: { Add_R(D); break; } //ADD A,D
                case 0x83: { Add_R(E); break; } //ADD A,E
                case 0x84: { Add_R(H); break; } //ADD A,H
                case 0x85: { Add_R(L); break; } //ADD A,L
                //Команда сложения значения аккумулятора с непосредственным 8-битным значением
                case 0xC6: { Add_R(next8()); break; } //ADD A,N
                //Команда сложения значения аккумулятора с 8-битным значением в памяти по адресу в регистре HL
                case 0x86: { Add_R(peek8(HL)); break; } //ADD A,(HL)
                //Команды сложения значений 16-битных регистровых пар
                case 0x09: { HL = Add_RR(HL, BC); break; } //ADD HL, BC
                case 0x19: { HL = Add_RR(HL, DE); break; } //ADD HL, DE
                case 0x29: { HL = Add_RR(HL, HL); break; } //ADD HL, HL
                case 0x39: { HL = Add_RR(HL, SP); break; } //ADD HL, SP
                //Команды инкремента значения 8-битного регистра
                case 0x3C: { A = Inc(A); break; } //INC A
                case 0x04: { B = Inc(B); break; } //INC B
                case 0x0C: { C = Inc(C); break; } //INC C
                case 0x14: { D = Inc(D); break; } //INC D
                case 0x1C: { E = Inc(E); break; } //INC E
                case 0x24: { H = Inc(H); break; } //INC H
                case 0x2C: { L = Inc(L); break; } //INC L
                //Команды инкремента 8-битного значения в памяти по адресу в регистре HL
                case 0x34: { poke8(HL, Inc(peek8(HL))); break; } //INC (HL)
                //Команды инкремента значения 16 - битного регистра
                case 0x03: { BC++; break; } //INC BC
                case 0x13: { DE++; break; } //INC DE
                case 0x23: { HL++; break; } //INC HL
                case 0x33: { SP++; break; } //INC SP
                //Команды сложения значения аккумулятора со значением 8-битного регистра с учетом флага переноса
                case 0x8F: { Adc_R(A); break; } //ADC A,A
                case 0x88: { Adc_R(B); break; } //ADC A,B
                case 0x89: { Adc_R(C); break; } //ADC A,C
                case 0x8A: { Adc_R(D); break; } //ADC A,D
                case 0x8B: { Adc_R(E); break; } //ADC A,E
                case 0x8C: { Adc_R(H); break; } //ADC A,H
                case 0x8D: { Adc_R(L); break; } //ADC A,L
                //Команда сложения значения аккумулятора с непосредственным 8-битным значением с учетом флага переноса
                case 0xCE: { Adc_R(next8()); break; } //ADC A,N
                //Команда сложения значения аккумулятора с 8-битным значением в памяти по адресу в регистре HL с учетом флага переноса
                case 0x8E: { Adc_R(peek8(HL)); break; } //ADC A,N
                //Команды вычитания значения 8-битного регистра из значения аккумулятора
                case 0x97: { Sub_R(A); break; } //SUB A
                case 0x90: { Sub_R(B); break; } //SUB B
                case 0x91: { Sub_R(C); break; } //SUB C
                case 0x92: { Sub_R(D); break; } //SUB D
                case 0x93: { Sub_R(E); break; } //SUB E
                case 0x94: { Sub_R(H); break; } //SUB H
                case 0x95: { Sub_R(L); break; } //SUB L
                //Команда вычитания непосредственного 8 - битного значения из значения аккумулятора
                case 0xD6: { Sub_R(next8()); break; } //SUB N
                //Команда вычитания непосредственного 8 - битного значения из значения аккумулятора
                case 0x96: { Sub_R(peek8(HL)); break; } //SUB N
                //Команды декремента значения 8 - битного регистра
                case 0x3D: { A = Dec_R(A); break; } //DEC A
                case 0x05: { B = Dec_R(B); break; } //DEC B
                case 0x0D: { C = Dec_R(C); break; } //DEC C
                case 0x15: { D = Dec_R(D); break; } //DEC D
                case 0x1D: { E = Dec_R(E); break; } //DEC E
                case 0x25: { H = Dec_R(H); break; } //DEC H
                case 0x2D: { L = Dec_R(L); break; } //DEC L
                //Команды декремента 8-битного значения в памяти по адресу в регистре HL
                case 0x35: { poke8(HL, Dec_R(peek8(HL))); break; } //DEC (HL)
                //Команды декремента значения 16-битного регистра
                case 0x0B: { BC--; break; } //DEC BC
                case 0x1B: { DE--; break; } //DEC DE
                case 0x2B: { HL--; break; } //DEC HL
                case 0x3B: { SP--; break; } //DEC SP
                //Команды вычитания значения 8-битного регистра из значения аккумулятора с учетом флага переноса
                case 0x9F: { Sbc_R(A); break; } //SBC A,A
                case 0x98: { Sbc_R(B); break; } //SBC A,B
                case 0x99: { Sbc_R(C); break; } //SBC A,C
                case 0x9A: { Sbc_R(D); break; } //SBC A,D
                case 0x9B: { Sbc_R(E); break; } //SBC A,E
                case 0x9C: { Sbc_R(H); break; } //SBC A,H
                case 0x9D: { Sbc_R(L); break; } //SBC A,L
                //Команда вычитания непосредственного 8-битного значения из значения аккумулятора с учетом флага переноса
                case 0xDE: { Sbc_R(next8()); break; } //SBC A,N
                //Команда вычитания 8 - битного значения в памяти по адресу в регистре HL из значения аккумулятора с учетом флага переноса
                case 0x9E: { Sbc_R(peek8(HL)); break; } //SBC A,(HL)
                //Команды сравнения значения 8-битного регистра со значением аккумулятора
                case 0xBF: { Cp_R(A); break; } //CP A
                case 0xB8: { Cp_R(B); break; } //CP B
                case 0xB9: { Cp_R(C); break; } //CP C
                case 0xBA: { Cp_R(D); break; } //CP D
                case 0xBB: { Cp_R(E); break; } //CP E
                case 0xBC: { Cp_R(H); break; } //CP H
                case 0xBD: { Cp_R(L); break; } //CP L
                //Команда сравнения непосредственного 8-битного значения со значением аккумулятора
                case 0xFE: { Cp_R(next8()); break; } //CP N
                //Команда сравнения 8-битного значения в памяти по адресу в регистре HL со значением аккумулятора
                case 0xBE: { Cp_R(peek8(HL)); break; } //CP (HL)
                //Команды логического «И» над значением 8-битного регистра и значением аккумулятора
                case 0xA7: { And_R(A); break; } //AND A
                case 0xA0: { And_R(B); break; } //AND B
                case 0xA1: { And_R(C); break; } //AND C
                case 0xA2: { And_R(D); break; } //AND D
                case 0xA3: { And_R(E); break; } //AND E
                case 0xA4: { And_R(H); break; } //AND H
                case 0xA5: { And_R(L); break; } //AND L
                //Команда логического «И» над непосредственным 8-битным значением и значением аккумулятора
                case 0xE6: { And_R(next8()); break; } //AND N
                //Команда логического «И» над 8-битным значением в памяти по адресу в регистре HL и значением аккумулятора
                case 0xA6: { And_R(peek8(HL)); break; } //AND (HL)
                //Команды логического «ИЛИ» над значением 8-битного регистра и значением аккумулятора
                case 0xB7: { Or_R(A); break; } //OR A
                case 0xB0: { Or_R(B); break; } //OR B
                case 0xB1: { Or_R(C); break; } //OR C
                case 0xB2: { Or_R(D); break; } //OR D
                case 0xB3: { Or_R(E); break; } //OR E
                case 0xB4: { Or_R(H); break; } //OR H
                case 0xB5: { Or_R(L); break; } //OR L
                //Команда логического «ИЛИ» над непосредственным 8-битным значением и значением аккумулятора
                case 0xF6: { Or_R(next8()); break; } //OR N
                //Команда логического «ИЛИ» над 8-битным значением в памяти по адресу в регистре HL и значением аккумулятора
                case 0xB6: { Or_R(peek8(HL)); break; } //OR (HL)
                //Команды логического «исключающего ИЛИ» над значением 8-битного регистра и значением аккумулятора
                case 0xAF: { Xor_R(A); break; } //XOR A
                case 0xA8: { Xor_R(B); break; } //XOR B
                case 0xA9: { Xor_R(C); break; } //XOR C
                case 0xAA: { Xor_R(D); break; } //XOR D
                case 0xAB: { Xor_R(E); break; } //XOR E
                case 0xAC: { Xor_R(H); break; } //XOR H
                case 0xAD: { Xor_R(L); break; } //XOR L
                //Команда логического «исключающего ИЛИ» над непосредственным 8 - битным значением и значением аккумулятора
                case 0xEE: { Xor_R(next8()); break; } //XOR N
                //Команда логического «исключающего ИЛИ» над 8-битным значением в памяти по адресу в регистре HL и значением аккумулятора
                case 0xAE: { Xor_R(peek8(HL)); break; } //XOR (HL)
                //Команда безусловного перехода по непосредственному адресу
                case 0xC3: { PC = next16(); break; } //JP NN
                //Команды безусловного перехода по адресу в 16-битном регистре
                case 0xE9: { PC = peek16(HL); break; } //JP (HL)
                //Команды условного перехода по непосредственному адресу
                case 0xC2: { PC = !F_ZERO ? next16() : (ushort)(PC + 2); break; } //JP NZ,NN
                case 0xCA: { PC = F_ZERO ? next16() : (ushort)(PC + 2); break; }  //JP Z,NN
                case 0xD2: { PC = !F_CARRY ? next16() : (ushort)(PC + 2); break; } //JP NZ,NN
                case 0xDA: { PC = F_CARRY ? next16() : (ushort)(PC + 2); break; }  //JP Z,NN
                case 0xE2: { PC = !F_PARITY ? next16() : (ushort)(PC + 2); break; } //JP NZ,NN
                case 0xEA: { PC = F_PARITY ? next16() : (ushort)(PC + 2); break; }  //JP Z,NN
                case 0xF2: { PC = !F_SIGN ? next16() : (ushort)(PC + 2); break; } //JP NZ,NN
                case 0xFA: { PC = F_SIGN ? next16() : (ushort)(PC + 2); break; }  //JP Z,NN
                //Команда безусловного относительного перехода
                case 0x18: { PC = (ushort)(PC + GetDisplacement(next8()) - 2);  break; }  //JP s
                //Команды условного относительного перехода
                case 0x20: { PC = !F_ZERO  ? (ushort)(PC + GetDisplacement(next8()) - 2) : (ushort)(PC + 1); break; } //JP NZ,NN
                case 0x28: { PC = F_ZERO   ? (ushort)(PC + GetDisplacement(next8()) - 2) : (ushort)(PC + 1); break; }  //JP Z,NN
                case 0x30: { PC = !F_CARRY ? (ushort)(PC + GetDisplacement(next8()) - 2) : (ushort)(PC + 1); break; } //JP NZ,NN
                case 0x38: { PC = F_CARRY  ? (ushort)(PC + GetDisplacement(next8()) - 2) : (ushort)(PC + 1); break; }  //JP Z,NN
                //Команда условного относительного перехода с организацией цикла по регистру B
                case 0x10: { PC = (--B != 0) ? (ushort)(PC + GetDisplacement(next8()) - 2) : ++PC; break;}  //DJNZ s
                //Команды помещения значения 16-битной регистровой пары в стек
                case 0xF5: { PushStack(AF); break; }  //PUSH AF
                case 0xC5: { PushStack(BC); break; }  //PUSH BC
                case 0xD5: { PushStack(DE); break; }  //PUSH DE
                case 0xE5: { PushStack(HL); break; }  //PUSH HL
                //Команды снятия значения 16-битной регистровой пары со стека
                case 0xF1: { AF = PopStack(); break; } //POP AF
                case 0xC1: { BC = PopStack(); break; } //POP BC
                case 0xD1: { DE = PopStack(); break; } //POP DE
                case 0xE1: { HL = PopStack(); break; } //POP HL


                case 0xDD:
                    {
                        opcode = next8();
                        switch (opcode)
                        {
                            //Команды загрузки 8-битного регистра непосредственным 8-битным значением
                            case 0x26: { IXH = next8(); break; } //LD XH,N
                            case 0x2E: { IXL = next8(); break; } //LD XL,N
                            //Команды загрузки 16-битного регистра непосредственным 16-битным значением
                            case 0x21: { IX  = next16(); break; } //LD X,NN
                            //Команды загрузки 8-битного регистра значением 8-битного регистра
                            case 0x7C: { A = IXH; break; } //LD A,XH
                            case 0x7D: { A = IXL; break; } //LD A,XL
                            case 0x44: { B = IXH; break; } //LD B,XH
                            case 0x45: { B = IXL; break; } //LD B,XL
                            case 0x4C: { C = IXH; break; } //LD C,XH
                            case 0x4D: { C = IXL; break; } //LD C,XL
                            case 0x54: { D = IXH; break; } //LD D,XH
                            case 0x55: { D = IXL; break; } //LD D,XL
                            case 0x5C: { E = IXH; break; } //LD E,XH
                            case 0x5D: { E = IXL; break; } //LD E,XL
                            case 0x67: { IXH = A; break; } //LD XH,A
                            case 0x60: { IXH = B; break; } //LD XH,B
                            case 0x61: { IXH = C; break; } //LD XH,C
                            case 0x62: { IXH = D; break; } //LD XH,D
                            case 0x63: { IXH = E; break; } //LD XH,E
                            case 0x64: { IXH = IXH; break; } //LD XH,XH
                            case 0x65: { IXH = IXL; break; } //LD XH,XL
                            case 0x6F: { IXL = A; break; } //LD XL,A
                            case 0x68: { IXL = B; break; } //LD XL,B
                            case 0x69: { IXL = C; break; } //LD XL,C
                            case 0x6A: { IXL = D; break; } //LD XL,D
                            case 0x6B: { IXL = E; break; } //LD XL,E
                            case 0x6C: { IXL = IXH; break; } //LD XL,XH
                            case 0x6D: { IXL = IXL; break; } //LD XL,XL
                            //Команды загрузки 16-битного регистра значением 16-битного регистра
                            case 0xF9: { SP = IX; break; } //LD SP,X
                            //Команды загрузки 8-битного регистра значением в памяти по абсолютному адресу
                            case 0x2A: { IX = peek16(next16()); break; } //LD X,(NN)
                            //Команды загрузки 8-битного регистра значением в памяти по адресу в индексном регистре (со смещением)
                            case 0x7E: { A = peek8((ushort)(IX + GetDisplacement(next8()))); break; } //LD A,(IX+s)
                            case 0x46: { B = peek8((ushort)(IX + GetDisplacement(next8()))); break; } //LD B,(IX+s) 
                            case 0x4E: { C = peek8((ushort)(IX + GetDisplacement(next8()))); break; } //LD C,(IX+s)
                            case 0x56: { D = peek8((ushort)(IX + GetDisplacement(next8()))); break; } //LD D,(IX+s)
                            case 0x5E: { E = peek8((ushort)(IX + GetDisplacement(next8()))); break; } //LD E,(IX+s)
                            case 0x66: { H = peek8((ushort)(IX + GetDisplacement(next8()))); break; } //LD H,(IX+s)
                            case 0x6E: { L = peek8((ushort)(IX + GetDisplacement(next8()))); break; } //LD L,(IX+s)
                            //Команды помещения значения регистра в память по абсолютному адресу
                            case 0x22: { poke16(next16(), IX); break; } //LD (NN),X
                            //Команды помещения значения 8-битного регистра в память по адресу в индексном регистре (со смещением)
                            case 0x77: { poke8((ushort)(IX + GetDisplacement(next8())), A); break; } //LD (IX+s),A
                            case 0x70: { poke8((ushort)(IX + GetDisplacement(next8())), B); break; } //LD (IX+s),B
                            case 0x71: { poke8((ushort)(IX + GetDisplacement(next8())), C); break; } //LD (IX+s),C
                            case 0x72: { poke8((ushort)(IX + GetDisplacement(next8())), D); break; } //LD (IX+s),D
                            case 0x73: { poke8((ushort)(IX + GetDisplacement(next8())), E); break; } //LD (IX+s),E
                            case 0x74: { poke8((ushort)(IX + GetDisplacement(next8())), H); break; } //LD (IX+s),H
                            case 0x75: { poke8((ushort)(IX + GetDisplacement(next8())), L); break; } //LD (IX+s),L
                            //Команды помещения непосредственного 8-битного значения в память по адресу в индексном регистре (со смещением)
                            case 0x36: { poke8((ushort)(IX + GetDisplacement(next8())), next8()); break; } //LD (IX+s),N
                            //Команды обмена значений 16-битных регистровых пар и памяти
                            case 0xE3: { tmp16 = peek16(SP); poke16(SP, IX); IX = tmp16; break; } //EX(SP),IX
                            //Команды сложения значения аккумулятора со значением 8-битного регистра
                            case 0x84: { Add_R(IXH); break; } //ADD A,XH
                            case 0x85: { Add_R(IXL); break; } //ADD A,XL
                            //Команды сложения значения аккумулятора с 8-битным значением в памяти по адресу в индексном регистре (со смещением)
                            case 0x86: { Add_R(peek8((ushort)(IX + GetDisplacement(next8())))); break; } //ADD A,XL
                            //Команды сложения значений 16-битных регистровых пар
                            case 0x09: { IX = Add_RR(IX, BC); break; } //ADD IX, BC
                            case 0x19: { IX = Add_RR(IX, DE); break; } //ADD IX, DE
                            case 0x29: { IX = Add_RR(IX, HL); break; } //ADD IX, HL
                            case 0x39: { IX = Add_RR(IX, SP); break; } //ADD IX, SP
                            //Команды инкремента значения 8-битного регистра
                            case 0x24: { IXH = Inc(IXH); break; } //INC IXH
                            case 0x2C: { IXL = Inc(IXL); break; } //INC IXL
                            //Команды инкремента 8-битного значения в памяти по адресу в индексном регистре (со смещением)
                            case 0x34: { ushort addr = (ushort)(IX + GetDisplacement(next8())); poke8(addr, Inc(peek8(addr))); break; } //INC(IX + s)
                            //Команды инкремента значения 16 - битного регистра
                            case 0x03: { IX++; break; } //INC IX
                            //Команды сложения значения аккумулятора со значением 8-битного регистра с учетом флага переноса
                            case 0x8C: { Adc_R(IXH); break; } //ADC A,IXH
                            case 0x8D: { Adc_R(IXL); break; } //ADC A,IXL
                            //Команды сложения значения аккумулятора с 8-битным значением в памяти по адресу в индексном регистре (со смещением) с учетом флага переноса
                            case 0x8E: { Adc_R((ushort)(IX + GetDisplacement(next8()))); break; } //ADC A,(IX+s)
                            //Команды вычитания значения 8-битного регистра из значения аккумулятора
                            case 0x94: { Sub_R(IXH); break; } //SUB IXH
                            case 0x95: { Sub_R(IXL); break; } //SUB IXL
                            //Команды вычитания 8-битного значения в памяти по адресу в индексном регистре (со смещением) из значения аккумулятора
                            case 0x96: { Sub_R(IX + GetDisplacement(next8())); break; } //SUB IX
                            //Команды декремента значения 8 - битного регистра
                            case 0x25: { IXH = Dec_R(IXH); break; } //DEC IXH
                            case 0x2D: { IXL = Dec_R(IXL); break; } //DEC IXL
                            //Команды декремента 8-битного значения в памяти по адресу в индексном регистре (со смещением)
                            case 0x35: { ushort addr = (ushort)(IX + GetDisplacement(next8())); poke8(addr, Dec_R(peek8(addr))); break; } //DEC(IX + s)
                            //Команды декремента значения 16-битного регистра
                            case 0x2B: { IX--; break; } //DEC IX
                            //Команды вычитания значения 8-битного регистра из значения аккумулятора с учетом флага переноса
                            case 0x9C: { Sbc_R(IXH); break; } //SBC A,IXH
                            case 0x9D: { Sbc_R(IXL); break; } //SBC A,IXL
                            //Команды вычитания 8-битного значения в памяти по адресу в индексном регистре (со смещением) из значения аккумулятора с учетом флага переноса
                            case 0x9E: { Sbc_R((ushort)(IX + GetDisplacement(next8()))); break; } //SBC A,(IX + s)
                            //Команды сравнения значения 8-битного регистра со значением аккумулятора
                            case 0xBC: { Cp_R(IXH); break; } //CP IXH
                            case 0xBD: { Cp_R(IXL); break; } //CP IXL
                            //Команды сравнения 8-битного значения в памяти по адресу в индексном регистре (со смещением) со значением аккумулятора
                            case 0xBE: { Cp_R((ushort)(IX + GetDisplacement(next8()))); break; }// CP(IX + s)
                            //Команды логического «И» над значением 8-битного регистра и значением аккумулятора
                            case 0xA4: { And_R(IXH); break; } //AND IXH
                            case 0xA5: { And_R(IXL); break; } //AND IXL
                            //Команды логического «И» над 8-битным значением в памяти по адресу в индексном регистре (со смещением) и значением аккумулятора
                            case 0xA6: { And_R((ushort)(IX + GetDisplacement(next8()))); break; }// AND(IX + s)
                            //Команды логического «ИЛИ» над значением 8-битного регистра и значением аккумулятора
                            case 0xB4: { Or_R(IXH); break; } //OR IXH
                            case 0xB5: { Or_R(IXL); break; } //OR IXL
                            //Команды логического «ИЛИ» над 8-битным значением в памяти по адресу в индексном регистре (со смещением) и значением аккумулятора
                            case 0xB6: { Or_R((ushort)(IX + GetDisplacement(next8()))); break; }// OR(IX + s)
                            //Команды логического «исключающего ИЛИ» над значением 8-битного регистра и значением аккумулятора
                            case 0xAC: { Xor_R(IXH); break; } //XOR IXH
                            case 0xAD: { Xor_R(IXL); break; } //XOR IXL
                            //Команды логического «исключающего ИЛИ» над 8-битным значением в памяти по адресу в индексном регистре (со смещением) и значением аккумулятора
                            case 0xAE: { Xor_R((ushort)(IX + GetDisplacement(next8()))); break; }// XOR(IX + s)
                            //Команды безусловного перехода по адресу в 16-битном регистре
                            case 0xE9: { PC = peek16(IX); break; } //JP (HL)
                            //Команды помещения значения 16-битной регистровой пары в стек
                            case 0xE5: { PushStack(IX); break; }  //PUSH IX
                            //Команды снятия значения 16-битной регистровой пары со стека
                            case 0xE1: { IX = PopStack(); break; } //POP IX
                        }
                        break;
                    }

                case 0xFD:
                    {
                        opcode = next8();
                        switch (opcode)
                        {
                            //Команды загрузки 8-битного регистра непосредственным 8-битным значением
                            case 0x26: { IYH = next8(); break; } //LD YH,N
                            case 0x2E: { IYL = next8(); break; } //LD YL,N
                            //Команды загрузки 16-битного регистра непосредственным 16-битным значением
                            case 0x21: { IY  = next16(); break; } //LD Y,NN
                            //Команды загрузки 8-битного регистра значением 8-битного регистра
                            case 0x7C: { A = IYH; break; }   //LD A,YH
                            case 0x7D: { A = IYL; break; }   //LD A,YL
                            case 0x44: { B = IYH; break; }   //LD B,YH
                            case 0x45: { B = IYL; break; }   //LD B,YL
                            case 0x4C: { C = IYH; break; }   //LD C,YH
                            case 0x4D: { C = IYL; break; }   //LD C,YL
                            case 0x54: { D = IYH; break; }   //LD D,YH
                            case 0x55: { D = IYL; break; }   //LD D,YL
                            case 0x5C: { E = IYH; break; }   //LD E,YH
                            case 0x5D: { E = IYL; break; }   //LD E,YL
                            case 0x67: { IYH = A; break; }   //LD YH,A
                            case 0x60: { IYH = B; break; }   //LD YH,B
                            case 0x61: { IYH = C; break; }   //LD YH,C
                            case 0x62: { IYH = D; break; }   //LD YH,D
                            case 0x63: { IYH = E; break; }   //LD YH,E
                            case 0x64: { IYH = IYH; break; } //LD YH,YH
                            case 0x65: { IYH = IYL; break; } //LD YH,YL
                            case 0x6F: { IYL = A; break; }   //LD YL,A
                            case 0x68: { IYL = B; break; }   //LD YL,B
                            case 0x69: { IYL = C; break; }   //LD YL,C
                            case 0x6A: { IYL = D; break; }   //LD YL,D
                            case 0x6B: { IYL = E; break; }   //LD YL,E
                            case 0x6C: { IYL = IYH; break; } //LD YL,YH
                            case 0x6D: { IYL = IYL; break; } //LD YL,YL
                            //Команды загрузки 16-битного регистра значением 16-битного регистра
                            case 0xF9: { SP = IY; break; } //LD SP,Y
                            //Команды загрузки 8-битного регистра значением в памяти по абсолютному адресу
                            case 0x2A: { IY = peek16(next16()); break; } //LD Y,(NN)
                            //Команды загрузки 8-битного регистра значением в памяти по адресу в индексном регистре (со смещением)
                            case 0x7E: { A = peek8((ushort)(IY + GetDisplacement(next8()))); break; } //LD A,(IY+s)
                            case 0x46: { B = peek8((ushort)(IY + GetDisplacement(next8()))); break; } //LD B,(IY+s)
                            case 0x4E: { C = peek8((ushort)(IY + GetDisplacement(next8()))); break; } //LD C,(IY+s)
                            case 0x56: { D = peek8((ushort)(IY + GetDisplacement(next8()))); break; } //LD D,(IY+s)
                            case 0x5E: { E = peek8((ushort)(IY + GetDisplacement(next8()))); break; } //LD E,(IY+s)
                            case 0x66: { H = peek8((ushort)(IY + GetDisplacement(next8()))); break; } //LD H,(IY+s)
                            case 0x6E: { L = peek8((ushort)(IY + GetDisplacement(next8()))); break; } //LD L,(IY+s)
                            //Команды помещения значения регистра в память по абсолютному адресу
                            case 0x22: { poke16(next16(), IY); break; } //LD (NN),Y
                            //Команды помещения значения 8-битного регистра в память по адресу в индексном регистре (со смещением)
                            case 0x77: { poke8((ushort)(IY + GetDisplacement(next8())), A); break; } //LD (IY+s),A
                            case 0x70: { poke8((ushort)(IY + GetDisplacement(next8())), B); break; } //LD (IY+s),B
                            case 0x71: { poke8((ushort)(IY + GetDisplacement(next8())), C); break; } //LD (IY+s),C
                            case 0x72: { poke8((ushort)(IY + GetDisplacement(next8())), D); break; } //LD (IY+s),D
                            case 0x73: { poke8((ushort)(IY + GetDisplacement(next8())), E); break; } //LD (IY+s),E
                            case 0x74: { poke8((ushort)(IY + GetDisplacement(next8())), H); break; } //LD (IY+s),H
                            case 0x75: { poke8((ushort)(IY + GetDisplacement(next8())), L); break; } //LD (IY+s),L
                            //Команды помещения непосредственного 8-битного значения в память по адресу в индексном регистре (со смещением)
                            case 0x36: { poke8((ushort)(IY + GetDisplacement(next8())), next8()); break; } //LD (IY+s),N
                            //Команды обмена значений 16-битных регистровых пар и памяти
                            case 0xE3: { tmp16 = peek16(SP); poke16(SP, IY); IY = tmp16; break; } //EX(SP),IY
                            //Команды сложения значения аккумулятора со значением 8-битного регистра
                            case 0x84: { Add_R(IYH); break; } //ADD A,YH
                            case 0x85: { Add_R(IYL); break; } //ADD A,YL
                            //Команды сложения значения аккумулятора с 8-битным значением в памяти по адресу в индексном регистре (со смещением)
                            case 0x86: { Add_R(peek8((ushort)(IY + GetDisplacement(next8())))); break; } //ADD A,YL
                            //Команды сложения значений 16-битных регистровых пар
                            case 0x09: { IY = Add_RR(IY, BC); break; } //ADD IY, BC
                            case 0x19: { IY = Add_RR(IY, DE); break; } //ADD IY, DE
                            case 0x29: { IY = Add_RR(IY, HL); break; } //ADD IY, HL
                            case 0x39: { IY = Add_RR(IY, SP); break; } //ADD IY, SP
                            //Команды инкремента значения 8-битного регистра
                            case 0x24: { IYH = Inc(IYH); break; } //INC IYH
                            case 0x2C: { IYL = Inc(IYL); break; } //INC IYL
                            //Команды инкремента 8-битного значения в памяти по адресу в индексном регистре (со смещением)
                            case 0x34: { ushort addr = (ushort)(IY + GetDisplacement(next8())); poke8(addr, Inc(peek8(addr))); break; } //INC(IY + s)
                            //Команды инкремента значения 16 - битного регистра
                            case 0x03: { IY++; break; } //INC IY
                            //Команды сложения значения аккумулятора со значением 8-битного регистра с учетом флага переноса
                            case 0x8C: { Adc_R(IYH); break; } //ADC A,IYH
                            case 0x8D: { Adc_R(IYL); break; } //ADC A,IYL
                            //Команды сложения значения аккумулятора с 8-битным значением в памяти по адресу в индексном регистре (со смещением) с учетом флага переноса
                            case 0x8E: { Adc_R((ushort)(IY + GetDisplacement(next8()))); break; } //ADC A,(IY+s)
                            //Команды вычитания значения 8-битного регистра из значения аккумулятора
                            case 0x94: { Sub_R(IYH); break; } //SUB IYH
                            case 0x95: { Sub_R(IYL); break; } //SUB IYL
                            //Команды вычитания 8-битного значения в памяти по адресу в индексном регистре (со смещением) из значения аккумулятора
                            case 0x96: { Sub_R(IY + GetDisplacement(next8())); break; } //SUB IY
                            //Команды декремента значения 8 - битного регистра
                            case 0x25: { IYH = Dec_R(IYH); break; } //DEC IYH
                            case 0x2D: { IYL = Dec_R(IYL); break; } //DEC IYL
                            //Команды декремента 8-битного значения в памяти по адресу в индексном регистре (со смещением)
                            case 0x35: { ushort addr = (ushort)(IY + GetDisplacement(next8())); poke8(addr, Dec_R(peek8(addr))); break; } //DEC(IY + s)
                            //Команды декремента значения 16-битного регистра
                            case 0x2B: { IY--; break; } //DEC IY
                            //Команды вычитания значения 8-битного регистра из значения аккумулятора с учетом флага переноса
                            case 0x9C: { Sbc_R(IYH); break; } //SBC A,IYH
                            case 0x9D: { Sbc_R(IYL); break; } //SBC A,IYL
                            //Команды вычитания 8-битного значения в памяти по адресу в индексном регистре (со смещением) из значения аккумулятора с учетом флага переноса
                            case 0x9E: { Sbc_R((ushort)(IY + GetDisplacement(next8()))); break; } //SBC A,(IY + s)
                            //Команды сравнения значения 8-битного регистра со значением аккумулятора
                            case 0xBC: { Cp_R(IYH); break; } //CP IYH
                            case 0xBD: { Cp_R(IYL); break; } //CP IYL
                            //Команды сравнения 8-битного значения в памяти по адресу в индексном регистре (со смещением) со значением аккумулятора
                            case 0xBE: { Cp_R((ushort)(IY + GetDisplacement(next8()))); break; }// CP(IY + s)
                            //Команды логического «И» над значением 8-битного регистра и значением аккумулятора
                            case 0xA4: { And_R(IYH); break; } //AND IYH
                            case 0xA5: { And_R(IYL); break; } //AND IYL
                            //Команды логического «И» над 8-битным значением в памяти по адресу в индексном регистре (со смещением) и значением аккумулятора
                            case 0xA6: { And_R((ushort)(IY + GetDisplacement(next8()))); break; }// AND(IY + s)
                            //Команды логического «ИЛИ» над значением 8-битного регистра и значением аккумулятора
                            case 0xB4: { Or_R(IYH); break; } //OR IYH
                            case 0xB5: { Or_R(IYL); break; } //OR IYL
                            //Команды логического «ИЛИ» над 8-битным значением в памяти по адресу в индексном регистре (со смещением) и значением аккумулятора
                            case 0xB6: { Or_R((ushort)(IY + GetDisplacement(next8()))); break; }// OR(IY + s)
                            //Команды логического «исключающего ИЛИ» над значением 8-битного регистра и значением аккумулятора
                            case 0xAC: { Xor_R(IYH); break; } //XOR IYH
                            case 0xAD: { Xor_R(IYL); break; } //XOR IYL
                            //Команды логического «исключающего ИЛИ» над 8-битным значением в памяти по адресу в индексном регистре (со смещением) и значением аккумулятора
                            case 0xAE: { Xor_R((ushort)(IY + GetDisplacement(next8()))); break; }// XOR(IY + s)
                            //Команды безусловного перехода по адресу в 16-битном регистре
                            case 0xE9: { PC = peek16(IY); break; } //JP (HL)
                            //Команды помещения значения 16-битной регистровой пары в стек
                            case 0xE5: { PushStack(IY); break; }  //PUSH IY
                            //Команды снятия значения 16-битной регистровой пары со стека
                            case 0xE1: { IY = PopStack(); break; } //POP IY
                        }
                        break;
                    }

                case 0xED:
                    {
                        opcode = next8();
                        switch (opcode)
                        {
                            //Команды загрузки, использующие служебный 8-битный регистр в качестве одного из операндов
                            case 0x47: { I = A; break; } //LD I,A
                            case 0x57: {
                                    A = I;
                                    F_PARITY = IFF2;
                                    F_NEG = false;
                                    F_HALF = false;
                                    F_3 = (I & (int)flags.n3) > 0;
                                    F_5 = (I & (int)flags.n5) > 0;
                                    F_SIGN = (I & (int)flags.S) > 0;
                                    F_ZERO = I == 0;
                                    break;
                                } //LD A,I
                            case 0x4F: { R = A; break; } //LD R,A
                            case 0x5F:
                                {
                                    A = R;
                                    F_PARITY = IFF2;
                                    F_NEG = false;
                                    F_HALF = false;
                                    F_3 = (R & (int)flags.n3) > 0;
                                    F_5 = (R & (int)flags.n5) > 0;
                                    F_SIGN = (I & (int)flags.S) > 0;
                                    F_ZERO = R == 0;
                                    break;
                                } //LD A, R

                            //Команды загрузки 8 - битного регистра значением в памяти по абсолютному адресу
                            case 0x4B: { BC = peek16(next16()); break; } //LD BC,(NN)
                            case 0x5B: { DE = peek16(next16()); break; } //LD DE,(NN)
                            case 0x6B: { HL = peek16(next16()); break; } //LD HL,(NN)
                            case 0x7B: { SP = peek16(next16()); break; } //LD SP,(NN)
                            //Команды помещения значения регистра в память по абсолютному адресу
                            case 0x43: { poke16(next16(), BC); break; } //LD (NN),BC
                            case 0x53: { poke16(next16(), DE); break; } //LD (NN),DE
                            case 0x63: { poke16(next16(), HL); break; } //LD (NN),HL
                            case 0x73: { poke16(next16(), SP); break; } //LD (NN),SP
                            //Команды сложения значений 16 - битных регистровых пар с учетом флага переноса
                            case 0x4A: { Adc_RR(BC); break; } //ADС HL,BC
                            case 0x5A: { Adc_RR(DE); break; } //ADС HL,DE
                            case 0x6A: { Adc_RR(HL); break; } //ADС HL,HL
                            case 0x7A: { Adc_RR(SP); break; } //ADС HL,SP
                            //Команды вычитания значений 16-битных регистровых пар с учетом флага переноса
                            case 0x42: { Sbc_RR(BC); break; } //SBC HL,BC
                            case 0x52: { Sbc_RR(DE); break; } //SBC HL,DE
                            case 0x62: { Sbc_RR(HL); break; } //SBC HL,HL
                            case 0x72: { Sbc_RR(SP); break; } //SBC HL,SP


                        }
                        break;
                    }

                        default: break;
            }
        }



    }
}
