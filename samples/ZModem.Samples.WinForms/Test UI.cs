using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;

namespace ZModem_example
{
    public partial class Test_UI : Form
    {

        SerialPort sp = null;
        ZModem_Protocol.ZModem zmodem = null;
        static int fileSize;

        public Test_UI()
        {
            InitializeComponent();
        }

        private void Test_UI_Load(object sender, EventArgs e)
        {
            string[] listofSerial = SerialPort.GetPortNames();
            cmbCOMPort.Items.Clear();
            foreach (string s in listofSerial)
                cmbCOMPort.Items.Add(s);

            cmbBaud.Items.Add(9600);
            cmbBaud.Items.Add(19200);
            cmbBaud.Items.Add(38400);
            cmbBaud.Items.Add(57600);
            cmbBaud.Items.Add(115200);

            cmbDataBit.Items.Add(5);
            cmbDataBit.Items.Add(6);
            cmbDataBit.Items.Add(7);
            cmbDataBit.Items.Add(8);

            foreach (Parity p in Enum.GetValues(typeof(Parity)))
                cmbParity.Items.Add(p);

            foreach (StopBits s in Enum.GetValues(typeof(StopBits)))
                cmbStopBit.Items.Add(s);

            try
            {
                cmbCOMPort.SelectedItem = "COM11";
                cmbBaud.SelectedItem = 115200;
                cmbDataBit.SelectedItem = 8;
                cmbParity.SelectedItem = Parity.None;
                cmbStopBit.SelectedItem = StopBits.One;
                txtLocalDest.Text = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
            }
            catch (Exception ex)
            {

            }
        }

        private void cmdInit_Click(object sender, EventArgs e)
        {
            if (sp != null)
            {
                sp.Dispose(); // Close previously opened serial Port?            
                sp = null;
            }

            try
            {
                SerialPort_Fix.SerialPortFixer.Execute((string)cmbCOMPort.SelectedItem);
                Console.WriteLine("Bug fixer executed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Impossible to open this serial port. Is it still existing? is it opened sowhere else?");
                Console.WriteLine(ex.Message);
                tabgrp.Enabled = false;
                return;
            }

            try
            {
                sp = new SerialPort((string)cmbCOMPort.SelectedItem, (int)cmbBaud.SelectedItem, (Parity)cmbParity.SelectedItem, (int)cmbDataBit.SelectedItem, (StopBits)cmbStopBit.SelectedItem);
                sp.Open();
                zmodem = new ZModem_Protocol.ZModem(sp);
                zmodem.TransfertStateEvent += new ZModem_Protocol.ZModem.TransfertStateHandler(zmodem_TransfertStateEvent);
                zmodem.FileInfoEvent += new ZModem_Protocol.ZModem.FileInfoHandler(zmodem_FileInfoEvent);
                zmodem.BytesTransferedEvent += new ZModem_Protocol.ZModem.BytesTransferedHandler(zmodem_BytesTransferedEvent);

                Console.WriteLine("Serial port setuped and opened successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Impossible to open this serial port with this settings!");
                Console.WriteLine(ex.Message);
                tabgrp.Enabled = false;
                return;
            }

            tabgrp.Enabled = true;
        }

        private void txtLocalDest_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fd = new FolderBrowserDialog())
            {
                fd.ShowNewFolderButton = true;
                fd.SelectedPath = txtLocalDest.Text;
                DialogResult dr = fd.ShowDialog();

                if (dr == DialogResult.OK)
                    txtLocalDest.Text = fd.SelectedPath;
            }
        }

        void zmodem_BytesTransferedEvent(object sender, ZModem_Protocol.ZModem.BytesTransferedEventArgs e)
        {
            Console.Write(e.size + "\r");
            //float f = (float)e.size * 100/ fileSize;
            ////backgroundWorker1.ReportProgress((int)f);
            //if (!progress.Visible)
            //    return;

            //progress.Value = e.size;
            //progress.Refresh();
        }

        void zmodem_FileInfoEvent(object sender, ZModem_Protocol.ZModem.FileInfoEventArgs e)
        {
            Console.WriteLine(e.fileName + " " + e.size);
            //fileSize = e.size;
            //backgroundWorker1.ReportProgress(0);
            //progress.Minimum = 0;
            //progress.Maximum = 100;
            //progress.Value = 0;
            //progress.Visible = true;
            //progress.Refresh();
        }

        void zmodem_TransfertStateEvent(object sender, ZModem_Protocol.ZModem.TransfertStateEventArgs e)
        {
            Console.WriteLine(e.state);
            //backgroundWorker1.ReportProgress(-1, e.state);
            //this.tssLState.Text = e.state.ToString();
            //if (e.state == ZModem_Protocol.ZModem.TransfertState.Ended)
            //    this.UseWaitCursor = false;
            //else
            //    this.UseWaitCursor = true;
            //this.statusStrip1.Refresh();
        }


        private void cmdTransferfile_Click(object sender, EventArgs e)
        {

            progress.Value = 0;
            progress.Refresh();

            //backgroundWorker1.RunWorkerAsync();
            MemoryStream ms = null;

            ////download file 
            ////Make sure you unsuscribe all DAtaReceived event from port before calling following line.
            ms = zmodem.ZModemTransfert(txtFileName.Text, (int)numUpDown.Value);

            if (ms != null)
            {
                string fn = Path.Combine(txtLocalDest.Text, Path.GetFileName(txtFileName.Text));
                File.WriteAllBytes(fn, ms.ToArray());
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            MemoryStream ms = null;

            //download file 
            //Make sure you unsuscribe all DAtaReceived event from port before calling following line.
            ms = zmodem.ZModemTransfert(txtFileName.Text, 10);

            e.Result = ms;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MemoryStream ms = (MemoryStream)e.Result;

            if (ms != null)
            {
                string fn = Path.Combine(txtLocalDest.Text, Path.GetFileName(txtFileName.Text));
                File.WriteAllBytes(fn, ms.ToArray());
            }
            this.UseWaitCursor = false;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage >= 0)
                progress.Value = e.ProgressPercentage;

            if (e.UserState == null)
                return;

            ZModem_Protocol.ZModem.TransfertState state = (ZModem_Protocol.ZModem.TransfertState)e.UserState;
            this.tssLState.Text = state.ToString();
            if (state == ZModem_Protocol.ZModem.TransfertState.Ended)
                this.UseWaitCursor = false;
            else
                this.UseWaitCursor = true;
            this.statusStrip1.Refresh();
        }
    }
}
