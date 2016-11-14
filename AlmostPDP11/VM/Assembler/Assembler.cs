using System;
using System.Collections.Generic;

namespace VM.Assembler
{
    public class Assembler
    {

        public Assembler(IEnumerable<String> program, int baseAddress)
        {
            this.program = program;
            BASEADDRESS = baseAddress;
        }

        public int BASEADDRESS { get; set; }
        public IEnumerable<String> program { get; set; }
        public IEnumerable<ushort> assambled { get; set; }

        public void assamble()
        {

        }
    }
}