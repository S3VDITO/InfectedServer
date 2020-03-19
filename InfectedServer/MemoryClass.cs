using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace InfectedServer
{
    public class MemoryClass
    {
        public static IntPtr Zero = IntPtr.Zero;

        #region Basic Stuff
        [DllImport("kernel32.dll")]
        public static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, UInt32 size, out IntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        public static extern Int32 WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, UInt32 size, out IntPtr lpNumberOfBytesWritten);
        public IntPtr pHandel = (IntPtr)(-1);

        private byte[] Read(int Address, int Length)
        {
            byte[] Buffer = new byte[Length];
            IntPtr Zero = IntPtr.Zero;
            ReadProcessMemory(pHandel, (IntPtr)Address, Buffer, (UInt32)Buffer.Length, out Zero);
            return Buffer;
        }
        private void Write(int Address, int Value)
        {
            byte[] Buffer = BitConverter.GetBytes(Value);
            IntPtr Zero = IntPtr.Zero;
            WriteProcessMemory(pHandel, (IntPtr)Address, Buffer, (UInt32)Buffer.Length, out Zero);
        }
        #endregion

        #region Adress

        #region TeamSoundPrefix
        public static IntPtr RU_ = (IntPtr)0x22A6A665;
        public static IntPtr AF_ = (IntPtr)0x22A6A783;
        public static IntPtr IC_ = (IntPtr)0x22A6A881;

        public static IntPtr US_ = (IntPtr)0x22A6AB66;
        public static IntPtr FR_ = (IntPtr)0x22A6AC61;
        public static IntPtr UK_ = (IntPtr)0x22A6A95E;
        public static IntPtr PC_ = (IntPtr)0x22A6AA5E;

        public static Byte[] RU_byte = Encoding.ASCII.GetBytes("RU_");
        public static Byte[] AF_byte = Encoding.ASCII.GetBytes("AF_");
        public static Byte[] IC_byte = Encoding.ASCII.GetBytes("IC_");

        public static Byte[] US_byte = Encoding.ASCII.GetBytes("US_");
        public static Byte[] FR_byte = Encoding.ASCII.GetBytes("FR_");
        public static Byte[] UK_byte = Encoding.ASCII.GetBytes("UK_");
        public static Byte[] PC_byte = Encoding.ASCII.GetBytes("PC_");
        #endregion


        #region FactionIconAdress

        public static IntPtr[] AlliesFactionIconAdress = new IntPtr[]
            {
                (IntPtr)0x22A6AB3D,
                (IntPtr)0x22A6A939,
                (IntPtr)0x22A6AC3A,
                (IntPtr)0x22A6AA39
            };

        public static IntPtr[] AxisFactionIconAdress = new IntPtr[]
            {
                (IntPtr)0x22A6A63E,
                (IntPtr)0x22A6A760,
                (IntPtr)0x22A6A85E,
            };

        #endregion

        #region Others
        public static IntPtr[] TextEliminated = new IntPtr[]
        {
                (IntPtr)0x22A6AB16,
                (IntPtr)0x22A6A611,
                (IntPtr)0x22A6AA16,
                (IntPtr)0x22A6A916,
                (IntPtr)0x22A6AC15,
                (IntPtr)0x22A6A737,
                (IntPtr)0x22A6A82B
        };

        public static IntPtr[] TeamNameSpawn = new IntPtr[]
        {
                (IntPtr)0x22A6AAF4,//Delta
                (IntPtr)0x22A6A8FD,//SAS
                (IntPtr)0x22A6A9FD,//PMC
                (IntPtr)0x22A6ABFA,//GIGN
                (IntPtr)0x22A6A600,//RU
                (IntPtr)0x22A6A817,//IC
                (IntPtr)0x22A6A720 //AF
            };

        public static IntPtr[] HeadIconAdress = new IntPtr[]
        {
                (IntPtr)0x22A6A6ED,
                (IntPtr)0x22A6A7DE,
                (IntPtr)0x22A6A8D0,
                (IntPtr)0x22A6A9CD,
                (IntPtr)0x22A6AAC4,
                (IntPtr)0x22A6ABC4,
                (IntPtr)0x22A6ACBA
            };
        #endregion

        #endregion

        #region Write Functions (Integer & String)
        public void WriteInteger(int Address, int Value)
        {
            Write(Address, Value);
        }
        public void WriteString(int Address, string Text)
        {
            byte[] Buffer = new ASCIIEncoding().GetBytes(Text);
            IntPtr Zero = IntPtr.Zero;
            WriteProcessMemory(pHandel, (IntPtr)Address, Buffer, (UInt32)Buffer.Length, out Zero);
        }
        public void WriteBytes(int Address, byte[] Bytes)
        {
            IntPtr Zero = IntPtr.Zero;
            WriteProcessMemory(pHandel, (IntPtr)Address, Bytes, (uint)Bytes.Length, out Zero);
        }
        public void WriteNOP(int Address)
        {
            byte[] Buffer = new byte[] { 0x90, 0x90, 0x90, 0x90, 0x90 };
            IntPtr Zero = IntPtr.Zero;
            WriteProcessMemory(pHandel, (IntPtr)Address, Buffer, (UInt32)Buffer.Length, out Zero);
        }


        #endregion
        #region Read Functions (Integer & String)
        public int ReadInteger(int Address, int Length = 4)
        {
            return BitConverter.ToInt32(Read(Address, Length), 0);
        }
        public string ReadString(int Address, int Length = 4)
        {
            return new ASCIIEncoding().GetString(Read(Address, Length));
        }
        public byte[] ReadBytes(int Address, int Length)
        {
            return Read(Address, Length);
        }
        #endregion
    }
}
