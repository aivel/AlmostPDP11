using System;
using AlmostPDP11.VM.Decoder;
using VM;

namespace AlmostPDP11.VM.Executor
{
    public class ComandHandler
    {
        private MemoryManager _memoryManager;

        public ComandHandler(MemoryManager mm)
        {
            this._memoryManager = mm;
        }

        public void Operation(Command command) {
            if (command.MnemonicType == MnemonicType.DoubleOperand)
            {
                string destaddr = "R" + command.Operands[DecoderConsts.DEST];
                string sourceaddr = "R" + command.Operands[DecoderConsts.SOURCE];

                if (command.Operands[DecoderConsts.DEST] == 6) {
                    destaddr = "SP";
                }
                if (command.Operands[DecoderConsts.DEST] == 7) {
                    destaddr = "PC";
                }
                if (command.Operands[DecoderConsts.SOURCE] == 6) {
                    sourceaddr = "SP";
                }
                if (command.Operands[DecoderConsts.SOURCE] == 7) {
                    sourceaddr = "PC";
                }

                ushort dest, destReg = _memoryManager.GetRegister(destaddr);
                ushort src, srcReg = _memoryManager.GetRegister(sourceaddr);


                if (command.Operands[DecoderConsts.SOURCE] == 7)
                {
                    switch (command.Operands[DecoderConsts.SOURCE_MODE])
                    {
                        case 2:
                            src = (ushort) command.Operands[DecoderConsts.VALUE];
                            break;
                        default:
                            src = srcReg;
                            break;
                    }
                }
                else
                {
                    if (command.Operands[DecoderConsts.SOURCE] == 6)
                    {
                        switch (command.Operands[DecoderConsts.SOURCE_MODE])
                        {
                            case 1:
                                src = srcReg;
                                break;
                            default:
                                src = srcReg;
                                break;
                        }
                    }
                    else
                    {
                        switch (command.Operands[DecoderConsts.SOURCE_MODE])
                        {
                            case 0:
                                src = srcReg;
                                break;
                            case 1:
                                byte[] word;
                                word = _memoryManager.GetMemory(srcReg, 2);
                                src = (ushort) (word[0] << 8);
                                src += word[1];
                                break;
                            default:
                                src = srcReg;
                                break;
                        }
                    }
                }

                switch (command.Operands[DecoderConsts.DEST_MODE]) {
                    case 0:
                        dest = destReg;
                        break;
                    case 1:
                        byte[] word;
                        word = _memoryManager.GetMemory(destReg, 2);
                        dest = (ushort) (word[0] << 8);
                        dest += word[1];
                        break;
                    default:
                        dest = destReg;
                        break;


                }

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
                        _memoryManager.SetStatusFlag("V", false);
                        break;
                    case Mnemonic.MOVB:
                        dest = src;
                        if ((short)(src) < 0)
                            _memoryManager.SetStatusFlag("N", true);
                        else
                            _memoryManager.SetStatusFlag("N", false);
                        if ((short)(src) == 0)
                            _memoryManager.SetStatusFlag("Z", true);
                        else
                            _memoryManager.SetStatusFlag("Z", false);
                        _memoryManager.SetStatusFlag("V", false);
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
                        _memoryManager.SetStatusFlag("N", false);
                        if (dest == 0)
                            _memoryManager.SetStatusFlag("Z", true);
                        else
                            _memoryManager.SetStatusFlag("Z", false);
                        _memoryManager.SetStatusFlag("V", false);
                        break;
                    case Mnemonic.BISB:
                        dest = (ushort)(dest | src);
                        _memoryManager.SetStatusFlag("N", false);
                        if (dest == 0)
                            _memoryManager.SetStatusFlag("Z", true);
                        else
                            _memoryManager.SetStatusFlag("Z", false);
                        _memoryManager.SetStatusFlag("V", false);
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

                switch (command.Operands[DecoderConsts.DEST_MODE])
                {
                    case 0:
                        _memoryManager.SetRegister(destaddr, dest);
                        break;
                    case 1:
                        byte[] word = new byte[2];
                        word[0] = (byte)(dest >> 8);
                        word[1] = (byte) ((dest << 8) >> 8);
                        _memoryManager.SetMemory(destReg, word);
                        break;
                    default:
                        _memoryManager.SetRegister(destaddr, dest);
                        break;


                }
             
            } else
                if (command.MnemonicType == MnemonicType.TwoOperand) {
                    string destaddr = "R" + command.Operands[DecoderConsts.DEST];
                    string sourceaddr = "R" + command.Operands[DecoderConsts.SOURCE];
                    int dest, src;
                    /*
                    if (destaddr % 2 == 0) {
                        dest = getReg(destaddr + 1, 0);
                        dest = dest << 16 + getReg(destaddr, 0);
                    } else
                        dest = getReg(destaddr, 0);
                    */
                    switch (command.Mnemonic) {
                        case Mnemonic.MUL:
                            //dest *= src;
                            break;
                        case Mnemonic.DIV:
                            //destaddr = dest / src;
                            break;
                        case Mnemonic.ASH:
                            
                            break;
                        case Mnemonic.ASHC:
                            break;
                        case Mnemonic.XOR:
                            break;
                        case Mnemonic.SOB:
                            break;
                        default:
                            break;
                    }
                } else
                    if (command.MnemonicType == MnemonicType.SingleOperand) {
                        string destaddr = "R" + command.Operands[DecoderConsts.DEST];
                        if (command.Operands[DecoderConsts.DEST] == 6)
                            destaddr = "SP";
                        if (command.Operands[DecoderConsts.DEST] == 7)
                            destaddr = "PC";
                        ushort dest, destReg = _memoryManager.GetRegister(destaddr);
                        switch (command.Operands[DecoderConsts.DEST_MODE]) {
                            case 0:
                                dest = destReg;
                                break;
                            case 1:
                                byte[] word;
                                word = _memoryManager.GetMemory(destReg, 2);
                                dest = (ushort) (word[0] << 8);
                                dest += word[1];
                                break;
                            default:
                                dest = destReg;
                                break;
                        }
                        switch (command.Mnemonic) {
                            case Mnemonic.INC:
                                dest++;
                                break;
                            case Mnemonic.DEC:
                                dest--;
                                break;
                            default:
                                break;
                        }
                        switch (command.Operands[DecoderConsts.DEST_MODE])
                        {
                            case 0:
                                _memoryManager.SetRegister(destaddr, dest);
                                break;
                            case 1:
                                byte[] word = new byte[2];
                                word[0] = (byte)(dest >> 8);
                                word[1] = (byte) ((dest << 8) >> 8);
                                _memoryManager.SetMemory(destReg, word);
                                break;
                            default:
                                _memoryManager.SetRegister(destaddr, dest);
                                break;
                        }
                    } else
                        if (command.MnemonicType == MnemonicType.ConditionalBranch) {
                            ushort programmCounter = _memoryManager.GetRegister("PC");
                            switch (command.Mnemonic) {
                                case Mnemonic.BR:
                                    programmCounter += (ushort)(2 * command.Operands[DecoderConsts.OFFSET]);
                                    break;
                                case Mnemonic.BNE:
                                    if (!_memoryManager.GetStatusFlag("Z"))
                                        programmCounter += (ushort)(2 * command.Operands[DecoderConsts.OFFSET]);
                                    break;
                                case Mnemonic.BEQ:
                                    if (_memoryManager.GetStatusFlag("Z"))
                                        programmCounter += (ushort)(2 * command.Operands[DecoderConsts.OFFSET]);
                                    break;
                                 case Mnemonic.BLT:
                                    if (_memoryManager.GetStatusFlag("N") || _memoryManager.GetStatusFlag("V"))
                                        programmCounter += (ushort)(2 * command.Operands[DecoderConsts.OFFSET]);
                                    break;
                                default:
                                    break;
                            }

                            _memoryManager.SetRegister("PC", programmCounter);
                        }
	    }
    }
}
