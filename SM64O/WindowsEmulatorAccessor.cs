using System.Runtime.InteropServices;
using System.Diagnostics;
using System;

namespace SM64O
{
    public class WindowsEmulatorAccessor : IEmulatorAccessor
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, uint lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(int hProcess, uint lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        const int PROCESS_WM_READ = 0x0010;

        private uint baseAddress;
        private IntPtr processHandle;
        private Process process;
        private int mainModuleAdd;

        public uint BaseAddress
        {
            get { return baseAddress; }
        }

        public int MainModuleAddress
        {
            get { return mainModuleAdd; }
        }

        private bool _attached;
        public bool Attached
        {
            get { return _attached; }
            private set { _attached = value; }
        }

        public string WindowName
        {
            get
            {
                return process.MainWindowTitle;
            }
        }

        public void Open(string processName, uint step = 1024)
        {
            process = Process.GetProcessesByName(processName)[0];

            baseAddress = ReadWritingMemory.GetBaseAddress(processName, step, 4);
            System.Windows.Forms.MessageBox.Show($"0x{baseAddress:X}");

            processHandle = OpenProcess(0x1F0FFF, true, process.Id);

            mainModuleAdd = process.MainModule.BaseAddress.ToInt32();

            Attached = true;
        }

        public int WriteMemory(int offset, byte[] buffer, int bufferLength)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException();
            int bytesWritten = 0;
            WriteProcessMemory((int)processHandle, baseAddress + (uint)offset, buffer, bufferLength, ref bytesWritten);
            return bytesWritten;
        }

        public int ReadMemory(int offset, byte[] buffer, int bufferLength)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException();
            int bytesRead = 0;
            ReadProcessMemory((int)processHandle, baseAddress + (uint)offset, buffer, bufferLength, ref bytesRead);
            return bytesRead;
        }

        public int WriteMemoryAbs(int address, byte[] buffer, int bufferLength)
        {
            int bytesWritten = 0;
            WriteProcessMemory((int)processHandle, address, buffer, bufferLength, ref bytesWritten);
            return bytesWritten;
        }

        public int ReadMemoryAbs(int address, byte[] buffer, int bufferLength)
        {
            int bytesRead = 0;
            ReadProcessMemory((int)processHandle, address, buffer, bufferLength, ref bytesRead);
            return bytesRead;
        }

        public int GetModuleBaseAddress(string module)
        {
            foreach (ProcessModule processModule in process.Modules)
            {
                if (processModule.ModuleName == module)
                    return processModule.BaseAddress.ToInt32();
            }
            return 0;
        }
    }
}