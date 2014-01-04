/* Capstone Disassembler Engine - C# Binding */
/* By Matt Graeber <@mattifestation>, 2014> */

using System;
using System.Text;
using System.Runtime.InteropServices;
using Capstone;

public class TestArm
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
        Capstone.Arm.CsArm arch = (Capstone.Arm.CsArm)insn.Arch;

        byte opcount = arch.OpCount;

        if (opcount > 0)
        {
            Console.WriteLine("\tOpCount: " + opcount);

            for (int i = 0; i < opcount; i++)
            {
                Console.WriteLine("\t\tOperand[" + i + "]: " + arch.Operands[i].Type.ToString());
                switch (arch.Operands[i].Type)
                {
                    case Capstone.Arm.OP.REG:
                        Console.WriteLine("\t\t\tRegister: " + arch.Operands[i].Value.Reg);
                        break;
                    case Capstone.Arm.OP.IMM:
                        Console.WriteLine("\t\t\tImmediate: 0x" +
                            arch.Operands[i].Value.Imm.ToString("X"));
                        break;
                    case Capstone.Arm.OP.PIMM:
                        Console.WriteLine("\t\t\tP-Immediate: " +
                            arch.Operands[i].Value.Imm);
                        break;
                    case Capstone.Arm.OP.CIMM:
                        Console.WriteLine("\t\t\tC-Immediate: " +
                            arch.Operands[i].Value.Imm);
                        break;
                    case Capstone.Arm.OP.FP:
                        Console.WriteLine("\t\t\tFloating Point: " + arch.Operands[i].Value.Fp);
                        break;
                    case Capstone.Arm.OP.MEM:
                        if (arch.Operands[i].Value.Mem.Base != 0)
                        {
                            Console.WriteLine("\t\t\tOperand[" + i + "].Mem.Base: " +
                                arch.Operands[i].Value.Mem.Base);
                        }
                        if (arch.Operands[i].Value.Mem.Index != 0)
                        {
                            Console.WriteLine("\t\t\tOperand[" + i + "].Mem.Index: " +
                                arch.Operands[i].Value.Mem.Index);
                        }
                        if (arch.Operands[i].Value.Mem.Scale != 1)
                        {
                            Console.WriteLine("\t\t\tOperand[" + i + "].Mem.Scale: " +
                                arch.Operands[i].Value.Mem.Scale);
                        }
                        if (arch.Operands[i].Value.Mem.Disp != 0)
                        {
                            Console.WriteLine("\t\t\tOperand[" + i + "].Mem.Disp: 0x" +
                                arch.Operands[i].Value.Mem.Disp.ToString("X"));
                        }
                        break;
                }

                if ((arch.Operands[i].Shift.Type != Capstone.Arm.SFT.INVALID) &&
                    arch.Operands[i].Shift.Value != 0)
                {
                    Console.WriteLine("\t\tShift Type: " + arch.Operands[i].Shift.Type +
                        ", Value: " + arch.Operands[i].Shift.Value);
                }
            }
        }

        if (arch.UpdateFlags) { Console.WriteLine("\tUpdate-flags: True"); }
        if (arch.Writeback) { Console.WriteLine("\tWrite-back: True"); }
        if ((arch.Cc != Capstone.Arm.CC.AL) && (arch.Cc != Capstone.Arm.CC.INVALID))
        {
            Console.WriteLine("\tCode condition: " + arch.Cc);
        }
    }

    public static void Main()
    {
        byte[] armCode = new byte[] { 0xED, 0xFF, 0xFF, 0xEB, 0x04, 0xe0, 0x2d, 0xe5, 0x00, 0x00, 0x00, 0x00, 0xe0, 0x83, 0x22, 0xe5, 0xf1, 0x02, 0x03, 0x0e, 0x00, 0x00, 0xa0, 0xe3, 0x02, 0x30, 0xc1, 0xe7, 0x00, 0x00, 0x53, 0xe3 };
        byte[] armCode2 = new byte[] { 0xd1, 0xe8, 0x00, 0xf0, 0xf0, 0x24, 0x04, 0x07, 0x1f, 0x3c, 0xf2, 0xc0, 0x00, 0x00, 0x4f, 0xf0, 0x00, 0x01, 0x46, 0x6c };
        byte[] thumbCode = new byte[] { 0x70, 0x47, 0xeb, 0x46, 0x83, 0xb0, 0xc9, 0x68, 0x1f, 0xb1 };
        byte[] thumbCode2 = new byte[] { 0x4f, 0xf0, 0x00, 0x01, 0xbd, 0xe8, 0x00, 0x88, 0xd1, 0xe8, 0x00, 0xf0 };

        uint address = 0x1000;
        UIntPtr insnCount = UIntPtr.Zero;

        platform[] platforms = {
			new platform(
					Architecture.Arm,
					Mode.Arm,
                    OptionValue.Off,
					armCode,
                    "ARM"
			),
			new platform(
					Architecture.Arm,
					Mode.Thumb,
                    OptionValue.Off,
					thumbCode,
                    "Thumb"
					),
			new platform(
					Architecture.Arm,
					Mode.Thumb,
                    OptionValue.Off,
					armCode2,
                    "Thumb-mixed"
					),
            new platform(
					Architecture.Arm,
					Mode.Thumb,
                    OptionValue.Off,
					thumbCode2,
                    "Thumb-2"
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