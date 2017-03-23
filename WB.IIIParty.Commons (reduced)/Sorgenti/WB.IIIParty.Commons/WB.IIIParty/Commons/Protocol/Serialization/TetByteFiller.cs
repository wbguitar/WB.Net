using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace WB.IIIParty.Commons.Protocol.Serialization
{
    /// <summary>
    /// Enumerato dei tipi gestiti
    /// </summary>
    public enum TetTypes {
        /// <summary>
        /// 
        /// </summary>
        Bool,
        /// <summary>
        /// 
        /// </summary>
        Byte,
        /// <summary>
        /// 
        /// </summary>
        SByte,
        /// <summary>
        /// 
        /// </summary>
        Short,
        /// <summary>
        /// 
        /// </summary>
        Ushort,
        /// <summary>
        /// 
        /// </summary>
        Int,
        /// <summary>
        /// 
        /// </summary>
        UInt,
        /// <summary>
        /// 
        /// </summary>
        Long,
        /// <summary>
        /// 
        /// </summary>
        ULong,
        /// <summary>
        /// 
        /// </summary>
        Single,
        /// <summary>
        /// 
        /// </summary>
        Double,
        /// <summary>
        /// 
        /// </summary>
        String,
        /// <summary>
        /// 
        /// </summary>
        Datetime,
        /// <summary>
        /// 
        /// </summary>
        Timespan,
        /// <summary>
        /// 
        /// </summary>
        Enum,
        /// <summary>
        /// 
        /// </summary>
        ArrayOfBool,
        /// <summary>
        /// 
        /// </summary>
        ArrayOfByte,
        /// <summary>
        /// 
        /// </summary>
        ArrayOfSByte,
        /// <summary>
        /// 
        /// </summary>
        ArrayOfShort,
        /// <summary>
        /// 
        /// </summary>
        ArrayOfUshort,
        /// <summary>
        /// 
        /// </summary>
        ArrayOfInt,
        /// <summary>
        /// 
        /// </summary>
        ArrayOfUInt,
        /// <summary>
        /// 
        /// </summary>
        ArrayOfLong,
        /// <summary>
        /// 
        /// </summary>
        ArrayOfULong,
        /// <summary>
        /// 
        /// </summary>
        ArrayOfSingle,
        /// <summary>
        /// 
        /// </summary>
        ArrayOfDouble,
        /// <summary>
        /// 
        /// </summary>
        ArrayOfString,
        /// <summary>
        /// 
        /// </summary>
        ArrayOfDatetime,
        /// <summary>
        /// 
        /// </summary>
        ArrayOfTimespan,
        /// <summary>
        /// 
        /// </summary>
        ArrayOfEnum,
        /// <summary>
        /// 
        /// </summary>
        ListOfBool,
        /// <summary>
        /// 
        /// </summary>
        ListOfByte,
        /// <summary>
        /// 
        /// </summary>
        ListOfSByte,
        /// <summary>
        /// 
        /// </summary>
        ListOfShort,
        /// <summary>
        /// 
        /// </summary>
        ListOfUshort,
        /// <summary>
        /// 
        /// </summary>
        ListOfInt,
        /// <summary>
        /// 
        /// </summary>
        ListOfUInt,
        /// <summary>
        /// 
        /// </summary>
        ListOfLong,
        /// <summary>
        /// 
        /// </summary>
        ListOfULong,
        /// <summary>
        /// 
        /// </summary>
        ListOfSingle,
        /// <summary>
        /// 
        /// </summary>
        ListOfDouble,
        /// <summary>
        /// 
        /// </summary>
        ListOfString,
        /// <summary>
        /// 
        /// </summary>
        ListOfDatetime,
        /// <summary>
        /// 
        /// </summary>
        ListOfTimespan,
        /// <summary>
        /// 
        /// </summary>
        ListOfEnum,        
        };

    /// <summary>
    /// 
    /// </summary>
    public class TetByteFiller
    {
        // contenitore dello stream di bytes 
        List<byte> stream=new List<byte>();  
        //byte[] stream;

        List<bool> blist = new List<bool>();

        /// <summary>
        /// 
        /// </summary>
        public int LastByte { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public byte LastBit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public byte[] Data { get { return stream.ToArray(); } }

        /// <summary>
        /// Lista/enumerato che ha lo scopo di facilitare la comprensione
        /// dei tipi usati nel codice per la specializzazione
        /// </summary>
        Dictionary<Type, int> typeDict = new Dictionary<Type, int>
        {
            {typeof(bool),(int)TetTypes.Bool},
            {typeof(byte),(int)TetTypes.Byte},
            {typeof(sbyte),(int)TetTypes.SByte},
            {typeof(short),(int)TetTypes.Short},
            {typeof(ushort),(int)TetTypes.Ushort},
            {typeof(int),(int)TetTypes.Int},
            {typeof(uint),(int)TetTypes.UInt},
            {typeof(long),(int)TetTypes.Long},
            {typeof(ulong),(int)TetTypes.ULong},
            {typeof(float),(int)TetTypes.Single},
            {typeof(double),(int)TetTypes.Double},
            {typeof(string),(int)TetTypes.String},
            {typeof(DateTime),(int)TetTypes.Datetime},
            {typeof(TimeSpan),(int)TetTypes.Timespan},
            {typeof(Enum),(int)TetTypes.Enum},
            {typeof(bool[]),(int)TetTypes.ArrayOfBool},
            {typeof(byte[]),(int)TetTypes.ArrayOfByte},
            {typeof(sbyte[]),(int)TetTypes.ArrayOfSByte},
            {typeof(short[]),(int)TetTypes.ArrayOfShort},
            {typeof(ushort[]),(int)TetTypes.ArrayOfUshort},
            {typeof(int[]),(int)TetTypes.ArrayOfInt},
            {typeof(uint[]),(int)TetTypes.ArrayOfUInt},
            {typeof(long[]),(int)TetTypes.ArrayOfLong},
            {typeof(ulong[]),(int)TetTypes.ArrayOfULong},
            {typeof(float[]),(int)TetTypes.ArrayOfSingle},
            {typeof(double[]),(int)TetTypes.ArrayOfDouble},
            {typeof(string[]),(int)TetTypes.ArrayOfString},            
            {typeof(DateTime[]),(int)TetTypes.ArrayOfDatetime},
            {typeof(TimeSpan[]),(int)TetTypes.ArrayOfTimespan},
            {typeof(Enum[]),(int)TetTypes.ArrayOfEnum},
            {typeof(List<bool>),(int)TetTypes.ListOfBool},
            {typeof(List<byte>),(int)TetTypes.ListOfByte},
            {typeof(List<sbyte>),(int)TetTypes.ListOfSByte},
            {typeof(List<short>),(int)TetTypes.ListOfShort},
            {typeof(List<ushort>),(int)TetTypes.ListOfUshort},
            {typeof(List<int>),(int)TetTypes.ListOfInt},
            {typeof(List<uint>),(int)TetTypes.ListOfUInt},
            {typeof(List<long>),(int)TetTypes.ListOfLong},
            {typeof(List<ulong>),(int)TetTypes.ListOfULong},
            {typeof(List<float>),(int)TetTypes.ListOfSingle},
            {typeof(List<double>),(int)TetTypes.ListOfDouble},
            {typeof(List<string>),(int)TetTypes.ListOfString},            
            {typeof(List<DateTime>),(int)TetTypes.ListOfDatetime},
            {typeof(List<TimeSpan>),(int)TetTypes.ListOfTimespan},
            {typeof(List<Enum>),(int)TetTypes.ListOfEnum},
        };

        /// <summary>
        /// Costruttore
        /// </summary>
        public TetByteFiller()
        {
            
            LastByte = 0;
            LastBit = 0;
        }

        /// <summary>
        /// Ritorna true se il tipo passato come parametro non fa
        /// parte della lista dei tipi supportati
        /// </summary>
        /// <param name="caino"></param>
        /// <returns></returns>
        public bool IsList(Type caino)
        {
            return ((caino.IsGenericType && caino.GetGenericTypeDefinition() == typeof(List<>)));
        }

        /// <summary>
        /// Ritorna true se il tipo passato come parametro non fa
        /// parte della lista dei tipi supportati
        /// </summary>
        /// <param name="caino"></param>
        /// <returns></returns>
        public bool IsArray(Type caino)
        {
            return caino.IsArray;
        }

        /// <summary>
        /// Ritorna true se il tipo passato come parametro non fa
        /// parte della lista dei tipi supportati
        /// </summary>
        /// <param name="caino"></param>
        /// <returns></returns>
        public bool IsClass(Type caino)
        {
            return (!typeDict.ContainsKey(caino) && !caino.IsEnum);
        }
        
        /// <summary>
        /// Ritorna la dimensione dello stream di byte
        /// </summary>
        /// <returns></returns>
        public int GetSize()
        {  
            int retval = stream.Count + (blist.Count>0 ? 1 : 0);
            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tipo"></param>
        /// <returns></returns>
        public int GetSize(Type tipo)
        {
            int retval = stream.Count + (blist.Count > 0 ? 1 : 0);
            return retval;
        }


        /// <summary>
        /// Ritorna l'array di bytes rappresentante l'oggetto passato
        /// </summary>
        /// <param name="poi"></param>
        /// <param name="idata"></param>
        /// <returns></returns>
//        public void Serialize(Type t, object o)
        public void Serialize(PropertyOrderInfo poi, object idata)
        {
            //PropertyInfo info=proplist[ii].PropInfo;
            //object val = info.GetValue(data,null);
            //filler.Serialize(val.GetType(), val);
            
            object o = idata;
            Type t = o.GetType();

            int tval=-1;
            bool typefound=typeDict.TryGetValue(t,out tval);

            // In caso di booleani residui da aggiunte precedenti,
            // vengono memorizzate per preparare il successivo inserimento
            // di un elemento non bool
            if (typefound && (((typeDict[t] != (int)TetTypes.Bool) || (!typefound)) && (blist.Count > 0)))
            {
                stream.Add(BitArrayToByte(0,blist.ToArray()));
                blist.Clear();
            }
            
            if (!typefound)
                if (t.IsEnum)
                    tval = (int)TetTypes.Enum;
                else
                    tval = -1;

            switch (tval)
            {
                case (int)TetTypes.Bool: // bool
                    if (blist.Count > 7)
                    {
                        stream.Add(BitArrayToByte(0, blist.ToArray()));
                        blist.Clear();
                    }
                    blist.Add((bool)o);
                    break;
                case (int)TetTypes.Byte: // byte
                    stream.Add((byte)o);
                    break;
                case (int)TetTypes.SByte: // byte
                    stream.Add((byte)o);
                    break;
                case (int)TetTypes.Short: // short
                    //Array.Copy();
                    stream.AddRange(BitConverter.GetBytes((short)o));
                    break;
                case (int)TetTypes.Ushort: // ushort
                    stream.AddRange(BitConverter.GetBytes((ushort)o));
                    break;
                case (int)TetTypes.Int: // int
                    stream.AddRange(BitConverter.GetBytes((int)o));
                    break;
                case (int)TetTypes.UInt: // uint
                    stream.AddRange(BitConverter.GetBytes((uint)o));
                    break;
                case (int)TetTypes.Long: // long
                    stream.AddRange(BitConverter.GetBytes((long)o));
                    break;
                case (int)TetTypes.ULong: // ulong
                    stream.AddRange(BitConverter.GetBytes((ulong)o));
                    break;
                case (int)TetTypes.Single: // single
                    stream.AddRange(BitConverter.GetBytes((float)o));
                    break;
                case (int)TetTypes.Double: // double
                    stream.AddRange(BitConverter.GetBytes((double)o));
                    break;
                case (int)TetTypes.String: // string
                    byte[] range = StringToByteArray((string)o, (poi.StringType == StringTypeEnum.Unicode));
                    stream.AddRange(BitConverter.GetBytes(((string)o).Length));
                    stream.AddRange(range);
                    break;
                case (int)TetTypes.Datetime: // DateTime
                    stream.AddRange(BitConverter.GetBytes(((DateTime)o).ToBinary()));
                    break;
                case (int)TetTypes.Timespan: // TimeSpan
                    stream.AddRange(BitConverter.GetBytes(((TimeSpan)o).Ticks));
                    break;
                case (int)TetTypes.Enum: // enum
                    stream.AddRange(BitConverter.GetBytes((int)o));
                    break;
                case (int)TetTypes.ArrayOfBool: // bool[] 
                    bool[] tbool = (bool[])o;
                    stream.AddRange(BitConverter.GetBytes(tbool.Length));
                    for (int ii = 0; ii < tbool.Length; ii++)
                    {
                        if (blist.Count > 7)
                        {
                            stream.Add(BitArrayToByte(0, blist.ToArray()));
                            blist.Clear();
                        }
                        blist.Add(tbool[ii]);
                    }
                    break;
                case (int)TetTypes.ListOfBool: // bool[] 
                    tbool = ((List<bool>)o).ToArray();
                    stream.AddRange(BitConverter.GetBytes(tbool.Length));
                    for (int ii = 0; ii < tbool.Length; ii++)
                    {
                        if (blist.Count > 7)
                        {
                            stream.Add(BitArrayToByte(0, blist.ToArray()));
                            blist.Clear();
                        }
                        blist.Add(tbool[ii]);
                    }
                    break;
                case (int)TetTypes.ArrayOfByte: // byte[] 
                    stream.AddRange(BitConverter.GetBytes(((byte[])o).Length));
                    stream.AddRange((byte[])o);
                    break;
                case (int)TetTypes.ListOfByte: // byte[] 
                    stream.AddRange(BitConverter.GetBytes((((List<byte>)o).ToArray()).Length));
                    stream.AddRange(((List<byte>)o).ToArray());
                    break;
                case (int)TetTypes.ArrayOfSByte: // sbyte[] 
                    stream.AddRange(BitConverter.GetBytes(((sbyte[])o).Length));
                    stream.AddRange((byte[])o);
                    break;
                case (int)TetTypes.ListOfSByte: // sbyte[] 
                    stream.AddRange(BitConverter.GetBytes((((List<sbyte>)o).ToArray()).Length));
                    stream.AddRange(((List<byte>)o).ToArray());
                    break;
                case (int)TetTypes.ArrayOfShort: // short[] 
                    short[] tshort = (short[])o;
                    stream.AddRange(BitConverter.GetBytes(tshort.Length));
                    for (int ii = 0; ii < tshort.Length; ii++)
                    {
                        stream.AddRange(BitConverter.GetBytes(tshort[ii]));
                    }
                    break;
                case (int)TetTypes.ListOfShort: // short[] 
                    tshort = ((List<short>)o).ToArray();
                    stream.AddRange(BitConverter.GetBytes(tshort.Length));
                    for (int ii = 0; ii < tshort.Length; ii++)
                    {
                        stream.AddRange(BitConverter.GetBytes(tshort[ii]));
                    }
                    break;
                case (int)TetTypes.ArrayOfUshort: // ushort[] 
                    ushort[] tushort = (ushort[])o;
                    stream.AddRange(BitConverter.GetBytes(tushort.Length));
                    for (int ii = 0; ii < tushort.Length; ii++)
                    {
                        stream.AddRange(BitConverter.GetBytes(tushort[ii]));
                    }
                    break;
                case (int)TetTypes.ListOfUshort: // ushort[] 
                    tushort = ((List<ushort>)o).ToArray();
                    stream.AddRange(BitConverter.GetBytes(tushort.Length));
                    for (int ii = 0; ii < tushort.Length; ii++)
                    {
                        stream.AddRange(BitConverter.GetBytes(tushort[ii]));
                    }
                    break;
                case (int)TetTypes.ArrayOfInt: // int[] 
                    int[] tint = (int[])o;
                    stream.AddRange(BitConverter.GetBytes(tint.Length));
                    for (int ii = 0; ii < tint.Length; ii++)
                    {
                        stream.AddRange(BitConverter.GetBytes(tint[ii]));
                    }
                    break;
                case (int)TetTypes.ListOfInt: // int[] 
                    tint = ((List<int>)o).ToArray();
                    stream.AddRange(BitConverter.GetBytes(tint.Length));
                    for (int ii = 0; ii < tint.Length; ii++)
                    {
                        stream.AddRange(BitConverter.GetBytes(tint[ii]));
                    }
                    break;
                case (int)TetTypes.ArrayOfUInt: // uint[] 
                    uint[] tuint = (uint[])o;
                    stream.AddRange(BitConverter.GetBytes(tuint.Length));
                    for (int ii = 0; ii < tuint.Length; ii++)
                    {
                        stream.AddRange(BitConverter.GetBytes(tuint[ii]));
                    }
                    break;
                case (int)TetTypes.ListOfUInt: // uint[] 
                    tuint = ((List<uint>)o).ToArray();
                    stream.AddRange(BitConverter.GetBytes(tuint.Length));
                    for (int ii = 0; ii < tuint.Length; ii++)
                    {
                        stream.AddRange(BitConverter.GetBytes(tuint[ii]));
                    }
                    break;
                case (int)TetTypes.ArrayOfLong: // long[] 
                    long[] tlong = (long[])o;
                    stream.AddRange(BitConverter.GetBytes(tlong.Length));
                    for (int ii = 0; ii < tlong.Length; ii++)
                    {
                        stream.AddRange(BitConverter.GetBytes(tlong[ii]));
                    }
                    break;
                case (int)TetTypes.ListOfLong: // long[] 
                    tlong = ((List<long>)o).ToArray();
                    stream.AddRange(BitConverter.GetBytes(tlong.Length));
                    for (int ii = 0; ii < tlong.Length; ii++)
                    {
                        stream.AddRange(BitConverter.GetBytes(tlong[ii]));
                    }
                    break;
                case (int)TetTypes.ArrayOfULong: // ulong[] 
                    ulong[] tulong = (ulong[])o;
                    stream.AddRange(BitConverter.GetBytes(tulong.Length));
                    for (int ii = 0; ii < tulong.Length; ii++)
                    {
                        stream.AddRange(BitConverter.GetBytes(tulong[ii]));
                    }
                    break;
                case (int)TetTypes.ListOfULong: // ulong[] 
                    tulong = ((List<ulong>)o).ToArray();
                    stream.AddRange(BitConverter.GetBytes(tulong.Length));
                    for (int ii = 0; ii < tulong.Length; ii++)
                    {
                        stream.AddRange(BitConverter.GetBytes(tulong[ii]));
                    }
                    break;
                case (int)TetTypes.ArrayOfSingle: // float[] 
                    float[] tfloat = (float[])o;
                    stream.AddRange(BitConverter.GetBytes(tfloat.Length));
                    for (int ii = 0; ii < tfloat.Length; ii++)
                    {
                        stream.AddRange(BitConverter.GetBytes(tfloat[ii]));
                    }
                    break;
                case (int)TetTypes.ListOfSingle: // float[] 
                    tfloat = ((List<float>)o).ToArray();
                    stream.AddRange(BitConverter.GetBytes(tfloat.Length));
                    for (int ii = 0; ii < tfloat.Length; ii++)
                    {
                        stream.AddRange(BitConverter.GetBytes(tfloat[ii]));
                    }
                    break;
                case (int)TetTypes.ArrayOfDouble: // double[] 
                    double[] tdouble = (double[])o;
                    stream.AddRange(BitConverter.GetBytes(tdouble.Length));
                    for (int ii = 0; ii < tdouble.Length; ii++)
                    {
                        stream.AddRange(BitConverter.GetBytes(tdouble[ii]));
                    }
                    break;
                case (int)TetTypes.ListOfDouble: // double[] 
                    tdouble = ((List<double>)o).ToArray();
                    stream.AddRange(BitConverter.GetBytes(tdouble.Length));
                    for (int ii = 0; ii < tdouble.Length; ii++)
                    {
                        stream.AddRange(BitConverter.GetBytes(tdouble[ii]));
                    }
                    break;
                case (int)TetTypes.ArrayOfString: // string[]
                    string[] tstring = (string[])o;
                    
                    stream.AddRange(BitConverter.GetBytes(tstring.Length));
                    for (int ii = 0; ii < tstring.Length; ii++)
                    {
                        byte[] srange = StringToByteArray(tstring[ii], (poi.StringType == StringTypeEnum.Unicode));
                        stream.AddRange(BitConverter.GetBytes(tstring[ii].Length));
                        stream.AddRange(srange);
                    }
                    break;
                case (int)TetTypes.ListOfString: // string[]
                    tstring = ((List<string>)o).ToArray();

                    stream.AddRange(BitConverter.GetBytes(tstring.Length));
                    for (int ii = 0; ii < tstring.Length; ii++)
                    {
                        byte[] srange = StringToByteArray(tstring[ii], (poi.StringType == StringTypeEnum.Unicode));
                        stream.AddRange(BitConverter.GetBytes(tstring[ii].Length));
                        stream.AddRange(srange);
                    }
                    break;
                case (int)TetTypes.ArrayOfDatetime: // DateTime[]
                    DateTime[] tDateTime = (DateTime[])o;
                    stream.AddRange(BitConverter.GetBytes(tDateTime.Length));
                    for (int ii = 0; ii < tDateTime.Length; ii++)
                    {
                        stream.AddRange(BitConverter.GetBytes(tDateTime[ii].ToBinary()));
                    }
                    break;
                case (int)TetTypes.ListOfDatetime: // DateTime[]
                    tDateTime = ((List<DateTime>)o).ToArray();
                    stream.AddRange(BitConverter.GetBytes(tDateTime.Length));
                    for (int ii = 0; ii < tDateTime.Length; ii++)
                    {
                        stream.AddRange(BitConverter.GetBytes(tDateTime[ii].ToBinary()));
                    }
                    break;
                case (int)TetTypes.ArrayOfTimespan: // TimeSpan[]
                    TimeSpan[] tTimeSpan = (TimeSpan[])o;
                    stream.AddRange(BitConverter.GetBytes(tTimeSpan.Length));
                    for (int ii = 0; ii < tTimeSpan.Length; ii++)
                    {
                        stream.AddRange(BitConverter.GetBytes(tTimeSpan[ii].Ticks));
                    }
                    break;
                case (int)TetTypes.ListOfTimespan: // TimeSpan[]
                    tTimeSpan = ((List<TimeSpan>)o).ToArray();
                    stream.AddRange(BitConverter.GetBytes(tTimeSpan.Length));
                    for (int ii = 0; ii < tTimeSpan.Length; ii++)
                    {
                        stream.AddRange(BitConverter.GetBytes(tTimeSpan[ii].Ticks));
                    }
                    break;
                case (int)TetTypes.ArrayOfEnum: // enum[]
                    int[] tenum = (int[])o;
                    stream.AddRange(BitConverter.GetBytes(tenum.Length));
                    for (int ii = 0; ii < tenum.Length; ii++)
                    {
                        stream.AddRange(BitConverter.GetBytes(tenum[ii]));
                    }
                    break;
                case (int)TetTypes.ListOfEnum: // enum[]
                    tenum = ((List<int>)o).ToArray();
                    stream.AddRange(BitConverter.GetBytes(tenum.Length));
                    for (int ii = 0; ii < tenum.Length; ii++)
                    {
                        stream.AddRange(BitConverter.GetBytes(tenum[ii]));
                    }
                    break;
                default:
                    throw new Exception("Serialize: ingestibile");
            }
        }

        /// <summary>
        /// DeserializeExternal
        /// </summary>
        /// <param name="bytesLength"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] DeserializeExternal(int bytesLength, byte[] data)
        {
            byte[] copy = new byte[bytesLength];
            Array.Copy(data, LastByte, copy, 0, bytesLength);
            LastByte += bytesLength;
            LastBit = 0;
            return copy;
        }

        /// <summary>
        /// Ritorna l'oggetto del tipo assegnato convertito da un array di bytes
        /// </summary>
        /// <param name="poi"></param>
        /// <param name="s"></param>
        /// <returns></returns>
//        public object Deserialize(Type t, ref byte[] s)
        public object Deserialize(PropertyOrderInfo poi, ref byte[] s)
        {
            Type t = poi.PropInfo.PropertyType;
            int bytepos = LastByte;
            int tval = -1;

            byte bitpos = LastBit;

            object retval = null;

            if (bytepos>s.Count())
                throw new Exception("Deserialize: bytepos out of range");

            bool typefound = typeDict.TryGetValue(t, out tval);

            if (!typefound)
                if (t.IsEnum)
                    tval = (int)TetTypes.Enum;
                else
                    tval = -1;

            int lunghezza;
            byte[] s1;
            //int tnum;
            //bool found=typeDict.TryGetValue(t, out tnum);
            //if (tval<0)
                
            switch (tval)
            {
                case (int)TetTypes.Bool: // bool
                    BitArray barr = new BitArray(new byte[1] {s[bytepos]});

                    //string testo = "";
                    //for (int ii = 0; ii < barr.Length; ii++)
                    //    testo += barr[ii].ToString() + " - ";
                    //System.Windows.Forms.MessageBox.Show(testo);
                    
                    retval = (object)barr[bitpos];
                    LastBit++;
                    if (LastBit > 7)
                    {
                        LastByte++;
                        LastBit = 0;
                    }
                    break;
                case (int)TetTypes.Byte: // byte
                    retval = (object)s[bytepos];
                    LastBit = 0;
                    LastByte++;
                    break;
                case (int)TetTypes.SByte: // sbyte
                    retval = (object)s[bytepos];
                    LastBit = 0;
                    LastByte++;
                    break;
                case (int)TetTypes.Short: // short
                    retval=(object)BitConverter.ToInt16(s, bytepos);
                    LastBit = 0;
                    LastByte+=sizeof(Int16);
                    break;
                case (int)TetTypes.Ushort: // ushort
                    retval = (object)BitConverter.ToUInt16(s, bytepos);
                    LastBit = 0;
                    LastByte += sizeof(UInt16);
                    break;
                case (int)TetTypes.Int: // int
                    retval = (object)BitConverter.ToInt32(s, bytepos);
                    LastBit = 0;
                    LastByte += sizeof(Int32);
                    break;
                case (int)TetTypes.UInt: // uint
                    retval = (object)BitConverter.ToUInt32(s, bytepos);
                    LastBit = 0;
                    LastByte += sizeof(UInt32);
                    break;
                case (int)TetTypes.Long: // long
                    retval = (object)BitConverter.ToInt64(s, bytepos);
                    LastBit = 0;
                    LastByte += sizeof(Int64);
                    break;
                case (int)TetTypes.ULong: // ulong
                    retval = (object)BitConverter.ToUInt64(s, bytepos);
                    LastBit = 0;
                    LastByte += sizeof(UInt64);
                    break;
                case (int)TetTypes.Single: // float
                    retval = (object)BitConverter.ToSingle(s, bytepos);
                    LastBit = 0;
                    LastByte += sizeof(float);
                    break;
                case (int)TetTypes.Double: // double
                    retval = (object)BitConverter.ToDouble(s, bytepos);
                    LastBit = 0;
                    LastByte += sizeof(double);
                    break;
                case (int)TetTypes.String: // string
                    // numero di elementi
                    int nchars = (int)BitConverter.ToInt32(s, bytepos);
                    bool uni=(poi.StringType == StringTypeEnum.Unicode);

                    if (uni)
                        lunghezza = nchars * 2;
                    else
                        lunghezza = nchars;

                    byte[] testo=new byte[lunghezza];
                    Array.Copy(s, bytepos + sizeof(Int32), testo, 0, lunghezza);
                    retval = (object)ByteArrayToString(testo, uni);
                    
                    LastBit = 0;
                    LastByte += (lunghezza + sizeof(Int32));
                    break;
                case (int)TetTypes.Datetime: // Datetime
                    retval = (object)DateTime.FromBinary(BitConverter.ToInt64(s, bytepos));
                    LastBit = 0;
                    LastByte += sizeof(long);
                    break;
                case (int)TetTypes.Timespan: // TimeSpan
                    retval = (object)TimeSpan.FromTicks((BitConverter.ToInt64(s, bytepos)));
                    LastBit = 0;
                    LastByte += sizeof(long);
                    break;
                case (int)TetTypes.Enum: // Enum
                    //retval = Convert.ChangeType(BitConverter.ToInt32(s, bytepos), t);
                    retval=Enum.ToObject(t, (BitConverter.ToInt32(s, bytepos)));
                    LastBit = 0;
                    LastByte += sizeof(Int32);
                    break;
                case (int)TetTypes.ArrayOfBool: // bool[]
                    int nbools = (int)BitConverter.ToInt32(s, bytepos);
                    lunghezza = (nbools / 8) + 1;
                    s1 = new byte[lunghezza];
                    Array.Copy(s, bytepos + sizeof(Int32), s1, 0, lunghezza);

                    BitArray barr1 = new BitArray(s1);
                    bool[] barr2 = new bool[barr1.Length];

                    for (int ii = 0; ii < barr1.Length; ii++)
                        barr2[ii] = barr1[ii];
                    
                    retval = (object)barr2;
                    
                    LastBit = 0;
                    LastByte += (lunghezza + sizeof(Int32));
                    break;
                case (int)TetTypes.ListOfBool: // bool[]
                    nbools = (int)BitConverter.ToInt32(s, bytepos);
                    lunghezza = (nbools / 8) + 1;
                    s1 = new byte[lunghezza];
                    Array.Copy(s, bytepos + sizeof(Int32), s1, 0, lunghezza);

                    barr1 = new BitArray(s1);
                    List<bool> lbarr2 = new List<bool>(barr1.Length);

                    for (int ii = 0; ii < barr1.Length; ii++)
                        lbarr2[ii] = barr1[ii];

                    retval = (object)lbarr2;

                    LastBit = 0;
                    LastByte += (lunghezza + sizeof(Int32));
                    break;
                case (int)TetTypes.ArrayOfByte: // byte[]
                    lunghezza = (int)BitConverter.ToInt32(s, bytepos);
                    s1=new byte[lunghezza];
                    Array.Copy(s, bytepos + sizeof(Int32), s1, 0, lunghezza);
                    retval = (object)s1;
                    LastBit = 0;
                    LastByte += (lunghezza + sizeof(Int32));
                    break;
                case (int)TetTypes.ListOfByte: // byte[]
                    lunghezza = (int)BitConverter.ToInt32(s, bytepos);
                    s1 = new byte[lunghezza];
                    Array.Copy(s, bytepos + sizeof(Int32), s1, 0, lunghezza);
                    retval = (object)s1.ToList();
                    LastBit = 0;
                    LastByte += (lunghezza + sizeof(Int32));
                    break;
                case (int)TetTypes.ArrayOfSByte: // sbyte[]
                    lunghezza = (int)BitConverter.ToInt32(s, bytepos);
                    sbyte[] tsbyte = new sbyte[lunghezza];
                    Array.Copy(s, bytepos + sizeof(Int32), tsbyte, 0, lunghezza);
                    retval = (object)tsbyte;
                    LastBit = 0;
                    LastByte += (lunghezza + sizeof(Int32));
                    break;
                case (int)TetTypes.ArrayOfShort: // short[]
                    lunghezza = (int)BitConverter.ToInt32(s, bytepos);
                    short[] tshort = new short[lunghezza];
                    for (int ii = 0; ii < lunghezza; ii++)
                    {
                        tshort[ii]=BitConverter.ToInt16(s, bytepos + sizeof(Int32) + ii * sizeof(Int16));
                    }
                    retval = (object)tshort;
                    LastBit = 0;
                    LastByte += (lunghezza*sizeof(Int16) + sizeof(Int32));
                    break;
                case (int)TetTypes.ListOfShort: // short[]
                    lunghezza = (int)BitConverter.ToInt32(s, bytepos);
                    tshort = new short[lunghezza];
                    for (int ii = 0; ii < lunghezza; ii++)
                    {
                        tshort[ii] = BitConverter.ToInt16(s, bytepos + sizeof(Int32) + ii * sizeof(Int16));
                    }
                    retval = (object)tshort.ToList();
                    LastBit = 0;
                    LastByte += (lunghezza * sizeof(Int16) + sizeof(Int32));
                    break;
                case (int)TetTypes.ArrayOfUshort: // ushort[]
                    lunghezza = (int)BitConverter.ToInt32(s, bytepos);
                    ushort[] tushort = new ushort[lunghezza];
                    for (int ii = 0; ii < lunghezza; ii++)
                    {
                        tushort[ii] = BitConverter.ToUInt16(s, bytepos + sizeof(Int32) + ii * sizeof(UInt16));
                    }
                    retval = (object)tushort;
                    LastBit = 0;
                    LastByte += (lunghezza * sizeof(UInt16) + sizeof(Int32));
                    break;
                case (int)TetTypes.ListOfUshort: // ushort[]
                    lunghezza = (int)BitConverter.ToInt32(s, bytepos);
                    tushort = new ushort[lunghezza];
                    for (int ii = 0; ii < lunghezza; ii++)
                    {
                        tushort[ii] = BitConverter.ToUInt16(s, bytepos + sizeof(Int32) + ii * sizeof(UInt16));
                    }
                    retval = (object)tushort.ToList();
                    LastBit = 0;
                    LastByte += (lunghezza * sizeof(UInt16) + sizeof(Int32));
                    break;
                case (int)TetTypes.ArrayOfInt: // int[]
                    lunghezza = (int)BitConverter.ToInt32(s, bytepos);
                    int[] tint   = new int[lunghezza];
                    for (int ii = 0; ii < lunghezza; ii++)
                    {
                        tint[ii] = BitConverter.ToInt32(s, bytepos + sizeof(Int32) + ii * sizeof(Int32));
                    }
                    retval = (object)tint;
                    LastBit = 0;
                    LastByte += (lunghezza * sizeof(Int32) + sizeof(Int32));
                    break;
                case (int)TetTypes.ListOfInt: // int[]
                    lunghezza = (int)BitConverter.ToInt32(s, bytepos);
                    tint = new int[lunghezza];
                    for (int ii = 0; ii < lunghezza; ii++)
                    {
                        tint[ii] = BitConverter.ToInt32(s, bytepos + sizeof(Int32) + ii * sizeof(Int32));
                    }
                    retval = (object)tint.ToList();
                    LastBit = 0;
                    LastByte += (lunghezza * sizeof(Int32) + sizeof(Int32));
                    break;
                case (int)TetTypes.ArrayOfUInt: // uint[]
                    lunghezza = (int)BitConverter.ToInt32(s, bytepos);
                    uint[] tuint = new uint[lunghezza];
                    for (int ii = 0; ii < lunghezza; ii++)
                    {
                        tuint[ii] = BitConverter.ToUInt32(s, bytepos + sizeof(Int32) + ii * sizeof(UInt32));
                    }
                    retval = (object)tuint;
                    LastBit = 0;
                    LastByte += (lunghezza * sizeof(UInt32) + sizeof(Int32));
                    break;
                case (int)TetTypes.ListOfUInt: // uint[]
                    lunghezza = (int)BitConverter.ToInt32(s, bytepos);
                    tuint = new uint[lunghezza];
                    for (int ii = 0; ii < lunghezza; ii++)
                    {
                        tuint[ii] = BitConverter.ToUInt32(s, bytepos + sizeof(Int32) + ii * sizeof(UInt32));
                    }
                    retval = (object)tuint.ToArray();
                    LastBit = 0;
                    LastByte += (lunghezza * sizeof(UInt32) + sizeof(Int32));
                    break;
                case (int)TetTypes.ArrayOfLong: // long[]
                    lunghezza = (int)BitConverter.ToInt32(s, bytepos);
                    long[] tlong = new long[lunghezza];
                    for (int ii = 0; ii < lunghezza; ii++)
                    {
                        tlong[ii] = BitConverter.ToInt64(s, bytepos + sizeof(Int32) + ii * sizeof(Int64));
                    }
                    retval = (object)tlong;
                    LastBit = 0;
                    LastByte += (lunghezza * sizeof(Int64) + sizeof(Int32));
                    break;
                case (int)TetTypes.ListOfLong: // long[]
                    lunghezza = (int)BitConverter.ToInt32(s, bytepos);
                    tlong = new long[lunghezza];
                    for (int ii = 0; ii < lunghezza; ii++)
                    {
                        tlong[ii] = BitConverter.ToInt64(s, bytepos + sizeof(Int32) + ii * sizeof(Int64));
                    }
                    retval = (object)tlong.ToList();
                    LastBit = 0;
                    LastByte += (lunghezza * sizeof(Int64) + sizeof(Int32));
                    break;
                case (int)TetTypes.ArrayOfULong: // ulong[]
                    lunghezza = (int)BitConverter.ToInt32(s, bytepos);
                    ulong[] tulong = new ulong[lunghezza];
                    for (int ii = 0; ii < lunghezza; ii++)
                    {
                        tulong[ii] = BitConverter.ToUInt64(s, bytepos + sizeof(Int32) + ii * sizeof(UInt64));
                    }
                    retval = (object)tulong;
                    LastBit = 0;
                    LastByte += (lunghezza * sizeof(UInt64) + sizeof(Int32));
                    break;
                case (int)TetTypes.ListOfULong: // ulong[]
                    lunghezza = (int)BitConverter.ToInt32(s, bytepos);
                    tulong = new ulong[lunghezza];
                    for (int ii = 0; ii < lunghezza; ii++)
                    {
                        tulong[ii] = BitConverter.ToUInt64(s, bytepos + sizeof(Int32) + ii * sizeof(UInt64));
                    }
                    retval = (object)tulong.ToList();
                    LastBit = 0;
                    LastByte += (lunghezza * sizeof(UInt64) + sizeof(Int32));
                    break;
                case (int)TetTypes.ArrayOfSingle: // float[]
                    lunghezza = (int)BitConverter.ToInt32(s, bytepos);
                    float[] tfloat = new float[lunghezza];
                    for (int ii = 0; ii < lunghezza; ii++)
                    {
                        tfloat[ii] = BitConverter.ToSingle(s, bytepos + sizeof(Int32) + ii * sizeof(float));
                    }
                    retval = (object)tfloat;
                    LastBit = 0;
                    LastByte += (lunghezza * sizeof(float) + sizeof(Int32));
                    break;
                case (int)TetTypes.ListOfSingle: // float[]
                    lunghezza = (int)BitConverter.ToInt32(s, bytepos);
                    tfloat = new float[lunghezza];
                    for (int ii = 0; ii < lunghezza; ii++)
                    {
                        tfloat[ii] = BitConverter.ToSingle(s, bytepos + sizeof(Int32) + ii * sizeof(float));
                    }
                    retval = (object)tfloat.ToList();
                    LastBit = 0;
                    LastByte += (lunghezza * sizeof(float) + sizeof(Int32));
                    break;
                case (int)TetTypes.ArrayOfDouble: // double[]
                    lunghezza = (int)BitConverter.ToInt32(s, bytepos);
                    double[] tdouble= new double[lunghezza];
                    for (int ii = 0; ii < lunghezza; ii++)
                    {
                        tdouble[ii] = BitConverter.ToDouble(s, bytepos + sizeof(Int32) + ii * sizeof(double));
                    }
                    retval = (object)tdouble;
                    LastBit = 0;
                    LastByte += (lunghezza * sizeof(double) + sizeof(Int32));
                    break;
                case (int)TetTypes.ListOfDouble: // double[]
                    lunghezza = (int)BitConverter.ToInt32(s, bytepos);
                    tdouble = new double[lunghezza];
                    for (int ii = 0; ii < lunghezza; ii++)
                    {
                        tdouble[ii] = BitConverter.ToDouble(s, bytepos + sizeof(Int32) + ii * sizeof(double));
                    }
                    retval = (object)tdouble.ToList();
                    LastBit = 0;
                    LastByte += (lunghezza * sizeof(double) + sizeof(Int32));
                    break;
                case (int)TetTypes.ArrayOfString: // string[]
                    lunghezza = (int)BitConverter.ToInt32(s, bytepos);
                    string[] tstring = new string[lunghezza];
                    bytepos += sizeof(Int32);
                    for (int ii = 0; ii < lunghezza; ii++)
                    {
                        int inchars = (int)BitConverter.ToInt32(s, bytepos);
                        bytepos += sizeof(Int32);

                        bool buni = (poi.StringType == StringTypeEnum.Unicode);
                        int slun = 0;  // lunghezza in bytes
                        
                        if (buni)
                            slun = inchars * 2;
                        else
                            slun = inchars;

                        byte[] stext = new byte[slun];

                        Array.Copy(s, bytepos, stext, 0, slun);
                        tstring[ii] = ByteArrayToString(stext, buni);

                        bytepos += slun;
                    }

                    retval = (object)tstring;
                    LastBit = 0;
                    LastByte = bytepos;
                    break;
                case (int)TetTypes.ListOfString: // string[]
                    lunghezza = (int)BitConverter.ToInt32(s, bytepos);
                    List<string> lstring = new List<string>(lunghezza);
                    bytepos += sizeof(Int32);
                    for (int ii = 0; ii < lunghezza; ii++)
                    {
                        int inchars = (int)BitConverter.ToInt32(s, bytepos);
                        bytepos += sizeof(Int32);

                        bool buni = (poi.StringType == StringTypeEnum.Unicode);
                        int slun = 0;  // lunghezza in bytes

                        if (buni)
                            slun = inchars * 2;
                        else
                            slun = inchars;

                        byte[] stext = new byte[slun];

                        Array.Copy(s, bytepos, stext, 0, slun);
                        lstring[ii] = ByteArrayToString(stext, buni);

                        bytepos += slun;
                    }

                    retval = (object)lstring;
                    LastBit = 0;
                    LastByte = bytepos;
                    break;
                case (int)TetTypes.ArrayOfDatetime: // DateTime[]
                    lunghezza = (int)BitConverter.ToInt32(s, bytepos);
                    DateTime[] tDateTime = new DateTime[lunghezza];
                    bytepos += sizeof(Int32);

                    for (int ii = 0; ii < lunghezza; ii++)
                    {
                        tDateTime[ii] = DateTime.FromBinary(BitConverter.ToInt64(s, bytepos));
                        bytepos += sizeof(Int64);
                    }

                    retval = (object)tDateTime;
                    LastBit = 0;
                    LastByte = bytepos;
                    break;
                case (int)TetTypes.ListOfDatetime: // DateTime[]
                    lunghezza = (int)BitConverter.ToInt32(s, bytepos);
                    tDateTime = new DateTime[lunghezza];
                    bytepos += sizeof(Int32);

                    for (int ii = 0; ii < lunghezza; ii++)
                    {
                        tDateTime[ii] = DateTime.FromBinary(BitConverter.ToInt64(s, bytepos));
                        bytepos += sizeof(Int64);
                    }

                    retval = (object)tDateTime.ToList();
                    LastBit = 0;
                    LastByte = bytepos;
                    break;
                case (int)TetTypes.ArrayOfTimespan: // TimeSpan[]
                    lunghezza = (int)BitConverter.ToInt32(s, bytepos);
                    TimeSpan[] tTimeSpan = new TimeSpan[lunghezza];
                    bytepos += sizeof(Int32);

                    for (int ii = 0; ii < lunghezza; ii++)
                    {
                        tTimeSpan[ii] = TimeSpan.FromTicks(BitConverter.ToInt64(s, bytepos));
                        bytepos += sizeof(Int64);
                    }

                    retval = (object)tTimeSpan;
                    LastBit = 0;
                    LastByte = bytepos;
                    break;
                case (int)TetTypes.ListOfTimespan: // TimeSpan[]
                    lunghezza = (int)BitConverter.ToInt32(s, bytepos);
                    tTimeSpan = new TimeSpan[lunghezza];
                    bytepos += sizeof(Int32);

                    for (int ii = 0; ii < lunghezza; ii++)
                    {
                        tTimeSpan[ii] = TimeSpan.FromTicks(BitConverter.ToInt64(s, bytepos));
                        bytepos += sizeof(Int64);
                    }

                    retval = (object)tTimeSpan.ToList();
                    LastBit = 0;
                    LastByte = bytepos;
                    break;
                case (int)TetTypes.ArrayOfEnum: // Enum[]
                    lunghezza = (int)BitConverter.ToInt32(s, bytepos);
                    int[] tEnum = new int[lunghezza];
                    bytepos += sizeof(Int32);

                    for (int ii = 0; ii < lunghezza; ii++)
                    {
                        tEnum[ii] = (BitConverter.ToInt32(s, bytepos));
                        bytepos += sizeof(Int32);
                    }

                    retval = (object)tEnum;
                    LastBit = 0;
                    LastByte = bytepos;
                    break;
                case (int)TetTypes.ListOfEnum: // Enum[]
                    lunghezza = (int)BitConverter.ToInt32(s, bytepos);
                    tEnum = new int[lunghezza];
                    bytepos += sizeof(Int32);

                    for (int ii = 0; ii < lunghezza; ii++)
                    {
                        tEnum[ii] = (BitConverter.ToInt32(s, bytepos));
                        bytepos += sizeof(Int32);
                    }

                    retval = (object)tEnum.ToList();
                    LastBit = 0;
                    LastByte = bytepos;
                    break;
                default: // raw (tipo non gestito) -> byte[]
                    throw new Exception("Deserialize: tipo non gestito");
            }
            return retval;
        }

        /// <summary>
        /// Elimina tutti gli elementi dalle liste
        /// </summary>
        public void Clear()
        {
            blist.Clear();
            stream.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arlist"></param>
        /// <param name="unicode"></param>
        /// <returns></returns>
        public string ByteArrayToString(byte[] arlist, bool unicode)
        {
            string retval = "";
            if (unicode)
                retval = System.Text.Encoding.Unicode.GetString(arlist);
            else
                retval = System.Text.ASCIIEncoding.ASCII.GetString(arlist);

            //string test = System.Text.Encoding.GetEncoding(1251).GetString(arlist);
            return retval;
        }

        /// <summary>
        /// Trasforma una stringa in un array di bytes
        /// con la possibilità di scegliere se usare stringhe unicode oppure no
        /// </summary>
        /// <param name="testo"></param>
        /// <param name="unicode"></param>
        /// <returns></returns>
        public byte[] StringToByteArray(string testo, bool unicode)
        {
            List<byte> lista=new List<byte>();
            char[] at=testo.ToCharArray();
            byte[] retval = null;

            if (!unicode)
            {
                ASCIIEncoding ascii = new ASCIIEncoding();
                retval = ascii.GetBytes(testo);
            }
            else
            {
                for (int ii = 0; ii < at.Length; ii++)
                    lista.AddRange(BitConverter.GetBytes(at[ii]));
                retval = lista.ToArray();
            }
            return retval;
        }

        /// <summary>
        /// Converte un Array di bits in un byte all'indice specificato
        /// </summary>
        /// <param name="byteindex">Serve in caso l'array sia più lungo di 8</param>
        /// <param name="bArray"></param>
        /// <returns></returns>
        public byte BitArrayToByte(int byteindex, bool[] bArray)
        {
            byte value = 0x00;
            int size = bArray.Count();
            int offset = byteindex * 8;
            
            if (offset>size)
                throw new Exception("BitArrayToByte: byteindex out of range");

            int offset_top = offset + 8;
            if (offset_top > size)
                offset_top = size;

            for (int x = offset; x < offset_top; x++)
            {
                byte shifter = (byte)(x % 8);
                value |= (byte)((bArray[x] == true) ? (0x01 << shifter) : 0x00);
            }
            return value;
        }

        /// <summary>
        /// Ritorna l'array di bytes serializzato
        /// </summary>
        /// <returns></returns>
        public byte[] GetStream()
        {
            List<byte> retval = stream;

            if (blist.Count > 0)
                retval.Add(BitArrayToByte(0, blist.ToArray()));
            return retval.ToArray();
        }
    }
}
