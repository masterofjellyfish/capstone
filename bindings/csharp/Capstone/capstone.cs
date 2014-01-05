// Capstone C# bindings
// By Matt Graeber <@mattifestation>, 2013>

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Capstone
{
    public struct Instruction
    {
	    public Object Id;
	    public ulong Address;
	    public ushort Size;
        public byte[] Bytes;
	    public string Mnemonic;
	    public string Operands;
	    public Object[] Regs_read;
        public Object[] Regs_write;
        public Object[] Groups;
        public Object Arch;

        public override string ToString()
        {
            return  "0x" + this.Address.ToString("X8") + ": " +
                this.Mnemonic.PadRight(8) + this.Operands;
        }
    }

    [Serializable]
    public class CapstoneException : Exception
    {
        public CapstoneException() : base("") { }

        public CapstoneException(string message) : base(message) { }

        internal CapstoneException(ErrorCode code) : base(NativeMethods.cs_strerror(code)) { }

        protected CapstoneException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    internal static class NativeMethods
    {
        #if __MonoCs__
        [DllImport("kernel32.so", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetDllDirectory(string lpPathName);
        [DllImport("libcapstone.so", CallingConvention = CallingConvention.Cdecl)]
        internal static extern ErrorCode cs_open(Architecture arch, Mode mode, ref UIntPtr handle);
        [DllImport("libcapstone.so", CallingConvention = CallingConvention.Cdecl)]
        internal static extern UIntPtr cs_disasm_ex(UIntPtr handle, byte[] code,
                UIntPtr code_size, ulong address, UIntPtr count, ref IntPtr insn);
        [DllImport("libcapstone.so", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool cs_support(Architecture arch);
        [DllImport("libcapstone.so", CallingConvention = CallingConvention.Cdecl)]
        internal static extern string cs_strerror(ErrorCode code);
        [DllImport("libcapstone.so", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool cs_free(IntPtr insn, UIntPtr count);
        [DllImport("libcapstone.so", CallingConvention = CallingConvention.Cdecl)]
        internal static extern ErrorCode cs_close(UIntPtr handle);
        [DllImport("libcapstone.so", CallingConvention = CallingConvention.Cdecl)]
        internal static extern ErrorCode cs_errno(UIntPtr handle);
        [DllImport("libcapstone.so", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int cs_version(ref int major, ref int minor);
        [DllImport("libcapstone.so", CallingConvention = CallingConvention.Cdecl)]
        internal static extern ErrorCode cs_option(UIntPtr handle, OptionType type, OptionValue value);
        #else
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetDllDirectory(string lpPathName);
        [DllImport("libcapstone.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern ErrorCode cs_open(Architecture arch, Mode mode, ref UIntPtr handle);
        [DllImport("libcapstone.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern UIntPtr cs_disasm_ex(UIntPtr handle, byte[] code,
                UIntPtr code_size, ulong address, UIntPtr count, ref IntPtr insn);
        [DllImport("libcapstone.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool cs_support(Architecture arch);
        [DllImport("libcapstone.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern string cs_strerror(ErrorCode code);
        [DllImport("libcapstone.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool cs_free(IntPtr insn, UIntPtr count);
        [DllImport("libcapstone.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern ErrorCode cs_close(UIntPtr handle);
        [DllImport("libcapstone.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern ErrorCode cs_errno(UIntPtr handle);
        [DllImport("libcapstone.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int cs_version(ref int major, ref int minor);
        [DllImport("libcapstone.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern ErrorCode cs_option(UIntPtr handle, OptionType type, OptionValue value);
        #endif
    }

    public class Capstone : IDisposable
    {
        private bool _disposed;
        private UIntPtr _handle;
        private ErrorCode _status;
        private Architecture _arch;
        public Architecture Architecture
        {
            get { return this._arch; }
        }
        public Version Version
        {
            // This would normally be a static property but
            // I need to make sure SetDllDirectory is called
            // so that the correct 32 vs. 64-bit library is
            // loaded first.
            get
            {
                int major = 0;
                int minor = 0;
                int version;

                version = NativeMethods.cs_version(ref major, ref minor);

                major = (version & 0xFF00) >> 8;
                minor = version & 0xFF;

                return new Version(major, minor);
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct _Insn
        {
            public uint id;
            public ulong address;
            public ushort size;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] bytes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string mnemonic;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 96)]
            public string operands;
            public IntPtr detail;
        }
        [StructLayout(LayoutKind.Sequential, Size = 48)]
        internal struct _InsnDetail
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
            public byte[] regs_read;
            public byte regs_read_count;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] regs_write;
            public byte regs_write_count;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] groups;
            public byte groups_count;
        }

        public Capstone(Architecture arch, Mode mode)
        {
            #if !____MonoCs__Cs__
            // Get the path to the respective unmanaged library
            string libPath = Path.Combine(Path.GetDirectoryName(typeof(_Insn).Assembly.Location),
                (IntPtr.Size == 8 ? "x64" : "x86"));

            // Set the path of the respective unmanaged library
            NativeMethods.SetDllDirectory(libPath);
            #endif

            this._arch = arch;
            this._status = NativeMethods.cs_open(arch, mode, ref this._handle);

            if (this._status != ErrorCode.Ok)
            {
                throw new CapstoneException(this._status);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                NativeMethods.cs_close(this._handle);
                this._handle = UIntPtr.Zero;

                this._disposed = true;
            }
        }

        ~Capstone()
        {
            Dispose(false);
        }

        public void SetDetail(bool opt)
        {
            if (opt == true)
            {
                this._status = NativeMethods.cs_option(this._handle, OptionType.Detail, OptionValue.On);
            }
            else
            {
                this._status = NativeMethods.cs_option(this._handle, OptionType.Detail, OptionValue.Off);
            }

            if (this._status != ErrorCode.Ok)
            {
                throw new CapstoneException(this._status);
            }
        }

        public void SetSyntax(OptionValue opt)
        {
            if ((opt == OptionValue.SyntaxATT) || (opt == OptionValue.SyntaxIntel))
            {
                this._status = NativeMethods.cs_option(this._handle, OptionType.Syntax, opt);
            }

            if (this._status != ErrorCode.Ok)
            {
                throw new CapstoneException(this._status);
            }
        }

        public Instruction[] Disassemble(byte[] code, ulong addr, UIntPtr count)
        {
            IntPtr ptr = IntPtr.Zero;

            if (code.Length == 0) { return null; }

            ulong numInst = (ulong) NativeMethods.cs_disasm_ex(this._handle, code, (UIntPtr)(uint)code.Length, addr, count, ref ptr);

            if (numInst > 0)
            {
                // TODO: dynamically calculate size
                // TODO: fix alignment issues in Cs* structs
                ulong size = (ulong) Marshal.SizeOf(typeof(_Insn));

                Instruction[] insns = new Instruction[numInst];

                for (ulong j = 0; j < numInst; j++)
                {
                    IntPtr data = new IntPtr(ptr.ToInt64() + (Int64)(size * j));

                    _Insn _insn = (_Insn) Marshal.PtrToStructure(data, typeof(_Insn));
                    insns[j].Address = _insn.address;
                    insns[j].Size = _insn.size;
                    insns[j].Bytes = _insn.bytes;
                    insns[j].Mnemonic = _insn.mnemonic;
                    insns[j].Operands = _insn.operands;

                    if (_insn.detail != IntPtr.Zero)
                    {
                        _InsnDetail detail = (_InsnDetail) Marshal.PtrToStructure(_insn.detail, typeof(_InsnDetail));

                        if (detail.regs_read_count > 0)
                        {
                            insns[j].Regs_read = new Object[detail.regs_read_count];
                            Array.Copy(detail.regs_read, insns[j].Regs_read, detail.regs_read_count);
                        }

                        if (detail.regs_write_count > 0)
                        {
                            insns[j].Regs_write = new Object[detail.regs_write_count];
                            Array.Copy(detail.regs_write, insns[j].Regs_write, detail.regs_write_count);
                        }

                        if (detail.groups_count > 0)
                        {
                            insns[j].Groups = new Object[detail.groups_count];
                            Array.Copy(detail.groups, insns[j].Groups, detail.groups_count);
                        }

                        // Point to architecture specific structs
                        IntPtr archPtr = new IntPtr(_insn.detail.ToInt64() + (Int64)(Marshal.SizeOf(typeof(_InsnDetail))));

                        // Cast architecture-specific enums/structs. This is ugly.
                        // I'm open to suggesstions for improvement.
                        switch (_arch)
                        {
                            case Architecture.Arm:
                                insns[j].Id = (Arm.INSN)_insn.id;
                                insns[j].Arch = Marshal.PtrToStructure(archPtr, typeof(Arm.CsArm));
                                if (detail.regs_read_count > 0) { for (int k = 0; k < insns[j].Regs_read.Length; k++) { insns[j].Regs_read[k] = (Arm.REG) Convert.ToInt32(insns[j].Regs_read[k]); }; }
                                if (detail.regs_write_count > 0) { for (int k = 0; k < insns[j].Regs_write.Length; k++) { insns[j].Regs_write[k] = (Arm.REG) Convert.ToInt32(insns[j].Regs_write[k]); }; }
                                if (detail.groups_count > 0) { for (int k = 0; k < insns[j].Groups.Length; k++) { insns[j].Groups[k] = (Arm.GRP) Convert.ToInt32(insns[j].Groups[k]); }; }
                                break;
                            case Architecture.Arm64:
                                insns[j].Id = (Arm64.INSN)_insn.id;
                                insns[j].Arch = Marshal.PtrToStructure(archPtr, typeof(Arm64.CsArm64));
                                if (detail.regs_read_count > 0) { for (int k = 0; k < insns[j].Regs_read.Length; k++) { insns[j].Regs_read[k] = (Arm64.REG) Convert.ToInt32(insns[j].Regs_read[k]); }; }
                                if (detail.regs_write_count > 0) { for (int k = 0; k < insns[j].Regs_write.Length; k++) { insns[j].Regs_write[k] = (Arm64.REG) Convert.ToInt32(insns[j].Regs_write[k]); }; }
                                if (detail.groups_count > 0) { for (int k = 0; k < insns[j].Groups.Length; k++) { insns[j].Groups[k] = (Arm64.GRP) Convert.ToInt32(insns[j].Groups[k]); }; }
                                break;
                            case Architecture.Mips:
                                insns[j].Id = (Mips.INSN)_insn.id;
                                insns[j].Arch = Marshal.PtrToStructure(archPtr, typeof(Mips.CsMips));
                                if (detail.regs_read_count > 0) { for (int k = 0; k < insns[j].Regs_read.Length; k++) { insns[j].Regs_read[k] = (Mips.REG) Convert.ToInt32(insns[j].Regs_read[k]); }; }
                                if (detail.regs_write_count > 0) { for (int k = 0; k < insns[j].Regs_write.Length; k++) { insns[j].Regs_write[k] = (Mips.REG) Convert.ToInt32(insns[j].Regs_write[k]); }; }
                                if (detail.groups_count > 0) { for (int k = 0; k < insns[j].Groups.Length; k++) { insns[j].Groups[k] = (Mips.GRP) Convert.ToInt32(insns[j].Groups[k]); }; }
                                break;
                            case Architecture.X86:
                                insns[j].Id = (X86.INSN)_insn.id;
                                insns[j].Arch = Marshal.PtrToStructure(archPtr, typeof(X86.CsX86));
                                if (detail.regs_read_count > 0) { for (int k = 0; k < insns[j].Regs_read.Length; k++) { insns[j].Regs_read[k] = (X86.REG) Convert.ToInt32(insns[j].Regs_read[k]); }; }
                                if (detail.regs_write_count > 0) { for (int k = 0; k < insns[j].Regs_write.Length; k++) { insns[j].Regs_write[k] = (X86.REG) Convert.ToInt32(insns[j].Regs_write[k]); }; }
                                if (detail.groups_count > 0) { for (int k = 0; k < insns[j].Groups.Length; k++) { insns[j].Groups[k] = (X86.GRP) Convert.ToInt32(insns[j].Groups[k]); }; }
                                break;
                            case Architecture.PPC:
                                insns[j].Id = (PowerPC.INSN)_insn.id;
                                insns[j].Arch = Marshal.PtrToStructure(archPtr, typeof(PowerPC.CsPowerPC));
                                if (detail.regs_read_count > 0) { for (int k = 0; k < insns[j].Regs_read.Length; k++) { insns[j].Regs_read[k] = (PowerPC.REG) Convert.ToInt32(insns[j].Regs_read[k]); }; }
                                if (detail.regs_write_count > 0) { for (int k = 0; k < insns[j].Regs_write.Length; k++) { insns[j].Regs_write[k] = (PowerPC.REG) Convert.ToInt32(insns[j].Regs_write[k]); }; }
                                if (detail.groups_count > 0) { for (int k = 0; k < insns[j].Groups.Length; k++) { insns[j].Groups[k] = (PowerPC.GRP) Convert.ToInt32(insns[j].Groups[k]); }; }
                                break;
                        }
                    }
                }

                NativeMethods.cs_free(ptr, (UIntPtr) numInst);

                return insns;
            }
            else
            {
                this._status = NativeMethods.cs_errno(this._handle);

                if (this._status != ErrorCode.Ok)
                {
                    throw new CapstoneException(this._status);
                }

                return null;
            }
        }
    }
}