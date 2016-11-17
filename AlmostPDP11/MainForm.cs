﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AlmostPDP11.VM.Extentions;
using VM;

namespace AlmostPDP11
{
    public partial class MainForm : Form
    {
        private readonly VirtualMachine _virtualMachine;

        private void InitASCIIMap()
        {
            var mappings = @"
                D0 > 11 > 48
                D1 > 2 > 49
                D2 > 3 > 50
                D3 > 4 > 51
                D4 > 5 > 52
                D5 > 6 > 53
                D6 > 7 > 54
                D7 > 8 > 55
                D8 > 9 > 56
                D9 > 10 > 57
                Q > 16 > 81
                W > 17 > 87
                E > 18 > 69
                R > 19 > 82
                T > 20 > 84
                Y > 21 > 89
                U > 22 > 85
                I > 23 > 73
                O > 24 > 79
                P > 25 > 80
                A > 30 > 65
                S > 31 > 83
                D > 32 > 68
                F > 33 > 70
                G > 34 > 71
                H > 35 > 72
                J > 36 > 74
                K > 37 > 75
                L > 38 > 76
                Z > 44 > 90
                X > 45 > 88
                C > 46 > 67
                V > 47 > 86
                B > 48 > 66
                N > 49 > 78
                M > 50 > 77
                Escape > 1 > 27
                Return > 28 > 13
                Back > 14 > 8
                NULL > 0 > 0";

            var mappingLines = mappings
                .Trim()
                .Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .Where(line => line.Length > 0);

            var lineElements = mappingLines
                .Select(line => line.Trim().Split('>'));

            foreach (var lineElement in lineElements)
            {
                DataGridASCIIMap.Rows.Add(lineElement[0], lineElement[1], lineElement[2]);
            }
        }

        public MainForm()
        {
            InitializeComponent();
            InitRegisterLabels();

            _virtualMachine = new VirtualMachine(this);

            _virtualMachine.OnRegistersUpdated += UpdateRegisters;
            _virtualMachine.OnVRAMUpdated += OnVRAMUpdated;

            _virtualMachine.UpdateViews();

            InitASCIIMap();

            UpdateControls();
        }

        private static Color GetColor(byte r, byte g, byte b)
        {
            byte nr;
            byte ng;
            byte nb;

            if (r >= 1)
            {
                nr = 255;
            }
            else
            {
                nr = 0;
            }

            if (g >= 1)
            {
                ng = 255;
            }
            else
            {
                ng = 0;
            }

            if (b >= 1)
            {
                nb = 255;
            }
            else
            {
                nb = 0;
            }

            return Color.FromArgb(nr, ng, nb);
        }

