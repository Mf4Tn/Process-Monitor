using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Management;

namespace Process_Monitor
{
    public partial class Form1 : Form
    {
        DateTime bdet = Process.GetCurrentProcess().StartTime;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            run();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listView3.Items.Count; i++)
            {
                if (listView3.Items[i].Selected)
                {
                    kill_task(listView3.Items[i].SubItems[0].Text);
                }
            }
            //kill_task(textBox1.Text);
        }

        private void button1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void listView3_DoubleClick(object sender, EventArgs e)
        {
            string[] names = { "PID","Name","Arugments","File Path","Working Path","Username"};
            string to_clipboard = "";
            for(int i = 0; i < listView3.Items.Count; i++)
            {
                if (listView3.Items[i].Selected)
                {
                    for(int j = 0; j < 5; j++)
                    {
                        if(!listView3.Items[i].SubItems[j].Text.Equals(""))
                        {
                            to_clipboard += (names[j] + ": " + listView3.Items[i].SubItems[j].Text+" - ");
                        }
                        
                    }
                    to_clipboard = to_clipboard.Substring(0, to_clipboard.Length - 2)+"\n";
                }
                
            }
            Clipboard.SetText(to_clipboard.Substring(0,to_clipboard.Length -1));
            MessageBox.Show("Process Informations Copied To Clipboard", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
        }

        public void kill_task(string id)
        {
            try
            {
                Process process = Process.GetProcessById(Int32.Parse(id));
                process.Kill();
                MessageBox.Show($"'{process.ProcessName}' Terminated Successfully", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show($"'{id}' Process ID Not Found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void run()
        {
            Thread thread = new Thread(() =>
            {

                while (true)
                {
                    Process[] processes = Process.GetProcesses();
                    foreach (Process process in processes)
                    {

                        try
                        {
                            if (bdet < process.StartTime)
                            {
                                listView3.Invoke(new MethodInvoker(delegate
                                {
                                    bool dup = false;
                                    for (int c = 0; c < listView3.Items.Count; c++)
                                    {
                                        if (listView3.Items[c].SubItems[0].Text == process.Id.ToString())
                                        {
                                            dup = true;
                                            break;
                                        }
                                    }
                                    if (!dup)
                                    {
                                        ListViewItem item = new ListViewItem(process.Id.ToString());
                                        item.SubItems.Add(process.ProcessName);
                                        item.SubItems.Add(process.StartInfo.Arguments.ToString());
                                        try
                                        {
                                            item.SubItems.Add(process.MainModule.ModuleName);
                                        }
                                        catch { }
                                        item.SubItems.Add(process.StartInfo.WorkingDirectory);
                                        item.SubItems.Add(process.StartInfo.UserName);
                                        listView3.Items.Add(item);
                                    }
                                }));
                            }
                        }
                        catch { }
                    }
                }
            })
            { IsBackground = true };
            thread.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                listView3.Invoke(new MethodInvoker(delegate
                {
                    listView3.Items.Clear();
                }));
            })
            { IsBackground=true}.Start();
            run();
        }
    }
}
