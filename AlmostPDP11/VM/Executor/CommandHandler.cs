using System;
using static VM.MemoryManager;

namespace ConsoleApplication
{
    public class Handler
    {
        ushort[] registers = new ushort[8];

        ushort getReg(ushort addr, ushort mode) {
            return registers[addr];
        }

        void putReg(short addr, ushort mode, ushort value) {
            registers[addr] = value;
        }

        void operation(String opcode, ushort destmode, ushort sourcemode, ushort destaddr, ushort sourceaddr) {
            if (sourcemode >= 0 && destmode >= 0) {
                ushort dest = getReg(destaddr, destmode);
                ushort src = getReg(sourceaddr, sourcemode);
                
                switch (opcode) {
                    case "MOV":
                        dest = src;
                        break;
                    case "MOVB":
                        dest = src;
                        break;
                    case "CMP":
                        break;
                    case "CMPB":
                        break;
                    case "BIT":
                        break;
                    case "BITB":
                        break;
                    case "BIC":
                        dest = (ushort)(dest & ~src);
                        break;
                    case "BICB":
                        dest = (ushort)(dest & ~src);
                        break;
                    case "BIS":
                        dest = (ushort)(dest | src);
                        break;
                    case "BISB":
                        dest = (ushort)(dest | src);
                        break;
                    case "ADD":
                        dest = (ushort)(dest + src);
                        break;
                    case "SUB":
                        dest = (ushort)(dest - src);
                        break;
                    default:
                        break;
                }

                //putReg(destaddr, destmode, dest);
                //putReg(sourceaddr, sourcemode, src);
            } /*else
                if (sourcemode < 0 && sourceaddr > 0) {
                    int dest, src = getReg(sourceaddr, sourcemode);

                    if (destaddr % 2 == 0) {
                        dest = getReg(destaddr + 1, 0);
                        dest = dest << 16 + getReg(destaddr, 0);
                    } else
                        dest = getReg(destaddr, 0);

                    switch (opcode) {
                        case "MUL":
                            dest *= src;
                            break;
                        case "DIV":
                            destaddr = dest / src;
                            break;
                        case "ASH":
                            
                            break;
                        case "ASHC":
                            break;
                        case "XOR":
                            break;
                        case "SOB":
                            break;
                        default:
                            break;
                    }
                } else {
                    if (sourceaddr < 0 && destmode > 0) {
                        switch (opcode) {

                        }
                    }
                }
                */
	    }

        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
