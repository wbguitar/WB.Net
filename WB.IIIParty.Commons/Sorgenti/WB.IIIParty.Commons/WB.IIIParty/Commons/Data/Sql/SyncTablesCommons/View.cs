// ------------------------------------------------------------------------
//Società:              WB IIIParty
//Anno:                 2008
//Progetto:             AMIL5 
//Autore:               Marziali Valentina
//Nome modulo software: SyncTablesCommons.dll
//Data ultima modifica: $LastChangedDate: 2011-10-20 10:21:02 +0200 (gio, 20 ott 2011) $
//Versione:             $Rev: 43 $
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace WB.IIIParty.Commons.Data.Sql.SyncTablesCommons
{
    /// <summary>
    /// Rappresenta una vista su una tabella
    /// </summary>
    public class View
    {
        #region PublicProperty
        /// <summary>
        /// Nome della tabella
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set
            {
                this.name = value;
                this.completeName = this.db.name + "." + this.schema + "." + this.name;
            }
        }

        /// <summary>
        /// Schema al quale la tabella appartiene
        /// </summary>
        public string Schema
        {
            get { return this.schema; }
            set
            {
                this.schema = value;
                this.completeName = this.db.name + "." + this.schema + "." + this.name;
            }
        }

        /// <summary>
        /// Nome del database al quale la tabella appartiene.
        /// </summary>
        public Database Db
        {
            get { return this.db; }
            set
            {
                this.db = value;
                this.completeName = this.db.name + "." + this.schema + "." + this.name;
            }
        }

        /// <summary>
        /// Nome completo della tabella: 
        /// "DbName.SchemaName.TableName".
        /// </summary>
        public string CompleteName
        {
            get { return this.completeName; }
        }

        #endregion PublicProperty

        #region PrivateField

        private string completeName = string.Empty;
        private string name;
        private string schema;
        private Database db;
        
        #endregion PrivateField

        #region PublicField
        /// <summary>
        /// Elenco di tutte le colonne presenti nella vista
        /// </summary>
        public List<string> allColumns;
        /// <summary>
        /// Nome della colonna che indica che una riga è cancellata
        /// </summary>
        public string isDeletedColumnName;

        #endregion PublicField

        #region Constructor
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_db">Nome del database</param>
        /// <param name="_schema">Nome dello schema</param>
        /// <param name="_name">Nome della vista</param>
        /// <param name="_allColumns">Elenco di tutte le colonne presenti nella vista</param>
        /// <param name="_isDeletedColumnName">Nome della colonna che indica che una riga è cancellata</param>
        public View(Database _db,
                       string _schema,
                       string _name,
                       List<string> _allColumns,
                       string _isDeletedColumnName)
        {
            this.Db = _db;
            this.Schema = _schema;
            this.Name = _name;
            this.allColumns = _allColumns;
            this.isDeletedColumnName = _isDeletedColumnName;
            
        }

        #endregion Constructor

    }
}
