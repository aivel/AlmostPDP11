using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AlmostPDP11.VM.Extentions;
using VM;

namespace AlmostPDP11
{
    partial class MainForm
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

        private void InitRSomeRegisterLabels(int regNameBaseX, int regBitValBaseX,
            int regBitValBaseY, int regBitValXStep, int regBitValYStep,
            int startFromRegister, int registersCount)
        {
            var toRegister = startFromRegister + registersCount;

            for (int i = startFromRegister; i < toRegister; i++)
            {
                var registerName = Consts.RegisterNames.ElementAt(i);

                var LblRegName = new Label();

                LblRegName.AutoSize = true;
                LblRegName.Location = new Point(regNameBaseX, i * regBitValYStep + regBitValBaseY);
                LblRegName.Name = $"LblRegName{registerName}";
                LblRegName.Size = new Size(30, 17);
                LblRegName.TabStop = false;
                LblRegName.Text = $"{registerName}:";

                this.groupBox1.Controls.Add(LblRegName);

                var registerBitsLabelsList = new List<Label>();

                for (int j = Consts.RegisterBits - 1; j >= 0; j--)
                {
                    var bitLabel = new Label();

                    bitLabel.AutoSize = true;
                    bitLabel.Location = new Point(j * regBitValXStep + regBitValBaseX, i * regBitValYStep + regBitValBaseY);
                    bitLabel.Visible = true;
                    bitLabel.Name = registerName;
                    bitLabel.TabStop = false;
                    bitLabel.Text = "0";

                    var bitNumber = Convert.ToByte(j);

                    bitLabel.Click += (sender, args) => RegisterBitOnClick(registerName, bitNumber);

                    this.groupBox1.Controls.Add(bitLabel);

                    registerBitsLabelsList.Add(bitLabel);
                }

                this.registerLabels[registerName] = registerBitsLabelsList;
            }
        }

        private void InitRegisterLabels()
        {
            this.registerLabels = new Dictionary<string, IList<Label>>();

            // General purpose registers

            InitRSomeRegisterLabels(10, 35, 20, 14, 21, 0, Consts.GeneralPurposeRegistersCount);

            // Done with General Purpose Registers

            // Leave some space between register sections

            // Additional registers

            InitRSomeRegisterLabels(10, 35, 45, 14, 21, 8, Consts.AdditionalRegistersCount);

            // Done with additional registers

            this.statusFlagBitLabels = new Dictionary<string, Label>();

            for (var i = 0; i < Consts.StatusFlagNames.Count(); i++)
            {
                var flagNameLabel = new Label();
                var flagName = Consts.StatusFlagNames.ElementAt(i);

                flagNameLabel.AutoSize = true;
                //flagNameLabel.Location = new Point(regNameBaseX, i * regBitValYStep + regBitValBaseY);
                flagNameLabel.Visible = true;
                flagNameLabel.Name = flagName;
                flagNameLabel.TabStop = false;
                flagNameLabel.Text = $"{flagName}:";
                groupBox2.Controls.Add(flagNameLabel);

                var flagBitLabel = new Label();
                flagBitLabel.AutoSize = true;
                //flagBitLabel.Location = new Point(regBitValBaseX, i * regBitValYStep + regBitValBaseY);
                flagBitLabel.Visible = true;
                //flagBitLabel.Name = ;
                flagBitLabel.TabStop = false;
                flagBitLabel.Text = "0";
                this.statusFlagBitLabels[flagName] = flagBitLabel;

                groupBox2.Controls.Add(flagBitLabel);

                flagBitLabel.Click += (sender, args) => StatusFlagBitOnClick(flagName);
            }

            //this.statusWordLabels = statusWordBitLabels;
        }

        private void RegisterBitOnClick(string regName, byte bitNumber)
        {
            _virtualMachine.FlipRegisterBit(regName, (byte) (Consts.RegisterBits - bitNumber - 1));
        }

        private void StatusFlagBitOnClick(string flagName)
        {
            _virtualMachine.FlipStatusFlag(flagName);
        }

