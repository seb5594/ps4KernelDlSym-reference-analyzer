using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElfIO
{
    public class ElfProgram
    {
        ElfPHDR header;

        public ElfProgram(ElfPHDR header)
        {
            this.header = header;
        }

        public ElfPHDR Header
        {
            get
            {
                return header;
            }
        }
    }
}