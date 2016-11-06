using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VM;

namespace AlmostPDP11
{
    public partial class MainForm : Form
    {
        private readonly VirtualMachine _virtualMachine;

        public MainForm()
        {
            _virtualMachine = new VirtualMachine();

            InitializeComponent();
            InitRegisterLabels();

            _virtualMachine.OnRegistersUpdated += UpdateRegisters;
            _virtualMachine.OnStatusFlagUpdated += UpdateStatusFlags;
            _virtualMachine.OnVRAMUpdated += OnVRAMUpdated;

            _virtualMachine.UpdateViews();

            UpdateControls();
        }

        private void OnVRAMUpdated(byte[] VRAMBytes)
        {
            // TODO: make it WORK m'faka
            VRAMBytes = new byte[1000];
            VRAMBytes = VRAMBytes.Select(x => (byte)9).ToArray();
            var bitmap = new Bitmap(Monitor.Width, Monitor.Height, PixelFormat.Format32bppArgb);
            var bitmap_data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(VRAMBytes, 0, bitmap_data.Scan0, VRAMBytes.Length);
            bitmap.UnlockBits(bitmap_data);
            var result = bitmap as Image;

            Monitor.Image = result; //Image.FromStream(new MemoryStream(VRAMBytes.Take(100).ToArray()));
        }

        [DllImport("MimicPDP11.dll")]
        public static extern int magic_sum(int a, int b);

        private void UpdateStatusLabel(string newStatus, Color color)
        {
            LblVMStatus.Text = $@"Status: {newStatus}";
            LblVMStatus.ForeColor = color;
        }

        private void UpdateControls()
        {
            switch (_virtualMachine.CurrentState)
            {
                case MachineState.Paused:
                    BtnStart.Enabled = true;
                    BtnPause.Enabled = false;
                    BtnStop.Enabled = true;
                    BtnStepForward.Enabled = true;
                    BtnReset.Enabled = true;
                    UpdateStatusLabel("paused", Color.DarkOrange);
                    break;
                case MachineState.Stopped:
                    BtnStart.Enabled = true;
                    BtnPause.Enabled = false;
                    BtnStop.Enabled = false;
                    BtnStepForward.Enabled = true;
                    BtnReset.Enabled = false;
                    UpdateStatusLabel("stopped", Color.Red);
                    break;
                case MachineState.Running:
                    BtnStart.Enabled = false;
                    BtnPause.Enabled = true;
                    BtnStop.Enabled = true;
                    BtnStepForward.Enabled = false;
                    BtnReset.Enabled = true;
                    UpdateStatusLabel("running", Color.Green);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnStateUpdated(MachineState oldState, MachineState newState)
        {
            // TODO: react on VM state update
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            // calling start button on virtual machine
            _virtualMachine.Start();
            
            UpdateControls();
        }

        private void BtnPause_Click(object sender, EventArgs e)
        {
            _virtualMachine.Pause();

            UpdateControls();
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            _virtualMachine.Stop();

            UpdateControls();
        }

        private void BtnStepForward_Click(object sender, EventArgs e)
        {
//            _virtualMachine.Start();
            _virtualMachine.StepForward();
            _virtualMachine.Pause();

            UpdateControls();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            
        }

        private void LblR0_Click(object sender, EventArgs e)
        {
            var me = (MouseEventArgs)e;
//            var txt = LblR0.Text;

//            var sz = TextRenderer.MeasureText(txt, LblR0.Font);
//            var letterWidth = LblR0.Width / txt.Length;
//            var letterNumber = (float)me.X / letterWidth;


            //            MessageBox.Show(string.Format($"{me.X}, {me.Y}"));
//            MessageBox.Show($"{letterNumber}, {LblR0.Font.Size}, {letterWidth}");
        }

        private const uint MAPVK_VK_TO_VSC = 0;

        [DllImport("user32.dll",
            CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Unicode,
            EntryPoint = "MapVirtualKey",
            SetLastError = true,
            ThrowOnUnmappableChar = false)]
        private static extern uint MapVirtualKey(
            uint uCode,
            uint uMapType);

        private static byte KeyCodeToScanCode(Keys keyCode)
        {
            var uCode = (uint)keyCode;
            var scanCode = (byte)MapVirtualKey(uCode, MAPVK_VK_TO_VSC);

            return scanCode;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (!_interceptKeyboard)
            {
                return;
            }

            // update keyboard_handler register

            var scanCode = KeyCodeToScanCode(e.KeyCode);
            _virtualMachine.GenerateKeyboardInterrupt(scanCode, false, e.Alt, e.Control, e.Shift);
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (!_interceptKeyboard)
            {
                return;
            }

            // update keyboard_handler register

            var scanCode = KeyCodeToScanCode(e.KeyCode);
            _virtualMachine.GenerateKeyboardInterrupt(scanCode, true, e.Alt, e.Control, e.Shift);
        }

        private void Monitor_MouseEnter(object sender, EventArgs e)
        {
            // allow updating keyboard_handler register
            _interceptKeyboard = true;
        }

        private void Monitor_MouseLeave(object sender, EventArgs e)
        {
            // prohibit updating keyboard_handler register
            _interceptKeyboard = false;
        }

        private bool _interceptKeyboard;

        private void TxtSourceCode_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void BtnUpload_Click(object sender, EventArgs e)
        {
            _virtualMachine.UploadCodeToROM(TxtSourceCode.Lines);
        }
    }
}
