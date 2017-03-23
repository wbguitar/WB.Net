// ------------------------------------------------------------------------
//Società: WB IIIParty
//Anno: 2008 
//Progetto: AMIL5
//Autore: Acquisti Leonardo
//Nome modulo software: WB.IIIParty.Commons.dll
//Data ultima modifica: $LastChangedDate: 2011-10-20 10:21:02 +0200 (gio, 20 ott 2011) $
//Versione: $Rev: 43 $
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;

namespace WB.IIIParty.Commons.Security.Cryptography
{
    /// <summary>
    /// Tools di Codifica e Decodifica Rijndael
    /// </summary>
    public class RijndaelStringCryptography
    {
        #region Constants

        private const string Key = "AxTYQWCvGTFRbgLLAxTYQWCvGTFRbgLL";//32 byte
        private const string Iv = "QWExcfTyUxxLOafOQWExcfTyUxxLOafO"; //32 byte

        #endregion

        #region Public static Members

        /// <summary>
        /// Codifica una stringa di testo su un file con algoritmo Rijndael
        /// </summary>
        /// <param name="str">Stringa da codificare</param>
        /// <param name="file">File di destinazione</param>
        /// <param name="overwrite">Sovrascrive il file se esiste</param>
        /// <param name="key">Chiave di codifica a 256 bit</param>
        public static void EncodeToFile(string str, string file, bool overwrite, string key)
        {
            if ((overwrite) && (System.IO.File.Exists(file)))
                System.IO.File.Delete(file);
            System.IO.FileStream stream = new System.IO.FileStream(file, System.IO.FileMode.CreateNew,
                 System.IO.FileAccess.Write);
            string encoded = Encode(str, key);
            byte[] data = ASCIIEncoding.UTF8.GetBytes(encoded);
            stream.Write(data, 0, data.Length);
            stream.Close();
            stream.Dispose();
        }

        /// <summary>
        /// Decodifica un file su una stringa di testo con algoritmo Rijndael
        /// </summary>
        /// <param name="file">File da decodificare</param>
        /// <param name="key">Chiave di codifica a 256 bit</param>
        /// <returns>Stringa decodificata</returns>
        public static string DecodeFromFile(string file,string key)
        {
            System.IO.FileStream stream = new System.IO.FileStream(file, System.IO.FileMode.Open,
                 System.IO.FileAccess.Read);
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            string decoded = Decode(ASCIIEncoding.UTF8.GetString(data), key); 
            stream.Close();
            stream.Dispose();
            return decoded;
        }

        /// <summary>
        /// Codifica una stringa di testo su un file con una chiave di codifica definita con algoritmo Rijndael
        /// </summary>
        /// <param name="str">Stringa da codificare</param>
        /// <param name="file">File di destinazione</param>
        /// <param name="overwrite">Sovrascrive il file se esiste</param>
        public static void EncodeToFile(string str, string file, bool overwrite)
        {
            EncodeToFile(str, file, overwrite, Key);            
        }

        /// <summary>
        /// Decodifica un file su una stringa di testo con una chiave di codifica definita con algoritmo Rijndael
        /// </summary>
        /// <param name="file">File da decodificare</param>
        /// <returns>Stringa decodificata</returns>
        public static string DecodeFromFile(string file)
        {
            return DecodeFromFile(file, Key);
        }

        /// <summary>
        /// Codifica una stringa con algoritmo Rijndael
        /// </summary>
        /// <param name="str">Stringa da codificare</param>
        /// <param name="key">Chiave di codifica a 256 bit</param>
        /// <returns>Ritorna la stringa codificata</returns>
        public static string Encode(string str,string key)
        {
            RijndaelManaged rjm = new RijndaelManaged();
            rjm.KeySize = 256;
            rjm.BlockSize = 256;
            rjm.Key = ASCIIEncoding.ASCII.GetBytes(key);
            rjm.IV = ASCIIEncoding.ASCII.GetBytes(Iv);
            Byte[] input = Encoding.UTF8.GetBytes(str);
            Byte[] output = rjm.CreateEncryptor().TransformFinalBlock(input, 0,
                input.Length);
            return Convert.ToBase64String(output);
        }
        /// <summary>
        /// Codifica una stringa con algoritmo Rijndael
        /// </summary>
        /// <param name="str">Stringa da codificare</param>
        /// <returns>Ritorna la stringa codificata</returns>
        public static string Encode(string str)
        {
            return Encode(str, Key);
        }

        /// <summary>
        /// Decodifica una stringa con algoritmo Rijndael
        /// </summary>
        /// <param name="str">Stringa da decodificare</param>
        /// <param name="key">Chiave di codifica a 256 bit</param>
        /// <returns>Ritorna la stringa decodificata</returns>
        public static string Decode(string str, string key)
        {
            RijndaelManaged rjm = new RijndaelManaged();
            rjm.KeySize = 256;
            rjm.BlockSize = 256;
            rjm.Key = ASCIIEncoding.ASCII.GetBytes(key);
            rjm.IV = ASCIIEncoding.ASCII.GetBytes(Iv);
            Byte[] input = Convert.FromBase64String(str);
            Byte[] output = rjm.CreateDecryptor().TransformFinalBlock(input, 0,
                input.Length);
            return Encoding.UTF8.GetString(output);
        }
        /// <summary>
        /// Decodifica una stringa con algoritmo Rijndael
        /// </summary>
        /// <param name="str">Stringa da decodificare</param>
        /// <returns>Ritorna la stringa decodificata</returns>
        public static string Decode(string str)
        {
            return Decode(str,Key);
        }

        #endregion
    }
}
