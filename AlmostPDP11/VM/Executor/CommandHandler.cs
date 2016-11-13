using System;
using AlmostPDP11.VM.Decoder;
using VM;

namespace AlmostPDP11.VM.Executor
{
    public class ComandHandler
    {
        ushort[] registers = new ushort[8];

        private MemoryManager _memoryManager;

        public ComandHandler(MemoryManager mm)
        {
            this._memoryManager = mm;
        }

        public void Operation(Command command) {
            if (command.MnemonicType == MnemonicType.DoubleOperand)
            {
                string destaddr = "R" + command.Operands[Decoder.Decoder.DEST];
                string sourceaddr = "R" + command.Operands[Decoder.Decoder.SOURCE];

                if (command.Operands[Decoder.Decoder.DEST] == 6) {
                    destaddr = "SP";
                }
                if (command.Operands[Decoder.Decoder.DEST] == 7) {
                    destaddr = "PC";
                }
                if (command.Operands[Decoder.Decoder.SOURCE] == 6) {
                    sourceaddr = "SP";
                }
                if (command.Operands[Decoder.Decoder.SOURCE] == 7) {
                    sourceaddr = "PC";
                }

                ushort dest = _memoryManager.GetRegister(destaddr);
                ushort src = _memoryManager.GetRegister(sourceaddr);
                
                switch (command.Mnemonic) {
                    case Mnemonic.MOV:
                        dest = src;
                        if ((short)(src) < 0)
                            _memoryManager.SetStatusFlag("N", true);
                        else
                            _memoryManager.SetStatusFlag("N", false);
                        if ((short)(src) == 0)
                            _memoryManager.SetStatusFlag("Z", true);
                        else
                            _memoryManager.SetStatusFlag("Z", false);
                        break;
                    case Mnemonic.MOVB:
                        dest = src;
                        break;
                    case Mnemonic.CMP:
                        if (dest - src < 0)
                            _memoryManager.SetStatusFlag("N", true);
                        else
                            _memoryManager.SetStatusFlag("N", false);
                        if (dest - src == 0)
                            _memoryManager.SetStatusFlag("Z", true);
                        else
                            _memoryManager.SetStatusFlag("Z", false);
                        break;
                    case Mnemonic.CMPB:
                        if (dest - src < 0)
                            _memoryManager.SetStatusFlag("N", true);
                        else
                            _memoryManager.SetStatusFlag("N", false);
                        if (dest - src == 0)
                            _memoryManager.SetStatusFlag("Z", true);
                        else
                            _memoryManager.SetStatusFlag("Z", false);
                        break;
                    case Mnemonic.BIT:
                        break;
                    case Mnemonic.BITB:
                        break;
                    case Mnemonic.BIC:
                        dest = (ushort)(dest & ~src);
                        break;
                    case Mnemonic.BICB:
                        dest = (ushort)(dest & ~src);
                        break;
                    case Mnemonic.BIS:
                        dest = (ushort)(dest | src);
                        break;
                    case Mnemonic.BISB:
                        dest = (ushort)(dest | src);
                        break;
                    case Mnemonic.ADD:
                        if ((ushort)(dest + src) < dest + src)
                            _memoryManager.SetStatusFlag("C", true);
                        else
                            _memoryManager.SetStatusFlag("C", false);
                        if ((short)(dest + src) < 0)
                            _memoryManager.SetStatusFlag("N", true);
                        else
                            _memoryManager.SetStatusFlag("N", false);

                        break;
                    case Mnemonic.SUB:
                        dest = (ushort)(dest - src);
                        break;
                    default:
                        break;
                }

                _memoryManager.SetRegister(destaddr, dest);
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
    }
}
