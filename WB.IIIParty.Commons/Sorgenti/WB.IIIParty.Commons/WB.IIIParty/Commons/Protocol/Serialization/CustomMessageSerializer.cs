// ------------------------------------------------------------------------
//Società: WB IIIParty
//Anno: 2010
//Progetto: AMIL5
//Autore: Acquisti Leonard
//Nome modulo software: WB.IIIParty.Commons.dll
//Data ultima modifica: $LastChangedDate: 2011-10-20 10:21:02 +0200 (gio, 20 ott 2011) $
//Versione: $Rev: 43 $
// ------------------------------------------------------------------------

using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Xml;

using AODL.Document.TextDocuments;
using AODL.Document.Content.Text;
using AODL.Document.Content.Tables;
using AODL.Document.Styles;
using AODL.Document.Styles.Properties;
using AODL.Document.Styles.MasterStyles;

namespace WB.IIIParty.Commons.Protocol.Serialization
{
    /// <summary>
    /// Implemena l'interfaccia IMessageParser per la serializzazione binaria di messaggi su uno stream
    /// </summary>
    public class CustomMessageSerializer : IMessageParser
    {
        #region Private Variables
        private uint BaseCustomMessageLength = 6;
        private Assembly assobj;

        // contiene l'associazione tra MsgId e MessageInfo (contenitore delle 
        // info e delle proprietà del messaggio)
        Dictionary<UInt16, MessageInfo> msgid2imsg = new Dictionary<UInt16, MessageInfo>();

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<UInt16, MessageInfo> MessageMap
        {
            get { return msgid2imsg; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Questo costruttore a differenza degli altri serializzatori 
        /// specializzati deve raccogliere informazioni relative alla classe
        /// in cui viene usato in modo da creare delle tabelle hash di
        /// associazione : MessageID/IMessage e Ordine/Property
        /// Queste tabelle hash verranno successivamente utilizzate nei
        /// metodi ParseMessage e SerializeMessage in modo da ricomporre
        /// le caratteristiche del messaggio.
        /// Per identificare le classi e le proprietà utili allo scopo
        /// sono state create le classi "AttributeMessage" e "AttributeProperty"
        /// </summary>
        /// <param name="nmspace"></param>
        /// <param name="assembly"></param>
        public CustomMessageSerializer(string nmspace, Assembly assembly)
        {
            TetByteFiller filler = new TetByteFiller();

            Type type;
//            Assembly assobj = Assembly.GetExecutingAssembly();
            assobj = assembly;
            
            Console.WriteLine("Assembly name: " + assobj.FullName);

            // Memorizza in Tipi[] tutte le classi dell'assembly con namespace come quello passato per parametro 
            Type[] Tipi = assobj.GetTypes().Where(t => ((t.Namespace!=null) && t.Namespace.Contains(nmspace)
                && (t.IsClass))).ToArray();

            int tipiCount = Tipi.Count();
            for (int ii = 0; ii < tipiCount; ii++)
            {
                type = Tipi[ii];
                
                    // Analizza solo le classi con l'attributo [TetMessageAttribute(msgid)]
                    object[] MsgAttributes = type.GetCustomAttributes(typeof(TetMessageAttribute), false);
                    if (MsgAttributes.Length > 0)
                    {
                        // Aggiunge l'associazione.
                        MessageInfo msginfoobj = new MessageInfo();
                        msginfoobj.msgtype = type;

                        //TetMessageAttribute msgattribute=(TetMessageAttribute)MsgAttributes[0];
                        // msgattribute.MsgId

                        // Scorre tutte le proprietà della classe Msg filtrando solo
                        // quelle con attributo "TetPropertyAttribute"
                        PropertyInfo[] propers = type.GetProperties();
                        int count = propers.Count();
                        for (int zz=0; zz<count; zz++)
                        {
                             PropertyInfo pInfo = propers[zz];
                            
                            // Analizza solo le proprietà con attributo TetPropertyAttribute
                            object[] PropAttributes = pInfo.GetCustomAttributes(typeof(TetPropertyAttribute), false);
                            if (PropAttributes.Length > 0)
                            {
                                // estrae il parametro ordine dall'attributo
                                TetPropertyAttribute attribute = (TetPropertyAttribute)PropAttributes[0];
                                PropertyOrderInfo poi = new PropertyOrderInfo();
                                poi.PropInfo = pInfo;
                                poi.OrderNum = attribute.PropertyOrder;

                                // se è una stringa si deve verificare se è unicode tramite
                                // attributo e comunque dobbiamo premettere
                                // la lunghezza.
                                if (pInfo.PropertyType == typeof(string) || pInfo.PropertyType == typeof(string[]))
                                {
                                    object[] PropUnicode = pInfo.GetCustomAttributes(typeof(TetPropertyUnicodeAttribute), false);
                                    if (PropUnicode.Length > 0)
                                        poi.StringType = StringTypeEnum.Unicode;
                                    else
                                        poi.StringType = StringTypeEnum.Ascii;
                                }
                                else
                                    poi.StringType = StringTypeEnum.Null;

                                // Memorizzare la classe
                                poi.IsClass = filler.IsClass(pInfo.PropertyType);
                                poi.IsArray = filler.IsArray(pInfo.PropertyType);
                                poi.IsList = filler.IsList(pInfo.PropertyType);
                                poi.IsEnum = pInfo.PropertyType.GetType().IsEnum;
                                
                                msginfoobj.ord2prop.Add(poi);
                            }
                        }

                        // Ordina gli elementi PropertyInfo per numero di ordine
                        msginfoobj.ord2prop.Sort(ComparePropInfo);
                        msgid2imsg.Add(((TetMessageAttribute)MsgAttributes[0]).MsgId, msginfoobj);
                    }
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Metodo per esportazione documento.
        /// </summary>
        public string ExportDocument()
        {
            // Esportatore che si basa solo su comandi locali
            string xmlfilename = Path.ChangeExtension(assobj.Location, ".xml");
            string docfilename = Path.ChangeExtension(assobj.Location, ".odt");

            // INSERIRE INTERRUZIONE DI PAGINA
            // Lettura file XML
            StreamReader streamReader;

            try
            {
                streamReader = new StreamReader(xmlfilename);
            }
            catch (FileNotFoundException exception)
            {
                // File non trovato
                throw new Exception("XML Document not found: Check compiler options.", exception);
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(streamReader);

            Dictionary<string,XmlElement> xmltags=new Dictionary<string,XmlElement>();
            // Carica tutti gli elementi dal file xml della documentazione
            foreach (XmlElement xmlElement in xmlDocument["doc"]["members"])
            {
                xmltags.Add(xmlElement.Attributes["name"].Value, xmlElement);
            }

            // Preparazione dell'oggetto TextDocument (AODL)
            TextDocument document = new TextDocument();
            document.New();

            // TITOLO PRINCIPALE
            DocAddTitle(document, "Descrizione Protocollo", Headings.Heading_20_1);

            // Used to keep previous boolean information
            string pnameold = "";
            string ptypeold = "";
            string psizeold = "";
            string pdescold = "";

            // Lista degli enumerati 
            List<Dictionary<int,string>> enumlist=new List<Dictionary<int,string>>();

            // Crea una tabella per ogni messaggio
            foreach (KeyValuePair<ushort, MessageInfo> msgpair in MessageMap)
            {
                // determina il fullname in modo da poterlo comparare con il Dictionary xmltags
                string msgname = msgpair.Value.msgtype.FullName;

                string fullname = "T:" + msgname;
                int msgpropcount = msgpair.Value.ord2prop.Count;

                enumlist.Clear();

                XmlElement xmlobj;
                // trova la descrizione xml all'interno del file della documentazione
                if (xmltags.TryGetValue(fullname, out xmlobj))
                {
                    // OK
                    string msgsummary = xmlobj["summary"].InnerText;
                    
                    // Pagina nuova.
                    AddPageBreak(document);

                    DocAddTitle(document, msgpair.Value.msgtype.Name + ":" + msgpair.Value.msgtype.BaseType.Name, Headings.Heading_20_2);
                    //DocAddTitle(document, msgsummary, Headings.Heading_20_2);
                    DocAddRow(document, "", msgsummary);

                    // Crea una tabella che ha il solito nome del messaggio
                    // Il numero di righe corrisponde al numero di proprietà del messaggio
                    // contiene 4 colonne (Proprietà, Tipo dati, Lunghezza (bytes), Descrizione (corta)
                    Table table = TableBuilder.CreateTextDocumentTable(document,msgname,"table1",1,4,16.99,true,true);

                    // Aggiunge la riga di intestazione della tabella (verrà ripetuta attraverso le pagine)
                    table.RowHeader.RowCollection[0].Cells[0].Content.Add(CreateParagraph(document,"", "Campo"));
                    table.RowHeader.RowCollection[0].Cells[1].Content.Add(CreateParagraph(document,"", "Data Type"));
                    table.RowHeader.RowCollection[0].Cells[2].Content.Add(CreateParagraph(document,"", "Size"));
                    table.RowHeader.RowCollection[0].Cells[3].Content.Add(CreateParagraph(document, "", "Description"));

                    int boolcount = 0;

                    // Elenca tutte le proprietà del messaggio
                    for (int ii=0;ii<msgpair.Value.ord2prop.Count ;ii++)
                    {
                        PropertyOrderInfo poi=msgpair.Value.ord2prop[ii];
                        string pname = poi.PropInfo.Name;
                        string ptype = "";
                        Dictionary<int, string> enums = new Dictionary<int, string>();
                        if (poi.PropInfo.PropertyType.IsEnum)
                        {
                            // In caso di enumerato vede
                            string[] enumtext = Enum.GetNames(poi.PropInfo.PropertyType);

                            for (int zz = 0; zz < enumtext.Length; zz++)
                                enums.Add((int)Enum.Parse(poi.PropInfo.PropertyType, enumtext[ii]),enumtext[zz]);
                                //enumval[ii] = Enum.Format(poi.PropInfo.PropertyType, Enum.Parse(poi.PropInfo.PropertyType, enumtext[ii]), "d");
                            
                            enumlist.Add(enums);

                            //foreach (string s in Enum.GetNames(poi.PropInfo.PropertyType))
                            //{
                            //    if (enumvals != "")
                            //        enumvals += ", ";

                            //    enumvals += s + "=" + Enum.Format(poi.PropInfo.PropertyType, Enum.Parse(poi.PropInfo.PropertyType, s), "d");
                            //}
                            ptype = poi.PropInfo.PropertyType.Name + " (enum)";
                        }
                        else
                        {
                            ptype = poi.PropInfo.PropertyType.Name;
                        }

                        string psize = "";
                        string pdesc = "";

                        string pfullname = "P:" + msgpair.Value.msgtype.Namespace + "." + msgpair.Value.msgtype.Name + "." + pname;

                        XmlElement xmlobj2;
                        if (xmltags.TryGetValue(pfullname, out xmlobj2))
                        {
                            pdesc = xmlobj2["summary"].InnerText;
                        }
                        else
                        {
                            // in caso di proprietà derivata da BaseMessage
                            if (msgpair.Value.msgtype.BaseType.ToString().EndsWith("BaseMessage"))
                            {
                                string pfullname2 = "P:" + msgpair.Value.msgtype.Namespace + ".BaseMessage." + pname;
                                XmlElement xmlobj3;
                                if (xmltags.TryGetValue(pfullname2, out xmlobj3))
                                {
                                    pdesc = xmlobj3["summary"].InnerText;
                                }
                                else
                                {
                                    pdesc = "PROPRIETA' BASE NON TROVATA";
                                }

                            }
                            else
                                pdesc = "PROPRIETA' NON TROVATA";
                            //pdesc = "PROPRIETA' NON TROVATA";
                        }

                        // Aggiunge i bits spare residui
                        if (poi.PropInfo.PropertyType != typeof(bool) && (boolcount > 0))
                        {
                            int nextidx = ((int)(boolcount / 8) + 1) * 8;
                            for (int ipi = boolcount; ipi < nextidx; ipi++)
                            {
                                TableAddRow(table, document, "(SPARE)", ptypeold, psizeold, "(SPARE)");
                            }
                            boolcount = 0;
                        }

                        // Collezioni
                        if (poi.IsArray || poi.IsList || 
                            (poi.StringType != StringTypeEnum.Null) ||
                            poi.IsClass)
                        {
                            // Inserisce il subcampo di lunghezza in caso di campi dinamici come array, liste o stringhe.
                            TableAddRow(table, document, "Len of " + pname, "Int32", "4 bytes", "Len of " + pname);

                            // Aggiunge il primo elemento dell' array 
                            TableAddRow(table, document, pname + "[0]", ptype, "1", pdesc);

                            // Aggiunge i puntarini 
                            TableAddRow(table, document, "...", "...", "...", "...");

                            // Aggiunge l'ultimo elemento dell' array
                            TableAddRow(table, document, pname + "[Len of " + pname + "-1]", ptype, "1", pdesc);
                            
                            // Add a description of spare boolean rest
                            if (poi.PropInfo.PropertyType == typeof(bool[]))
                            {
                                // Aggiunge i puntarini 
                                TableAddRow(table, document, "...", "...", "...", "...");

                                // Aggiunge gli spare
                                TableAddRow(table, document, pname + "[(Int(Len of " + pname + " / 8) + 1) * 8 - 1]", ptype, "1", "(SPARE)");
                                //TableAddRow(table, document, pname + "[Next multiple of 8 - 1]", ptype, "1", pdesc + " (SPARE)");
                             }
                        }
                        else
                        {
                            if (poi.PropInfo.PropertyType == typeof(bool))
                            {
                                pnameold = pname;
                                ptypeold = ptype;
                                psizeold = psize;
                                pdescold = pdesc;

                                boolcount++;
                            }

                            psize=GetTypeSize(poi.PropInfo.PropertyType);
                            TableAddRow(table, document, pname, ptype, psize, pdesc);
                        }
                    }

                    // Aggiunge i bits spare residui
                    if (boolcount > 0)
                    {
                        int nextidx = ((int)(boolcount / 8) + 1) * 8;
                        for (int ipi = boolcount; ipi < nextidx; ipi++)
                        {
                            TableAddRow(table, document, "(SPARE)", ptypeold, "1 bit", "(SPARE)");
                        }
                        boolcount = 0;
                    }
                    document.Content.Add(table);

                    if (enumlist.Count > 0)
                    {
                        foreach (Dictionary<int,string> elc in enumlist)
                        {
                            // Aggiunta tabella
                            
                            
                        }

                        // Aggiungere una tabella per ogni enumerato usato nelle proprietà del messaggio
                    }

                    DocAddRow(document, "", "");
                }
                else
                {
                    // non trovato
                    DocAddRow(document, "Intestazione 1", " MESSAGGIO NON TROVATO");
                    DocAddRow(document, "", "");
                }
            }

            DocAddHeaderAndFooter(document, "T&T Sistemi Protocol Documenter", DateTime.Now.ToString());

            document.SaveTo(docfilename);
            return docfilename;
        }

        /// <summary>
        /// Aggiunge una riga alla tabella
        /// </summary>
        /// <param name="table"></param>
        /// <param name="document"></param>
        /// <param name="pname"></param>
        /// <param name="ptype"></param>
        /// <param name="psize"></param>
        /// <param name="pdesc"></param>
        private void TableAddRow(Table table, TextDocument document,string pname, string ptype, string psize, string pdesc)
        {
            Row zii = new Row(table);
            zii.Cells.Add(new Cell(document,"cstyle"));
            zii.Cells.Add(new Cell(document,"cstyle"));
            zii.Cells.Add(new Cell(document,"cstyle"));
            zii.Cells.Add(new Cell(document,"cstyle"));
            zii.Cells[0].Content.Add(CreateParagraph(document, "", pname));
            zii.Cells[0].CellStyle.CellProperties.Border = "0.002cm solid #000000";
            zii.Cells[1].Content.Add(CreateParagraph(document, "", ptype));
            zii.Cells[1].CellStyle.CellProperties.Border = "0.002cm solid #000000";
            zii.Cells[2].Content.Add(CreateParagraph(document, "", psize));
            zii.Cells[2].CellStyle.CellProperties.Border = "0.002cm solid #000000";
            zii.Cells[3].Content.Add(CreateParagraph(document, "", pdesc));
            zii.Cells[3].CellStyle.CellProperties.Border = "0.002cm solid #000000";
            table.Rows.Add(zii);

        }

        /// <summary>
        /// Return descriptive size of a Type
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private string GetTypeSize(Type t)
        {
            string size = "";
            
            if (t == typeof(bool))
                size = "1 bit";
            else if (t == typeof(byte))
                size = "1 byte";
            else if (t == typeof(sbyte))
                size = "1 byte";
            else if (t == typeof(Int16))
                size = "2 bytes";
            else if (t == typeof(UInt16))
                size = "2 bytes";
            else if (t == typeof(Int32))
                size = "4 bytes";
            else if (t == typeof(UInt32))
                size = "4 bytes";
            else if (t == typeof(Int64))
                size = "8 bytes";
            else if (t == typeof(UInt64))
                size = "8 bytes";
            else if (t == typeof(Single))
                size = "4 bytes";
            else if (t == typeof(Double))
                size = "8 bytes";
            else if (t == typeof(DateTime))
                size = "8 bytes";
            else if (t == typeof(TimeSpan))
                size = "8 bytes";
            else if (t.IsEnum)
                size = "4 bytes";
            else
                size = "Unknown";
            
            return size;
        }

        /// <summary>
        /// Add a page break to the document
        /// </summary>
        /// <param name="document"></param>
        private void AddPageBreak(TextDocument document)
        {
            Paragraph p1 = new Paragraph(document,"P3");

            p1.ParagraphStyle.ParagraphProperties.BreakAfter = "page";
            document.Content.Add(p1);
        }

        /// <summary>
        /// Creazione di un paragrafo
        /// </summary>
        /// <param name="document"></param>
        /// <param name="stile"></param>
        /// <param name="testo"></param>
        /// <returns></returns>
        private Paragraph CreateParagraph(TextDocument document, string stile, string testo)
        {
            FormatedText ftxt = new FormatedText(document, stile, testo);
            Paragraph p1 = ParagraphBuilder.CreateStandardTextParagraph(document);
            p1.TextContent.Add(ftxt);
            return p1;
        }

        private void DocAddHeaderAndFooter(TextDocument document, string header, string footer)
        {
            TextMasterPage txtMP = document.TextMasterPageCollection.GetDefaultMasterPage();
            txtMP.ActivatePageHeaderAndFooter();
            
            //txtMP.TextPageHeader.MarginLeft = "4cm";

            Paragraph phead = new Paragraph(document, "P1"); // ParagraphBuilder.CreateStandardTextParagraph(document);

            phead.ParagraphStyle.ParagraphProperties.Alignment = TextAlignments.center.ToString();
            
            phead.TextContent.Add(new SimpleText(document, header));
            txtMP.TextPageHeader.ContentCollection.Add(phead);

            Paragraph pfoot = new Paragraph(document, "P2");

            pfoot.ParagraphStyle.ParagraphProperties.Alignment = TextAlignments.center.ToString();

            pfoot.TextContent.Add(new SimpleText(document, footer));
            txtMP.TextPageFooter.ContentCollection.Add(pfoot);
        }

        private void DocAddTitle(TextDocument document, string headingText, Headings headtype)
        {
            Header header = new Header(document, headtype);
            header.TextContent = TextBuilder.BuildTextCollection(document, headingText);
            document.Content.Add(header);
        }

        private void DocAddRow(TextDocument document, string stile, string testo)
        {
            FormatedText ftxt = new FormatedText(document, stile, testo);
            Paragraph p1 = ParagraphBuilder.CreateStandardTextParagraph(document);
            
            p1.TextContent.Add(ftxt);
            document.Content.Add(p1);
        }

        #endregion
        #region IMessageParser Members

        /// <summary>
        /// Determina le condizioni minime per poter conoscere 
        /// la lunghezza del messaggio
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool CanReadLength(byte[] data)
        {
            // minima lunghezza contenente almeno lunghezza (4) e ID (2)
            if (data.Length >= BaseCustomMessageLength)
            {
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Ritorna la lunghezza del messaggio
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int GetLength(byte[] data)
        {
            int length = BitConverter.ToInt32(data, 2);
            return length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IMessage ParseMessage(byte[] data)
        {
            return (IMessage)ParseMessage(data,false);
        }

        /// <summary>
        /// Deserializza:
        /// Converte un array di bytes solitamente ricevuto tramite TCP
        /// in una classe IMessage da poter usare nell'applicazione
        /// </summary>
        /// <param name="data"></param>
        /// <param name="foo"></param>
        /// <returns></returns>
        private object ParseMessage(byte[] data, bool foo)
        {
            object retval=null;
            TetByteFiller filler = new TetByteFiller();
            object obj = null;

            // I primi 2 bytes contengono il msgid
            //obj = filler.Deserialize(typeof(ushort), ref data);
            //ushort msgid = (ushort)obj;
            ushort msgid = BitConverter.ToUInt16(data, 0);

            // Recupera la lunghezza 
//            obj = filler.Deserialize(typeof(int), ref data);
//            int length = (int)obj;
            int length = BitConverter.ToInt32(data, 2);


            byte[] body = new byte[data.Length - 6];
            Array.Copy(data, 6, body, 0, body.Length);

            MessageInfo msginfo=new MessageInfo();

            // ricerca Informazioni dal Dictionary a partire dal msgid 
            if (!msgid2imsg.TryGetValue(msgid, out msginfo))
            {
                // indice non valido
                throw new Exception("ParseMessage: Code not found: " + msgid.ToString());
            }

            // Crea un istanza di IMessage che fa riferimento al tipo del messaggio
            // che stiamo elaborando

            Type tipo = msginfo.msgtype;
            //ConstructorInfo cifo = tipo.GetConstructor(new Type[1] { typeof(bool) });
            ConstructorInfo cifo = tipo.GetConstructor(new Type[0] { });
            retval = (object)cifo.Invoke(new object[0]);
            
            // codice trovato
            for (int ii = 0; ii < msginfo.ord2prop.Count; ii++)
            {
                PropertyInfo pinfo = msginfo.ord2prop[ii].PropInfo;
                if (msginfo.ord2prop[ii].IsClass)
                {
                    if ((msginfo.ord2prop[ii].IsArray) || (msginfo.ord2prop[ii].IsList))
                    {
                        int nextdataLen = BitConverter.ToInt32(body, filler.LastByte);
                        filler.LastByte += 4;
                        int listlength = BitConverter.ToInt32(body, filler.LastByte);
                        filler.LastByte += sizeof(Int32);

                        obj = (IList)msginfo.ord2prop[ii].PropInfo.PropertyType.GetConstructor(new Type[1] { typeof(int) }).Invoke(new object[1] { listlength });

                        for (int i = 0; i < listlength; i++)
                        {
                            int classlength = BitConverter.ToInt32(body, filler.LastByte);
                            filler.LastByte += sizeof(Int32);
                            object element = ParseMessage(filler.DeserializeExternal(classlength, body), false);
                            if (msginfo.ord2prop[ii].IsArray)
                                ((IList)obj)[i] = element;
                            if (msginfo.ord2prop[ii].IsList)
                                ((IList)obj).Add(element);
                        }

                    }
                    if ((!msginfo.ord2prop[ii].IsArray) && (!msginfo.ord2prop[ii].IsList))
                    {
                        int classlength = BitConverter.ToInt32(body, filler.LastByte);
                        filler.LastByte += sizeof(Int32);
                        obj = ParseMessage(filler.DeserializeExternal(classlength, body), false);
                    }
                }
                else
                {
                    //obj = filler.Deserialize(pinfo.PropertyType, ref data);
                    obj = filler.Deserialize(msginfo.ord2prop[ii], ref body);
                }
                pinfo.SetValue(retval, obj, null);
            }

            return retval;
        }

        private static int ComparePropInfo(PropertyOrderInfo x, PropertyOrderInfo y)
        {
            return x.OrderNum.CompareTo(y.OrderNum);
        }
        
        /// <summary>
        /// Serializza:
        /// Converte una generica classe IMessage in un array di bytes
        /// da poter inviare tramite TCP
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] SerializeMessage(IMessage data)
        {
            return this.SerializeMessage((object)data);
        }

        /// <summary>
        /// Serializza:
        /// Converte una generica classe IMessage in un array di bytes
        /// da poter inviare tramite TCP
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private byte[] SerializeMessage(object data)
        {
            byte[] message;

            // Verifica che il messaggio sia serializzabile tramite CustomMessageSerializer
            object[] MsgAttributes = data.GetType().GetCustomAttributes(typeof(TetMessageAttribute), false);
            if (MsgAttributes.Length == 0)
            {
                throw new Exception("SerializeMessage: Message not serializable");
            }

            // intestazione del custom message (msgid e lunghezza)
            byte[] header = new byte[BaseCustomMessageLength];
            ushort msgid = ((TetMessageAttribute)MsgAttributes[0]).MsgId;
            Array.Copy(BitConverter.GetBytes(msgid), 0, header, 0, 2);

            // Filtra solo i messaggi precedentemente aggiunti alla lista dal costruttore 
            MessageInfo msginfo = new MessageInfo();
            if (!msgid2imsg.TryGetValue(msgid, out msginfo))
            {
                throw new Exception("SerializeMessage: Message not found");
            }

            TetByteFiller filler = new TetByteFiller();
            // scorre le proprietà del messaggio
            PropertyOrderInfo[] proplist = msginfo.ord2prop.ToArray();

            for (int ii = 0; ii < proplist.Length; ii++)
            {
                //PropertyInfo info=proplist[ii].PropInfo;
                //object val = info.GetValue(data,null);
                //filler.Serialize(val.GetType(), val);
                if (proplist[ii].IsClass)
                {
                    bool isList = false;
                    IList subValue = null;
                    int listLen=0;
                    if ((proplist[ii].IsArray) && (!proplist[ii].IsList))
                    {
                        object paramVal = proplist[ii].PropInfo.GetValue(data, null);
                        listLen = (int)paramVal.GetType().GetProperty("Length").GetValue(paramVal, null);
                        isList = true;
                    }
                    if ((!proplist[ii].IsArray) && (proplist[ii].IsList))
                    {
                        object paramVal = proplist[ii].PropInfo.GetValue(data, null);
                        listLen = (int)paramVal.GetType().GetProperty("Count").GetValue(paramVal, null);
                        isList = true;
                    }
                    if (isList)
                    {
                        filler.Serialize(proplist[ii], BitConverter.GetBytes(listLen));
                        for (int i = 0; i < listLen; i++)
                        {
                            subValue = (IList)proplist[ii].PropInfo.GetValue(data, null);
                            byte[] classe = this.SerializeMessage(subValue[i]);
                            filler.Serialize(proplist[ii], classe);
                        }
                    }
                    if ((!proplist[ii].IsArray) && (!proplist[ii].IsList))
                    {
                        byte[] classe = this.SerializeMessage(proplist[ii].PropInfo.GetValue(data, null));
                        filler.Serialize(proplist[ii], classe);
                    }
                }
                else
                    filler.Serialize(proplist[ii], proplist[ii].PropInfo.GetValue(data, null));
            }

            byte[] body = filler.GetStream();
            Array.Copy(BitConverter.GetBytes(body.Length + BaseCustomMessageLength), 0, header, 2, 4);

            message = new byte[header.Length + body.Length];
            Array.Copy(header, 0, message, 0, header.Length);
            Array.Copy(body, 0, message, header.Length, body.Length);
            return message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool SerializeIsSupported()
        {
            return true;
        }

        #endregion
    }

    /// <summary>
    /// Definisce la classe contenitore delle informazioni di ogni messaggio
    /// </summary>
    public class MessageInfo
    {
        // contiene l'associazione ordinale delle proprietà del messaggio
//        public List<PropertyInfo> ord2prop = new List<PropertyInfo>();
        /// <summary>
        /// 
        /// </summary>
        public List<PropertyOrderInfo> ord2prop = new List<PropertyOrderInfo>();

        // contiene il tipo del messaggio
        /// <summary>
        /// 
        /// </summary>
        public Type msgtype;
    }

    /// <summary>
    /// 
    /// </summary>
    public enum StringTypeEnum
    {
        /// <summary>
        /// 
        /// </summary>
        Null,
        /// <summary>
        /// 
        /// </summary>
        Ascii,
        /// <summary>
        /// 
        /// </summary>
        Unicode
    }

    /// <summary>
    /// 
    /// </summary>
    public class PropertyOrderInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public ushort OrderNum {get; set; }
        /// <summary>
        /// 
        /// </summary>
        public PropertyInfo PropInfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public StringTypeEnum StringType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsClass { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsArray { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsEnum { get; set; }
        
        //public SetValue();
    }

    /// <summary>
    /// Definisce la classe base da cui utilizzare gli attributi per
    /// identificazione dei messaggi
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TetMessageAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public ushort MsgId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgId"></param>
        public TetMessageAttribute(ushort msgId)
        {
            MsgId = msgId;
        }
    } //eof class 

    /// <summary>
    /// Attributo utilizzato per identificare un Message utilizzato come header del
    /// protocollo
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TetBaseMessageAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public TetBaseMessageAttribute()
        {

        }
    } //eof class 

    /// <summary>
    /// Definisce la classe base da cui utilizzare gli attributi per
    /// identificazione delle proprietà (componenti) dei messaggi
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TetPropertyAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public ushort PropertyOrder { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TetPropertyAttribute(ushort propId)
        {
            PropertyOrder = propId;
        }
    } //eof class 

    /// <summary>
    /// Definisce l'attributo che indica una stringa di tipo unicode
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TetPropertyUnicodeAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public TetPropertyUnicodeAttribute()
        {

        }
    } //eof class 
}
