/* Capstone Disassembler Engine - C# Binding */
/* By Matt Graeber <@mattifestation>, 2014> */

using System;
using System.Text;
using System.Runtime.InteropServices;
using Capstone;

public class TestPowerPC
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
        Capstone.PowerPC.CsPowerPC arch = (Capstone.PowerPC.CsPowerPC) insn.Arch;

        int opcount = arch.Operands.Length;

        if (opcount > 0)
        {
            Console.WriteLine("\tOpCount: " + opcount);

            for (int i = 0; i < opcount; i++)
            {
                Console.WriteLine("\t\tOperand[" + i + "]: " + arch.Operands[i].Type);
                switch (arch.Operands[i].Type)
                {
                    case Capstone.PowerPC.OP.REG:
                        Console.WriteLine("\t\t\tRegister: " + arch.Operands[i].Value.Reg);
                        break;
                    case Capstone.PowerPC.OP.IMM:
                        Console.WriteLine("\t\t\tImmediate: 0x" +
                            arch.Operands[i].Value.Imm.ToString("X"));
                        break;
                    case Capstone.PowerPC.OP.MEM:
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

        if (arch.Bc != 0) { Console.WriteLine("\tBranch code: " + arch.Bc); }
        if (arch.Bh != 0) { Console.WriteLine("\tBranch hint: " + arch.Bh); }
        if (arch.UpdateCR0) { Console.WriteLine("\tUpdate-CR0: True"); }
    }

    public static void Main()
    {
        byte[] PowerPCCode = new byte[] { 0x80, 0x20, 0x00, 0x00, 0x80, 0x3f, 0x00, 0x00, 0x10, 0x43, 0x23, 0x0e, 0xd0, 0x44, 0x00, 0x80, 0x4c, 0x43, 0x22, 0x02, 0x2d, 0x03, 0x00, 0x80, 0x7c, 0x43, 0x20, 0x14, 0x7c, 0x43, 0x20, 0x93, 0x4f, 0x20, 0x00, 0x21, 0x4c, 0xc8, 0x00, 0x21 };

        uint address = 0x1000;
        UIntPtr insnCount = UIntPtr.Zero;

        platform[] platforms = {
			new platform(
					Architecture.PPC,
					Mode.BigEndian,
                    OptionValue.SyntaxDefault,
					PowerPCCode,
                    "PPC-64"
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