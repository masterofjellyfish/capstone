/* Capstone Disassembler Engine - C# Binding */
/* By Matt Graeber <@mattifestation>, 2014> */

using System;
using System.Text;
using System.Runtime.InteropServices;
using Capstone;

public class TestMips
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

    internal static string PrintBytes(byte[] bytes)
    {
        string[] hexBytes = new string[bytes.Length];
        for (int i = 0; i < bytes.Length; i++)
        {
            hexBytes[i] = "0x" + bytes[i].ToString("X2");
        }

        return string.Join(", ", hexBytes);
    }

    internal static void PrintInsnDetail(Mode mode, Instruction insn)
    {
        Capstone.Mips.CsMips arch = (Capstone.Mips.CsMips)insn.Arch;

        byte opcount = arch.OpCount;

        if (opcount > 0)
        {
            Console.WriteLine("\tOpCount: " + opcount);

            for (int i = 0; i < opcount; i++)
            {
                Console.WriteLine("\t\tOperand[" + i + "]: " + arch.Operands[i].Type.ToString());
                switch (arch.Operands[i].Type)
                {
                    case Capstone.Mips.OP.REG:
                        Console.WriteLine("\t\t\tRegister: " + arch.Operands[i].Value.Reg);
                        break;
                    case Capstone.Mips.OP.IMM:
                        Console.WriteLine("\t\t\tImmediate: 0x" +
                            arch.Operands[i].Value.Imm.ToString("X"));
                        break;
                    case Capstone.Mips.OP.MEM:
                        if (arch.Operands[i].Value.Mem.Base != 0)
                        {
                            Console.WriteLine("\t\t\tOperand[" + i + "].Mem.Base: " +
                                arch.Operands[i].Value.Mem.Base);
                        }
                        if (arch.Operands[i].Value.Mem.Disp != 0)
                        {
                            Console.WriteLine("\t\t\tOperand[" + i + "].Mem.Disp: 0x" +
                                arch.Operands[i].Value.Mem.Disp.ToString("X"));
                        }
                        break;
                }
            }
        }
    }

    public static void Main()
    {
        byte[] mipsCode = new byte[] { 0x0C, 0x10, 0x00, 0x97, 0x00, 0x00, 0x00, 0x00, 0x24, 0x02, 0x00, 0x0c, 0x8f, 0xa2, 0x00, 0x00, 0x34, 0x21, 0x34, 0x56 };
        byte[] mipsCode2 = new byte[] { 0x56, 0x34, 0x21, 0x34, 0xc2, 0x17, 0x01, 0x00 };

        uint address = 0x1000;
        UIntPtr insnCount = UIntPtr.Zero;

        platform[] platforms = {
			new platform(
					Architecture.Mips,
					Mode.Mode32 | Mode.BigEndian,
                    OptionValue.SyntaxDefault,
					mipsCode,
                    "MIPS-32 (Big-endian)"
			),
			new platform(
					Architecture.Mips,
					Mode.Mode64 | Mode.LittleEndian,
                    OptionValue.SyntaxDefault,
					mipsCode2,
                    "MIPS-64-EL (Little-endian)"
					)
		};

        for (int j = 0; j < platforms.Length; j++)
        {
            System.Console.WriteLine();
            System.Console.WriteLine("************");
            System.Console.WriteLine("Platform: {0}", platforms[j].comment);
            System.Console.WriteLine();

            Capstone.Capstone cs = new Capstone.Capstone(platforms[j].arch, platforms[j].mode);
            cs.SetSyntax(platforms[j].syntax);
            cs.SetDetail(true);
            Instruction[] insns = cs.Disassemble(platforms[j].code, address, insnCount);
            for (int i = 0; i < insns.Length; i++)
            {
                Console.WriteLine(insns[i]);
                PrintInsnDetail(platforms[j].mode, insns[i]);
                Console.WriteLine();
            }
        }

        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}