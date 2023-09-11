using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XYControl;
using System.Numerics;
using System.IO;

namespace XYControlGUI
{
    /*
        > Progress Bar when printing...
        > Plotter Dimension setup (+ pen up etc ) 
        > READY MESSAGE
    
    [2]  Plotter dimension - plotter config file;... (50-120)  

     */
    public partial class Form1 : Form
    {
        public PathEditor mEditor;
        public Form1()
        {
            this.KeyPreview = true;
            InitializeComponent();
            InitFiles();
            mEditor = new PathEditor(this, panel1);
            numericUpDown1.Validated += NumericUpDown1_Validated;
            COMModule comModule = new COMModule(
                mEditor,
                new COMModule.ControlMap()
                {
                    portListBox = portNameList,
                    baudListBox = baudRateList,
                    connectButton = ConnectButton,
                    startButton = StartButton,
                    stopButton = StopButton,
                    SendButton = COMSendButton,
                    commandBox = CLITextBox,
                    downValue = numericUpDown2,
                    upValue = numericUpDown3,
                    penTest = button1
                }
                );; ;
        }

        private void NumericUpDown1_Validated(object sender, EventArgs e)
        {
            mEditor.PlotterDimension = (int)((NumericUpDown)sender).Value;
        }

        public void InitFiles() 
        {
            if (!Directory.Exists("temp")) 
                Directory.CreateDirectory("temp");
        
        }

       
    }
}
