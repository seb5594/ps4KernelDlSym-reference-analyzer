using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ElfIO
{
    public static class ElfXtensions
    {
        public static Array Reverse(this Array a)
        {
            Array.Reverse(a);
            return a;
        }

        public static T FromByteArray<T>(this Byte[] @this)
        {
            return default(T);
        }

        public static Byte[] ToByteArray<T>(this T @this, Boolean reverse = false)
        {
            if (@this == null || @this is Byte[])
                return @this as Byte[];

            var type = typeof(T);
            var buffer = null as Byte[];

            Func<T, Byte[]> Serialize = obj =>
            {
                var ba = new Byte[Marshal.SizeOf(obj)];
                GCHandle gch = GCHandle.Alloc(ba, GCHandleType.Pinned);
                try
                {
                    Marshal.StructureToPtr(obj, gch.AddrOfPinnedObject(), false);
                }
                catch (Exception e)
                {
                    throw new Exception(String.Format("Could not serialize data of type '{0}'.", type.Name), e);
                }
                finally
                {
                    gch.Free();
                }
                return reverse ? ba.Reverse() as Byte[] : ba;
            };
            Func<String, Byte[]> StringToBytes = str => Encoding.ASCII.GetBytes(str + "\0");

            try
            {
                // indicate whether the datatype is a integral number ( (Un)signed Integer or Floating Points ) or a sequential structure
                if (type.IsValueType && type.IsSerializable || type.IsLayoutSequential)
                    buffer = Serialize(@this);
                else if (@this is String || @this is Char[])
                    buffer = StringToBytes(@this is String ? @this as String : new String(@this as Char[]));

                else if (@this is Array)
                {
                    var array = @this as Array;
                    var list = new List<Byte>();
                    for (var i = 0; i < array.Length; i++)
                    {
                        var val = array.GetValue(i);
                        if (val is String)
                            list.AddRange(StringToBytes(val as String));
                        else
                            list.AddRange(Serialize((T)val));
                    }
                    buffer = list.ToArray();
                }
            }
            catch (Exception ex) { }

            return buffer;
        }
    }
}