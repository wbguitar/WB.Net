// ------------------------------------------------------------------------
//Società:              WB IIIParty
//Anno:                 2010
//Progetto:             FAC 
//Autore:               Gabriele Valentini
//Nome modulo software: Fac.Commons.dll
//Data ultima modifica: $LastChangedDate: 2011-10-20 10:21:02 +0200 (gio, 20 ott 2011) $
//Versione:             $Rev: 43 $
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Threading;
using WB.IIIParty.Commons.Logger;
using WB.IIIParty.Commons.Sql;
using WB.IIIParty.Commons.TimeStamp;

namespace WB.IIIParty.Commons.Data.Sql.NoRedundantTables
{

    /// <summary>
    /// Rappresenta una tabella di database.
    /// </summary>
    public class NoRedundantTable: IDisposable
    {
        #region Static
        //private static string CustomDateTime = "yyyyMMdd HH:mm:ss.fff";
        private static DateTime defaultSyncDate = new DateTime(2000,01,01,00,00,00);
        #endregion Static

        #region PublicProperty
        /// <summary>
        /// Nome della tabella
        /// </summary>
        public string Name  
        {
            get { return this.name.ToUpper(); }
            set 
            { 
                this.name = value;
                this.completeName = this.db.name + "." + this.schema + "." + this.name;
                if ((this.completeName != null) && (this.completeName != ""))
                    this.completeName = this.completeName.ToUpper();
            }
        }

        /// <summary>
        /// Schema al quale la tabella appartiene
        /// </summary>
        public string Schema
        {
            get { return this.schema.ToUpper(); }
            set
            {
                this.schema = value;
                this.completeName = this.db.name + "." + this.schema + "." + this.name;
                if ((this.completeName != null) && (this.completeName != "")) 
                    this.completeName = this.completeName.ToUpper();
            }
        }

        /// <summary>
        /// Nome del database al quale la tabella appartiene.
        /// </summary>
        public NoRedundantDatabase Db
        {
            get { return this.db; }
            set
            {
                this.db = value;
                this.completeName = this.db.name + "." + this.schema + "." + this.name;
                if ((this.completeName != null) && (this.completeName != ""))
                    this.completeName = this.completeName.ToUpper();
            }
        }

        /// <summary>
        /// Nome completo della tabella: 
        /// "DbName.SchemaName.TableName".
        /// </summary>
        public string CompleteName
        {
            get { return this.completeName.ToUpper(); }
        }

        ///// <summary>
        ///// Livello di relazione fra tabelle in un db
        ///// </summary>
        //public Int32 Level
        //{
        //     get { return this.level; }
        //}

        #endregion PublicProperty

        #region PrivateField
     
        private string completeName = string.Empty;
        private string name;
        private string schema;
        private NoRedundantDatabase db;
        private IMessageLog log;
       
        #endregion PrivateField
                
        #region PublicField

        ///// <summary>
        ///// Elenco delle colonne che compongono la chiave primaria.
        ///// </summary>
        //public List<string> primaryKeys;

        ///// <summary>
        ///// Elenco di tutte le colonne.
        ///// </summary>
        //public List<string> allColumns;

                
        #endregion PublicField

        #region Constructor
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_db">Database di appartenenza</param>
        /// <param name="_schema">Schema di appartenenza</param>
        /// <param name="_name">Nome della tabella</param>
        public NoRedundantTable(NoRedundantDatabase _db,
                       string _schema,
                       string _name)
                       //List<string> _primaryKeys,
                       //List<string> _allColumns)
                       
        {
            log = LoggerToFile.LoggerToFile_singleton.GetLogger();
            this.Db = _db;
            this.Schema = _schema;
            this.Name = _name;
            //this.primaryKeys = _primaryKeys;
            //this.allColumns = _allColumns;            
        }

        #endregion Constructor
        
            

        
       
        
        #region IDisposable Members

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (log != null)
            LoggerToFile.LoggerToFile_singleton.Dispose();
        }

        #endregion

    }// END CLASS DEFINITION Table
}
