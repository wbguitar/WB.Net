// ------------------------------------------------------------------------
//Società:              WB IIIParty
//Anno:                 2008
//Progetto:             AMIL5
//Autore:               Marziali Valentina ll
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
    /// Rappresenta una riga di tabella.
    /// </summary>
    public abstract class Row
    {

        #region PublicField
        /// <summary>
        /// Nome del database al quale appartiene la tabella
        /// </summary>
        public string dbName;

        /// <summary>
        /// Nome dello schema al quale appartiene la tabella
        /// </summary>
        public string schema;

        /// <summary>
        /// Nome della tabella alla quale appartiene la riga
        /// </summary>
        public string tableName;

        #endregion PublicField

        #region Constructor
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_dbName">Nome del database</param>
        /// <param name="_schema">Nome dello schema</param>
        /// <param name="_tableName">Nome della tabella</param>
        public Row(string _dbName,
                         string _schema,
                         string _tableName)
        {
            this.dbName = _dbName;
            this.schema = _schema;
            this.tableName = _tableName;
        }
        
        /// <summary>
        /// Costruttore
        /// </summary>
        public Row()
        {
            this.dbName = string.Empty;
            this.schema = string.Empty;
            this.tableName = string.Empty;
        }

        #endregion Constructor

    }// END CLASS DEFINITION DataRow
}
