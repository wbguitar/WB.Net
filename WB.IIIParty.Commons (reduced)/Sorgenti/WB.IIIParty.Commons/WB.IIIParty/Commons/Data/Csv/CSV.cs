using System;
using System.Collections;
using System.IO;
using System.IO.IsolatedStorage;

namespace WB.IIIParty.Commons.Data.Csv
{
	/// <summary>
    /// Summary description for CSV
	/// </summary>
	public class CSV
	{
		#region Variable Declaration

		string[] m_header=null;
		string m_filepath=null;
		bool m_IsOpen;
		StreamWriter write;
		//StreamReader read;
        TextReader read;
		FileStream m_file;
		Exception m_exception;

		#endregion

		#region Constructor

		/// <summary>
		/// Nuovo Oggetto
		/// </summary>
		/// <param name="header">Formattazione dell'intestazione del File CSV</param>
		/// <param name="filepath">Path e Nome del file CSV</param>
		public CSV(string[] header,string filepath)
		{
            
			this.m_filepath=filepath;
			this.m_header=header;

			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Nuovo Oggetto
		/// </summary>
		public CSV()
		{
		
			//
			// TODO: Add constructor logic here
			//
		}

		#endregion

		#region Properties

		/// <summary>
		/// Formattazione dell'intestazione del File CSV
		/// </summary>
		public string[] Header
		{
			set{this.m_header=value;}
			get{return this.m_header;}
		}

		/// <summary>
		/// Path e Nome del file CSV
		/// </summary>
		public string FilePath
		{
			set{this.m_filepath=value;}
			get{return this.m_filepath;}
		}
        /// <summary>
        /// 
        /// </summary>
        public bool IsOpen
        {
            get { return m_IsOpen; }
        }
        /// <summary>
        /// 
        /// </summary>
		public Exception LastException
		{
			get{return this.m_exception;}
		}

		#endregion

		#region Public Method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encoding"></param>
        public void OpenFileAppend(System.Text.Encoding encoding)
        {
            try
            {
                FileMode access = FileMode.Append;                

                this.m_file = new FileStream(this.m_filepath, access);
                //this.read = new StreamReader(this.m_file, encoding);
                this.write = new StreamWriter(this.m_file, encoding);

                this.m_IsOpen = true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                this.m_exception = ex;
                this.m_IsOpen = false;
            }
        }

		/// <summary>
		/// Apre il File impostato sulla Proprietà FilePath
		/// </summary>
		/// <param name="_createnew">Crea o Sovrascrive un File</param>
        /// <param name="encoding"></param>
        public void OpenFile(bool _createnew,System.Text.Encoding encoding)
		{
			try
			{
				FileMode access;
				if(_createnew)
				{
					access = FileMode.Create;
				}
				else
				{
					access = FileMode.Open;
				}

				this.m_file=new FileStream(this.m_filepath,access);
                this.read = new StreamReader(this.m_file, encoding);
                this.write = new StreamWriter(this.m_file, encoding);

				this.m_IsOpen=true;
			}
			catch(Exception ex)
			{
                System.Windows.Forms.MessageBox.Show(ex.Message);
				this.m_exception=ex;
				this.m_IsOpen=false;
			}
		}
        /// <summary>
        /// 
        /// </summary>
		public void CloseFile()
		{
			try
			{
				if(this.m_file!=null)
				{
					this.m_file.Close();
				}
				this.m_IsOpen=false;
			}
			catch(Exception ex)
			{
				this.m_exception=ex;
			}
		}

		/// <summary>
		/// Scrive una linea sul File CSV contenente l'Header
		/// </summary>
		public void WriteHeader(char separator)
		{
			try
			{
				this.CheckFilePath();
				this.CheckHeader();

				//WRITE HEADER
				for(int i=0;i<this.m_header.Length;i++)
				{
					write.Write(this.m_header[i]);
					if(i<(this.m_header.Length-1))
					{
                        write.Write(separator);
					}
				}
				write.Write(write.NewLine);
				write.Flush();
			}
			catch(IOException ex)
			{
				this.m_exception=ex;
				this.m_IsOpen=false;
			}
			catch(Exception ex)
			{
				this.m_exception=ex;
			}

		}

		/// <summary>
		/// Scrive una linea sul File CSV
		/// </summary>
		/// <param name="currentline"></param>
        /// <param name="separator"></param>
        public void WriteLine(string[] currentline, char separator)
		{
			try
			{
				this.CheckFilePath();
				this.CheckHeader();

				if(currentline.Length==this.m_header.Length)
				{
					for(int y=0;y<this.m_header.Length;y++)
					{
						write.Write(currentline[y]);
						if(y<(this.m_header.Length-1))
						{
                            write.Write(separator);
						}
					}
					write.Write(write.NewLine);
				}
				write.Flush();
			}
			catch(IOException ex)
			{
				this.m_exception=ex;
				this.m_IsOpen=false;
			}
			catch(Exception ex)
			{
				this.m_exception=ex;
			}

		}

		/// <summary>
		/// Scrive Array di linee su File CSV
		/// </summary>
		/// <param name="dataexport"></param>
        /// <param name="separator"></param>
        public void WriteData(ArrayList dataexport,char separator)
		{
			try
			{
				this.CheckFilePath();
				this.CheckHeader();

				//WRITE LINS
				for(int i=0;i<dataexport.Count;i++)
				{
					string[] currentline = (string[])dataexport[i];
					if(currentline.Length==this.m_header.Length)
					{
						for(int y=0;y<this.m_header.Length;y++)
						{
							write.Write(currentline[y]);
							if(y<(this.m_header.Length-1))
							{
								write.Write(separator);
							}
						}
						write.Write(write.NewLine);
					}
				}
				write.Flush();
			}
			catch(IOException ex)
			{
				this.m_exception=ex;
				this.m_IsOpen=false;
			}
			catch(Exception ex)
			{
				this.m_exception=ex;
			}
			
		}

		/// <summary>
		/// Legge da un file CSV i campi specificati sulla Proprietà Header
		/// </summary>
		/// <returns></returns>
		public ArrayList ReadData(char[] separators)
		{
			try
			{
				string line;
				int linecounter=0;
				Hashtable currentheader=new Hashtable();
				ArrayList dataimport = new ArrayList();

				this.CheckFilePath();
				this.CheckHeader();
		
				while((line = read.ReadLine())!=null)
				{
                    string[] formatline = line.Split(separators);

					if(linecounter>0)
					{	

						string[] currentline = new string[this.m_header.Length];
						for(int i = 0;i<this.m_header.Length;i++)
						{
							if(currentheader.ContainsKey(this.m_header[i]))
							{
                                if (i >= formatline.Length) { currentline[i] = ""; }
                                else
								currentline[i]=formatline[(int)currentheader[this.m_header[i]]];
							}
							else
							{
								currentline[i]="";
							}
						}
						dataimport.Add(currentline);
				
					}
					else
					{
						for(int i=0;i<formatline.Length;i++)
						{
							if(!currentheader.ContainsKey(formatline[i]))
							{
								currentheader.Add(formatline[i],i);
							}
							else
							{
								throw new Exception("Duplicate Column Name!");
							}
						}
					}
					linecounter++;
				}
				return dataimport;
			}
			catch(IOException ex)
			{
				this.m_exception=ex;
				this.m_IsOpen=false;
				return null;
			}
			catch(Exception ex)
			{
				this.m_exception=ex;
				return null;
			}

		}

		#endregion

		#region Private Method

		private void CheckHeader()
		{
			if(this.m_header==null)
			{
				throw new Exception("Header Empty");
			}
		}

		private void CheckFilePath()
		{
			if(this.m_filepath==null)
			{
				throw new Exception("File Path Empty");
			}
		}

		
		#endregion


	}
}