        private IEnumerable<Color> VRAMToPixels(byte[] VRAMBytes)
        {
            var totalPixels = Consts.MonitorWidth * Consts.MonitorHeight;
            var totalPixelColorComponents = totalPixels * Consts.ColorComponents;
            var totalColorComponentsBits = totalPixelColorComponents * Consts.BitsInColorComponent;

            var VRAMBitStream = VRAMBytes.ToBitStream();
            var bitstream = VRAMBitStream.Take(totalColorComponentsBits).ToArray();
            var pixelColorComponents = new List<byte>();

            var s = @"7777000007777770000000000077777777000000077770000000007777770000000000000770000000000000777777000000000770000777770000777000000000007777777777770000770000777770000770000777777777770000777770000770000777770000777700000000077770000000000077777700000000077770
7700007000077770000777770000777700007770000770000777000077770000777777777770000777777777777700007777777770000777770000777777700077777777777777770000770000777000077770000777777777770000007000000770000007770000770000777770000770000777770000770000777770000770
7700077700077770000777770000777700077770000770000777000077770000777777777770000777777777777700007777777770000777770000777777700077777777777777770000770000777000077770000777777777770000000000000770000007770000770000777770000770000777770000770000777770000770
0000777770000770000777770000770000777777777770000777770000770000777777777770000777777777770000777777777770000777770000777777700077777777777777770000770000770007777770000777777777770000000000000770000000770000770000777770000770000777770000770000777770000770
0000777770000770000000000077770000777777777770000777770000770000777777777770000777777777770000777777777770000777770000777777700077777777777777770000770000000077777770000777777777770000000000000770000000000000770000777770000770000777770000770000777770000770
0000777770000770000000000077770000777777777770000777770000770000000000077770000000000077770000777000000770000000000000777777700077777777777777770000770000000777777770000777777777770000000000000770000000000000770000777770000770000777770000770000777770000770
0000000000000770000777770000770000777777777770000777770000770000000000077770000000000077770000777000000770000000000000777777700077777777777777770000770000000007777770000777777777770000000000000770000000000000770000777770000770000000000077770000770000000770
0000000000000770000777770000770000777777777770000777770000770000777777777770000777777777770000777770000770000777770000777777700077777777777777770000770000000007777770000777777777770000770770000770000770000000770000777770000770000000000077770000770000000770
0000777770000770000777770000777700077770000770000777700077770000777777777770000777777777777700077770000770000777770000777777700077777770000777770000770000770000077770000777777777770000777770000770000770000000770000777770000770000777777777770000777000077770
0000777770000770000777770000777700007770000770000777000077770000777777777770000777777777777700007770000770000777770000777777700077777770000777770000770000770000077770000777777777770000777770000770000777000000770000777770000770000777777777770000777000077770
0000777770000770000000000077777777000000077770000000007777770000000000000770000777777777777777000000000770000777770000777000000000007777700000000077770000777000000770000000000000770000777770000770000777770000777700000000077770000777777777777700000007700770
0000777770000770000000000077777777000000077770000000007777770000000000000770000777777777777777000000000770000777770000777000000000007777700000000077770000777000000770000000000000770000777770000770000777770000777700000000077770000777777777777700000007700770
";

            var lines = s.Split(new []{"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            var text = lines.Aggregate("", (current, line) => current + line);

            var bytes = text.Select(l => byte.Parse(l.ToString()));

            foreach (var b in bytes)
            {
                pixelColorComponents.Add(b);
            }

            for (var i = 0; i < totalColorComponentsBits; i += Consts.BitsInColorComponent)
            {
                byte colorComponent = 0;

                for (byte j = 0; j < Consts.BitsInColorComponent; j++)
                {
                    var currentBit = i + j;

                    if (bitstream[currentBit])
                    {
                        colorComponent += j.TwoPow();
                    }
                }

                pixelColorComponents.Add(colorComponent);
            }

            var colors = new List<Color>();

            for (var i = 0; i < totalPixelColorComponents; i += Consts.ColorComponents)
            {
                // for 3 color components
                var rByte = pixelColorComponents[i + 0];
                var gByte = pixelColorComponents[i + 1];
                var bByte = pixelColorComponents[i + 2];

                var pixelColor = GetColor(rByte, gByte, bByte);

                colors.Add(pixelColor);
            }

            return colors;
        }

        private void OnVRAMUpdated(byte[] VRAMBytes)
        {
            var scaledMonitorWidth = Consts.MonitorWidth*Consts.PictureScaleFactor;
            var scaledMonitorHeight = Consts.MonitorHeight*Consts.PictureScaleFactor;

            var monitorBitmap = new Bitmap(scaledMonitorWidth, scaledMonitorHeight);

            Monitor.Image?.Dispose();
            Monitor.Image = monitorBitmap;

            var pixels = VRAMToPixels(VRAMBytes).ToArray();

            var totalScaledPixels = pixels.Length * Consts.PictureScaleFactor * Consts.PictureScaleFactor;
            
            for (var i = 0; i < totalScaledPixels; i++)
            {
                var x = i % monitorBitmap.Width;
                var y = i / monitorBitmap.Width;

                var indexInColors = (x/Consts.PictureScaleFactor)
                                    + (y/Consts.PictureScaleFactor)*Consts.MonitorHeight;

                monitorBitmap.SetPixel(x, y, pixels[indexInColors]);
            }

            Monitor.Refresh();
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
            _virtualMachine.Start();
            _virtualMachine.StepForward();
            _virtualMachine.Pause();

            UpdateControls();
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

        private void BtnUpload_Click(object sender, EventArgs e)
        {
            var pureCodeLines = TxtSourceCode.Lines
                .Select(line => line.Trim())
                .Where(line => line.Length > 0)
                .ToArray();

            _virtualMachine.UploadCodeToROM(pureCodeLines);
        }

        private void BtnShowMem_Click(object sender, EventArgs e)
        {
            var fromAddressStr = TxtROMFromAddress.Text;
            var toAddressStr = TxtROMToAddress.Text;

            if (fromAddressStr == null || toAddressStr == null
                || fromAddressStr.Length < 1 || toAddressStr.Length < 1)
            {
                return;
            }

            var fromAddress = int.Parse(TxtROMFromAddress.Text);
            var toAddress = int.Parse(toAddressStr);

            var memoryBytes = _virtualMachine.GetMemory(fromAddress, toAddress);

            var hexBytes = memoryBytes.Select(bt => bt.ToString("X2")).ToArray();

            TxtHexMemory.Text = string.Join(" ", hexBytes);
        }

        private void BtnUploadASCIIMapping_Click(object sender, EventArgs e)
        {
            var ASCIIMap = new Dictionary<byte, byte>();

            foreach (DataGridViewRow row in DataGridASCIIMap.Rows)
            {
                var scanCodeStr = (string) row.Cells[1].Value;
                var asciiCodeStr = (string) row.Cells[2].Value;

                var scanCode = (byte) (scanCodeStr == null ? 0 : byte.Parse(scanCodeStr));
                var asciiCode = (byte) (asciiCodeStr == null ? 0 : byte.Parse(asciiCodeStr));

                ASCIIMap[scanCode] = asciiCode;
            }

            _virtualMachine.UploadASCIIMap(ASCIIMap);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var memorySegmentName = ComboBoxMemorySegment.Text;

            if (!Consts.MemoryOffsets.ContainsKey(memorySegmentName) ||
                !Consts.MemoryOffsets.ContainsKey(memorySegmentName))
            {
                return;
            }

            var segmentStart = Consts.MemoryOffsets[memorySegmentName];
            var segmentEnd = segmentStart + Consts.MemorySizes[memorySegmentName];

            TxtROMFromAddress.Text = segmentStart.ToString();
            TxtROMToAddress.Text = segmentEnd.ToString();
        }

        private void Monitor_Click(object sender, EventArgs e)
        {
            _virtualMachine.OnVRAMUpdated(_virtualMachine.GetVRAMBytes().ToArray());
        }
    }
}
