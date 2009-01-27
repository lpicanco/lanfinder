using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;
using System.IO;
using Trinet.Networking;

namespace LanFinder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Print(String msg)
        {
            textBox1.Text += msg + Char.ConvertFromUtf32(13) + Char.ConvertFromUtf32(10);
            textBox1.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String oldLabel = button1.Text;
            button1.Text = "Scanning";
            button1.Refresh();
            textBox1.Clear();

            try
            {
                int depth = Int32.Parse(txtDepth.Text);

                var interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface it in interfaces)
                {
                    foreach (var ua in it.GetIPProperties().UnicastAddresses)
                    {
                        IPAddress ip = ua.Address;
                        if (!ip.IsIPv6LinkLocal)
                        {
                            //Print(ip.ToString());
                            //Print("-----------------------------");
                            ShareCollection folders = new ShareCollection(ip.ToString());
                            foreach (Share s in folders)
                            {
                                //Print(s.Path);

                                try
                                {
                                    if (s.IsFileSystem && s.Root != null && s.Root.Exists)
                                    {
                                        foreach (var d in s.Root.GetDirectories())
                                        {
                                            var files = FileHelper.GetFilesRecursive(d.FullName, txtExtension.Text, depth);
                                            foreach (var file in files)
                                            {
                                                Print(file);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //Print("ERROR: " + ex.Message);
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                button1.Text = oldLabel;
            }

            /*
            Socket sock = new Socket(AddressFamily.InterNetwork,
            SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 9050);
            sock.Bind(iep);
            EndPoint ep = (EndPoint)iep;
            Print("Ready to receive…");
            byte[] data = new byte[1024];
            int recv = sock.ReceiveFrom(data, SocketFlags.Broadcast, ref ep);
            string stringData = Encoding.ASCII.GetString(data, 0, recv);
            Print(String.Format("received: {0} from: {1}", stringData, ep.ToString()));
            data = new byte[1024];
            recv = sock.ReceiveFrom(data, ref ep);
            stringData = Encoding.ASCII.GetString(data, 0, recv);
            Print(String.Format("received: {0} from: {1}",
                       stringData, ep.ToString()));
            sock.Close();*/
        }


    }
}
