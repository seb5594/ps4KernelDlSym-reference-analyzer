using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElfIO
{
    public class ElfEndian
    {
        Boolean reverse;

        public ElfEndian(ElfData elfdata)
        {
            reverse = elfdata == ElfData.ED_LSB ? !BitConverter.IsLittleEndian : elfdata == ElfData.ED_MSB ? BitConverter.IsLittleEndian : false;
            /*if (elfdata == ElfData.ED_MSB)
            {
                if (BitConverter.IsLittleEndian)
                {
                    // Reverse if elf is big endian and host little endian
                    reverse = true;
                }
            }
            else
            {
                if (!BitConverter.IsLittleEndian)
                {
                    // Reverse if elf is little endian and host big endian
                    reverse = true;
                }
            }*/
        }

        public Byte Convert(Byte value)
        {
            return value;
        }

        public SByte Convert(SByte value)
        {
            return value;
        }

        public UInt16 Convert(UInt16 value)
        {
            if (reverse)
                return (UInt16)(((value & 0x00FF) << 8) |
                                ((value & 0xFF00) >> 8));
            return value;
        }

        public Int16 Convert(Int16 value)
        {
            return (Int16)Convert((UInt16)value);
        }

        public UInt32 Convert(UInt32 value)
        {
            if (reverse)
                return (UInt32)(((value & 0x000000FF) << 24) |
                                ((value & 0x0000FF00) << 8) |
                                ((value & 0x00FF0000) >> 8) |
                                ((value & 0xFF000000) >> 24));
            return value;
        }

        public Int32 Convert(Int32 value)
        {
            return (Int32)Convert((UInt32)value);
        }

        public UInt64 Convert(UInt64 value)
        {
            if (reverse)
                return (UInt64)(((value & 0x00000000000000FFul) << 56) |
                                ((value & 0x000000000000FF00ul) << 40) |
                                ((value & 0x0000000000FF0000ul) << 24) |
                                ((value & 0x00000000FF000000ul) << 8) |
                                ((value & 0x000000FF00000000ul) >> 8) |
                                ((value & 0x0000FF0000000000ul) >> 24) |
                                ((value & 0x00FF000000000000ul) >> 40) |
                                ((value & 0xFF00000000000000ul) >> 56));
            return value;
        }

        public Int64 Convert(Int64 value)
        {
            return (Int64)Convert((UInt64)value);
        }
    }
}