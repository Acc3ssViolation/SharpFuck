using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharpFuck
{
    public partial class MainForm : Form
    {
        BrainfuckInterpreter bf;
        Thread bfThread;
        string bfText;
        delegate void LogDelegate(string msg);

        public MainForm()
        {
            InitializeComponent();

            bf = new BrainfuckInterpreter();

            if(string.IsNullOrEmpty(Properties.Settings.Default.LastPath))
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            else
            {
                openFileDialog.InitialDirectory = Properties.Settings.Default.LastPath;
                saveFileDialog.InitialDirectory = Properties.Settings.Default.LastPath;
            }

            this.Text = Program.Name;
        }

        private void LaunchBrainfuckThread(string source)
        {
            if(bfThread == null || !bfThread.IsAlive)
            {
                Log("Starting brainfuck execution...");
                bfText = source;
                bfThread = new Thread(RunBrainfuck);
                bfThread.Start();
                watchdogTimer.Start();
            }
        }

        private void RunBrainfuck()
        {
            Log("Output:\r\n" + new string(bf.Run(bfText)));
            watchdogTimer.Stop();
        }

        private void Log(string message)
        {
            if(logBox.InvokeRequired)
            {
                Invoke(new LogDelegate(Log), new object[] { message });
            }
            else
            {
                logBox.AppendText(message + "\r\n");
            }
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs a)
        {
            Properties.Settings.Default.LastPath = Path.GetDirectoryName(openFileDialog.FileName);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.LastPath = Path.GetDirectoryName(saveFileDialog.FileName);
                openFileDialog.FileName = saveFileDialog.FileName;
                try
                {
                    if(!File.Exists(saveFileDialog.FileName))
                    {
                        File.Create(saveFileDialog.FileName).Close();
                    }
                    using(StreamWriter sw = new StreamWriter(saveFileDialog.FileName, false, Encoding.ASCII))
                    {
                        sw.Write(editBox.Text);
                    }
                    this.Text = saveFileDialog.FileName + " - " + Program.Name;
                    logBox.Clear();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("An error occured saving file " + Path.GetFileName(saveFileDialog.FileName), "Error saving file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.LastPath = Path.GetDirectoryName(openFileDialog.FileName);
                saveFileDialog.FileName = openFileDialog.FileName;
                try
                {
                    using(StreamReader sr = new StreamReader(openFileDialog.FileName, Encoding.ASCII))
                    {
                        editBox.Text = sr.ReadToEnd();
                    }
                    this.Text = openFileDialog.FileName + " - " + Program.Name;
                    logBox.Clear();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("An error occured opening file " + Path.GetFileName(openFileDialog.FileName), "Error opening file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            LaunchBrainfuckThread(editBox.Text.Trim());
        }

        private void watchdogTimer_Tick(object sender, EventArgs e)
        {
            if(bfThread != null && bfThread.IsAlive)
            {
                bfThread.Abort();
                Log("Timeout!");
                Log("Output at time of timeout:\r\n" + new string(bf.GetOutput()));
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editBox.Clear();
            this.Text = Program.Name;
            logBox.Clear();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
