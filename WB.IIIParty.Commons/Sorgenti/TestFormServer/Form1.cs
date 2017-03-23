using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TetSistemi.Commons.Net.Sockets;
using TetSistemi.Commons.Protocol.Serialization;
using TetSistemi.Commons.Protocol;

namespace TestFormServer
{
    public partial class Form1 : Form
    {
        TcpServer srv;
        TcpServerConnection conn;
        StreamParser parser;

        public Form1()
        {
            InitializeComponent();

            srv = new TcpServer(6789);
            srv.BeginConnect += new TcpServer.ConnectEventHandler(srv_BeginConnect);
            srv.BeginDisconnect += new TcpServer.DisconnectEventHandler(srv_BeginDisconnect);
            srv.Start();
        }

        void srv_BeginDisconnect(object sender, TCPEventArgs e)
        {
           
        }

        void parser_MessageReceived(IMessage msg, MessageValidationException error)
        {
            Console.WriteLine(msg);

            //XmlMessage.ProvaMessaggi b = new XmlMessage.ProvaMessaggi();
            //b.prova = "ciao";
            //List<string> pio = new List<string>();
            //for (int i = 0; i < 2000; i++)
            //{
            //    pio.Add("ciao signor piooo come va ti fa male il culo? " + i.ToString());
            //}
            //b.provato = pio;
            //b.Id = ((XmlMessage.ProvaMessaggiRequest)msg).Id;

            parser.Send(msg);
        }

        void srv_BeginConnect(object sender, TCPEventArgs e)
        {
        }

        void parser_OnMessageParseError(MessageParseException error)
        {
            
        }

    

       
    }
}
