using System;
using System.Collections.Generic;
using System.Linq;

namespace VM
{
    // event defenitions
    public delegate void StateChangedEventHandler(MachineState oldState, MachineState newState);

    public delegate void RegistersUpdatedEventHandler(IDictionary<string, ushort> newRegisterValues);

    public delegate void StatusFlagUpdatedEventHandler(IDictionary<string, bool> newFlags);

    public delegate void VRAMUpdatedEventHandler(byte[] vramBytes);
    // **

    public class VirtualMachine
    {
        // events
        
        public StateChangedEventHandler OnStateChanged;

        public RegistersUpdatedEventHandler OnRegistersUpdated;

        public StatusFlagUpdatedEventHandler OnStatusFlagUpdated;

        public VRAMUpdatedEventHandler OnVRAMUpdated;
        //

        // constants

        // **

        private MachineState _currentState;
        private readonly MemoryManager _memoryManager;

        public MachineState CurrentState {
            get { return _currentState; }
            set
            {
                OnStateChanged?.Invoke(_currentState, value);
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

            CurrentState = MachineState.Running;
        }

        public void StepForward()
        {
            var oldPCvalue = _memoryManager.GetRegister("PC");
            _memoryManager.SetRegister("PC", (ushort) (oldPCvalue + Consts.PCIncBytes));
            OnRegistersUpdated?.Invoke(_memoryManager.GetRegisters());
        }

        public void FlipRegisterBit(string regName, byte bitNumber)
        {
            _memoryManager.SetRegisterBit(regName, bitNumber, !_memoryManager.GetRegisterBit(regName, bitNumber));
            OnRegistersUpdated?.Invoke(_memoryManager.GetRegisters());
        }

        public VirtualMachine()
        {
            _memoryManager = new MemoryManager();
            _currentState = MachineState.Stopped;

            _memoryManager.SetRegister("SP", (ushort) (Consts.MemoryOffsets["RAM"] + Consts.MemorySizes["RAM"]));
            _memoryManager.SetRegister("PC", (ushort) Consts.MemoryOffsets["ROM"]);

            UpdateState();
        }

        public void UpdateVRAM()
        {
            OnVRAMUpdated?.Invoke(_memoryManager.GetVRAM().ToArray());
        }

        public void FlipStatusFlag(string flagName)
        {
            _memoryManager.SetStatusFlag(flagName, !_memoryManager.GetStatusFlag(flagName));
            OnStatusFlagUpdated?.Invoke(_memoryManager.GetStatusFlags());
        }

        public ushort PopFromStack()
        {
            // TODO: interrupt on stack corruption
            // pop from stack
            var sp = _memoryManager.GetRegister("SP");

            var wordBytes = _memoryManager.GetMemory(sp, Consts.SPIncBytes);

            var newSp = (ushort)(sp < Consts.MemorySizes["RAM"] - Consts.SPIncBytes ? sp + Consts.SPIncBytes : Consts.MemorySizes["RAM"]);
            _memoryManager.SetRegister("SP", newSp);

            return BitConverter.ToUInt16(wordBytes, 0);
        }

        public void PushToStack(ushort word)
        {
            // TODO: interrupt on stack corruption
            // push to stack
            var wordBytes = BitConverter.GetBytes(word);
            var sp = _memoryManager.GetRegister("SP");
            var newSp = (ushort)(sp > 0 ? sp - Consts.SPIncBytes : 0);

            _memoryManager.SetMemory(newSp, wordBytes);

            _memoryManager.SetRegister("SP", newSp);
        }

        public void GenerateKeyboardInterrupt(byte scanCode, bool keyUp, bool alt, bool ctrl, bool shift)
        {
            // TODO: generate actual interrupt
            
        }

        // TODO: DON'T FORGET TO CALL EVENT ON VRAM UPDATES
        public void UpdateState()
        {
            OnRegistersUpdated?.Invoke(_memoryManager.GetRegisters());
            OnStateChanged?.Invoke(_currentState, _currentState);
            OnStatusFlagUpdated?.Invoke(_memoryManager.GetStatusFlags());
            OnVRAMUpdated?.Invoke(_memoryManager.GetVRAM().ToArray());
        }
    }
}
