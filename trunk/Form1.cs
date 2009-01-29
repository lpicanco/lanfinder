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
using LanFinder.Lib;
using System.Collections;
using System.Threading;

namespace LanFinder
{
    public partial class Form1 : Form
    {
        private delegate void SetTextCallback(string text);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Print(String msg)
        {
            SetTextCallback del = new SetTextCallback(PrintSafe);
            this.Invoke(del, new object[] { msg });            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(new ThreadStart(SearchFile));
            t.Start();
        }

        private void SetInitStatus(String msg)
        {
            button1.Enabled = false;
            button1.Text = "Scanning";
            textBox1.Clear();
        }

        private void SetFinalStatus(String msg)
        {
            button1.Text = msg;
            button1.Enabled = true;
        }

        private void PrintSafe(String msg)
        {
            textBox1.Text += msg + Char.ConvertFromUtf32(13) + Char.ConvertFromUtf32(10);
            //textBox1.Refresh();
        }

        private void SearchFile()
        {
            String oldLabel = button1.Text;
            SetTextCallback del = new SetTextCallback(SetInitStatus);
            this.Invoke(del, new object[] { "" });
            
            try
            {
                IList<Thread> threadList = new List<Thread>();

                int depth = Int32.Parse(txtDepth.Text);
                String extension = txtExtension.Text;

                NetworkBrowser nb = new NetworkBrowser();
                ArrayList hosts = nb.GetNetworkComputers();

                foreach (String host in hosts)
                {
                    Thread thread = new Thread(new ParameterizedThreadStart(SearchHostFiles));
                    thread.Start(new SearchHostBean(){ Host = host, Extension = extension, Depth = depth});
                    threadList.Add(thread);
                }

                foreach (Thread thread in threadList)
                {
                    thread.Join();
                }
            }
            finally
            {
                del = new SetTextCallback(SetFinalStatus);
                this.Invoke(del, new object[] { oldLabel});
            }
        }

        private void SearchHostFiles(object beanObj)
        {
            SearchHostBean bean = (SearchHostBean)beanObj;
            String host = bean.Host;
            String extension = bean.Extension;
            int depth = bean.Depth;

            Print(host);
            //Print("-----------------------------");
            ShareCollection folders = new ShareCollection(host);
            foreach (Share s in folders)
            {
                //Print(s.Path);

                try
                {
                    if (s.IsFileSystem && s.Root != null && s.Root.Exists)
                    {
                        foreach (var d in s.Root.GetDirectories())
                        {
                            var files = FileHelper.GetFilesRecursive(d.FullName, extension, depth);
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

    class SearchHostBean 
    {
        public String Host { get; set; }
        public String Extension {get;set;}
        public int Depth {get; set; }
    }
}
