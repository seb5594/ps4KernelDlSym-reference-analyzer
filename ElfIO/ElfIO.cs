using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ElfIO
{
    public class ElfIO
    {
        private Elf elf;

        public void Load(String filename)
        {
            // initialize stream
            FileStream stream = File.OpenRead(filename);

            // Read Elf ID
            ElfHDR elfhdr = new ElfHDR();
            Byte[] elfid = new Byte[(int)ElfIdent.EI_NIDENT];
            stream.Read(elfid, 0, elfid.Length);
            elfhdr.Identification = elfid;
            if (!elfhdr.IsElf)
                throw new ArgumentException("Given file is not an elf");

            // Initialize Elf endianness
            ElfEndian endian = new ElfEndian(elfhdr.Encoding);

            // Read Elf header
            Byte[] elfhdrData = new Byte[elfhdr.DataSize];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(elfhdrData, 0, elfhdr.DataSize);
            elfhdr.Load(endian, elfhdrData);

            // Read programs
            ElfProgram[] elfprogs = new ElfProgram[elfhdr.ProgramHeaderCount];
            Byte[] elfphdrData = new Byte[elfhdr.ProgramHeaderSize];
            for (int i = 0; i < elfhdr.ProgramHeaderCount; i++)
            {
                ElfPHDR phdr = new ElfPHDR(elfhdr.Class);
                stream.Seek((Int64)elfhdr.ProgramHeaderOffset + i * elfhdr.ProgramHeaderSize, SeekOrigin.Begin);
                stream.Read(elfphdrData, 0, elfhdr.ProgramHeaderSize);
                phdr.Load(endian, elfhdr.Class, elfphdrData);
                elfprogs[i] = new ElfProgram(phdr);
            }

            // Read sections
            ElfSection[] elfsects = new ElfSection[elfhdr.SectionHeaderCount];
            Byte[] elfshdrData = new Byte[elfhdr.SectionHeaderSize];
            for (int i = 0; i < elfhdr.SectionHeaderCount; i++)
            {
                ElfSHDR shdr = new ElfSHDR(elfhdr.Class);
                stream.Seek((Int64)elfhdr.SectionHeaderOffset + i * elfhdr.SectionHeaderSize, SeekOrigin.Begin);
                stream.Read(elfshdrData, 0, elfhdr.SectionHeaderSize);
                shdr.Load(endian, elfhdr.Class, elfshdrData);
                Byte[] section = null;
                if (shdr.Type != ElfSectionType.SHT_NOBITS && shdr.Type != ElfSectionType.SHT_NULL)
                {
                    section = new Byte[shdr.Size];
                    stream.Seek((Int64)shdr.FileOffset, SeekOrigin.Begin);
                    stream.Read(section, 0, section.Length);
                }
                elfsects[i] = new ElfSection(shdr, section);
            }

            // Initialize Elf
            elf = new Elf(elfhdr, elfprogs, elfsects);

            // Close stream
            stream.Close();
        }

        public void Save(String filename)
        {
            // initialize stream
            FileStream stream = File.Create(filename);

            // Initialize Elf endianness
            ElfEndian endian = new ElfEndian(elf.Header.Encoding);

            // Save Elf header
            ElfHDR hdr = elf.Header;
            Byte[] hdrData = new Byte[hdr.DataSize];
            elf.Header.Save(endian, ref hdrData);
            stream.Write(hdrData, 0, hdrData.Length);

            // Save programs
            Byte[] phdrData = new Byte[hdr.ProgramHeaderSize];
            stream.Seek((Int64)hdr.ProgramHeaderOffset, SeekOrigin.Begin);
            for (int i = 0; i < hdr.ProgramHeaderCount; i++)
            {
                // Save program headers
                elf.Programs[i].Header.Save(endian, hdr.Class, ref phdrData);
                stream.Write(phdrData, 0, phdrData.Length);
            }


            // Save sections
            Byte[] shdrData = new Byte[hdr.SectionHeaderSize];
            stream.Seek((Int64)hdr.SectionHeaderOffset, SeekOrigin.Begin);
            for (int i = 0; i < hdr.SectionHeaderCount; i++)
            {
                // Save section header
                ElfSHDR shdr = elf.Sections[i].Header;
                ElfSectionType type = shdr.Type;
                shdr.Save(endian, hdr.Class, ref shdrData);
                stream.Write(shdrData, 0, shdrData.Length);

                if (type != ElfSectionType.SHT_NULL && type != ElfSectionType.SHT_NOBITS)
                {
                    Byte[] section = elf.Sections[i].Section;
                    //stream.Write(elf.Sections[i].Section, 0, ;
                }
            }

            for (int i = 0; i < hdr.SectionHeaderCount; i++)
            {
                // Save section data
                ElfSection section = elf.Sections[i];
                ElfSectionType type = section.Header.Type;
                if (type != ElfSectionType.SHT_NULL && type != ElfSectionType.SHT_NOBITS)
                {
                    stream.Seek((Int64)section.Header.FileOffset, SeekOrigin.Begin);
                    stream.Write(section.Section, 0, (int)section.Header.Size);
                }
            }

            // Close stream
            stream.Close();
        }

        public Elf Elf
        {
            get
            {
                return elf;
            }
            set
            {
                elf = value;
            }
        }

        private void Read(ref Byte[] buffer, UInt32 address, Int32 count, Int32 offset)
        {
            if (count > 0)
            {
                for (int i = 0; i < elf.Header.SectionHeaderCount; i++)
                {
                    ElfSection section = elf.Sections[i];
                    ElfSHDR shdr = section.Header;
                    ElfSectionType type = shdr.Type;
                    if (type != ElfSectionType.SHT_NULL && type != ElfSectionType.SHT_NOBITS)
                    {
                        if (address >= shdr.Address && address < (shdr.Address + shdr.Size))
                        {
                            int soff = (int)(address - shdr.Address);
                            int num = Math.Min(count, (int)shdr.Size - soff);
                            Array.Copy(section.Section, soff, buffer, offset, num);
                            Read(ref buffer, address + (uint)num, count - num, offset + num);
                            return;
                        }
                    }
                }
            }
        }

        public void Read(ref Byte[] buffer, UInt32 address, Int32 count)
        {
            Read(ref buffer, address, count, 0);
        }

        private void Write(Byte[] buffer, UInt32 address, Int32 count, Int32 offset)
        {
            if (count > 0)
            {
                for (int i = 0; i < elf.Header.SectionHeaderCount; i++)
                {
                    ElfSection section = elf.Sections[i];
                    ElfSHDR shdr = section.Header;
                    ElfSectionType type = shdr.Type;
                    if (type != ElfSectionType.SHT_NULL && type != ElfSectionType.SHT_NOBITS)
                    {
                        if (address >= shdr.Address && address < (shdr.Address + shdr.Size))
                        {
                            int soff = (int)(address - shdr.Address);
                            int num = Math.Min(count, (int)shdr.Size - soff);
                            Array.Copy(buffer, offset, section.Section, soff, num);
                            Write(buffer, address + (uint)num, count - num, offset + num);
                            return;
                        }
                    }
                }
            }
        }

        public void Write(Byte[] buffer, UInt32 address, Int32 count)
        {
            Write(buffer, address, count, 0);
        }

        #region DEBUG
        public void Dump()
        {
            elf.DumpString();
        }
        #endregion
    }
}