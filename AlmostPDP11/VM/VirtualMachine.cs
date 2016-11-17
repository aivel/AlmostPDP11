using System;
using System.Collections.Generic;
using System.Linq;
<<<<<<< HEAD
using System.Threading;
using AlmostPDP11;
=======
using AlmostPDP11.VM.Assembler;
>>>>>>> dca77febe9528d33edafc3c9562b33dde69d287d
using AlmostPDP11.VM.Decoder;
using AlmostPDP11.VM.Executor;

namespace VM
{
    // event defenitions
    public delegate void StateChangedEventHandler(MachineState oldState, MachineState newState);

    public delegate void RegistersUpdatedEventHandler(IDictionary<string, ushort> newRegisterValues);

    public delegate void VRAMUpdatedEventHandler(byte[] vramBytes);
    // **

    public class VirtualMachine
    {
        // events

        public StateChangedEventHandler OnStateChanged;

        public RegistersUpdatedEventHandler OnRegistersUpdated;

        public VRAMUpdatedEventHandler OnVRAMUpdated;
        //

        // constants

        // **

        private MachineState _currentState;
        private readonly MemoryManager _memoryManager;
        private readonly ComandHandler _commandHandler;

        // threading
        private Thread _thread;
        private MainForm _invoker;

        public MachineState CurrentState
        {
            get { return _currentState; }
            set
            {
                // TODO: generalize
                if (_invoker.InvokeRequired)
                {
                    this._invoker?.Invoke(new Func<int>(() =>
                    {
                        OnStateChanged?.Invoke(_currentState, value);
                        return 0;
                    }));
                }

                _currentState = value;
            }
        }

        //

        public void Start()
        {
            CurrentState = MachineState.Running;
        }

        public void Pause()
        {
            CurrentState = MachineState.Paused;
        }

        public void Stop()
        {
            CurrentState = MachineState.Stopped;
        }

        public void Reset()
        {
            CurrentState = MachineState.Stopped;

            // TODO: do the actual reset
            ResetCounters();

            CurrentState = MachineState.Running;
        }

        public bool StepForward()
        {
            var oldPCvalue = _memoryManager.GetRegister("PC");

            var commandsBytesToRead = Consts.WordsInCommand*Consts.BytesInWord;
            var commandWordsBytes = _memoryManager.GetMemory(oldPCvalue, commandsBytesToRead);
            var commandsWords = new List<ushort>();

            for (var i = 0; i < commandsBytesToRead; i += Consts.BytesInWord)
            {
                if (i >= Consts.BytesInWord)
                {
                    break;
                }

                var commandWordBytes = new List<byte>();

                for (var j = 0; j < Consts.BytesInWord; j++)
                {
                    commandWordBytes.Add(commandWordsBytes[i + j]);
                }

                var commandWord = BitConverter.ToUInt16(commandWordBytes.ToArray(), 0);

                commandsWords.Add(commandWord);
            }

            var command = Decoder.Decode(commandsWords);
            var incPCBy = command.Operands[DecoderConsts.COMMANDWORDSLENGTH] * Consts.BytesInWord;

            if (command.Mnemonic == Mnemonic.ERR)
            {
                return false;
            }

            var newPCvalue = (ushort) (oldPCvalue + incPCBy);

            _memoryManager.SetRegister("PC", newPCvalue);

            // perform the operation
            _commandHandler.Operation(command);

            var safety = new object();

            lock (safety)
            {
                UpdateRegistersViews();
            }

            return true;
        }

        public void UpdateRegistersViews()
        {
            if (_invoker.InvokeRequired)
            {
                this._invoker?.Invoke(new Func<int>(() =>
                {
                    OnRegistersUpdated?.Invoke(_memoryManager.GetRegisters());
                    return 0;
                }));
            }
        }

        public void FlipRegisterBit(string regName, byte bitNumber)
        {
            _memoryManager.SetRegisterBit(regName, bitNumber, !_memoryManager.GetRegisterBit(regName, bitNumber));
            UpdateRegistersViews();
        }

