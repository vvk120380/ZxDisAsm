using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZxDisAsm
{
    class Z80Core
    {
        private enum regs
        {
            B, C,
            D, E,
            L, H,
            A, F,
            IXH, IXL,
            IYH, IYL,
            I,
            R
        };

        public enum flags
        {
            C  = 0x01,
            N  = 0x02,
            P  = 0x04,
            n3 = 0x08,
            H  = 0x10,
            n5 = 0x20,
            Z  = 0x40,
            S  = 0x80
        };

        private byte[] reg = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };  //B, C, D, E, L, H, A, F, IXH, IXL, IYH, IYL, I, R 
        private byte[] reg_ = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}; //B`, C`, D`, E`, L`, H`, A`, F` 


        public bool IFF1;
        public bool IFF2;
        public byte IM;

        //регистры
        private ushort regSP;
        private ushort regPC;

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
            get { return reg[(int)regs.R]; }
            set { reg[(int)regs.R] = value; }
        }

        public ushort BC
        {
            get { return (ushort)((reg[(int)regs.B] << 8) + reg[(int)regs.C]); }
            set {
                reg[(int)regs.C] = (byte)(value & 0x00FF);
                reg[(int)regs.B] = (byte)(value >> 8);
            }
        }

        public ushort DE
        {
            get { return (ushort)((reg[(int)regs.D] << 8) + reg[(int)regs.E]); }
            set
            {
                reg[(int)regs.D] = (byte)(value & 0x00FF);
                reg[(int)regs.E] = (byte)(value >> 8);
            }
        }

        public ushort HL
        {
            get { return (ushort)((reg[(int)regs.H] << 8) + reg[(int)regs.L]); }
            set
            {
                reg[(int)regs.H] = (byte)(value & 0x00FF);
                reg[(int)regs.L] = (byte)(value >> 8);
            }
        }

        public ushort AF
        {
            get { return (ushort)((reg[(int)regs.A] << 8) + reg[(int)regs.F]); }
            set
            {
                reg[(int)regs.A] = (byte)(value & 0x00FF);
                reg[(int)regs.F] = (byte)(value >> 8);
            }
        }

        public ushort IX
        {
            get { return (ushort)((reg[(int)regs.IXH] << 8) + reg[(int)regs.IXL]); }
            set
            {
                reg[(int)regs.IXH] = (byte)(value & 0x00FF);
                reg[(int)regs.IXL] = (byte)(value >> 8);
            }
        }

        public ushort IY
        {
            get { return (ushort)((reg[(int)regs.IYH] << 8) + reg[(int)regs.IYL]); }
            set
            {
                reg[(int)regs.IYH] = (byte)(value & 0x00FF);
                reg[(int)regs.IYL] = (byte)(value >> 8);
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

    }
}
