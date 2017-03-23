// ------------------------------------------------------------------------
//Società: WB IIIParty
//Anno: 2010
//Progetto: AMIL5
//Autore: Acquisti Leonardo
//Nome modulo software: WB.IIIParty.Commons.dll
//Data ultima modifica: $LastChangedDate: 2011-10-20 10:21:02 +0200 (gio, 20 ott 2011) $
//Versione: $Rev: 43 $
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Collections;
using System.Linq;
using System.Text;
using WB.IIIParty.Commons.Logger;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace WB.IIIParty.Commons.Collections
{
    /// <summary>
    /// Consente la gestione di array e liste generiche
    /// </summary>
    public class Array
    {
          
        #region Public Methods

        /// <summary>
        /// Set bit
        /// </summary>
        /// <param name="data">Dati</param>
        /// <param name="bitSet">Indice del bit</param>
        /// <param name="value">Valore del bit</param>
        public static void BitSet(ref byte data, int bitSet, bool value)
        {
            byte[] byteArray = new byte[1]{data};

            BitSet(ref byteArray, bitSet, value);

            data = byteArray[0];

        }

        /// <summary>
        /// Get Bit
        /// </summary>
        /// <param name="data">Dati</param>
        /// <param name="bitGet">Indice del bit</param>
        /// <returns>Valore del bit</returns>
        public static bool BitGet(byte data, int bitGet)
        {
            byte[] byteArray = new byte[1] { data };

            return BitGet(byteArray, bitGet);

        }

        /// <summary>
        /// Set bit
        /// </summary>
        /// <param name="data">Dati</param>
        /// <param name="bitSet">Indice del bit</param>
        /// <param name="value">Valore del bit</param>
        public static void BitSet(ref char data, int bitSet, bool value)
        {
            byte[] byteArray = BitConverter.GetBytes(data);

            BitSet(ref byteArray, bitSet, value);

            data = BitConverter.ToChar(byteArray, 0);

        }

        /// <summary>
        /// Get Bit
        /// </summary>
        /// <param name="data">Dati</param>
        /// <param name="bitGet">Indice del bit</param>
        /// <returns>Valore del bit</returns>
        public static bool BitGet(char data, int bitGet)
        {
            byte[] byteArray = BitConverter.GetBytes(data);

            return BitGet(byteArray, bitGet);

        }

        /// <summary>
        /// Set bit
        /// </summary>
        /// <param name="data">Dati</param>
        /// <param name="bitSet">Indice del bit</param>
        /// <param name="value">Valore del bit</param>
        public static void BitSet(ref double data, int bitSet, bool value)
        {
            byte[] byteArray = BitConverter.GetBytes(data);

            BitSet(ref byteArray, bitSet, value);

            data = BitConverter.ToDouble(byteArray, 0);

        }

        /// <summary>
        /// Get Bit
        /// </summary>
        /// <param name="data">Dati</param>
        /// <param name="bitGet">Indice del bit</param>
        /// <returns>Valore del bit</returns>
        public static bool BitGet(double data, int bitGet)
        {
            byte[] byteArray = BitConverter.GetBytes(data);

            return BitGet(byteArray, bitGet);

        }

        /// <summary>
        /// Set bit
        /// </summary>
        /// <param name="data">Dati</param>
        /// <param name="bitSet">Indice del bit</param>
        /// <param name="value">Valore del bit</param>
        public static void BitSet(ref float data, int bitSet, bool value)
        {
            byte[] byteArray = BitConverter.GetBytes(data);

            BitSet(ref byteArray, bitSet, value);

            data = BitConverter.ToSingle(byteArray, 0);

        }

        /// <summary>
        /// Get Bit
        /// </summary>
        /// <param name="data">Dati</param>
        /// <param name="bitGet">Indice del bit</param>
        /// <returns>Valore del bit</returns>
        public static bool BitGet(float data, int bitGet)
        {
            byte[] byteArray = BitConverter.GetBytes(data);

            return BitGet(byteArray, bitGet);

        }

        /// <summary>
        /// Set bit
        /// </summary>
        /// <param name="data">Dati</param>
        /// <param name="bitSet">Indice del bit</param>
        /// <param name="value">Valore del bit</param>
        public static void BitSet(ref int data, int bitSet, bool value)
        {
            byte[] byteArray = BitConverter.GetBytes(data);

            BitSet(ref byteArray, bitSet, value);

            data = BitConverter.ToInt32(byteArray, 0);

        }

        /// <summary>
        /// Get Bit
        /// </summary>
        /// <param name="data">Dati</param>
        /// <param name="bitGet">Indice del bit</param>
        /// <returns>Valore del bit</returns>
        public static bool BitGet(int data, int bitGet)
        {
            byte[] byteArray = BitConverter.GetBytes(data);

            return BitGet(byteArray, bitGet);

        }

        /// <summary>
        /// Set bit
        /// </summary>
        /// <param name="data">Dati</param>
        /// <param name="bitSet">Indice del bit</param>
        /// <param name="value">Valore del bit</param>
        public static void BitSet(ref long data, int bitSet, bool value)
        {
            byte[] byteArray = BitConverter.GetBytes(data);

            BitSet(ref byteArray, bitSet, value);

            data = BitConverter.ToInt64(byteArray, 0);

        }

        /// <summary>
        /// Get Bit
        /// </summary>
        /// <param name="data">Dati</param>
        /// <param name="bitGet">Indice del bit</param>
        /// <returns>Valore del bit</returns>
        public static bool BitGet(long data, int bitGet)
        {
            byte[] byteArray = BitConverter.GetBytes(data);

            return BitGet(byteArray, bitGet);

        }

        /// <summary>
        /// Set bit
        /// </summary>
        /// <param name="data">Dati</param>
        /// <param name="bitSet">Indice del bit</param>
        /// <param name="value">Valore del bit</param>
        public static void BitSet(ref short data, int bitSet, bool value)
        {
            byte[] byteArray = BitConverter.GetBytes(data);

            BitSet(ref byteArray, bitSet, value);

            data = BitConverter.ToInt16(byteArray, 0);

        }

        /// <summary>
        /// Get Bit
        /// </summary>
        /// <param name="data">Dati</param>
        /// <param name="bitGet">Indice del bit</param>
        /// <returns>Valore del bit</returns>
        public static bool BitGet(short data, int bitGet)
        {
            byte[] byteArray = BitConverter.GetBytes(data);

            return BitGet(byteArray, bitGet);

        }

        /// <summary>
        /// Set bit
        /// </summary>
        /// <param name="data">Dati</param>
        /// <param name="bitSet">Indice del bit</param>
        /// <param name="value">Valore del bit</param>
        public static void BitSet(ref uint data, int bitSet, bool value)
        {
            byte[] byteArray = BitConverter.GetBytes(data);

            BitSet(ref byteArray, bitSet, value);

            data = BitConverter.ToUInt32(byteArray, 0);

        }

        /// <summary>
        /// Get Bit
        /// </summary>
        /// <param name="data">Dati</param>
        /// <param name="bitGet">Indice del bit</param>
        /// <returns>Valore del bit</returns>
        public static bool BitGet(uint data, int bitGet)
        {
            byte[] byteArray = BitConverter.GetBytes(data);

            return BitGet(byteArray, bitGet);

        }

        /// <summary>
        /// Set bit
        /// </summary>
        /// <param name="data">Dati</param>
        /// <param name="bitSet">Indice del bit</param>
        /// <param name="value">Valore del bit</param>
        public static void BitSet(ref ulong data, int bitSet, bool value)
        {
            byte[] byteArray = BitConverter.GetBytes(data);

            BitSet(ref byteArray, bitSet, value);

            data = BitConverter.ToUInt64(byteArray, 0);

        }

        /// <summary>
        /// Get Bit
        /// </summary>
        /// <param name="data">Dati</param>
        /// <param name="bitGet">Indice del bit</param>
        /// <returns>Valore del bit</returns>
        public static bool BitGet(ulong data, int bitGet)
        {
            byte[] byteArray = BitConverter.GetBytes(data);

            return BitGet(byteArray, bitGet);

        }

        /// <summary>
        /// Set bit
        /// </summary>
        /// <param name="data">Dati</param>
        /// <param name="bitSet">Indice del bit</param>
        /// <param name="value">Valore del bit</param>
        public static void BitSet(ref ushort data, int bitSet, bool value)
        {
            byte[] byteArray = BitConverter.GetBytes(data);

            BitSet(ref byteArray, bitSet, value);

            data = BitConverter.ToUInt16(byteArray, 0);

        }

        /// <summary>
        /// Get Bit
        /// </summary>
        /// <param name="data">Dati</param>
        /// <param name="bitGet">Indice del bit</param>
        /// <returns>Valore del bit</returns>
        public static bool BitGet(ushort data, int bitGet)
        {
            byte[] byteArray = BitConverter.GetBytes(data);

            return BitGet(byteArray, bitGet);

        }

        /// <summary>
        /// Set bit
        /// </summary>
        /// <param name="data">Dati</param>
        /// <param name="bitSet">Indice del bit</param>
        /// <param name="value">Valore del bit</param>
        public static void BitSet(ref byte[] data,int bitSet,bool value)
        {
            BitArray f = new BitArray(data);

            f.Set(bitSet, value);
            
            f.CopyTo(data, 0);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="bitGet "></param>
        /// <returns></returns>
        public static bool BitGet(byte[] data, int bitGet)
        {
            BitArray f = new BitArray(data);
            
            return f.Get(bitGet);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bits"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(BitArray bits)
        {
            int numBytes = bits.Count / 8;
            if (bits.Count % 8 != 0) numBytes++;

            byte[] bytes = new byte[numBytes];
            int byteIndex = 0, bitIndex = 0;

            for (int i = 0; i < bits.Count; i++)
            {
                if (bits[i])
                    bytes[byteIndex] |= (byte)(1 << (bitIndex));

                bitIndex++;
                if (bitIndex == 8)
                {
                    bitIndex = 0;
                    byteIndex++;
                }
            }

            return bytes;
        }

        /// <summary>
        /// Compara due liste di tipo T
        /// </summary>
        /// <param name="dataA">Lista A</param>
        /// <param name="dataB">Lista B</param>
        public static bool Equals<T>(List<T> dataA,List<T> dataB)
        {
            //Caso lunghezza diversa
            if(dataA.Count!=dataB.Count) return false;

            for (int i = 0; i < dataA.Count; i++)
            {
                //Caso uno dei due valori null
                if ((dataA[i] == null) && (dataB[i] != null) ||
                    (dataA[i] != null) && (dataB[i] == null)) return false;

                //Controllo solo se i due valori sono != da null
                if((dataA[i]!=null)&&(dataB[i]!=null))
                    if (!dataA[i].Equals(dataB[i]))
                        return false;   

            }
            return true;

        }

        /// <summary>
        /// Compara due array di tipo T
        /// </summary>
        /// <param name="dataA">Array A</param>
        /// <param name="dataB">Array B</param>
        public static bool Equals<T>(T[] dataA, T[] dataB)
        {
            //Caso lunghezza diversa
            if (dataA.Length != dataB.Length) return false;

            for (int i = 0; i < dataA.Length; i++)
            {
                //Caso uno dei due valori null
                if ((dataA[i] == null) && (dataB[i] != null) ||
                    (dataA[i] != null) && (dataB[i] == null)) return false;

                //Controllo solo se i due valori sono != da null
                if ((dataA[i] != null) && (dataB[i] != null))
                    if (!dataA[i].Equals(dataB[i]))
                        return false;

            }
            return true;

        }

        #endregion
    }
}
