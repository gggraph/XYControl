using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XYControl;
using System.Threading;

namespace XYControlGUI
{
    public class COMModule
    {
        public struct ControlMap 
        {
            public ComboBox portListBox;
            public ComboBox baudListBox;
            public Button connectButton; // Done
            public Button startButton;
            public Button stopButton;
            public Button SendButton;
            public TextBox commandBox;
            public NumericUpDown downValue;
            public NumericUpDown upValue;
            public Button penTest;
        }

        private ControlMap ctrlMap;

        public PathEditor editor;

        public Form mForm;
        public int linesCounter;

        public COMModule(PathEditor editor, ControlMap controlMap)
        {

            this.editor = editor;
            ctrlMap = controlMap;
            ctrlMap.portListBox.Items.Clear();
            ctrlMap.portListBox.Click += PortListBox_Click;

            mForm = ctrlMap.commandBox.FindForm();
            mForm.Text = "SVG TO PLOT";

            List<int> rates = new List<int>()
            { 110, 300, 600, 1200, 2400, 4800, 9600,
              14400, 19200, 38400, 57600, 115200,
              128000, 256000
            };

            foreach (int s in rates)
                ctrlMap.baudListBox.Items.Add(s);

            UpdatePortList();
            ctrlMap.portListBox.SelectedIndex = 0;
            ctrlMap.baudListBox.SelectedIndex = 6;
            AllowCOMTransfer(false);
            ctrlMap.connectButton.Click += ConnectButton_Click;
            ctrlMap.startButton.Click += StartButton_Click;
            ctrlMap.stopButton.Click += StopButton_Click;
            ctrlMap.SendButton.Click += SendButton_Click;

            ctrlMap.downValue.ValueChanged += DownValue_ValueChanged;
            ctrlMap.upValue.ValueChanged += UpValue_ValueChanged;
            ctrlMap.penTest.Click += PenTest_Click;
            new Thread(GetJobPourcentage) { IsBackground = true }.Start();
        }

        private void PenTest_Click(object sender, EventArgs e)
        {
            COM.msgQueue.Insert(0, "M1 " + COM.penUpValue.ToString());
            COM.msgQueue.Insert(0, "M1 " + COM.penDownValue.ToString());
        }

        private void UpValue_ValueChanged(object sender, EventArgs e)
        {
            COM.SetPenDistance((int)ctrlMap.downValue.Value,(int) ctrlMap.upValue.Value);
         
        }

        private void DownValue_ValueChanged(object sender, EventArgs e)
        {
            COM.SetPenDistance((int)ctrlMap.downValue.Value, (int)ctrlMap.upValue.Value);
            //Console.WriteLine("")
        }

        public void GetJobPourcentage() 
        {
            while (true)
            {
                Thread.Sleep(1000);
                string title = "SVG TO PLOT";
                if (COM.msgQueue.Count > 0 && linesCounter > 0) 
                {
                    float prct =  ((linesCounter-COM.msgQueue.Count)/ linesCounter)* 100f;
                    title += " - "  
                        + (COM.executionPaused ? "PAUSED - " : string.Empty)
                        + (int)prct+"%";
                }
                mForm.Invoke(new MethodInvoker(delegate
                {
                    mForm.Text = title;
                }));

               
            }
        }

        private void PortListBox_Click(object sender, EventArgs e)
        {
            UpdatePortList();
        }

        private void UpdatePortList() 
        {
            ctrlMap.portListBox.Items.Clear();
            foreach (string s in COM.GetAvalaiblePortName())
                ctrlMap.portListBox.Items.Add(s);
        }
        private void SendButton_Click(object sender, EventArgs e)
        {
            XYCommandLine.RunCommand(ctrlMap.commandBox.Text);
            ctrlMap.commandBox.Text = string.Empty;
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            COM.StopExecution();
            ctrlMap.startButton.Text = "Start";
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (COM.executionPaused) 
            {
                if ( COM.msgQueue.Count>0)
                    COM.ContinueExecution();
                else
                {

                    XYPath path = editor.DumpFullPath();
                    linesCounter = path.lineCount();
                    path.SaveAsBmp();
                    COM.StartExecution(path);
                }
                ctrlMap.startButton.Text = "Pause";
            }
            else 
            {
                COM.PauseExecution();
                ctrlMap.startButton.Text = "Start";
            }
        }

        private void AllowCOMTransfer(bool value) 
        {
            ctrlMap.startButton.Enabled = value;
            ctrlMap.SendButton.Enabled = value;
            ctrlMap.stopButton.Enabled = value;
            ctrlMap.penTest.Enabled = value;
        }
        private void ConnectButton_Click(object sender, EventArgs e)
        {
            if (!COM.isConnected) 
            {
                if (XYControl.COM.ConnectToPort(
                   (string)ctrlMap.portListBox.SelectedItem,
                   (int)ctrlMap.baudListBox.SelectedItem
                   ))
                {
                    Console.WriteLine("Connection Success!");
                    AllowCOMTransfer(true);
                    ctrlMap.connectButton.Text = "Disconnect";
                }
            }
            else 
            {
                COM.TryClosePort(); 
                AllowCOMTransfer(false);
                ctrlMap.connectButton.Text = "Connect";
            }
           
        }
    }
}
