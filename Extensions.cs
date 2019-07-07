using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ps4sdk_ps4KernelDlSym_List_Creator
{
    public static class Extensions
    {
        public static Boolean IsNull<T>(this T @this)
            => @this is String ? String.IsNullOrEmpty(@this as String) : @this == null;

        public static T Reverse<T>(this T @this)
            => @this.Reverse(0, Marshal.SizeOf(@this));

        public static T Reverse<T>(this T @this, int index, int length)
            => (new T[] { @this }).Reverse(index, length).First();

        public static T[] Reverse<T>(this T[] @this, int index, int length)
        {
            if (@this.Count() < 1)
                return default(T[]);

            for (var i = 0; i < @this.Count(); i++)
            {
                var e_type = @this.GetValue(i).GetType();
                var e_name = e_type.GetField(e_type.Name);
                var e_size = Marshal.SizeOf(@this.GetValue(i));

                foreach(var field in e_type.GetFields())
                {
                    var f_type = field.GetType();
                    var f_tref = __makeref(f_type);

                    if (field.IsStatic || f_type == typeof(String))
                        continue;

                    // handle enums
                    if (f_type.IsEnum)
                        f_type = Enum.GetUnderlyingType(f_type);

                    var old_val = Convert.ChangeType(field.GetValueDirect(f_tref), f_type);
                    if(old_val is Array)
                        field.SetValueDirect(f_tref, old_val.Reverse());

                    field.SetValueDirect(f_tref, old_val.Reverse());
                }

                //@this[i] = @this[i].Reverse(Marshal.OffsetOf(e_type, e_name.ToString()).ToInt32(), e_size);
                //@this[i] = @this[i].Reverse(0, e_size);
                ////Array.Reverse(elem.ToBytes(), Marshal.OffsetOf(e_type, e_name.ToString()).ToInt32(), e_size);
            }

            return @this;
        }



        /*public static T Reverse<T>(this T @this)
        {
            try
            {
                foreach (var field in @this.GetType().GetFields())
                {
                    var f_type = field.GetType();
                    var f_tref = __makeref(@this);

                    if (field.IsStatic || f_type == typeof(String))
                        continue;

                    // handle enums
                    if (f_type.IsEnum)
                        f_type = Enum.GetUnderlyingType(f_type);

                    //MessageBox.Show("Handling°!");

                    foreach(field)

                    var old_val = field.GetValueDirect(f_tref);
                    field.SetValueDirect(f_tref, old_val.ToBytes().Reverse());

                    /*if (!f_type.IsArray)
                    {
                        field.SetValueDirect(__makeref(@this), field.GetRawConstantValue().Reverse());
                        MessageBox.Show("Reverse(): handled single object!");
                    }
                    else
                    {
                        foreach (var elem in Type.GetTypeArray(new Object[] { field.GetRawConstantValue() }))
                        {
                            elem.Reverse();
                        }
                        MessageBox.Show("Reverse(): handled array!");
                    }
                    

                    //return Type.field.GetRawConstantValue().Reverse();
                    //Array.Reverse(fn.Get//fn.Reverse();

                    // check for sub-fields to recurse if necessary
                    var subFields = f_type.GetFields().Where(subField => !subField.IsStatic).Where(f => f.GetCustomAttribute<CompilerGeneratedAttribute>() == null).ToArray();
                    
                    
                    if (subFields.Length == 0)
                    {
                        field.SetValueDirect(__makeref(@this), field.GetRawConstantValue().Reverse());
                        MessageBox.Show("Reverse(): handled single object!");

                        //return @this.GetTy.ToBytes<T[]>().Reverse<T>(Marshal.OffsetOf(f_type, field.Name).ToInt32(), Marshal.SizeOf(f_type));
                        //
                        //return @this.Reverse();
                        //return (@this.ToBytes<T[]>()).Reverse<T>(Marshal.OffsetOf(f_type, field.Name).ToInt32(), Marshal.SizeOf(f_type));
                        //Array.Reverse(obj.ToByteArray(), offset, Marshal.SizeOf(f_type));
                        //Array.Reverse(@this.ToBytes(), Marshal.OffsetOf(f_type, field.Name).ToInt32(), Marshal.SizeOf(f_type));
                        //return @this;
                    }
                    else
                    {
                        foreach (var elem in Type.GetTypeArray(new Object[] { field.GetRawConstantValue() }))
                        {
                            elem.Reverse();
                        }
                        MessageBox.Show("Reverse(): handled array!");
                    }
                    
                    // recurse
                    {
                        /*MessageBox.Show("lol");
                        foreach (var sf in subFields)
                        {
                            sf.SetValueDirect(__makeref(@this), sf.GetRawConstantValue().ToBytes().Reverse());
                            //f.SetValueDirect(__makeref(typeof(@this)), f.GetRawConstantValue().Reverse());
                            //f.ToBytes().Reverse();
                        }
                        return @this;
                        //f.Reverse();
                    }//return @this.Reverse<T>();
                    
                }
                return @this;
                
            }
            catch
            {
                
            }
            return @this;
        }*/

        public static Byte[] Get(this Byte[] @this, Int32 length)
            => @this.Take(length).ToArray();

        public static T ToStructure<T>(this Byte[] @this, Boolean swap = false) where T : struct
        {
            T result = default(T);
            GCHandle handle = GCHandle.Alloc(@this, GCHandleType.Pinned);
            try
            {
                IntPtr rawDataPtr = handle.AddrOfPinnedObject();
                result = (T)Marshal.PtrToStructure(rawDataPtr, typeof(T));

                if (swap)
                    result = result.Reverse();
            }
            finally
            {
                handle.Free();
            }
            return result;
        }

        public static Byte[] ToBytes<T>(this T @this)
        {
            if (@this == null || @this is Byte[])
                return @this as Byte[];

            var type = typeof(T);
            var buffer = null as Byte[];

            Func<String, Byte[]> StringToBytes = str => Encoding.ASCII.GetBytes(str + "\0");

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
                    throw new Exception($"Could not serialize data of type '{type.Name}'.\nException: {e.ToString()}");
                }
                finally
                {
                    gch.Free();
                }
                return ba;
            };

            try
            {
                // indicate whether the datatype is a integral number ( (Un)signed Integer or Floating Point ) or a sequential structure
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

        /*public static Byte[] ToBytes<T>(this T @this)
        {
            var length = Marshal.SizeOf(@this);
            var rawData = new Byte[Marshal.SizeOf(@this)];
            GCHandle handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);
            try
            {
                IntPtr rawDataPtr = handle.AddrOfPinnedObject();
                Marshal.StructureToPtr(@this, rawDataPtr, true);
                Marshal.Copy(rawDataPtr, rawData, 0, length);
            }
            finally
            {
                handle.Free();
            }
            return rawData;
        }*/

        /*public static Byte[] ToByteArray<T>(this T obj) where T : struct
        {
            var length = Marshal.SizeOf(obj);
            var barray = new Byte[length];

            IntPtr ptr = Marshal.AllocHGlobal(length);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, barray, 0, length);
            Marshal.FreeHGlobal(ptr);
            return barray;
        }

        public static T BytesToStruct<T>(this Byte[] rawData, Boolean swap = false) where T : struct
        {
            T result = default(T);

            GCHandle handle = GCHandle.Alloc(rawData, GCHandleType.Pinned);

            try
            {
                IntPtr rawDataPtr = handle.AddrOfPinnedObject();
                result = (T)Marshal.PtrToStructure(rawDataPtr, typeof(T));
            }
            finally
            {
                handle.Free();
            }

            if (swap)
                result = result.Reverse();

            return result;
        }*/
    }
}
