using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElfIO
{
    public enum ElfIdent
    {
        EI_MAG0 = 0,
        EI_MAG1 = 1,
        EI_MAG2 = 2,
        EI_MAG3 = 3,
        EI_CLASS = 4,
        EI_DATA = 5,
        EI_VERSION = 6,
        EI_OSABI = 7,
        EI_ABIVERSION = 8,
        EI_PAD = 9,
        EI_NIDENT = 16
    }

    public enum ElfClass
    {
        EC_NONE = 0,
        EC_32 = 1,
        EC_64 = 2
    }

    public enum ElfData
    {
        ED_NONE = 0,
        ED_LSB = 1,
        ED_MSB = 2
    }

    public enum ElfType
    {
        ET_NONE = 0,
        ET_REL = 1,
        ET_EXEC = 2,
        ET_DYN = 3,
        ET_CORE = 4,
        ET_LOOS = 0xFE00,
        ET_HIOS = 0xFEFF,
        ET_LOPROC = 0xFF00,
        ET_HIPROC = 0xFFFF
    }

    public enum ElfMachine
    {
        EM_NONE = 0,
        EM_M32 = 1,
        EM_SPARC = 2,
        EM_386 = 3,
        EM_68K = 4,
        EM_88K = 5,
        EM_860 = 7,
        EM_MIPS = 8,
        EM_S370 = 9,
        EM_MIPS_RS3_LE = 10,
        EM_PARISC = 15,
        reserved = 16,
        EM_VPP500 = 17,
        EM_SPARC32PLUS = 18,
        EM_960 = 19,
        EM_PPC = 20,
        EM_PPC64 = 21,
        EM_S390 = 22,
        EM_V800 = 36,
        EM_FR20 = 37,
        EM_RH32 = 38,
        EM_RCE = 39,
        EM_ARM = 40,
        EM_ALPHA = 41,
        EM_SH = 42,
        EM_SPARCV9 = 43,
        EM_TRICORE = 44,
        EM_ARC = 45,
        EM_H8_300 = 46,
        EM_H8_300H = 47,
        EM_H8S = 48,
        EM_H8_500 = 49,
        EM_IA_64 = 50,
        EM_MIPS_X = 51,
        EM_COLDFIRE = 52,
        EM_68HC12 = 53,
        EM_MMA = 54,
        EM_PCP = 55,
        EM_NCPU = 56,
        EM_NDR1 = 57,
        EM_STARCORE = 58,
        EM_ME16 = 59,
        EM_ST100 = 60,
        EM_TINYJ = 61,
        EM_X86_64 = 62,
        EM_PDSP = 63,
        EM_PDP10 = 64,
        EM_PDP11 = 65,
        EM_FX66 = 66,
        EM_ST9PLUS = 67,
        EM_ST7 = 68,
        EM_68HC16 = 69,
        EM_68HC11 = 70,
        EM_68HC08 = 71,
        EM_68HC05 = 72,
        EM_SVX = 73,
        EM_ST19 = 74,
        EM_VAX = 75,
        EM_CRIS = 76,
        EM_JAVELIN = 77,
        EM_FIREPATH = 78,
        EM_ZSP = 79,
        EM_MMIX = 80,
        EM_HUANY = 81,
        EM_PRISM = 82,
        EM_AVR = 83,
        EM_FR30 = 84,
        EM_D10V = 85,
        EM_D30V = 86,
        EM_V850 = 87,
        EM_M32R = 88,
        EM_MN10300 = 89,
        EM_MN10200 = 90,
        EM_PJ = 91,
        EM_OPENRISC = 92,
        EM_ARC_A5 = 93,
        EM_XTENSA = 94,
        EM_VIDEOCORE = 95,
        EM_TMM_GPP = 96,
        EM_NS32K = 97,
        EM_TPC = 98,
        EM_SNP1K = 99,
        EM_ST200 = 100,
        EM_IP2K = 101,
        EM_MAX = 102,
        EM_CR = 103,
        EM_F2MC16 = 104,
        EM_MSP430 = 105,
        EM_BLACKFIN = 106,
        EM_SE_C33 = 107,
        EM_SEP = 108,
        EM_ARCA = 109,
        EM_UNICORE = 110,
        EM_EXCESS = 111,
        EM_DXP = 112,
        EM_ALTERA_NIOS2 = 113,
        EM_CRX = 114,
        EM_XGATE = 115,
        EM_C166 = 116,
        EM_M16C = 117,
        EM_DSPIC30F = 118,
        EM_CE = 119,
        EM_M32C = 120,
        EM_res121 = 121,
        EM_res122 = 122,
        EM_res123 = 123,
        EM_res124 = 124,
        EM_res125 = 125,
        EM_res126 = 126,
        EM_res127 = 127,
        EM_res128 = 128,
        EM_res129 = 129,
        EM_res130 = 130,
        EM_TSK3000 = 131,
        EM_RS08 = 132,
        EM_res133 = 133,
        EM_ECOG2 = 134,
        EM_SCORE = 135,
        EM_SCORE7 = 135,
        EM_DSP24 = 136,
        EM_VIDEOCORE3 = 137,
        EM_LATTICEMICO32 = 138,
        EM_SE_C17 = 139,
        EM_TI_C6000 = 140,
        EM_TI_C2000 = 141,
        EM_TI_C5500 = 142,
        EM_res143 = 143,
        EM_res144 = 144,
        EM_res145 = 145,
        EM_res146 = 146,
        EM_res147 = 147,
        EM_res148 = 148,
        EM_res149 = 149,
        EM_res150 = 150,
        EM_res151 = 151,
        EM_res152 = 152,
        EM_res153 = 153,
        EM_res154 = 154,
        EM_res155 = 155,
        EM_res156 = 156,
        EM_res157 = 157,
        EM_res158 = 158,
        EM_res159 = 159,
        EM_MMDSP_PLUS = 160,
        EM_CYPRESS_M8C = 161,
        EM_R32C = 162,
        EM_TRIMEDIA = 163,
        EM_QDSP6 = 164,
        EM_8051 = 165,
        EM_STXP7X = 166,
        EM_NDS32 = 167,
        EM_ECOG1 = 168,
        EM_ECOG1X = 168,
        EM_MAXQ30 = 169,
        EM_XIMO16 = 170,
        EM_MANIK = 171,
        EM_CRAYNV2 = 172,
        EM_RX = 173,
        EM_METAG = 174,
        EM_MCST_ELBRUS = 175,
        EM_ECOG16 = 176,
        EM_CR16 = 177,
        EM_ETPU = 178,
        EM_SLE9X = 179,
        EM_L1OM = 180,
        EM_INTEL181 = 181,
        EM_INTEL182 = 182,
        EM_res183 = 183,
        EM_res184 = 184,
        EM_AVR32 = 185,
        EM_STM8 = 186,
        EM_TILE64 = 187,
        EM_TILEPRO = 188,
        EM_MICROBLAZE = 189,
        EM_CUDA = 190,
        EM_TILEGX = 191,
        EM_CLOUDSHIELD = 192,
        EM_COREA_1ST = 193,
        EM_COREA_2ND = 194,
        EM_ARC_COMPACT2 = 195,
        EM_OPEN8 = 196,
        EM_RL78 = 197,
        EM_VIDEOCORE5 = 198,
        EM_78KOR = 199,
        EM_56800EX = 200,
        EM_BA1 = 201,
        EM_BA2 = 202,
        EM_XCORE = 203,
        EM_MCHP_PIC = 204,
        EM_INTEL205 = 205,
        EM_INTEL206 = 206,
        EM_INTEL207 = 207,
        EM_INTEL208 = 208,
        EM_INTEL209 = 209,
        EM_KM32 = 210,
        EM_KMX32 = 211,
        EM_KMX16 = 212,
        EM_KMX8 = 213,
        EM_KVARC = 214,
        EM_CDP = 215,
        EM_COGE = 216,
        EM_COOL = 217,
        EM_NORC = 218,
        EM_CSR_KALIMBA = 219,
        EM_Z80 = 220,
        EM_VISIUM = 221,
        EM_FT32 = 222,
        EM_MOXIE = 223,
        EM_AMDGPU = 224,
        EM_RISCV = 243,
        EM_LANAI = 244,
        EM_CEVA = 245,
        EM_CEVA_X2 = 246,
        EM_BPF = 247
    }

    public enum ElfVersion
    {
        EV_NONE = 0,
        EV_CURRENT = 1,
    }

    public enum ElfProgramType
    {
        PT_NULL = 0,
        PT_LOAD = 1,
        PT_DYNAMIC = 2,
        PT_INTERP = 3,
        PT_NOTE = 4,
        PT_SHLIB = 5,
        PT_PHDR = 6,
        PT_TLS = 7,
        PT_LOOS = 0x60000000,
        PT_HIOS = 0x6FFFFFFF,
        PT_LOPROC = 0x70000000,
        PT_HIPROC = 0x7FFFFFFF
    }

    [FlagsAttribute]
    public enum ElfProgramFlags : uint
    {
        PF_X = 1,
        PF_W = 2,
        PF_R = 4,
        PF_MASKOS = 0x0FF00000,
        PF_MASKPROC = 0xF0000000,
    }

    public enum ElfSectionType : uint
    {
        SHT_NULL = 0,
        SHT_PROGBITS = 1,
        SHT_SYMTAB = 2,
        SHT_STRTAB = 3,
        SHT_RELA = 4,
        SHT_HASH = 5,
        SHT_DYNAMIC = 6,
        SHT_NOTE = 7,
        SHT_NOBITS = 8,
        SHT_REL = 9,
        SHT_SHLIB = 10,
        SHT_DYNSYM = 11,
        SHT_INIT_ARRAY = 14,
        SHT_FINI_ARRAY = 15,
        SHT_PREINIT_ARRAY = 16,
        SHT_GROUP = 17,
        SHT_SYMTAB_SHNDX = 18,
        SHT_LOOS = 0x60000000,
        SHT_SUNW_MOVE = 0x6FFFFFFA,
        SHT_SUNW_COMDAT = 0x6FFFFFFB,
        SHT_SUNW_SYMINFO = 0x6FFFFFFC,
        SHT_SUNW_VERDEF = 0x6FFFFFFD,
        SHT_SUNW_VERNEED = 0x6FFFFFFE,
        SHT_SUNW_VERSYM = 0x6FFFFFFF,
        SHT_HIOS = 0x6FFFFFFF,
        SHT_LOPROC = 0x70000000,
        SHT_HIPROC = 0x7FFFFFFF,
        SHT_LOUSER = 0x80000000,
        SHT_HIUSER = 0xFFFFFFFF
    }

    [FlagsAttribute]
    public enum ElfSectionFlags : uint
    {
        SHF_WRITE = 0x1,
        SHF_ALLOC = 0x2,
        SHF_EXECINSTR = 0x4,
        SHF_MERGE = 0x10,
        SHF_STRINGS = 0x20,
        SHF_INFO_LINK = 0x40,
        SHF_LINK_ORDER = 0x80,
        SHF_OS_NONCONFORMING = 0x100,
        SHF_GROUP = 0x200,
        SHF_TLS = 0x400,
        SHF_MASKOS = 0x0FF00000,
        SHF_MASKPROC = 0xF0000000,
    }

    public class ElfHDR
    {
        Byte[] e_ident;
        UInt16 e_type;
        UInt16 e_machine;
        UInt32 e_version;
        UInt64 e_entry;
        UInt64 e_phoff;
        UInt64 e_shoff;
        UInt32 e_flags;
        UInt16 e_ehsize;
        UInt16 e_phentsize;
        UInt16 e_phnum;
        UInt16 e_shentsize;
        UInt16 e_shnum;
        UInt16 e_shstrndx;

        public ElfHDR()
        {
            e_ident = new Byte[(int)ElfIdent.EI_NIDENT];
        }

        public override String ToString() =>
            String.Format("ID: {0}\nType: {1}\nMachine: {2}\nVersion: {3}\nEntryPoint: 0x{4:X8}\nPHOffset: 0x{5:X8}\nSHOffset: 0x{6:X8}\nFlags: 0x{7:X4}\nHeaderSize: 0x{8:X2}\nPHSize: 0x{9:X2}\nPHCount: 0x{10:X2}\nSHSize: 0x{11:X2}\nSHCount: 0x{12:X2}\nStringTable: 0x{13:X2}\n\n",
                     BitConverter.ToString(Identification),
                     Type.ToString(),
                     MachineType.ToString(),
                     Version.ToString(),
                     EntryPoint,
                     ProgramHeaderOffset,
                     SectionHeaderOffset,
                     Flags,
                     HeaderSize,
                     ProgramHeaderSize,
                     ProgramHeaderCount,
                     SectionHeaderSize,
                     SectionHeaderCount,
                     StringTable);

        public void Load(ElfEndian endian, Byte[] data)
        {
            if (Class == ElfClass.EC_32)
            {
                e_type = endian.Convert(BitConverter.ToUInt16(data, 0x10));
                e_machine = endian.Convert(BitConverter.ToUInt16(data, 0x12));
                e_version = endian.Convert(BitConverter.ToUInt32(data, 0x14));
                e_entry = endian.Convert(BitConverter.ToUInt32(data, 0x18));
                e_phoff = endian.Convert(BitConverter.ToUInt32(data, 0x1C));
                e_shoff = endian.Convert(BitConverter.ToUInt32(data, 0x20));
                e_flags = endian.Convert(BitConverter.ToUInt32(data, 0x24));
                e_ehsize = endian.Convert(BitConverter.ToUInt16(data, 0x28));
                e_phentsize = endian.Convert(BitConverter.ToUInt16(data, 0x2A));
                e_phnum = endian.Convert(BitConverter.ToUInt16(data, 0x2C));
                e_shentsize = endian.Convert(BitConverter.ToUInt16(data, 0x2E));
                e_shnum = endian.Convert(BitConverter.ToUInt16(data, 0x30));
                e_shstrndx = endian.Convert(BitConverter.ToUInt16(data, 0x32));
            }
            else
            {
                e_type = endian.Convert(BitConverter.ToUInt16(data, 0x10));
                e_machine = endian.Convert(BitConverter.ToUInt16(data, 0x12));
                e_version = endian.Convert(BitConverter.ToUInt32(data, 0x14));
                e_entry = endian.Convert(BitConverter.ToUInt64(data, 0x18));
                e_phoff = endian.Convert(BitConverter.ToUInt64(data, 0x20));
                e_shoff = endian.Convert(BitConverter.ToUInt64(data, 0x28));
                e_flags = endian.Convert(BitConverter.ToUInt32(data, 0x30));
                e_ehsize = endian.Convert(BitConverter.ToUInt16(data, 0x34));
                e_phentsize = endian.Convert(BitConverter.ToUInt16(data, 0x36));
                e_phnum = endian.Convert(BitConverter.ToUInt16(data, 0x38));
                e_shentsize = endian.Convert(BitConverter.ToUInt16(data, 0x3A));
                e_shnum = endian.Convert(BitConverter.ToUInt16(data, 0x3C));
                e_shstrndx = endian.Convert(BitConverter.ToUInt16(data, 0x3E));
            }
        }

        public void Save(ElfEndian endian, ref Byte[] data)
        {
            Array.Copy(e_ident, 0, data, 0, (int)ElfIdent.EI_NIDENT);
            if (Class == ElfClass.EC_32)
            {
                Array.Copy(BitConverter.GetBytes(endian.Convert(e_type)), 0, data, 0x10, 2);
                Array.Copy(BitConverter.GetBytes(endian.Convert(e_machine)), 0, data, 0x12, 2);
                Array.Copy(BitConverter.GetBytes(endian.Convert(e_version)), 0, data, 0x14, 4);
                Array.Copy(BitConverter.GetBytes(endian.Convert((UInt32)e_entry)), 0, data, 0x18, 4);
                Array.Copy(BitConverter.GetBytes(endian.Convert((UInt32)e_phoff)), 0, data, 0x1C, 4);
                Array.Copy(BitConverter.GetBytes(endian.Convert((UInt32)e_shoff)), 0, data, 0x20, 4);
                Array.Copy(BitConverter.GetBytes(endian.Convert(e_flags)), 0, data, 0x24, 4);
                Array.Copy(BitConverter.GetBytes(endian.Convert(e_ehsize)), 0, data, 0x28, 2);
                Array.Copy(BitConverter.GetBytes(endian.Convert(e_phentsize)), 0, data, 0x2A, 2);
                Array.Copy(BitConverter.GetBytes(endian.Convert(e_phnum)), 0, data, 0x2C, 2);
                Array.Copy(BitConverter.GetBytes(endian.Convert(e_shentsize)), 0, data, 0x2E, 2);
                Array.Copy(BitConverter.GetBytes(endian.Convert(e_shnum)), 0, data, 0x30, 2);
                Array.Copy(BitConverter.GetBytes(endian.Convert(e_shstrndx)), 0, data, 0x32, 2);
            }
            else
            {
                Array.Copy(BitConverter.GetBytes(endian.Convert(e_type)), 0, data, 0x10, 2);
                Array.Copy(BitConverter.GetBytes(endian.Convert(e_machine)), 0, data, 0x12, 2);
                Array.Copy(BitConverter.GetBytes(endian.Convert(e_version)), 0, data, 0x14, 4);
                Array.Copy(BitConverter.GetBytes(endian.Convert(e_entry)), 0, data, 0x18, 8);
                Array.Copy(BitConverter.GetBytes(endian.Convert(e_phoff)), 0, data, 0x20, 8);
                Array.Copy(BitConverter.GetBytes(endian.Convert(e_shoff)), 0, data, 0x28, 8);
                Array.Copy(BitConverter.GetBytes(endian.Convert(e_flags)), 0, data, 0x30, 4);
                Array.Copy(BitConverter.GetBytes(endian.Convert(e_ehsize)), 0, data, 0x34, 2);
                Array.Copy(BitConverter.GetBytes(endian.Convert(e_phentsize)), 0, data, 0x36, 2);
                Array.Copy(BitConverter.GetBytes(endian.Convert(e_phnum)), 0, data, 0x38, 2);
                Array.Copy(BitConverter.GetBytes(endian.Convert(e_shentsize)), 0, data, 0x3A, 2);
                Array.Copy(BitConverter.GetBytes(endian.Convert(e_shnum)), 0, data, 0x3C, 2);
                Array.Copy(BitConverter.GetBytes(endian.Convert(e_shstrndx)), 0, data, 0x3E, 2);
            }
        }

        /// <summary>
        /// Size of header
        /// </summary>
        public Int32 DataSize
        {
            get;
            private set;
        }

        public Boolean IsElf
        {
            get
            {
                return e_ident[(int)ElfIdent.EI_MAG0] == '\x7F' &&
                        e_ident[(int)ElfIdent.EI_MAG1] == 'E' &&
                        e_ident[(int)ElfIdent.EI_MAG2] == 'L' &&
                        e_ident[(int)ElfIdent.EI_MAG3] == 'F';
            }
        }

        /// <summary>
        /// ELF identification
        /// </summary>
        public Byte[] Identification
        {
            get
            {
                return e_ident;
            }
            set
            {
                e_ident = value;
                switch (Class)
                {
                    case ElfClass.EC_32:
                        DataSize = 0x34;
                        break;
                    case ElfClass.EC_64:
                        DataSize = 0x40;
                        break;
                    default:
                        DataSize = -1;
                        break;
                }
            }
        }

        /// <summary>
        /// Magic number
        /// </summary>
        public Byte[] Magic
        {
            get
            {
                Byte[] magic = new Byte[(int)ElfIdent.EI_MAG3 + 1];
                Array.Copy(e_ident, (int)ElfIdent.EI_MAG0, magic, 0, (int)ElfIdent.EI_MAG3 + 1);
                return magic;
            }
            set
            {
                Array.Copy(value, 0, e_ident, (int)ElfIdent.EI_MAG0, (int)ElfIdent.EI_MAG3 + 1);
            }
        }

        /// <summary>
        /// File class
        /// </summary>
        public ElfClass Class
        {
            get
            {
                return (ElfClass)e_ident[(int)ElfIdent.EI_CLASS];
            }
            set
            {
                e_ident[(int)ElfIdent.EI_CLASS] = (Byte)value;
            }
        }

        /// <summary>
        /// Data encoding
        /// </summary>
        public ElfData Encoding
        {
            get
            {
                return (ElfData)e_ident[(int)ElfIdent.EI_DATA];
            }
            set
            {
                e_ident[(int)ElfIdent.EI_DATA] = (Byte)value;
            }
        }

        /// <summary>
        /// File version
        /// </summary>
        public Byte IDVersion
        {
            get
            {
                return e_ident[(int)ElfIdent.EI_VERSION];
            }
            set
            {
                e_ident[(int)ElfIdent.EI_VERSION] = value;
            }
        }

        /// <summary>
        /// Object file type
        /// </summary>
        public ElfType Type
        {
            get
            {
                return (ElfType)e_type;
            }
            set
            {
                e_type = (UInt16)value;
            }
        }

        /// <summary>
        /// Machine type
        /// </summary>
        public ElfMachine MachineType
        {
            get
            {
                return (ElfMachine)e_machine;
            }
            set
            {
                e_machine = (UInt16)value;
            }
        }

        /// <summary>
        /// Object file version
        /// </summary>
        public ElfVersion Version
        {
            get
            {
                return (ElfVersion)e_version;
            }
            set
            {
                e_version = (UInt32)value;
            }
        }

        /// <summary>
        /// Entry point address
        /// </summary>
        public UInt64 EntryPoint
        {
            get
            {
                return e_entry;
            }
            set
            {
                e_entry = value;
            }
        }

        /// <summary>
        /// Program header offset
        /// </summary>
        public UInt64 ProgramHeaderOffset
        {
            get
            {
                return e_phoff;
            }
            set
            {
                e_phoff = value;
            }
        }

        /// <summary>
        /// Section header offset
        /// </summary>
        public UInt64 SectionHeaderOffset
        {
            get
            {
                return e_shoff;
            }
            set
            {
                e_shoff = value;
            }
        }

        /// <summary>
        /// Processor-specific flags
        /// </summary>
        public UInt32 Flags
        {
            get
            {
                return e_flags;
            }
            set
            {
                e_flags = value;
            }
        }

        /// <summary>
        /// ELF header size
        /// </summary>
        public UInt16 HeaderSize
        {
            get
            {
                return e_ehsize;
            }
            set
            {
                e_ehsize = value;
            }
        }

        /// <summary>
        /// Size of program header entry
        /// </summary>
        public UInt16 ProgramHeaderSize
        {
            get
            {
                return e_phentsize;
            }
            set
            {
                e_phentsize = value;
            }
        }

        /// <summary>
        /// Number of program header entries
        /// </summary>
        public UInt16 ProgramHeaderCount
        {
            get
            {
                return e_phnum;
            }
            set
            {
                e_phnum = value;
            }
        }

        /// <summary>
        /// Size of section header entry
        /// </summary>
        public UInt16 SectionHeaderSize
        {
            get
            {
                return e_shentsize;
            }
            set
            {
                e_shentsize = value;
            }
        }

        /// <summary>
        /// Number of section header entries
        /// </summary>
        public UInt16 SectionHeaderCount
        {
            get
            {
                return e_shnum;
            }
            set
            {
                e_shnum = value;
            }
        }

        /// <summary>
        /// Section name string table index
        /// </summary>
        public UInt16 StringTable
        {
            get
            {
                return e_shstrndx;
            }
            set
            {
                e_shstrndx = value;
            }
        }
    }

    public class ElfPHDR
    {
        UInt32 p_type;
        UInt32 p_flags;
        UInt64 p_offset;
        UInt64 p_vaddr;
        UInt64 p_paddr;
        UInt64 p_filesz;
        UInt64 p_memsz;
        UInt64 p_align;

        public ElfPHDR(ElfClass elfclass)
        {
            if (elfclass == ElfClass.EC_32)
                DataSize = 0x20;
            else
                DataSize = 0x38;
        }

        public override string ToString() =>
            String.Format("Type = {0}\nFlags = {1}\nOffset = 0x{2:X8} => 0x{8:X8} => 0x{9:X8}\nVaddr = 0x{3:X8}\nPaddr = 0x{4:X8}\nFilesz = 0x{5:X8}\nMemsz = 0x{6:X8}\nAlign = 0x{7:X8}\n\n",
                    Type,
                    Flags,
                    FileOffset,
                    VirtualAddress,
                    PhysicalAddress,
                    FileSize,
                    MemorySize,
                    Align,
                    FileOffset + FileSize,
                    (FileOffset + FileSize + Align - 1) & ~(Align - 1));

        public void Load(ElfEndian endian, ElfClass elfclass, Byte[] data)
        {
            if (elfclass == ElfClass.EC_32)
            {
                p_type = endian.Convert(BitConverter.ToUInt32(data, 0));
                p_offset = endian.Convert(BitConverter.ToUInt32(data, 4));
                p_vaddr = endian.Convert(BitConverter.ToUInt32(data, 8));
                p_paddr = endian.Convert(BitConverter.ToUInt32(data, 0xC));
                p_filesz = endian.Convert(BitConverter.ToUInt32(data, 0x10));
                p_memsz = endian.Convert(BitConverter.ToUInt32(data, 0x14));
                p_flags = endian.Convert(BitConverter.ToUInt32(data, 0x18));
                p_align = endian.Convert(BitConverter.ToUInt32(data, 0x1C));
            }
            else
            {
                p_type = endian.Convert(BitConverter.ToUInt32(data, 0));
                p_flags = endian.Convert(BitConverter.ToUInt32(data, 4));
                p_offset = endian.Convert(BitConverter.ToUInt64(data, 8));
                p_vaddr = endian.Convert(BitConverter.ToUInt64(data, 0x10));
                p_paddr = endian.Convert(BitConverter.ToUInt64(data, 0x18));
                p_filesz = endian.Convert(BitConverter.ToUInt64(data, 0x20));
                p_memsz = endian.Convert(BitConverter.ToUInt64(data, 0x28));
                p_align = endian.Convert(BitConverter.ToUInt64(data, 0x30));
            }
        }

        public void Save(ElfEndian endian, ElfClass elfclass, ref Byte[] data)
        {
            if (elfclass == ElfClass.EC_32)
            {
                Array.Copy(BitConverter.GetBytes(endian.Convert(p_type)), 0, data, 0, 4);
                Array.Copy(BitConverter.GetBytes(endian.Convert((UInt32)p_offset)), 0, data, 4, 4);
                Array.Copy(BitConverter.GetBytes(endian.Convert((UInt32)p_vaddr)), 0, data, 8, 4);
                Array.Copy(BitConverter.GetBytes(endian.Convert((UInt32)p_paddr)), 0, data, 0xC, 4);
                Array.Copy(BitConverter.GetBytes(endian.Convert((UInt32)p_filesz)), 0, data, 0x10, 4);
                Array.Copy(BitConverter.GetBytes(endian.Convert((UInt32)p_memsz)), 0, data, 0x14, 4);
                Array.Copy(BitConverter.GetBytes(endian.Convert(p_flags)), 0, data, 0x18, 4);
                Array.Copy(BitConverter.GetBytes(endian.Convert((UInt32)p_align)), 0, data, 0x1C, 4);
            }
            else
            {
                Array.Copy(BitConverter.GetBytes(endian.Convert(p_type)), 0, data, 0, 4);
                Array.Copy(BitConverter.GetBytes(endian.Convert(p_flags)), 0, data, 4, 4);
                Array.Copy(BitConverter.GetBytes(endian.Convert(p_offset)), 0, data, 8, 8);
                Array.Copy(BitConverter.GetBytes(endian.Convert(p_vaddr)), 0, data, 0x10, 8);
                Array.Copy(BitConverter.GetBytes(endian.Convert(p_paddr)), 0, data, 0x18, 8);
                Array.Copy(BitConverter.GetBytes(endian.Convert(p_filesz)), 0, data, 0x20, 8);
                Array.Copy(BitConverter.GetBytes(endian.Convert(p_memsz)), 0, data, 0x28, 8);
                Array.Copy(BitConverter.GetBytes(endian.Convert(p_align)), 0, data, 0x30, 8);
            }
        }

        /// <summary>
        /// Size of header
        /// </summary>
        public Int32 DataSize
        {
            get;
            private set;
        }

        /// <summary>
        /// Type of segment
        /// </summary>
        public ElfProgramType Type
        {
            get
            {
                return (ElfProgramType)p_type;
            }
            set
            {
                p_type = (UInt32)value;
            }
        }

        /// <summary>
        /// Segment attributes
        /// </summary>
        public ElfProgramFlags Flags
        {
            get
            {
                return (ElfProgramFlags)p_flags;
            }
            set
            {
                p_flags = (UInt32)value;
            }
        }

        /// <summary>
        /// Offset in file
        /// </summary>
        public UInt64 FileOffset
        {
            get
            {
                return p_offset;
            }
            set
            {
                p_offset = value;
            }
        }

        /// <summary>
        /// Virtual address in memory
        /// </summary>
        public UInt64 VirtualAddress
        {
            get
            {
                return p_vaddr;
            }
            set
            {
                p_vaddr = value;
            }
        }

        /// <summary>
        /// Physical address in memory
        /// </summary>
        public UInt64 PhysicalAddress
        {
            get
            {
                return p_paddr;
            }
            set
            {
                p_paddr = value;
            }
        }

        /// <summary>
        /// Size of segment in file
        /// </summary>
        public UInt64 FileSize
        {
            get
            {
                return p_filesz;
            }
            set
            {
                p_filesz = value;
            }
        }

        /// <summary>
        /// Size of segment in memory
        /// </summary>
        public UInt64 MemorySize
        {
            get
            {
                return p_memsz;
            }
            set
            {
                p_memsz = value;
            }
        }

        /// <summary>
        /// Alignment of segment
        /// </summary>
        public UInt64 Align
        {
            get
            {
                return p_align;
            }
            set
            {
                p_align = value;
            }
        }
    }

    public class ElfSHDR
    {
        UInt32 sh_name;
        UInt32 sh_type;
        UInt64 sh_flags;
        UInt64 sh_addr;
        UInt64 sh_offset;
        UInt64 sh_size;
        UInt32 sh_link;
        UInt32 sh_info;
        UInt64 sh_addralign;
        UInt64 sh_entsize;

        public ElfSHDR(ElfClass elfclass)
        {
            if (elfclass == ElfClass.EC_32)
                DataSize = 0x28;
            else
                DataSize = 0x40;
        }

        public override String ToString() =>
            String.Format("Name: 0x{0:X4}\nType: {1}\nFlags: {2}\nAddress: 0x{3:X8}\nOffset: 0x{4:X8} => 0x{10:X8} => 0x{11:X8}\nSize: 0x{5:X8}\nLink: 0x{6:X4}\nInfo: 0x{7:X4}\nAlign: 0x{8:X8}\nEntries: 0x{9:X8}\n\n",
                            Name,
                            Type,
                            Flags,
                            Address,
                            FileOffset,
                            Size,
                            Link,
                            Info,
                            Align,
                            EntrySize,
                            FileOffset + Size,
                            (FileOffset + Size + Align - 1) & ~(Align - 1));

        public void Load(ElfEndian endian, ElfClass elfclass, Byte[] data)
        {
            if (elfclass == ElfClass.EC_32)
            {
                sh_name = endian.Convert(BitConverter.ToUInt32(data, 0));
                sh_type = endian.Convert(BitConverter.ToUInt32(data, 4));
                sh_flags = endian.Convert(BitConverter.ToUInt32(data, 8));
                sh_addr = endian.Convert(BitConverter.ToUInt32(data, 0xC));
                sh_offset = endian.Convert(BitConverter.ToUInt32(data, 0x10));
                sh_size = endian.Convert(BitConverter.ToUInt32(data, 0x14));
                sh_link = endian.Convert(BitConverter.ToUInt32(data, 0x18));
                sh_info = endian.Convert(BitConverter.ToUInt32(data, 0x1C));
                sh_addralign = endian.Convert(BitConverter.ToUInt32(data, 0x20));
                sh_entsize = endian.Convert(BitConverter.ToUInt32(data, 0x24));
            }
            else
            {
                sh_name = endian.Convert(BitConverter.ToUInt32(data, 0));
                sh_type = endian.Convert(BitConverter.ToUInt32(data, 4));
                sh_flags = endian.Convert(BitConverter.ToUInt64(data, 8));
                sh_addr = endian.Convert(BitConverter.ToUInt64(data, 0x10));
                sh_offset = endian.Convert(BitConverter.ToUInt64(data, 0x18));
                sh_size = endian.Convert(BitConverter.ToUInt64(data, 0x20));
                sh_link = endian.Convert(BitConverter.ToUInt32(data, 0x28));
                sh_info = endian.Convert(BitConverter.ToUInt32(data, 0x2C));
                sh_addralign = endian.Convert(BitConverter.ToUInt64(data, 0x30));
                sh_entsize = endian.Convert(BitConverter.ToUInt64(data, 0x38));
            }
        }

        public void Save(ElfEndian endian, ElfClass elfclass, ref Byte[] data)
        {
            if (elfclass == ElfClass.EC_32)
            {
                Array.Copy(BitConverter.GetBytes(endian.Convert(sh_name)), 0, data, 0, 4);
                Array.Copy(BitConverter.GetBytes(endian.Convert(sh_type)), 0, data, 4, 4);
                Array.Copy(BitConverter.GetBytes(endian.Convert((UInt32)sh_flags)), 0, data, 8, 4);
                Array.Copy(BitConverter.GetBytes(endian.Convert((UInt32)sh_addr)), 0, data, 0xC, 4);
                Array.Copy(BitConverter.GetBytes(endian.Convert((UInt32)sh_offset)), 0, data, 0x10, 4);
                Array.Copy(BitConverter.GetBytes(endian.Convert((UInt32)sh_size)), 0, data, 0x14, 4);
                Array.Copy(BitConverter.GetBytes(endian.Convert(sh_link)), 0, data, 0x18, 4);
                Array.Copy(BitConverter.GetBytes(endian.Convert(sh_info)), 0, data, 0x1C, 4);
                Array.Copy(BitConverter.GetBytes(endian.Convert((UInt32)sh_addralign)), 0, data, 0x20, 4);
                Array.Copy(BitConverter.GetBytes(endian.Convert((UInt32)sh_entsize)), 0, data, 0x24, 4);
            }
            else
            {
                Array.Copy(BitConverter.GetBytes(endian.Convert(sh_name)), 0, data, 0, 4);
                Array.Copy(BitConverter.GetBytes(endian.Convert(sh_type)), 0, data, 4, 4);
                Array.Copy(BitConverter.GetBytes(endian.Convert(sh_flags)), 0, data, 8, 8);
                Array.Copy(BitConverter.GetBytes(endian.Convert(sh_addr)), 0, data, 0x10, 8);
                Array.Copy(BitConverter.GetBytes(endian.Convert(sh_offset)), 0, data, 0x18, 8);
                Array.Copy(BitConverter.GetBytes(endian.Convert(sh_size)), 0, data, 0x20, 8);
                Array.Copy(BitConverter.GetBytes(endian.Convert(sh_link)), 0, data, 0x28, 4);
                Array.Copy(BitConverter.GetBytes(endian.Convert(sh_info)), 0, data, 0x2C, 4);
                Array.Copy(BitConverter.GetBytes(endian.Convert(sh_addralign)), 0, data, 0x30, 8);
                Array.Copy(BitConverter.GetBytes(endian.Convert(sh_entsize)), 0, data, 0x38, 8);
            }
        }

        public Int32 DataSize
        {
            get;
            private set;
        }

        /// <summary>
        /// Section name
        /// </summary>
        public UInt32 Name
        {
            get
            {
                return sh_name;
            }
            set
            {
                sh_name = value;
            }
        }

        /// <summary>
        /// Section type
        /// </summary>
        public ElfSectionType Type
        {
            get
            {
                return (ElfSectionType)sh_type;
            }
            set
            {
                sh_type = (UInt32)value;
            }
        }

        /// <summary>
        /// Section attributes
        /// </summary>
        public ElfSectionFlags Flags
        {
            get
            {
                return (ElfSectionFlags)sh_flags;
            }
            set
            {
                sh_flags = (UInt64)value;
            }
        }

        /// <summary>
        /// Virtual address in memory
        /// </summary>
        public UInt64 Address
        {
            get
            {
                return sh_addr;
            }
            set
            {
                sh_addr = value;
            }
        }

        /// <summary>
        /// Offset in file
        /// </summary>
        public UInt64 FileOffset
        {
            get
            {
                return sh_offset;
            }
            set
            {
                sh_offset = value;
            }
        }

        /// <summary>
        /// Size of section
        /// </summary>
        public UInt64 Size
        {
            get
            {
                return sh_size;
            }
            set
            {
                sh_size = value;
            }
        }

        /// <summary>
        /// Link to other section
        /// </summary>
        public UInt32 Link
        {
            get
            {
                return sh_link;
            }
            set
            {
                sh_link = value;
            }
        }

        /// <summary>
        /// Miscellaneous information
        /// </summary>
        public UInt32 Info
        {
            get
            {
                return sh_info;
            }
            set
            {
                sh_info = value;
            }
        }

        /// <summary>
        /// Address alignment boundary
        /// </summary>
        public UInt64 Align
        {
            get
            {
                return sh_addralign;
            }
            set
            {
                sh_addralign = value;
            }
        }

        /// <summary>
        /// Size of entries, if section has table
        /// </summary>
        public UInt64 EntrySize
        {
            get
            {
                return sh_entsize;
            }
            set
            {
                sh_entsize = value;
            }
        }
    }
}