        private void ThreadRun()
        { 
            var safety = new object();

            while (true)
            {
                Monitor.Enter(safety);

                try
                {
                    if (CurrentState == MachineState.Running)
                    {
                        // if running, do a step

                        var stepDone = StepForward();

                        if (!stepDone)
                        {
                            // no step done so probably we should stop here
                            CurrentState = MachineState.Stopped;
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(safety);
                }

                Thread.Sleep(Consts.VMSleepTimeout);
            }
        }

        public VirtualMachine(): this(null)
        {
            ;
        }

        public VirtualMachine(MainForm invoker)
        {
            this._invoker = invoker;

            _memoryManager = new MemoryManager();
            _commandHandler = new ComandHandler(_memoryManager);

            _thread = new Thread(ThreadRun);
            _thread.Start();

            _currentState = MachineState.Stopped;

            ResetCounters();

            UpdateViews();
        }

        private void ResetCounters()
        {
            _memoryManager.SetRegister("SP", (ushort) (Consts.MemoryOffsets["RAM"] + Consts.MemorySizes["RAM"]));
            _memoryManager.SetRegister("PC", (ushort) Consts.MemoryOffsets["ROM"]);
        }

        public void UpdateVRAMViews()
        {
            if (_invoker.InvokeRequired)
            {
                this._invoker?.Invoke(new Func<int>(() =>
                {
                    OnVRAMUpdated?.Invoke(_memoryManager?.GetVRAM().ToArray());
                    return 0;
                }));
            }
        }

        public void FlipStatusFlag(string flagName)
        {
            _memoryManager.SetStatusFlag(flagName, !_memoryManager.GetStatusFlag(flagName));
        }

        public void GenerateKeyboardInterrupt(byte scanCode, bool keyUp, bool alt, bool ctrl, bool shift)
        {
            // TODO: generate actual interrupt;
            _memoryManager.HandleKeyboardEvent(keyUp, alt, ctrl, shift, scanCode);

            PushToStack(_memoryManager.GetRegister("PC"));
            PushToStack((ushort) Consts.EPROMOffsets["ASCII"]);

            UpdateRegistersViews();
        }

        // TODO: DON'T FORGET TO CALL EVENT ON VRAM UPDATES
        public void UpdateViews()
        {
            // TODO: generalize
            UpdateRegistersViews();
            //OnRegistersUpdated?.Invoke(_memoryManager?.GetRegisters());
            if (_invoker.InvokeRequired)
            {
                this._invoker?.Invoke(new Func<int>(() =>
                {
                    OnStateChanged?.Invoke(_currentState, _currentState);
                    return 0;
                }));
            }
            //OnStateChanged?.Invoke(_currentState, _currentState);
            UpdateVRAMViews();
            //OnVRAMUpdated?.Invoke(_memoryManager?.GetVRAM().ToArray());
        }

        public void UploadCodeToROM(string[] codeLines)
        {
            var codeBytes = new List<byte>();

            var words = Assembler.Assembly(codeLines, Consts.MemoryOffsets["ROM"]);

            foreach (var word in words)
            {
                codeBytes.AddRange(BitConverter.GetBytes(word));
            }

            _memoryManager.SetMemory(Consts.MemoryOffsets["ROM"], codeBytes);
        }

        public IEnumerable<byte> GetMemory(int fromAddress, int toAddress)
        {
            return _memoryManager.GetMemory(fromAddress, toAddress - fromAddress);
        }

        public void UploadASCIIMap(Dictionary<byte, byte> asciiMap)
        {
            var EPROMBytes = new List<byte>();

            foreach (var scanToAscii in asciiMap)
            {
                EPROMBytes.Add(scanToAscii.Key);
                EPROMBytes.Add(scanToAscii.Value);
            }

            _memoryManager.SetMemory(Consts.EPROMOffsets["ASCII"], EPROMBytes);
        }

        public void PushToStack(ushort word)
        {
            _memoryManager.PushToStack(word);
        }

        public ushort PeekStack()
        {
            return _memoryManager.PeekStack();
        }

        public ushort PopFromStack()
        {
            return _memoryManager.PopFromStack();
        }

        public IEnumerable<byte> GetVRAMBytes()
        {
            return _memoryManager.GetVRAM();
        }
    }
}
