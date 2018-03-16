using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZxDisAsm
{
    public class Z80Core
    {

        public Z80Core()
        {
            int i, j, k;
            byte p;

            for (i = 0; i < 256; i++)
            {
                j = i; p = 0;
                for (k = 0; k < 8; k++) { p ^= (byte)(j & 1); j >>= 1; }
                parity[i] = (byte)(p > 0 ? 0 : (byte)flags.P);
            }
            Interrupt = false;

            R_ = 0x00;
        }

        public bool HaltOn = false;
        public int lastOpcodeWasEI = 0;        //used for re-triggered interrupts
        public byte border;

        private enum regs
        {
            B, C,
            D, E,
            L, H,
            A, F,
            R,
            IXH, IXL,
            IYH, IYL,
            I
        };

        public enum flags
        {
            C = 0x01,
            N = 0x02,
            P = 0x04,
            n3 = 0x08,
            H = 0x10,
            n5 = 0x20,
            Z = 0x40,
            S = 0x80
        };

        private byte[] reg = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };  //B, C, D, E, L, H, A, F, R, IXH, IXL, IYH, IYL, I 
        private byte[] reg_ = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }; //B`, C`, D`, E`, L`, H`, A`, F`, R' 

        public bool IFF1;
        public bool IFF2;
        public byte IM; //0 = IM0, 1 = IM1, 2 = IM2

        public bool Interrupt; //true - произошло прерывание

        //регистры
        private ushort regSP;
        private ushort regPC;

        protected byte[] parity = new byte[256];

        protected int memPtr = 0;
        public int MemPtr
        {
            get
            {
                return memPtr;
            }
            set
            {
                memPtr = value & 0xffff;
            }
        }

        public bool IM0
        {
            get { return (IM & 0x01) > 0;  }
            set { IM = value ? (byte)0x01 : (byte)(IM & (0xff ^ 0x01)); }
        }

        public bool IM1
        {
            get { return (IM & 0x02) > 0; }
            set { IM = value ? (byte)0x02 : (byte)(IM & (0xff ^ 0x02)); }
        }

        public bool IM2
        {
            get { return (IM & 0x04) > 0; }
            set { IM = value ? (byte)0x04 : (byte)(IM & (0xff ^ 0x04)); }
        }

        public byte B
        {
            get { return reg[(int)regs.B]; }
            set { reg[(int)regs.B] = value; }
        }

        public byte C
        {
            get { return reg[(int)regs.C]; }
            set { reg[(int)regs.C] = value; }
        }

        public byte D
        {
            get { return reg[(int)regs.D]; }
            set { reg[(int)regs.D] = value; }
        }

        public byte E
        {
            get { return reg[(int)regs.E]; }
            set { reg[(int)regs.E] = value; }
        }

        public byte L
        {
            get { return reg[(int)regs.L]; }
            set { reg[(int)regs.L] = value; }
        }

        public byte H
        {
            get { return reg[(int)regs.H]; }
            set { reg[(int)regs.H] = value; }
        }

        public byte A
        {
            get { return reg[(int)regs.A]; }
            set { reg[(int)regs.A] = value; }
        }

        public byte F
        {
            get { return reg[(int)regs.F]; }
            set { reg[(int)regs.F] = value; }
        }

        public byte IXH
        {
            get { return reg[(int)regs.IXH]; }
            set { reg[(int)regs.IXH] = value; }
        }

        public byte IXL
        {
            get { return reg[(int)regs.IXL]; }
            set { reg[(int)regs.IXL] = value; }
        }

        public byte IYH
        {
            get { return reg[(int)regs.IYH]; }
            set { reg[(int)regs.IYH] = value; }
        }

        public byte IYL
        {
            get { return reg[(int)regs.IYL]; }
            set { reg[(int)regs.IYL] = value; }
        }

        public byte I
        {
            get { return reg[(int)regs.I]; }
            set { reg[(int)regs.I] = value; }
        }

        public byte R
        {
            get { return (byte)(reg_[(int)regs.R] | (reg[(int)regs.R] & 7)); }
            set { reg[(int)regs.R] = (byte)(value & 0x7F); }
        }

        public byte R_
        {
            //get { return (byte)(reg_[(int)regs.R] | (reg[(int)regs.R] & 7)); }
            set { reg_[(int)regs.R] = (byte)(value & 0x80); R = value; }
        }

        public ushort BC
        {
            get { return (ushort)((reg[(int)regs.B] << 8) + reg[(int)regs.C]); }
            set
            {
                reg[(int)regs.C] = (byte)(value & 0x00FF);
                reg[(int)regs.B] = (byte)(value >> 8);
            }
        }

        public ushort DE
        {
            get { return (ushort)((reg[(int)regs.D] << 8) + reg[(int)regs.E]); }
            set
            {
                reg[(int)regs.E] = (byte)(value & 0x00FF);
                reg[(int)regs.D] = (byte)(value >> 8);
            }
        }

        public ushort HL
        {
            get { return (ushort)((reg[(int)regs.H] << 8) + reg[(int)regs.L]); }
            set
            {
                reg[(int)regs.L] = (byte)(value & 0x00FF);
                reg[(int)regs.H] = (byte)(value >> 8);
            }
        }

        public ushort AF
        {
            get { return (ushort)((reg[(int)regs.A] << 8) + reg[(int)regs.F]); }
            set
            {
                reg[(int)regs.F] = (byte)(value & 0x00FF);
                reg[(int)regs.A] = (byte)(value >> 8);
            }
        }

        public ushort IX
        {
            get { return (ushort)((reg[(int)regs.IXH] << 8) + reg[(int)regs.IXL]); }
            set
            {
                reg[(int)regs.IXL] = (byte)(value & 0x00FF);
                reg[(int)regs.IXH] = (byte)(value >> 8);
            }
        }

        public ushort IY
        {
            get { return (ushort)((reg[(int)regs.IYH] << 8) + reg[(int)regs.IYL]); }
            set
            {
                reg[(int)regs.IYL] = (byte)(value & 0x00FF);
                reg[(int)regs.IYH] = (byte)(value >> 8);
            }
        }

        public ushort SP
        {
            get { return regSP; }
            set { regSP = value; }
        }

        public ushort PC
        {
            get { return regPC; }
            set { regPC = value; }
        }

        public byte B_
        {
            get { return reg_[(int)regs.B]; }
            set { reg_[(int)regs.B] = value; }
        }

        public byte C_
        {
            get { return reg_[(int)regs.C]; }
            set { reg_[(int)regs.C] = value; }
        }

        public byte D_
        {
            get { return reg_[(int)regs.D]; }
            set { reg_[(int)regs.D] = value; }
        }

        public byte E_
        {
            get { return reg_[(int)regs.E]; }
            set { reg_[(int)regs.E] = value; }
        }

        public byte L_
        {
            get { return reg_[(int)regs.L]; }
            set { reg_[(int)regs.L] = value; }
        }

        public byte H_
        {
            get { return reg_[(int)regs.H]; }
            set { reg_[(int)regs.H] = value; }
        }

        public byte A_
        {
            get { return reg_[(int)regs.A]; }
            set { reg_[(int)regs.A] = value; }
        }

        public byte F_
        {
            get { return reg_[(int)regs.F]; }
            set { reg_[(int)regs.F] = value; }
        }

        public ushort AF_
        {
            get { return (ushort)((reg_[(int)regs.A] << 8) + reg_[(int)regs.F]); }
            set
            {
                reg_[(int)regs.F] = (byte)(value & 0x00FF);
                reg_[(int)regs.A] = (byte)(value >> 8);
            }
        }

        public ushort BC_
        {
            get { return (ushort)((reg_[(int)regs.B] << 8) + reg_[(int)regs.C]); }
            set
            {
                reg_[(int)regs.C] = (byte)(value & 0x00FF);
                reg_[(int)regs.B] = (byte)(value >> 8);
            }
        }

        public ushort DE_
        {
            get { return (ushort)((reg_[(int)regs.D] << 8) + reg_[(int)regs.E]); }
            set
            {
                reg_[(int)regs.E] = (byte)(value & 0x00FF);
                reg_[(int)regs.D] = (byte)(value >> 8);
            }
        }

        public ushort HL_
        {
            get { return (ushort)((reg_[(int)regs.H] << 8) + reg_[(int)regs.L]); }
            set
            {
                reg_[(int)regs.L] = (byte)(value & 0x00FF);
                reg_[(int)regs.H] = (byte)(value >> 8);
            }
        }

        public bool F_CARRY
        {
            get
            {
                return (reg[(int)regs.F] & (int)flags.C) > 0 ? true : false;
            }
            set
            {
                if (value)
                    reg[(int)regs.F] = (byte)(reg[(int)regs.F] | (int)flags.C);
                else
                    reg[(int)regs.F] = (byte)(reg[(int)regs.F] & (0xFFFF ^ (int)flags.C));
            }
        }

        public bool F_NEG
        {
            get
            {
                return (reg[(int)regs.F] & (int)flags.N) > 0 ? true : false;
            }
            set
            {
                if (value)
                    reg[(int)regs.F] = (byte)(reg[(int)regs.F] | (int)flags.N);
                else
                    reg[(int)regs.F] = (byte)(reg[(int)regs.F] & (0xFFFF ^ (int)flags.N));
            }
        }

        public bool F_PARITY
        {
            get
            {
                return (reg[(int)regs.F] & (int)flags.P) > 0 ? true : false;
            }
            set
            {
                if (value)
                    reg[(int)regs.F] = (byte)(reg[(int)regs.F] | (int)flags.P);
                else
                    reg[(int)regs.F] = (byte)(reg[(int)regs.F] & (0xFFFF ^ (int)flags.P));
            }
        }

        public bool F_3
        {
            get
            {
                return (reg[(int)regs.F] & (int)flags.n3) > 0 ? true : false;
            }
            set
            {
                if (value)
                    reg[(int)regs.F] = (byte)(reg[(int)regs.F] | (int)flags.n3);
                else
                    reg[(int)regs.F] = (byte)(reg[(int)regs.F] & (0xFFFF ^ (int)flags.n3));
            }
        }

        public bool F_HALF
        {
            get
            {
                return (reg[(int)regs.F] & (int)flags.H) > 0 ? true : false;
            }
            set
            {
                if (value)
                    reg[(int)regs.F] = (byte)(reg[(int)regs.F] | (int)flags.H);
                else
                    reg[(int)regs.F] = (byte)(reg[(int)regs.F] & (0xFFFF ^ (int)flags.H));
            }
        }

        public bool F_5
        {
            get
            {
                return (reg[(int)regs.F] & (int)flags.n5) > 0 ? true : false;
            }
            set
            {
                if (value)
                    reg[(int)regs.F] = (byte)(reg[(int)regs.F] | (int)flags.n5);
                else
                    reg[(int)regs.F] = (byte)(reg[(int)regs.F] & (0xFFFF ^ (int)flags.n5));
            }
        }

        public bool F_ZERO
        {
            get
            {
                return (reg[(int)regs.F] & (int)flags.Z) > 0 ? true : false;
            }
            set
            {
                if (value)
                    reg[(int)regs.F] = (byte)(reg[(int)regs.F] | (int)flags.Z);
                else
                    reg[(int)regs.F] = (byte)(reg[(int)regs.F] & (0xFFFF ^ (int)flags.Z));
            }
        }

        public bool F_SIGN
        {
            get
            {
                return (reg[(int)regs.F] & (int)flags.S) > 0 ? true : false;
            }
            set
            {
                if (value)
                    reg[(int)regs.F] = (byte)(reg[(int)regs.F] | (int)flags.S);
                else
                    reg[(int)regs.F] = (byte)(reg[(int)regs.F] & (0xFFFF ^ (int)flags.S));
            }
        }

        public int GetDisplacement(byte val)
        {
            int res = (short)((128 ^ val) - 128);
            return res;
        }


        public void Add_R(int reg)
        {
            F_NEG = false;
            F_HALF = ((((A & 0x0f) + (reg & 0x0f)) & (int)flags.H) != 0);
            int ans = (A + reg) & 0xff;
            F_CARRY = ((A + reg) & 0x100) != 0;
            F_PARITY = ((A ^ ~reg) & (A ^ ans) & 0x80) != 0;
            F_SIGN = (ans & (int)flags.S) != 0;
            F_ZERO = ans == 0;
            F_3 = (ans & (int)flags.n3) != 0;
            F_5 = (ans & (int)flags.n5) != 0;
            A = (byte)ans;
        }
        public ushort Add_RR(int rr1, int rr2)
        {
            F_NEG = false;
            F_HALF = (((rr1 & 0xfff) + (rr2 & 0xfff)) & 0x1000) != 0;
            rr1 += rr2;
            F_CARRY = (rr1 & 0x10000) != 0;
            F_3 = ((rr1 >> 8) & (int)flags.n3) != 0;
            F_5 = ((rr1 >> 8) & (int)flags.n5) != 0;
            return (ushort)(rr1 & 0xffff);
        }

        public byte Inc(int reg)
        {
            F_PARITY = (reg == 0x7f);
            F_NEG = false;
            F_HALF = (((reg & 0x0f) + 1) & (int)flags.H) != 0;
            reg = (reg + 1) & 0xff;
            F_3 = (reg & (int)flags.n3) != 0;
            F_5 = (reg & (int)flags.n5) != 0;
            F_ZERO = reg == 0;
            F_SIGN = (reg & (int)flags.S) != 0;
            return (byte)reg;
        }

        public void Adc_R(int reg)
        {
            F_NEG = false;
            int fc = ((F & (int)flags.C) != 0 ? 1 : 0);
            F_HALF = (((A & 0x0f) + (reg & 0x0f) + fc) & (int)flags.H) != 0;
            int ans = (A + reg + fc) & 0xff;
            F_CARRY = ((A + reg + fc) & 0x100) != 0;
            F_PARITY = ((A ^ ~reg) & (A ^ ans) & 0x80) != 0;
            F_SIGN = (ans & (int)flags.S) != 0;
            F_ZERO = ans == 0;
            F_3 = (ans & (int)flags.n3) != 0;
            F_5 = (ans & (int)flags.n5) != 0;
            A = (byte)ans;
        }

        //Add with carry into HL
        public void Adc_RR(int reg)
        {
            F_NEG = false;
            int fc = ((F & (int)flags.C) != 0 ? 1 : 0);
            int ans = (HL + reg + fc) & 0xffff;
            F_CARRY = ((HL + reg + fc) & 0x10000) != 0;
            F_HALF = (((HL & 0xfff) + (reg & 0xfff) + fc) & 0x1000) != 0;
            F_PARITY = ((HL ^ ~reg) & (HL ^ ans) & 0x8000) != 0;
            F_SIGN = (ans & ((int)flags.S << 8)) != 0;
            F_ZERO = ans == 0;
            F_3 = ((ans >> 8) & (int)flags.n3) != 0;
            F_5 = ((ans >> 8) & (int)flags.n5) != 0;
            HL = (ushort)ans;
        }

        public void Sub_R(int reg)
        {
            F_NEG = true;
            int ans = (A - reg) & 0xff;
            F_CARRY = ((A - reg) & 0x100) != 0;
            F_3 = (ans & (int)flags.n3) != 0;
            F_5 = (ans & (int)flags.n5) != 0;
            F_PARITY = ((A ^ reg) & (A ^ ans) & 0x80) != 0;
            F_SIGN = (ans & (int)flags.S) != 0;
            F_HALF = (((A & 0x0f) - (reg & 0x0f)) & (int)flags.H) != 0;
            F_ZERO = ans == 0;
            F_NEG = true;
            A = (byte)ans;
        }

        public byte Dec_R(int reg)
        {
            F_NEG = true;
            F_PARITY = (reg == 0x80);
            F_HALF = (((reg & 0x0f) - 1) & (int)flags.H) != 0;
            reg = (reg - 1) & 0xff;
            F_3 = (reg & (int)flags.n3) != 0;
            F_5 = (reg & (int)flags.n5) != 0;
            F_ZERO = reg == 0;
            F_SIGN = (reg & (int)flags.S) != 0;
            return (byte)reg;
        }

        public void Sbc_R(int reg)
        {
            F_NEG = true;
            int fc = ((F & (int)flags.C) != 0 ? 1 : 0);
            int ans = (A - reg - fc) & 0xff;
            F_CARRY = ((A - reg - fc) & 0x100) != 0;
            F_PARITY = ((A ^ reg) & (A ^ ans) & 0x80) != 0;
            F_SIGN = (ans & (int)flags.S) != 0;
            F_HALF = (((A & 0x0f) - (reg & 0x0f) - fc) & (int)flags.H) != 0;
            F_ZERO = ans == 0;
            F_3 = (ans & (int)flags.n3) != 0;
            F_5 = (ans & (int)flags.n5) != 0;
            A = (byte)ans;
        }

        public void Sbc_RR(int reg)
        {
            F_NEG = true;
            int fc = ((F & (int)flags.C) != 0 ? 1 : 0);
            F_HALF = (((HL & 0xfff) - (reg & 0xfff) - fc) & 0x1000) != 0;
            int ans = (HL - reg - fc) & 0xffff;
            F_CARRY = ((HL - reg - fc) & 0x10000) != 0;
            F_PARITY = ((HL ^ reg) & (HL ^ ans) & 0x8000) != 0;
            F_SIGN = (ans & ((int)flags.S << 8)) != 0;
            F_ZERO = ans == 0;
            F_3 = ((ans >> 8) & (int)flags.n3) != 0;
            F_5 = ((ans >> 8) & (int)flags.n5) != 0;
            HL = (ushort)ans;
        }

        public void Cp_R(int reg)
        {
            F_NEG = true;
            int result = A - reg;
            int ans = result & 0xff;
            F_3 = (reg & (int)flags.n3) != 0;
            F_5 = (reg & (int)flags.n5) != 0;
            F_HALF = (((A & 0x0f) - (reg & 0x0f)) & (int)flags.H) != 0;
            F_PARITY = ((A ^ reg) & (A ^ ans) & 0x80) != 0;
            F_SIGN = (ans & (int)flags.S) != 0;
            F_ZERO = ans == 0;
            F_CARRY = (result & 0x100) != 0;
        }

        public void And_R(int reg)
        {
            F_CARRY = false;
            F_NEG = false;
            int ans = A & reg;
            F_SIGN = (ans & (int)flags.S) != 0;
            F_HALF = true;
            F_ZERO = ans == 0;
            F_PARITY = (parity[ans] & (int)flags.P) > 0;
            //F_PARITY = GetParity(ans);
            F_3 = (ans & (int)flags.n3) != 0;
            F_5 = (ans & (int)flags.n5) != 0;
            A = (byte)ans;
        }

        public void Xor_R(int reg)
        {
            F_CARRY = false;
            F_NEG = false;
            int ans = (A ^ reg) & 0xff;
            F_SIGN = (ans & (int)flags.S) != 0;
            F_HALF = false;
            F_ZERO = ans == 0;
            F_PARITY = (parity[ans] & (int)flags.P) > 0;
            //F_PARITY = GetParity(ans);
            F_3 = (ans & (int)flags.n3) != 0;
            F_5 = (ans & (int)flags.n5) != 0;
            A = (byte)ans;
        }

        public void Or_R(int reg)
        {
            F_CARRY = false;
            F_NEG = false;
            int ans = A | reg;
            F_SIGN = (ans & (int)flags.S) != 0;
            F_HALF = false;
            F_ZERO = ans == 0;
            F_PARITY = (parity[ans] & (int)flags.P) > 0;
            //F_PARITY = GetParity(ans);

            F_3 = (ans & (int)flags.n3) != 0;
            F_5 = (ans & (int)flags.n5) != 0;
            A = (byte)ans;
        }
        
        public byte Sla_R(int reg)
        {
            int msb = reg & (int)flags.S; //save the msb bit
            reg = (reg << 1) & 0xff;
            F_CARRY = msb != 0;
            F_HALF = false;
            F_NEG = false;
            //F_PARITY = GetParity(reg);

            F_PARITY = (parity[reg] & (int)flags.P) > 0;
            F_ZERO = reg == 0;
            F_SIGN = (reg & (int)flags.S) != 0;
            F_3 = (reg & (int)flags.n3) != 0;
            F_5 = (reg & (int)flags.n5) != 0;
            return (byte)reg;
        }

        public byte Sra_R(int reg)
        {
            int lsb = reg & (int)flags.C; //save the lsb bit
            reg = (reg >> 1) | (reg & (int)flags.S);
            F_CARRY = lsb != 0;
            F_HALF = false;
            F_NEG = false;
            F_PARITY = (parity[reg] & (int)flags.P) > 0;
            //F_PARITY = GetParity(reg);

            F_ZERO = reg == 0;
            F_SIGN = (reg & 0x80) != 0;
            F_3 = (reg & (int)flags.n3) != 0;
            F_5 = (reg & (int)flags.n5) != 0;
            return (byte)reg;
        }

        public byte Sli_R(int reg)
        {
            int msb = reg & (int)flags.C; //save the msb bit
            reg = reg << 1;
            reg = (reg | 0x01) & 0xff;
            F_CARRY = msb != 0;
            F_HALF = false;
            F_NEG = false;
            F_PARITY = (parity[reg] & (int)flags.P) > 0;
            //F_PARITY = GetParity(reg);
            F_ZERO = reg == 0;
            F_SIGN = (reg & (int)flags.S) != 0;
            F_3 = (reg & (int)flags.n3) != 0;
            F_5 = (reg & (int)flags.n5) != 0;
            return (byte)reg;
        }

        public byte Srl_R(int reg)
        {
            int lsb = reg & (int)flags.C; //save the lsb bit
            reg = reg >> 1;
            F_CARRY = lsb != 0;
            F_HALF = false;
            F_NEG = false;
            F_PARITY = (parity[reg] & (int)flags.P) > 0;
            //F_PARITY = GetParity(reg);
            F_ZERO = reg == 0;
            F_SIGN = (reg & (int)flags.S) != 0;
            F_3 = (reg & (int)flags.n3) != 0;
            F_5 = (reg & (int)flags.n5) != 0;
            return (byte)reg;
        }

        public byte Rlc_R(int reg)
        {
            int msb = reg & (int)flags.S;
            if (msb != 0)
            {
                reg = ((reg << 1) | 0x01) & 0xff;
            }
            else
                reg = (reg << 1) & 0xff;
            F_CARRY = msb != 0;
            F_HALF = false;
            F_NEG = false;
            F_PARITY = (parity[reg] & (int)flags.P) > 0;
            //F_PARITY = GetParity(reg);
            F_ZERO = reg == 0;
            F_SIGN = (reg & (int)flags.S) != 0;
            F_3 = (reg & (int)flags.n3) != 0;
            F_5 = (reg & (int)flags.n5) != 0;
            return (byte)reg;
        }

        public byte Rrc_R(int reg)
        {
            int lsb = reg & (int)flags.C; //save the lsb bit
            if (lsb != 0)
            {
                reg = (reg >> 1) | 0x80;
            }
            else
                reg = reg >> 1;
            F_CARRY = lsb != 0;
            F_HALF = false;
            F_NEG = false;
            F_PARITY = (parity[reg] & (int)flags.P) > 0;
            //F_PARITY = GetParity(reg);
            F_ZERO = reg == 0;
            F_SIGN = (reg & (int)flags.S) != 0;
            F_3 = (reg & (int)flags.n3) != 0;
            F_5 = (reg & (int)flags.n5) != 0;
            return (byte)reg;
        }

        public byte Rl_R(int reg)
        {
            bool rc = (reg & (int)flags.S) != 0;
            int msb = F & (int)flags.C; //save the msb bit
            if (msb != 0)
            {
                reg = ((reg << 1) | 0x01) & 0xff;
            }
            else
            {
                reg = (reg << 1) & 0xff;
            }
            F_CARRY = rc;
            F_HALF = false;
            F_NEG = false;
            F_PARITY = (parity[reg] & (int)flags.P) > 0;
            //F_PARITY = GetParity(reg);
            F_ZERO = reg == 0;
            F_SIGN = (reg & (int)flags.S) != 0;
            F_3 = (reg & (int)flags.n3) != 0;
            F_5 = (reg & (int)flags.n5) != 0;
            return (byte)reg;
        }

        public byte Rr_R(int reg)
        {
            bool rc = (reg & (int)flags.C) != 0;
            int lsb = F & (int)flags.C; //save the lsb bit
            if (lsb != 0)
            {
                reg = (reg >> 1) | 0x80;
            }
            else
                reg = reg >> 1;
            F_CARRY = rc;
            F_HALF = false;
            F_NEG = false;
            F_PARITY = (parity[reg] & (int)flags.P) > 0;
            //F_PARITY = GetParity(reg);
            F_ZERO = reg == 0;
            F_SIGN = (reg & (int)flags.S) != 0;
            F_3 = (reg & (int)flags.n3) != 0;
            F_5 = (reg & (int)flags.n5) != 0;
            return (byte)reg;
        }

        public void Rlca()
        {
            bool ac = (A & (int)flags.S) != 0; //save the msb bit
            if (ac) {
                A = (byte)(((A << 1) | (int)flags.C) & 0xff);
            } else {
                A = (byte)((A << 1) & 0xff);
            }

            F_3 = (A & (int)flags.n3) != 0;
            F_5 = (A & (int)flags.n5) != 0;
            F_CARRY = ac;
            F_HALF = false;
            F_NEG = false;
        }

        public void Rrca()
        {

            bool ac = (A & (int)flags.C) != 0; //save the lsb bit
            if (ac) {
                A = (byte)((A >> 1) | (int)flags.S);
            } else {
                A = (byte)(A >> 1);
            }
            F_3 = (A & (int)flags.n3) != 0;
            F_5 = (A & (int)flags.n5) != 0;
            F_CARRY = ac;
            F_HALF = false;
            F_NEG = false;
        }

        public void Rla()
        {
            bool ac = ((A & (int)flags.S) != 0);
            int msb = F & (int)flags.C;

            if (msb != 0) {
                A = (byte)(((A << 1) | (int)flags.C));
            } else {
                A = (byte)((A << 1));
            }
            F_3 = (A & (int)flags.n3) != 0;
            F_5 = (A & (int)flags.n5) != 0;
            F_CARRY = ac;
            F_HALF = false;
            F_NEG = false;
        }

        public void Rra()
        {
            bool ac = (A & (int)flags.C) != 0; //save the lsb bit
            int lsb = F & (int)flags.C;

            if (lsb != 0) {
                A = (byte)((A >> 1) | (int)flags.S);
            } else {
                A = (byte)(A >> 1);
            }
            F_3 = (A & (int)flags.n3) != 0;
            F_5 = (A & (int)flags.n5) != 0;
            F_CARRY = ac;
            F_HALF = false;
            F_NEG = false;
        }

        public void Bit_R(int b, int reg)
        {
            bool bitset = ((reg & (1 << b)) != 0);  //true if bit is set
            F_ZERO = !bitset;                       //true if bit is not set, false if bit is set
            F_PARITY = !bitset;                     //copy of Z
            F_NEG = false;
            F_HALF = true;
            F_SIGN = (b == 7) ? bitset : false;
            F_3 = (reg & (int)flags.n3) != 0;
            F_5 = (reg & (int)flags.n5) != 0;
            //F = (byte)((F & (int)flags.C) | (int)flags.H | (reg & ((int)flags.n3 | (int)flags.n5)));
            //if (!((reg & (0x01 << (b))) > 0)) F |= (int)flags.P | (int)flags.Z;
            //if ((b == 7) && ((reg & 0x80) > 0)) F |= (int)flags.S;
        }

        //Reset bit operation (RES b, r)
        public byte Res_R(int b, int reg)
        {
            reg = reg & ~(1 << b);
            return (byte)reg;
        }

        //Set bit operation (SET b, r)
        public byte Set_R(int b, int reg)
        {
            reg = reg | (1 << b);
            return (byte)reg;
        }

        public ushort port = 0x00;
        public byte key = 0x00;

        public void DAA()
        {
            int ans = A;
            int incr = 0;
            bool carry = (F & (int)flags.C) != 0;

            if (((F & (int)flags.H) != 0) || ((ans & 0x0f) > 0x09))
            {
                incr |= 0x06;
            }

            if (carry || (ans > 0x9f) || ((ans > 0x8f) && ((ans & 0x0f) > 0x09)))
            {
                incr |= 0x60;
            }

            if (ans > 0x99)
            {
                carry = true;
            }

            if ((F & (int)flags.N) != 0)
            {
                Sub_R(incr);
            }
            else
            {
                Add_R(incr);
            }

            ans = A;
            F_CARRY = carry;
            F_PARITY  = (parity[ans] & (int)flags.P) > 0;
            //F_PARITY = GetParity(ans);

        }

    }

}
