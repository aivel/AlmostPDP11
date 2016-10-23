using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AlmostPDP11.VM.Extentions;

namespace VM {
    public class MemoryManager {
        private readonly byte[] _memoryPool;

        public MemoryManager() {
            _memoryPool = new byte[Consts.FullMemorySize];
        }

        public IEnumerable<byte> GetRAM()
        {
            return _memoryPool.Skip(Consts.MemoryOffsets["RAM"]).Take(Consts.MemorySizes["RAM"]);
        }

        public IEnumerable<byte> GetROM()
        {
            return _memoryPool.Skip(Consts.MemoryOffsets["ROM"]).Take(Consts.MemorySizes["ROM"]);
        }

        public IEnumerable<byte> GetVRAM()
        {
            return _memoryPool.Skip(Consts.MemoryOffsets["VRAM"]).Take(Consts.MemorySizes["VRAM"]);
        }

        public byte[] GetMemory(int offset, int length)
        {
            return _memoryPool
                .Skip(Math.Min(offset, _memoryPool.Length))
                .Take(Math.Min(length, _memoryPool.Length)).ToArray();
        }

        public void SetMemory(int offset, IEnumerable<byte> bytes)
        {
            var bytesArray = bytes as byte[] ?? bytes.ToArray();

            for (var i = 0; i < bytesArray.Length; i++)
            {
                var byteOffset = offset + i;
                _memoryPool[byteOffset] = bytesArray.ElementAt(i);
            }
        }

        public IDictionary<string, ushort> GetRegisters()
        {
            var registersBytes = GetMemory(Consts.MemoryOffsets["REGISTERS"], Consts.MemorySizes["REGISTERS"]);

            var registersBytePairs = registersBytes
                .Split(Consts.GeneralPurposeRegistersCount);

            var registers = Consts.RegisterNames
                .Zip(registersBytePairs, (k, v) => new {k, v})
                .ToDictionary(x => x.k, x => BitConverter.ToUInt16(x.v.ToArray(), 0));

            return registers;
        }

        public void SetRegisters(IDictionary<string, ushort> registers)
        {
            foreach (var registerItem in registers)
            {
                var registerOffset = Consts.RegistersOffsets[registerItem.Key];
                SetMemory(registerOffset, BitConverter.GetBytes(registerItem.Value));
            }
        }

        public ushort GetStatusWord()
        {
            return BitConverter.ToUInt16(GetMemory(Consts.MemoryOffsets["STATUS_WORD"], Consts.MemorySizes["STATUS_WORD"]).ToArray(), 0);
        }

        private void SetStatusWord(ushort statusWord)
        {
            SetMemory(Consts.MemoryOffsets["STATUS_WORD"], BitConverter.GetBytes(statusWord));
        }

        public void SetStatusFlag(string flagName, bool value)
        {
            var statusWord = GetStatusWord();

            var newStatusWord = statusWord.SetBit(Consts.StatusFlagBitOffsets[flagName], value);
           
            SetStatusWord(newStatusWord);
        }

        public bool GetStatusFlag(string flagName)
        {
            var statusWord = GetStatusWord();

            return statusWord.GetBit(Consts.StatusFlagBitOffsets[flagName]);
        }

        public void SetRegister(string regName, ushort value)
        {
            SetRegisters(new Dictionary<string, ushort> { { regName, value } });
        }

        public ushort GetRegister(string regName)
        {
            return GetRegisters()[regName];
        }

        public void SetRegisterBit(string regName, int bitIndex, bool value)
        {
            var registerValue = GetRegister(regName);
            var newRegisterValue = registerValue.SetBit(bitIndex, value);

            SetRegister(regName, newRegisterValue);
        }

        public bool GetRegisterBit(string regName, byte bitIndex)
        {
            var registerValue = GetRegister(regName);

            return registerValue.GetBit(bitIndex);
        }

        public IDictionary<string, bool> GetStatusFlags()
        {
            return Consts.StatusFlagNames.ToDictionary(flagName => flagName, GetStatusFlag);
        }
    }
}
