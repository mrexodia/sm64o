namespace SM64O
{
    public interface IEmulatorAccessor
    {
        bool Attached { get; }
        uint BaseAddress { get; }
        int MainModuleAddress { get; }
        string WindowName { get; }
        void Open(string processName, uint step = 1024);
        int WriteMemory(int offset, byte[] buffer, int bufferLength);
        int ReadMemory(int offset, byte[] buffer, int bufferLength);

        int WriteMemoryAbs(int address, byte[] buffer, int bufferLength);
        int ReadMemoryAbs(int address, byte[] buffer, int bufferLength);
        int GetModuleBaseAddress(string module);
    }
}