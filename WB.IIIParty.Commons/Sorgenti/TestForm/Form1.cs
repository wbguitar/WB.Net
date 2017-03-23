using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Security.Cryptography;
using TetSistemi.Commons.Net.Sockets;
using TetSistemi.Commons.Protocol.Serialization;
using TetSistemi.Commons.Protocol;
using TetSistemi.Commons.Logger;

namespace TestForm
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();

            System.Data.SqlClient.SqlConnectionStringBuilder scb = new System.Data.SqlClient.SqlConnectionStringBuilder();
            scb.DataSource = @"10.138.25.37\SQLEXPRESS";
            scb.InitialCatalog = @"KCRAlarm";
            scb.UserID = @"sa";
            scb.Password = @"CopeMetro";
            TetSistemi.Commons.Sql.DbFileStore.RemoveFile("pollo",scb.ToString());

            TetSistemi.Commons.Sql.DbFileStore.RemoveFile(@"prova file.doc", scb.ToString());

            TetSistemi.Commons.Sql.DbFileStore.Clear(scb.ToString());

            TetSistemi.Commons.Sql.DbFileStore.ImportFile(@"c:\prova file.doc", scb.ToString(), false);

            TetSistemi.Commons.Sql.DbFileStore.ExportFile(@"prova file.doc", @"c:\prova file import.doc", scb.ToString());

        }

     
    }
}
