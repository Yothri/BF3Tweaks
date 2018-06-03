using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BF3Tweaks
{
    public class TMem : IDisposable
    {
        private static TMem instance;
        public static TMem Instance
        {
            get
            {
                return instance == null ? (instance = new TMem()) : instance;
            }
        }

        public IntPtr ProcessHandle { get; private set; }

        public bool TryOpen(string processName, WinApi.ProcessAccessFlags desiredAccess = WinApi.ProcessAccessFlags.All)
        {
            var processes = Process.GetProcessesByName(processName);
            if (processes.Length == 0)
                return false;

            try
            {
                ProcessHandle = WinApi.OpenProcess(processes[0], desiredAccess);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public bool TryClose()
        {
            if (ProcessHandle == IntPtr.Zero)
                return false;

            return WinApi.CloseHandle(ProcessHandle);
        }

        public T Read<T>(in IntPtr address) where T : new()
        {
            var obj = new T();
            var size = Marshal.SizeOf(obj);
            var buf = new byte[size];

            if (!WinApi.ReadProcessMemory(ProcessHandle, address, buf, size, out var bRead))
                return default;

            var handle = GCHandle.Alloc(buf, GCHandleType.Pinned);

            obj = Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());

            handle.Free();

            return obj;
        }

        public void Write<T>(in IntPtr address, T value) where T : new()
        {
            var size = Marshal.SizeOf<T>();
            var buffer = new byte[size];

            var ptr = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr(value, ptr, true);
            Marshal.Copy(ptr, buffer, 0, size);
            Marshal.FreeHGlobal(ptr);

            WinApi.WriteProcessMemory(ProcessHandle, address, buffer, buffer.Length, out var bytesWritten);
        }

        public void Dispose()
        {
            TryClose();
        }
    }
}