using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using AlmostPDP11;
using AlmostPDP11.VM.Assembler;
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
        private readonly Func<Delegate, object> _invoker;
        private Func<Delegate, object> invoke;

        public MachineState CurrentState
        {
            get { return _currentState; }
            set
            {
                _currentState = value;

                InvokeActionInGUIThread(() =>
                {
                    OnStateChanged?.Invoke(_currentState, value);
                });

               
            }
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
                            // not a single step done so probably we should stop here
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

        public VirtualMachine(Func<Delegate, object> invoker)
        {
            this._invoker = invoker;

            _memoryManager = new MemoryManager();
            _commandHandler = new ComandHandler(_memoryManager);

            _currentState = MachineState.Stopped;

            ResetCounters();
        }

        // Views related code

        private void InvokeActionInGUIThread(Action action)
        {
            _invoker.Invoke(action);
        }

        public void UpdateVRAMViews()
        {
            InvokeActionInGUIThread(() =>
            {
                OnVRAMUpdated?.Invoke(_memoryManager?.GetVRAM().ToArray());
            });
        }

        public void UpdateRegistersViews()
        {
            InvokeActionInGUIThread(() =>
            {
                OnRegistersUpdated?.Invoke(_memoryManager.GetRegisters());
            });
        }

        // TODO: DON'T FORGET TO CALL EVENT ON VRAM UPDATES
        public void UpdateViews()
        {
            UpdateRegistersViews();
            InvokeActionInGUIThread(() =>
            {
                OnStateChanged?.Invoke(_currentState, _currentState);
            });
            UpdateVRAMViews();
        }

        // Interrupts related code

        public void GenerateKeyboardInterrupt(byte scanCode, bool keyUp, bool alt, bool ctrl, bool shift)
        {
            // TODO: generate actual interrupt;
            _memoryManager.HandleKeyboardEvent(keyUp, alt, ctrl, shift, scanCode);

            PushToStack(_memoryManager.GetRegister("PC"));
            PushToStack((ushort)Consts.EPROMOffsets["ASCII"]);

            UpdateRegistersViews();
        }

        // General memory management code

        public IEnumerable<byte> GetMemory(int fromAddress, int toAddress)
        {
            return _memoryManager.GetMemory(fromAddress, toAddress - fromAddress);
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

        public void FlipRegisterBit(string regName, byte bitNumber)
        {
            var newBitValue = !_memoryManager.GetRegisterBit(regName, bitNumber);
            _memoryManager.SetRegisterBit(regName, bitNumber, newBitValue);
            UpdateRegistersViews();
        }

        public void FlipStatusFlag(string flagName)
        {
            _memoryManager.SetStatusFlag(flagName, !_memoryManager.GetStatusFlag(flagName));
        }

        // High-level memory management code

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

        public void UploadVRAM(IEnumerable<byte> bytes)
        {
            _memoryManager.SetMemory(Consts.MemoryOffsets["VRAM"], bytes);
            UpdateVRAMViews();
        }

        public void UploadKeyboardHandler(IEnumerable<string> codeLines)
        {
            var codeBytes = new List<byte>();

            var words = Assembler.Assembly(codeLines, Consts.MemoryOffsets["ROM"]);

            foreach (var word in words)
            {
                codeBytes.AddRange(BitConverter.GetBytes(word));
            }

            _memoryManager.SetMemory(Consts.EPROMOffsets["KB_HANDLER"], codeBytes);
        }

        // High-level VM functionality

        public void ResetRegisters()
        {
            foreach (var registerName in Consts.RegisterNames)
            {
                _memoryManager.SetRegister(registerName, 0);
            }
        }

        public void ResetCounters()
        {
            _memoryManager.SetRegister("SP", (ushort)(Consts.MemoryOffsets["RAM"] + Consts.MemorySizes["RAM"]));
            _memoryManager.SetRegister("PC", (ushort)Consts.MemoryOffsets["ROM"]);
        }

        public void Start()
        {
            CurrentState = MachineState.Running;
            _thread = new Thread(ThreadRun);
            _thread.Start();
        }

        public void Pause()
        {
            CurrentState = MachineState.Paused;
        }

        public void Stop()
        {
            CurrentState = MachineState.Stopped;
            _thread?.Abort();
        }

        public void Reset()
        {
            CurrentState = MachineState.Stopped;

            ResetRegisters();
            ResetCounters();
            UpdateViews();
        }

        public bool StepForward()
        {
            var oldPCvalue = _memoryManager.GetRegister("PC");

            var commandsBytesToRead = Consts.WordsInCommand * Consts.BytesInWord;
            var commandWordsBytes = _memoryManager.GetMemory(oldPCvalue, commandsBytesToRead);
            var commandsWords = new List<ushort>();

            for (var i = 0; i < commandsBytesToRead; i += Consts.BytesInWord)
            {
                if (i >= commandsBytesToRead)
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

            var newPCvalue = (ushort)(oldPCvalue + incPCBy);

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
    }
}