        private void UpdateRegisters(IDictionary<string, ushort>  newRegisters)
        {
            foreach (var register in newRegisters)
            {
                var regName = register.Key;
                var regVal = register.Value;

                var registerBitsLabelsList = registerLabels[regName];

                for (byte i = 0; i < Consts.RegisterBits; i++)
                {
                    var regBitLabel = registerBitsLabelsList[i];
                    regBitLabel.ForeColor = Color.Black; // reset text color

                    var newBitVal = regVal.GetBit(i);
                    var oldBitVal = regBitLabel.Text == "1";

                    if (newBitVal != oldBitVal)
                    {
                        regBitLabel.Text = $"{Convert.ToInt16(newBitVal)}";
                        regBitLabel.ForeColor = Color.Green; // the value has been updated so show it with green text color
                    }
                }
            }
        }

        private void UpdateStatusFlags(IDictionary<string, bool> newStatusFlags)
        {
            foreach (var statusFlag in newStatusFlags)
            {
                var flagName = statusFlag.Key;
                var flagVal = statusFlag.Value;
                
                var regBitLabel = statusFlagBitLabels[flagName];
                regBitLabel.ForeColor = Color.Black; // reset text color

                var newBitVal = flagVal;
                var oldBitVal = regBitLabel.Text == "1";

                if (newBitVal != oldBitVal)
                {
                    regBitLabel.Text = $"{Convert.ToInt16(newBitVal)}";
                    regBitLabel.ForeColor = Color.Green; // the value has been updated so show it with green text color
                }
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.ButtonsImageList = new System.Windows.Forms.ImageList(this.components);
            this.LblVMStatus = new System.Windows.Forms.Label();
            this.BtnPause = new System.Windows.Forms.Button();
            this.BtnStepForward = new System.Windows.Forms.Button();
            this.BtnStop = new System.Windows.Forms.Button();
            this.BtnStart = new System.Windows.Forms.Button();
            this.BtnReset = new System.Windows.Forms.Button();
            this.Monitor = new System.Windows.Forms.PictureBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.TxtSourceCode = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.Monitor)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonsImageList
            // 
            this.ButtonsImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ButtonsImageList.ImageStream")));
            this.ButtonsImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.ButtonsImageList.Images.SetKeyName(0, "pause.png");
            this.ButtonsImageList.Images.SetKeyName(1, "play.png");
            this.ButtonsImageList.Images.SetKeyName(2, "reset.png");
            this.ButtonsImageList.Images.SetKeyName(3, "step.png");
            this.ButtonsImageList.Images.SetKeyName(4, "stop.png");
            // 
            // LblVMStatus
            // 
            this.LblVMStatus.AutoSize = true;
            this.LblVMStatus.Location = new System.Drawing.Point(249, 329);
            this.LblVMStatus.Name = "LblVMStatus";
            this.LblVMStatus.Size = new System.Drawing.Size(0, 20);
            this.LblVMStatus.TabIndex = 6;
            // 
            // BtnPause
            // 
            this.BtnPause.ImageIndex = 0;
            this.BtnPause.ImageList = this.ButtonsImageList;
            this.BtnPause.Location = new System.Drawing.Point(54, 318);
            this.BtnPause.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnPause.Name = "BtnPause";
            this.BtnPause.Size = new System.Drawing.Size(38, 42);
            this.BtnPause.TabIndex = 5;
            this.BtnPause.UseVisualStyleBackColor = true;
            this.BtnPause.Click += new System.EventHandler(this.BtnPause_Click);
            // 
            // BtnStepForward
            // 
            this.BtnStepForward.ImageIndex = 3;
            this.BtnStepForward.ImageList = this.ButtonsImageList;
            this.BtnStepForward.Location = new System.Drawing.Point(144, 318);
            this.BtnStepForward.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnStepForward.Name = "BtnStepForward";
            this.BtnStepForward.Size = new System.Drawing.Size(38, 42);
            this.BtnStepForward.TabIndex = 4;
            this.BtnStepForward.TabStop = false;
            this.BtnStepForward.UseVisualStyleBackColor = true;
            this.BtnStepForward.Click += new System.EventHandler(this.BtnStepForward_Click);
            // 
            // BtnStop
            // 
            this.BtnStop.ImageIndex = 4;
            this.BtnStop.ImageList = this.ButtonsImageList;
            this.BtnStop.Location = new System.Drawing.Point(99, 318);
            this.BtnStop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnStop.Name = "BtnStop";
            this.BtnStop.Size = new System.Drawing.Size(38, 42);
            this.BtnStop.TabIndex = 3;
            this.BtnStop.TabStop = false;
            this.BtnStop.UseVisualStyleBackColor = true;
            this.BtnStop.Click += new System.EventHandler(this.BtnStop_Click);
            // 
            // BtnStart
            // 
            this.BtnStart.ImageIndex = 1;
            this.BtnStart.ImageList = this.ButtonsImageList;
            this.BtnStart.Location = new System.Drawing.Point(9, 318);
            this.BtnStart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnStart.Name = "BtnStart";
            this.BtnStart.Size = new System.Drawing.Size(38, 42);
            this.BtnStart.TabIndex = 2;
            this.BtnStart.TabStop = false;
            this.BtnStart.UseVisualStyleBackColor = true;
            this.BtnStart.Click += new System.EventHandler(this.BtnStart_Click);
            // 
            // BtnReset
            // 
            this.BtnReset.ImageIndex = 2;
            this.BtnReset.ImageList = this.ButtonsImageList;
            this.BtnReset.Location = new System.Drawing.Point(189, 318);
            this.BtnReset.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnReset.Name = "BtnReset";
            this.BtnReset.Size = new System.Drawing.Size(38, 42);
            this.BtnReset.TabIndex = 0;
            this.BtnReset.TabStop = false;
            this.BtnReset.UseVisualStyleBackColor = true;
            // 
            // Monitor
            // 
            this.Monitor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Monitor.Location = new System.Drawing.Point(9, 10);
            this.Monitor.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Monitor.Name = "Monitor";
            this.Monitor.Size = new System.Drawing.Size(864, 300);
            this.Monitor.TabIndex = 0;
            this.Monitor.TabStop = false;
            this.Monitor.MouseEnter += new System.EventHandler(this.Monitor_MouseEnter);
            this.Monitor.MouseLeave += new System.EventHandler(this.Monitor_MouseLeave);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(577, 326);
            this.textBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(293, 385);
            this.textBox1.TabIndex = 7;
            this.textBox1.TabStop = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(14, 368);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(561, 484);
            this.tabControl1.TabIndex = 9;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Size = new System.Drawing.Size(553, 451);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Registers";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(406, 8);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Size = new System.Drawing.Size(135, 300);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Status flags";
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(3, 8);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(397, 435);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General purpose registers:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.TxtSourceCode);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage2.Size = new System.Drawing.Size(553, 316);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Source Code";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // TxtSourceCode
            // 
            this.TxtSourceCode.Location = new System.Drawing.Point(7, 20);
            this.TxtSourceCode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TxtSourceCode.Multiline = true;
            this.TxtSourceCode.Name = "TxtSourceCode";
            this.TxtSourceCode.Size = new System.Drawing.Size(462, 284);
            this.TxtSourceCode.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(882, 861);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.LblVMStatus);
            this.Controls.Add(this.BtnPause);
            this.Controls.Add(this.BtnStepForward);
            this.Controls.Add(this.BtnStop);
            this.Controls.Add(this.BtnStart);
            this.Controls.Add(this.BtnReset);
            this.Controls.Add(this.Monitor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainForm";
            this.Text = "Almost PDP-11";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.Monitor)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox Monitor;
        private System.Windows.Forms.Button BtnReset;
        private System.Windows.Forms.ImageList ButtonsImageList;
        private System.Windows.Forms.Button BtnStart;
        private System.Windows.Forms.Button BtnStop;
        private System.Windows.Forms.Button BtnStepForward;
        private System.Windows.Forms.Button BtnPause;
        private System.Windows.Forms.Label LblVMStatus;
        private System.Windows.Forms.TextBox textBox1;

        //
        private IDictionary<string, IList<System.Windows.Forms.Label>> registerLabels;
        private IDictionary<string, System.Windows.Forms.Label> statusFlagBitLabels;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private GroupBox groupBox1;
        private TabPage tabPage2;
        private GroupBox groupBox2;
        private TextBox TxtSourceCode;
    }
}

