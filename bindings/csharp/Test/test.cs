/* Capstone Disassembler Engine */
/* By Nguyen Anh Quynh <aquynh@gmail.com>, 2013> */

using System;
using System.Runtime.InteropServices;
using Capstone;

public class Test
{
    struct platform
    {
        public Architecture arch;
        public Mode mode;
        public OptionValue syntax;
        public byte[] code;
        public string comment;

        public platform(Architecture a, Mode m, OptionValue x, byte[] c, string s)
        {
            arch = a;
            mode = m;
            syntax = x;
            code = c;
            comment = s;
        }
    }

    public static void Main() {
		platform[] platforms = {
			new platform(
					Architecture.X86,
					Mode.Mode16,
                    OptionValue.SyntaxIntel,
					new byte[] { 0x8d, 0x4c, 0x32, 0x08, 0x01, 0xd8, 0x81, 0xc6, 0x34, 0x12, 0x00, 0x00 },
					"X86 16bit (Intel syntax)"
			),
			new platform(
					Architecture.X86,
					Mode.Mode32,
                    OptionValue.SyntaxATT,
					new byte[] { 0x8d, 0x4c, 0x32, 0x08, 0x01, 0xd8, 0x81, 0xc6, 0x34, 0x12, 0x00, 0x00 },
					"X86 32bit (ATT syntax)"
					),
			new platform(
					Architecture.X86,
					Mode.Mode32,
                    OptionValue.SyntaxIntel,
					new byte[] { 0x8d, 0x4c, 0x32, 0x08, 0x01, 0xd8, 0x81, 0xc6, 0x34, 0x12, 0x00, 0x00 },
					"X86 32bit (Intel syntax)"
					),
			new platform(
					Architecture.X86,
					Mode.Mode64,
                    OptionValue.SyntaxIntel,
					new byte[] { 0x55, 0x48, 0x8b, 0x05, 0xb8, 0x13, 0x00, 0x00 },
					"X86 64 (Intel syntax)"
					),
			new platform(
					Architecture.Arm,
					Mode.Arm,
                    OptionValue.SyntaxDefault,
					new byte[] { 0xED, 0xFF, 0xFF, 0xEB, 0x04, 0xe0, 0x2d, 0xe5, 0x00, 0x00, 0x00, 0x00, 0xe0, 0x83, 0x22, 0xe5, 0xf1, 0x02, 0x03, 0x0e, 0x00, 0x00, 0xa0, 0xe3, 0x02, 0x30, 0xc1, 0xe7, 0x00, 0x00, 0x53, 0xe3 },
					"ARM"
					),
			new platform(
					Architecture.Arm,
					Mode.Thumb,
                    OptionValue.SyntaxDefault,
					new byte[] { 0x4f, 0xf0, 0x00, 0x01, 0xbd, 0xe8, 0x00, 0x88, 0xd1, 0xe8, 0x00, 0xf0 },
					"THUMB-2"
					),
			new platform(
					Architecture.Arm,
					Mode.Arm,
                    OptionValue.SyntaxDefault,
					new byte[] { 0x10, 0xf1, 0x10, 0xe7, 0x11, 0xf2, 0x31, 0xe7, 0xdc, 0xa1, 0x2e, 0xf3, 0xe8, 0x4e, 0x62, 0xf3 },
					"ARM: Cortex-A15 + NEON"
					),
			new platform(
					Architecture.Arm,
					Mode.Thumb,
                    OptionValue.SyntaxDefault,
					new byte[] { 0x70, 0x47, 0xeb, 0x46, 0x83, 0xb0, 0xc9, 0x68 },
					"THUMB"
					),
            new platform(
					Architecture.Arm64,
					Mode.Arm,
                    OptionValue.SyntaxDefault,
					new byte [] { 0x21, 0x7c, 0x02, 0x9b, 0x21, 0x7c, 0x00, 0x53, 0x00, 0x40, 0x21, 0x4b, 0xe1, 0x0b, 0x40, 0xb9 },
					"ARM-64"
					),
			new platform(
					Architecture.Mips,
					Mode.Mode32 | Mode.BigEndian,
                    OptionValue.SyntaxDefault,
					new byte[] { 0x0C, 0x10, 0x00, 0x97, 0x00, 0x00, 0x00, 0x00, 0x24, 0x02, 0x00, 0x0c, 0x8f, 0xa2, 0x00, 0x00, 0x34, 0x21, 0x34, 0x56 },
					"MIPS-32 (Big-endian)"
					),
			new platform(
					Architecture.Mips,
					Mode.Mode64 | Mode.LittleEndian,
                    OptionValue.SyntaxDefault,
					new byte[] { 0x56, 0x34, 0x21, 0x34, 0xc2, 0x17, 0x01, 0x00 },
					"MIPS-64-EL (Little-endian)"
					),
            new platform(
					Architecture.PPC,
					Mode.BigEndian,
                    OptionValue.SyntaxDefault,
					new byte[] { 0x80, 0x20, 0x00, 0x00, 0x80, 0x3f, 0x00, 0x00, 0x10, 0x43, 0x23, 0x0e, 0xd0, 0x44, 0x00, 0x80, 0x4c, 0x43, 0x22, 0x02, 0x2d, 0x03, 0x00, 0x80, 0x7c, 0x43, 0x20, 0x14, 0x7c, 0x43, 0x20, 0x93, 0x4f, 0x20, 0x00, 0x21, 0x4c, 0xc8, 0x00, 0x21 },
					"PPC-64"
					)
		};

		for (int j = 0; j < platforms.Length; j++) {
            System.Console.WriteLine();
			System.Console.WriteLine("************");
			System.Console.WriteLine("Platform: {0}", platforms[j].comment);
			System.Console.WriteLine();

			Capstone.Capstone cs = new Capstone.Capstone(platforms[j].arch, platforms[j].mode);
            cs.SetSyntax(platforms[j].syntax);
			Instruction[] insns = cs.Disassemble(platforms[j].code, (uint) 0x1000, UIntPtr.Zero);
			for(int i = 0; i < insns.Length; i++) { Console.WriteLine(insns[i]); }
		}

        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
	}
}