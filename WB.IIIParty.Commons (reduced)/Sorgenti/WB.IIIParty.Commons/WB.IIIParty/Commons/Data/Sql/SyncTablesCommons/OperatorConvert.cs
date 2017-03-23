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
using WB.IIIParty.Commons.Data;

namespace WB.IIIParty.Commons.Data.Sql.SyncTablesCommons
{
    
    /// <summary>
    /// Convertitore da enumerato a stringa degli operatori
    /// logici e di confronto.
    /// </summary>
    public class OperatorConvert
    {
        /// <summary>
        /// Costruttore
        /// </summary>
        public OperatorConvert()
        { }
      
        /// <summary>
        /// Convertitore di un enumerato in stringa
        /// </summary>
        /// <param name="_operator">Operatore logico in forma di enumerato</param>
        /// <returns>Operatore logico</returns>
        public string GetStringFromEnum(LogicalOperatorEnum _operator)
        {
            string result = string.Empty;


            switch (_operator)
            {
                case LogicalOperatorEnum.AND:
                    {
                        result = " AND ";
                        break;
                    }
                case LogicalOperatorEnum.OR:
                    {
                        result = " OR ";
                        break;
                    }
                case LogicalOperatorEnum.NONE:
                    {
                        result = string.Empty;
                        break;
                    }
                default:
                    {
                        result = string.Empty;
                        break;
                    }
            }
            return result;
        }
        
        /// <summary>
        /// Convertitore di un enumerato in stringa
        /// </summary>
        /// <param name="_operator">Operatore di confronto in forma di enumerato</param>
        /// <returns>operatore di confronto</returns>
        public string GetStringFromEnum(ComparisonOperatorEnum _operator)
        {
            string result = string.Empty;
            switch (_operator)
            {
                case ComparisonOperatorEnum.Equals:
                    {
                        result = " = ";
                        break;
                    }
                case ComparisonOperatorEnum.Greater:
                    {
                        result = " > ";
                        break;
                    }
                case ComparisonOperatorEnum.GreaterEqual:
                    {
                        result = " >= ";
                        break;
                    }

                case ComparisonOperatorEnum.Less:
                    {
                        result = " < ";
                        break;
                    }


                case ComparisonOperatorEnum.LessEqual:
                    {
                        result = " <= ";
                        break;
                    }


                case ComparisonOperatorEnum.NotEqual:
                    {
                        result = " != ";
                        break;
                    }
                case ComparisonOperatorEnum.Is:
                    {
                        result = " is ";
                        break;
                    }
                case ComparisonOperatorEnum.NotIs:
                    {
                        result = " not is ";
                        break;
                    }
                default:
                    {
                        result = string.Empty;
                        break;
                    }
            }
            return result;
        }
    }
}
