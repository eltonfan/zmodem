namespace ZModem_example
{
    partial class Test_UI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpConnection = new System.Windows.Forms.GroupBox();
            this.cmdInit = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbStopBit = new System.Windows.Forms.ComboBox();
            this.cmbParity = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbDataBit = new System.Windows.Forms.ComboBox();
            this.cmbBaud = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbCOMPort = new System.Windows.Forms.ComboBox();
            this.tabgrp = new System.Windows.Forms.TabControl();
            this.tabFile = new System.Windows.Forms.TabPage();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.cmdTransferfile = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtLocalDest = new System.Windows.Forms.TextBox();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.tabAdvanced = new System.Windows.Forms.TabPage();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tssLState = new System.Windows.Forms.ToolStripStatusLabel();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.numUpDown = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.grpConnection.SuspendLayout();
            this.tabgrp.SuspendLayout();
            this.tabFile.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // grpConnection
            // 
            this.grpConnection.Controls.Add(this.cmdInit);
            this.grpConnection.Controls.Add(this.label4);
            this.grpConnection.Controls.Add(this.label5);
            this.grpConnection.Controls.Add(this.cmbStopBit);
            this.grpConnection.Controls.Add(this.cmbParity);
            this.grpConnection.Controls.Add(this.label3);
            this.grpConnection.Controls.Add(this.label2);
            this.grpConnection.Controls.Add(this.cmbDataBit);
            this.grpConnection.Controls.Add(this.cmbBaud);
            this.grpConnection.Controls.Add(this.label1);
            this.grpConnection.Controls.Add(this.cmbCOMPort);
            this.grpConnection.Location = new System.Drawing.Point(12, 12);
            this.grpConnection.Name = "grpConnection";
            this.grpConnection.Size = new System.Drawing.Size(322, 100);
            this.grpConnection.TabIndex = 2;
            this.grpConnection.TabStop = false;
            this.grpConnection.Text = "Settings";
            // 
            // cmdInit
            // 
            this.cmdInit.Location = new System.Drawing.Point(177, 15);
            this.cmdInit.Name = "cmdInit";
            this.cmdInit.Size = new System.Drawing.Size(130, 23);
            this.cmdInit.TabIndex = 10;
            this.cmdInit.Text = "Init SerialPort";
            this.cmdInit.UseVisualStyleBackColor = true;
            this.cmdInit.Click += new System.EventHandler(this.cmdInit_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(174, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "StopBit:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 73);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Parity:";
            // 
            // cmbStopBit
            // 
            this.cmbStopBit.FormattingEnabled = true;
            this.cmbStopBit.Location = new System.Drawing.Point(232, 69);
            this.cmbStopBit.Name = "cmbStopBit";
            this.cmbStopBit.Size = new System.Drawing.Size(75, 21);
            this.cmbStopBit.TabIndex = 6;
            // 
            // cmbParity
            // 
            this.cmbParity.FormattingEnabled = true;
            this.cmbParity.Location = new System.Drawing.Point(65, 69);
            this.cmbParity.Name = "cmbParity";
            this.cmbParity.Size = new System.Drawing.Size(75, 21);
            this.cmbParity.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(174, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "DataBit:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "BaudRate:";
            // 
            // cmbDataBit
            // 
            this.cmbDataBit.FormattingEnabled = true;
            this.cmbDataBit.Location = new System.Drawing.Point(232, 42);
            this.cmbDataBit.Name = "cmbDataBit";
            this.cmbDataBit.Size = new System.Drawing.Size(75, 21);
            this.cmbDataBit.TabIndex = 4;
            // 
            // cmbBaud
            // 
            this.cmbBaud.FormattingEnabled = true;
            this.cmbBaud.Location = new System.Drawing.Point(65, 42);
            this.cmbBaud.Name = "cmbBaud";
            this.cmbBaud.Size = new System.Drawing.Size(75, 21);
            this.cmbBaud.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "COMport:";
            // 
            // cmbCOMPort
            // 
            this.cmbCOMPort.FormattingEnabled = true;
            this.cmbCOMPort.Location = new System.Drawing.Point(65, 16);
            this.cmbCOMPort.Name = "cmbCOMPort";
            this.cmbCOMPort.Size = new System.Drawing.Size(75, 21);
            this.cmbCOMPort.TabIndex = 2;
            // 
            // tabgrp
            // 
            this.tabgrp.Controls.Add(this.tabFile);
            this.tabgrp.Controls.Add(this.tabAdvanced);
            this.tabgrp.Enabled = false;
            this.tabgrp.Location = new System.Drawing.Point(12, 119);
            this.tabgrp.Name = "tabgrp";
            this.tabgrp.SelectedIndex = 0;
            this.tabgrp.Size = new System.Drawing.Size(411, 292);
            this.tabgrp.TabIndex = 3;
            // 
            // tabFile
            // 
            this.tabFile.Controls.Add(this.numUpDown);
            this.tabFile.Controls.Add(this.progress);
            this.tabFile.Controls.Add(this.cmdTransferfile);
            this.tabFile.Controls.Add(this.label8);
            this.tabFile.Controls.Add(this.label7);
            this.tabFile.Controls.Add(this.label6);
            this.tabFile.Controls.Add(this.txtLocalDest);
            this.tabFile.Controls.Add(this.txtFileName);
            this.tabFile.Location = new System.Drawing.Point(4, 22);
            this.tabFile.Name = "tabFile";
            this.tabFile.Padding = new System.Windows.Forms.Padding(3);
            this.tabFile.Size = new System.Drawing.Size(403, 266);
            this.tabFile.TabIndex = 0;
            this.tabFile.Text = "Simple FileTransfert";
            this.tabFile.UseVisualStyleBackColor = true;
            // 
            // progress
            // 
            this.progress.Location = new System.Drawing.Point(126, 102);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(270, 23);
            this.progress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progress.TabIndex = 3;
            // 
            // cmdTransferfile
            // 
            this.cmdTransferfile.Location = new System.Drawing.Point(9, 103);
            this.cmdTransferfile.Name = "cmdTransferfile";
            this.cmdTransferfile.Size = new System.Drawing.Size(110, 23);
            this.cmdTransferfile.TabIndex = 2;
            this.cmdTransferfile.Text = "Transfert !";
            this.cmdTransferfile.UseVisualStyleBackColor = true;
            this.cmdTransferfile.Click += new System.EventHandler(this.cmdTransferfile_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(58, 42);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Destination";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 19);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(113, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Remote file to transfert";
            // 
            // txtLocalDest
            // 
            this.txtLocalDest.Location = new System.Drawing.Point(126, 39);
            this.txtLocalDest.Name = "txtLocalDest";
            this.txtLocalDest.Size = new System.Drawing.Size(271, 20);
            this.txtLocalDest.TabIndex = 0;
            this.txtLocalDest.Click += new System.EventHandler(this.txtLocalDest_Click);
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(126, 16);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(271, 20);
            this.txtFileName.TabIndex = 0;
            this.txtFileName.Text = "ILTLog.txt";
            // 
            // tabAdvanced
            // 
            this.tabAdvanced.Location = new System.Drawing.Point(4, 22);
            this.tabAdvanced.Name = "tabAdvanced";
            this.tabAdvanced.Padding = new System.Windows.Forms.Padding(3);
            this.tabAdvanced.Size = new System.Drawing.Size(403, 266);
            this.tabAdvanced.TabIndex = 1;
            this.tabAdvanced.Text = "Advanced";
            this.tabAdvanced.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssLState});
            this.statusStrip1.Location = new System.Drawing.Point(0, 434);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(435, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tssLState
            // 
            this.tssLState.Name = "tssLState";
            this.tssLState.Size = new System.Drawing.Size(31, 17);
            this.tssLState.Text = "none";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            // 
            // numUpDown
            // 
            this.numUpDown.Location = new System.Drawing.Point(126, 66);
            this.numUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numUpDown.Name = "numUpDown";
            this.numUpDown.Size = new System.Drawing.Size(120, 20);
            this.numUpDown.TabIndex = 4;
            this.numUpDown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(73, 68);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(45, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Timeout";
            // 
            // Test_UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 456);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tabgrp);
            this.Controls.Add(this.grpConnection);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Test_UI";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Test_UI";
            this.Load += new System.EventHandler(this.Test_UI_Load);
            this.grpConnection.ResumeLayout(false);
            this.grpConnection.PerformLayout();
            this.tabgrp.ResumeLayout(false);
            this.tabFile.ResumeLayout(false);
            this.tabFile.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpConnection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbCOMPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbBaud;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbDataBit;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbStopBit;
        private System.Windows.Forms.ComboBox cmbParity;
        private System.Windows.Forms.Button cmdInit;
        private System.Windows.Forms.TabControl tabgrp;
        private System.Windows.Forms.TabPage tabFile;
        private System.Windows.Forms.TabPage tabAdvanced;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Button cmdTransferfile;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtLocalDest;
        private System.Windows.Forms.ProgressBar progress;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tssLState;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.NumericUpDown numUpDown;
        private System.Windows.Forms.Label label8;
    }
}