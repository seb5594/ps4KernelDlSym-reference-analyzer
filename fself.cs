using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ps4sdk_ps4KernelDlSym_List_Creator
{
    public class fself
    {
        private self_header shead = new self_header();

        public fself(Byte[] shead_buffer)
        {
            System.Windows.Forms.MessageBox.Show(BitConverter.IsLittleEndian.ToString());
            shead = shead_buffer.ToStructure<self_header>(true);
        }

        public self_header Get() => shead;

        [StructLayout(LayoutKind.Sequential)]
        public struct self_header
        {
            private static self_header _reference;

            public UInt32 magic;
            public UInt32 unk1;
            public Byte content_type;
            public Byte product_type;
            public UInt16 padding1;
            public UInt16 header_size;
            public UInt16 sig_size;
            public UInt32 self_size;
            public UInt32 seg_count;
            public UInt16 unk2;
            public UInt32 padding2;

            public static self_header InitByBuffer(Byte[] buffer)
            {
                return _reference = buffer.ToStructure<self_header>(true);
            }

            public static self_header Value
            {
                get => _reference;

                private set
                {
                    _reference = value;
                }
            }

            public override String ToString()
            {
                return $"self_header:\nmagic: {_reference.magic.ToString("X2")}\nunk1:{_reference.unk1:2}\ncontent_type: {_reference.content_type}\nproduct_type: {product_type}\nheader_size: {_reference.header_size:2}\nsig_size: {_reference.sig_size}\nself_size: {_reference.self_size:2}\nseg_count: {_reference.seg_count}";
            }

            /*public void Set(self_header shead)
            {

            }
            public self_header Get()
            {
                return default(self_header);
            }*/
        }
    }
}
