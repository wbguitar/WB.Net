using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace WB.IIIParty.Commons.Sql
{
    /// <summary>
    /// Consente l'import e l'esport di file sulla tabella FileStore
    /// </summary>
    public class DbFileStore
    {
        /// <summary>
        /// Importa il file nella tabella FileStore
        /// </summary>
        /// <param name="filePath">Path e FileName di input</param>
        /// <param name="connectionString">Stringa di Connessione al database</param>
        /// <param name="useCompression">Utilizza compressione GZipStream</param>
        public static void ImportFile(string filePath, string connectionString,bool useCompression)
        {

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string fileName = System.IO.Path.GetFileName(filePath);
                
                using(MemoryStream ms = new MemoryStream())
                using(FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (System.IO.Compression.GZipStream gzip = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress))
                {
                    BinaryReader brin = new BinaryReader(fs);
                    Byte[] inbytes = brin.ReadBytes((Int32)fs.Length);
                    gzip.Write(inbytes, 0, inbytes.Length);
                    gzip.Close();
                    Byte[] outbytes = ms.ToArray();

                    //insert the file into database

                    string strQuery = "insert into FileStore(FileName, FileData)" +
                       " values (@Name, @Data)";

                    using (SqlCommand cmd = new SqlCommand(strQuery, conn))
                    {
                        cmd.Parameters.Add("@Name", System.Data.SqlDbType.VarChar).Value = fileName;
                        cmd.Parameters.Add("@Data", System.Data.SqlDbType.VarBinary).Value = outbytes;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Esporta il file dalla tabella FileStore
        /// </summary>
        /// <param name="fileName">Nome del file su FileStore</param>
        /// <param name="destFilePath">Path e FileName di output</param>
        /// <param name="connectionString">Stringa di Connessione al database</param>
        public static void ExportFile(string fileName,string destFilePath, string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                //insert the file into database

                string strQuery = "select FileName,FileData from FileStore where FileName='" + fileName + "'";

                using(SqlCommand cmd = new SqlCommand(strQuery, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                        throw new Exception("File not found on table FileStore. FileName: " + fileName);
                    reader.Read();

                    string bi = (string)reader["FileName"];
                    byte[] bu = (byte[])reader["FileData"];

                    using (MemoryStream ms = new MemoryStream(bu))
                    using (FileStream fs = new FileStream(destFilePath, FileMode.Create, FileAccess.Write))
                    using (System.IO.Compression.GZipStream gzip = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Decompress, false))
                    {
                        using (BinaryReader br = new BinaryReader(gzip))
                        {
                            int readed = 0;
                            while ((readed = gzip.ReadByte()) != -1)
                            {
                                fs.WriteByte((byte)readed);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Rimuove un file dal FileStore
        /// </summary>
        /// <param name="fileName">Nome del file da eliminare</param>
        /// <param name="connectionString">Stringa di Connessione al database</param>
        public static void RemoveFile(string fileName, string connectionString)
        {

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string strQuery = "delete from FileStore where FileName='" + fileName + "'";

                using (SqlCommand cmd = new SqlCommand(strQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }

            }
        }

        /// <summary>
        /// Cancella tutti i files sul FileStore
        /// </summary>
        /// <param name="connectionString">Stringa di Connessione al database</param>
        public static void Clear(string connectionString)
        {

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                
                string strQuery = "delete from FileStore";

                using (SqlCommand cmd = new SqlCommand(strQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            
            }
        }
    }
}
