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
using System.Collections;
using System.Text;
  
namespace WB.IIIParty.Commons.Data.Sql.SyncTablesCommons
{

    /// <summary>
    /// Rappresenta una riga da inserire 
    /// </summary>
    public class InsertDataRow : Row
    {

        #region PublicField
        /// <summary>
        /// Elenco delle colonne oggetto della insert
        /// </summary>
        public List<string> columnsName;

        /// <summary>
        /// Elenco dei valori da inserire 
        /// </summary>
        public List<object> values;

        #endregion PublicField

        #region Constructor
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="_dbName">Nome del database</param>
        /// <param name="_dbSchema">Nome dello schema</param>
        /// <param name="_tableName">Nome della tabella di destinazione</param>
        /// <param name="_columnsName">Elenco delle colonne oggetto della insert</param>
        /// <param name="_values">Elenco dei valori da inserire</param>
        public InsertDataRow(string _dbName,string _dbSchema,
                             string _tableName,List<string> _columnsName,
                             List<object> _values
                            )
                             : base (_dbName,_dbSchema,_tableName)

        {
            this.columnsName = _columnsName;
            this.values = _values;
        }

        #endregion Constructor

    }// END CLASS DEFINITION InsertDataRow
}
