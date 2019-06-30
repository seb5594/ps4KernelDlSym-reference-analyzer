using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElfIO
{
    public class ElfSection
    {
        ElfSHDR header;
        Byte[] section;

        public ElfSection(ElfSHDR header, Byte[] section)
        {
            // Initialize header
            this.header = header;

            // Initialize section
            this.section = section;
        }

        public ElfSHDR Header
        {
            get
            {
                return header;
            }
        }

        public Byte[] Section
        {
            get
            {
                return section;
            }
        }

        public Int32 Read(ref Byte[] buffer, UInt32 offset, Int32 count)
        {
            if (header.Type != ElfSectionType.SHT_NOBITS && header.Type != ElfSectionType.SHT_NULL)
            {
                Int32 ret = (Int32)Math.Min(header.Size - offset, (UInt32)count);
                Array.Copy(section, offset, buffer, 0, ret);
                return ret;
            }
            return -1;
        }

        public Int32 Write(Byte[] buffer, UInt32 offset, Int32 count)
        {
            if (header.Type != ElfSectionType.SHT_NOBITS && header.Type != ElfSectionType.SHT_NULL)
            {
                Int32 ret = (int)Math.Min(header.Size - offset, (UInt32)count);
                Array.Copy(buffer, 0, section, offset, ret);
                return ret;
            }
            return -1;
        }
    }
}