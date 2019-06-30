using Gee.External.Capstone;
using Gee.External.Capstone.X86;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ps4sdk_ps4KernelDlSym_List_Creator
{
    public partial class Main : Form
    {
        private SymbolPool sympool = new SymbolPool(); 
        private ElfIO.ElfIO elfio;
        private String ps4ElfPath;
        private Byte[] ps4ElfBuffer;
        private Int32 ps4KernelDlSym_offset;
        private String resultString;
        //private List<String> ps4KernelDlSym_Symbols = new List<String>();

        #region "GUI"
        public Main()
        {
            InitializeComponent();
            AllowDrop = true;
            DragEnter += Main_DragEnter;
            DragDrop += Main_DragDrop;
        }

        private void Main_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Move;
        }

        private void Main_DragDrop(object sender, DragEventArgs e)
        {
            textBox.Text = ps4ElfPath = "";
            String[] files = (String[])e.Data.GetData(DataFormats.FileDrop);

            foreach (String file in files)
            {
                if (file.EndsWith("elf"))
                    ps4ElfPath = file;
            }

            bool elfLoaded = ProcessPS4Elf(ps4ElfPath);
            UpdateControls(elfLoaded);
        }


        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select a ps4sdk (Hitodamas sdk) compiled elf ";
                ofd.Filter = "elf files|*.elf;*.self";
                ofd.Multiselect = false;
                ofd.CheckFileExists = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                    ps4ElfPath = ofd.FileName;
            }
            bool elfLoaded = ProcessPS4Elf(ps4ElfPath);
            UpdateControls(elfLoaded);
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
            => UpdateControls(ProcessPS4Elf(ps4ElfPath));

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ps4ElfPath = "";
            ps4ElfBuffer = null;
            UpdateControls();
        }

        private void copyToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
            => Clipboard.SetText(resultString);

        private void toolStripButton1_Click(object sender, EventArgs e)
            => MessageBox.Show("This tool is made for ps4sdk compiled executables.\nJust drag and drop a binary into the form or use the File->Open strip to load a file.\nThe tool will output all ps4KernelDlSym symbol names, which are used in the payload!", "Help");

        private void UpdateResultOutput(Object sender, EventArgs e)
        {
            var option = sender.ToString();

            if (option == "Raw" & !rawToolStripMenuItem.Checked)
            {
                rawToolStripMenuItem.Checked = true;
                cArrayToolStripMenuItem.Checked = false;
            }
            else if (option == "C-Array" & !cArrayToolStripMenuItem.Checked)
            {
                rawToolStripMenuItem.Checked = false;
                cArrayToolStripMenuItem.Checked = true;
            }
            else if (option == "Name" & !nameToolStripMenuItem.Checked)
            {
                nameToolStripMenuItem.Checked = true;
                executionToolStripMenuItem.Checked = false;
            }
            else if (option == "Execution" & !executionToolStripMenuItem.Checked)
            {
                nameToolStripMenuItem.Checked = false;
                executionToolStripMenuItem.Checked = true;
            }

            UpdateControls(textBox.Visible);
        }

        public void UpdateControls(Boolean state = false)
        {
            if(state)
            {
                bool isRaw = rawToolStripMenuItem.Checked;
                bool isSorted = nameToolStripMenuItem.Checked;
                //String[] sym_array = new String[ps4KernelDlSym_Symbols.Count];
                var result = "";

                //Array.Copy(ps4KernelDlSym_Symbols.ToArray(), sym_array, ps4KernelDlSym_Symbols.Count);
                var sym_pool = sympool.Get;
                if (!(state = sym_pool != null))
                    goto update;

                var sym_count = sym_pool.Count;
                if (sym_count < 1)
                    goto update;

                var sym_list = sym_pool.GetRange(0, sym_count);//sym_array.ToList();

                // order style
                if (isSorted)
                    sym_list.Sort();

                result = String.Join("\n", sym_list);

                // output format
                if(!isRaw) // c style array
                {
                    result = "sym_t symtable[] =\n{\n";
                    Array.ForEach(sym_list.ToArray(), s => result += "\t{ \"" + s + "\", 0xDEADC0DE },\n");
                    result += "};";
                }

                if (resultString != "" & textBox.Text.Contains(resultString))
                    textBox.Text = textBox.Text.Replace(resultString, result);
                else
                    textBox.Text += result;

                resultString = result;
            }

            update:
            textBox.Visible = reloadToolStripMenuItem.Visible = closeToolStripMenuItem.Visible = resultToolStripDropDownButton.Visible = state;
            if (state)
            {
                labelLoadedFile.Text = "Loaded file: " + Path.GetFileName(ps4ElfPath) ?? "/";
                reloadToolStripMenuItem.Text = "Reload: " + Path.GetFileName(ps4ElfPath) ?? "/";
            }
        }
        #endregion

        private UInt64 GetMemoryAddress(UInt64 offset) => elfio.Elf.Header.EntryPoint | offset;

        private UInt64 GetPhysicalAddress(UInt64 offset) => offset & 0xFFFFFF;

        private Int32 ps4KernelDlSymRetrieveOffset(Byte[] buffer)
        {
            Byte[] ps4KernelDlSym_buffer =
            {
                0x55,               // push rbp
                0x48, 0x89, 0xE5,   // mov  rbp, rsp
                0x41, 0x57,         // push r15
                0x41, 0x56,         // push r14
                0x41, 0x54,         // push r12
                0x53,               // push rbx
                0x49, 0x89, 0xFC,   // mov  r12, rdi
                0x45, 0x31, 0xF6,   // xor  r14d, r14d
                0x4D, 0x85, 0xE4    // test r12, r12
            };
            return SearchBytes(buffer, ps4KernelDlSym_buffer);
        }

        private string GetInstructionTxt(X86Instruction instruction)
        {
            if (instruction == null)
                return "INVALID";

            if (!instruction.IsDietModeEnabled)
                return string.Format("{0:X}: \t {1} \t {2}", instruction.Address, instruction.Mnemonic, instruction.Operand) + string.Format("\t Instruction Id = {0}", instruction.Id);

            return string.Format("{0:X}:", instruction.Address) + string.Format("\t Id = {0}", instruction.Id);
        }

        private String ParseSymbolReference(X86Instruction instruction, out String reg)
        {
            String symbolName = "", operandTarget = "";
            reg = "";
            try
            {
                var operand = instruction.Operand.Split(',');
                reg = operand[0];
                operandTarget = operand[1];
                if (!operandTarget.Contains("0x"))
                {
                    //MessageBox.Show("Invalid operand: " + operandTarget);
                    //throw new Exception("Invalid operand");
                    return symbolName;
                }

                operandTarget = operandTarget.Replace(" ", "").Replace("0x", "");
                UInt64 symbolOffset = Convert.ToUInt64(operandTarget, 16);
                if (symbolOffset != 0)
                {
                    UInt64 physicalSymbolOffset = GetPhysicalAddress(symbolOffset);
                    symbolName = Encoding.Default.GetString(ps4ElfBuffer, (int)physicalSymbolOffset, 32);
                    if (symbolName.Contains("\0"))
                        symbolName = new String(symbolName.Take(symbolName.IndexOf("\0")).ToArray());
                }
            }
            catch (Exception e)
            {
                /*
                var addr = GetMemoryAddress((ulong)instruction.Address) - 0x1000;
                Clipboard.SetText(addr.ToString("X2"));
                MessageBox.Show("Could not parse opereand " + operandTarget + " symbol offset at address " + addr.ToString("X2") + "\n\n" + e.ToString());
                */
            }

            return symbolName;
        }

        class SymbolPool
        {
            List<String> Symbols;
            
            public SymbolPool()
            {
                Symbols = new List<String>();
            }

            public void Add(Int64 address, String symbol)
            {
                if (address > 0 & symbol != "")
                    Symbols.Add($"{address}|{symbol}");
            }

            public void Clear() => Symbols.Clear();

            public List<String> Get
            {
                get
                {
                    int count = Symbols.Count;
                    if (count <= 0)
                        return null;//throw new NullReferenceException();

                    var sym_list = Symbols.GetRange(0, count);
                    for (var i = 0; i < count; i++)
                        sym_list[i] = sym_list[i].Split('|')[1];

                    return sym_list.Distinct().ToList();
                }
            }

            public Int32 GetRefCount()
            {
                int count = Symbols.Count, ref_count = -1;
                if (count <= 0)
                    goto ret;

                var sym_list = Symbols.GetRange(0, count).Distinct().ToList();
                ref_count = sym_list.Count;

                ret:
                return ref_count;
            }

            public override String ToString() => String.Join("\n", Symbols);
        }

        private void ps4KernelDlSymRetrieveSymbols(Byte[] buffer)
        {
            using (CapstoneX86Disassembler disassembler = CapstoneDisassembler.CreateX86Disassembler(X86DisassembleMode.Bit64))
            {
                disassembler.EnableInstructionDetails = true;
                disassembler.DisassembleSyntax = DisassembleSyntax.Intel;
                //disassembler.EnableSkipDataMode = true;
                X86Instruction[] instructions = disassembler.Disassemble(buffer);
                int i = 0;
                foreach (X86Instruction instruction in instructions)
                {
                    i++;
                    X86Instruction lastInsn = (i > 1) ? instructions[i - 2] : null;
                    long address = instruction.Address;
                    X86InstructionId id = instruction.Id;
                    String curr_instruction = GetInstructionTxt(instruction);
                    String last_instruction = GetInstructionTxt(lastInsn);

                    //if (address == 0x14362)
                       // MessageBox.Show(curr_instruction);

                    if (!instruction.IsSkippedData)
                    {
                        String ps4KernelDlSym = GetMemoryAddress((UInt64)ps4KernelDlSym_offset).ToString("x");
                        //if (instruction.Operand.Contains(ps4KernelDlSym))
                        //  MessageBox.Show("call to ps4KernelDlSym: " + curr_instruction);
                        //if(id == X86InstructionId.X86_INS_MOVABS)
                        //  MessageBox.Show("moveabs: " + curr_instruction);
                        //MessageBox.Show(ps4KernelDlSym);
                        
                        if (instruction.Operand.Contains(ps4KernelDlSym) && id == X86InstructionId.X86_INS_MOVABS)
                        {
                            String ps4KernelDlSym_call = instructions[i + 1].Operand;//instructions[i + 1].Id == X86InstructionId.X86_INS_CALL ? 
                            string last_operand = lastInsn.Operand;
                            if (lastInsn.Id == X86InstructionId.X86_INS_MOVABS)
                            {
                                String symbolReg = "";
                                var symbolName = ParseSymbolReference(lastInsn, out symbolReg);
                                if (symbolName != "")
                                    sympool.Add(lastInsn.Address, symbolName);//ps4KernelDlSym_Symbols.Add(symbolName);

                                // after we have got a symbol, find for near ps4KernelDlSym references
                                var targetReg = instructions[i].Operand;
                                var targetBytes = instructions[i].Bytes;
                                //MessageBox.Show(targetReg);
                                //MessageBox.Show(GetInstructionTxt(instructions[i+1]));
                                for (int x = i + 2; x < instructions.Count(); x++)
                                {
                                    var instruct = instructions[x];
                                    if (instruct == null || instruct.IsSkippedData || instruct.Id == X86InstructionId.X86_INS_RET || instruct.Id == X86InstructionId.X86_INS_MOVABS & instruct.Operand.Split(',')[0] == targetReg)//GetInstructionTxt(instruct).Contains("ret"))
                                        break;

                                    if (instruct.Id == X86InstructionId.X86_INS_CALL)// && instruct.Operand == ps4KernelDlSym_call.Split(',')[0])
                                    {
                                        //MessageBox.Show(GetInstructionTxt(instruct));
                                        //MessageBox.Show(instruct.Operand + "\n\n" + ps4KernelDlSym_call);
                                        var reg = instruct.Operand;
                                        var prev_sym_reg = symbolReg;
                                        if (reg == targetReg & (symbolName = ParseSymbolReference(instructions[x - 1], out symbolReg)) != "")
                                        {
                                            if(targetBytes.SequenceEqual(instruct.Bytes))//if ((symbolName = ParseSymbolReference(instructions[x - 1], out symbolReg)) != "")
                                            {
                                                if (symbolReg == prev_sym_reg)
                                                {
                                                    sympool.Add(instructions[x - 1].Address, symbolName);
                                                    /*
                                                    var addr = GetMemoryAddress((ulong)instructions[x - 1].Address) - 0x1000;
                                                    Clipboard.SetText(addr.ToString("X2"));
                                                    MessageBox.Show("Addr: " + addr.ToString("X2") + ", " + symbolName + "\n\n" + GetInstructionTxt(instructions[x-1]));
                                                    */
                                                }
                                                //MessageBox.Show(last_instruction + "\n" + curr_instruction + "\n\n" + GetInstructionTxt(instructions[x-1]) + "\n" + GetInstructionTxt(instruct), "Near " + symbolName);


                                            }
                                        }
                                        //MessageBox.Show(curr_instruction + "\n" + GetInstructionTxt(instruct));
                                    }
                                    //MessageBox.Show(last_instruction + "\n" + curr_instruction + "\n\n" + targetReg);
                                }
                            }
                            /*else if (id == X86InstructionId.X86_INS_CALL)
                            {
                                MessageBox.Show(last_instruction + "\n" + curr_instruction);
                            }*/
                        }
                    }
                }
            }
        }

        private bool ProcessPS4Elf(String elf_path)
        {
            if (!File.Exists(elf_path))
                return false;

            sympool.Clear();
            textBox.Text = resultString = "";
            ps4ElfBuffer = File.ReadAllBytes(elf_path);
            elfio = new ElfIO.ElfIO();
            elfio.Load(elf_path);
            UInt64 entrypoint = elfio.Elf.Header.EntryPoint;
            textBox.Text += "entrypoint: 0x" + entrypoint.ToString("X2") + "; ps4ElfBuffer size: " + ps4ElfBuffer.Length + "\n";
            bool loaded = ps4ElfBuffer != null;
            if (loaded)
            {
                ps4KernelDlSym_offset = ps4KernelDlSymRetrieveOffset(ps4ElfBuffer);
                if (ps4KernelDlSym_offset != ~0)
                    textBox.Text += "ps4KernelDlSym offset: 0x" + ps4KernelDlSym_offset.ToString("X2") + "\n";
                else
                    MessageBox.Show("Could not find ps4KernelDlSym in elf");


                foreach (var prog in elfio.Elf.Programs)
                {
                    var prog_size = prog.Header.FileSize;
                    var buffer = new Byte[prog_size];
                    Array.Copy(ps4ElfBuffer, (int)prog.Header.FileOffset, buffer, 0, (int)prog_size);
                    ps4KernelDlSymRetrieveSymbols(buffer);
                }

                var sym_count = sympool.Get.Count;
                var ref_count = sympool.GetRefCount();
                if (sym_count < 1 & ref_count == -1)
                    throw new Exception();

                textBox.Text += String.Format("Resolved {0} symbol names and observed {1} calls to ps4KernelDlSym\n\n", sym_count, ref_count);
                
            }
            else
                MessageBox.Show("Could not load");

            return loaded;
        }

        private static int SearchBytes(byte[] haystack, byte[] needle)
        {
            int len = needle.Length;
            int limit = haystack.Length - len;
            for (int j = 0; j <= limit; j++)
            {
                int i;
                for (i = 0; i < len && needle[i] == haystack[j + i]; i++) ;
                if (i == len)
                    return j;
            }
            return -1;
        }

    }
}
