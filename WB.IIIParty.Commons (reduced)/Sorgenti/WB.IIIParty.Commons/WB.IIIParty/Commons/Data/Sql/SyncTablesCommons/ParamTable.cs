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
    /// Tabella dei parametri di configurazione
    /// </summary>
    public class ParamTable
    {

        #region PrivateField
        private string dbName;
        private string schema;
        private string tableName;

        #endregion PrivateField

        #region Property
        /// <summary>
        /// Nome del databese
        /// </summary>
        public string DbName
        {
            get { return this.dbName; }
            set { }
        }

        /// <summary>
        /// Nome dello schema
        /// </summary>
        public string Schema
        {
            get { return this.schema; }
            set { }
        }

        /// <summary>
        /// Nome della tabella 
        /// </summary>
        public string TableName
        {
            get { return this.tableName; }
            set { }
        }

        #endregion Property

        #region Costructor
        /// <summary>
        /// Tabella dei parametri di configurazione.
        /// </summary>
        /// <param name="_dbName">Nome del database di appartenenza.</param>
        /// <param name="_schema">Nome dello schema.</param>
        /// <param name="_tableName">Nome della tabella.</param>
        public ParamTable(string _dbName, string _schema, string _tableName )
        {
            this.dbName = _dbName;
            this.schema = _schema;
            this.tableName = _tableName;
        }
        #endregion Costructor
    }
}
