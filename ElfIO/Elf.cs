using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ElfIO
{
    public class Elf
    {
        ElfHDR header;
        List<ElfProgram> programs;
        List<ElfSection> sections;

        public Elf(ElfHDR elfhdr, ElfProgram[] programs, ElfSection[] sections)
        {
            // Initialize elf header
            this.header = elfhdr;

            // Initialize program headers
            this.programs = new List<ElfProgram>(programs);

            // Initialize section headers
            this.sections = new List<ElfSection>(sections);
        }

        public ElfHDR Header
        {
            get
            {
                return header;
            }
            set
            {
                header = value;
            }
        }

        public List<ElfProgram> Programs
        {
            get
            {
                return programs;
            }
            set
            {
                programs = value;
            }
        }

        public List<ElfSection> Sections
        {
            get
            {
                return sections;
            }
            set
            {
                sections = value;
            }
        }

        #region DEBUG
        public void DumpString()
        {
            ElfHDR elfhdr = header;
            ElfPHDR elfphdr;// = programs;
            ElfSHDR elfshdr;

            // Initialze output file
            System.IO.BinaryWriter writer = new System.IO.BinaryWriter(System.IO.File.Create("dump.txt"));

            // Write Elf
            writer.Write(String.Format("****ELF HEADER****\n{0}\n", elfhdr.ToString()));

            // Write PHDR
            writer.Write("****PROGRAM HEADERS****\n");
            for (int i = 0; i < elfhdr.ProgramHeaderCount; i++)
            {
                writer.Write(String.Format("Program[{0}]\n{1}\n", i, programs[i].Header.ToString()));
            }

            // WRITE SHDR
            writer.Write("****SECTION HEADERS****\n");
            for (int i = 0; i < elfhdr.SectionHeaderCount; i++)
            {
                writer.Write(String.Format("Section[{0}]\n{1}\n", i, sections[i].Header.ToString()));
            }

            // Close writer
            writer.Close();
            writer.Dispose();
        }
        #endregion
    }
}