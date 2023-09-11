using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO.Ports;

namespace XYControl
{
    public class COM
    {
        public static SerialPort sp;
        public static Thread rcvThread;
        public static Thread sndThread;
        public static bool _Busy = false;

        public static List<string> msgQueue = new List<string>();
        public static bool executionPaused = false;
        public static bool isConnected = false;

        public static void SetPortKiller() 
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
        }
        public static string[] GetAvalaiblePortName() 
        {
            return SerialPort.GetPortNames();
        }
        public static bool ConnectToPort(string portName, int baudRate)
        {
            TryClosePort();
            try
            {
                sp = new SerialPort(portName, baudRate);
                sp.Open();
                sp.ReadTimeout = 100;
                Console.WriteLine("Port  " + portName + " opened.");
            }
            catch
            {
                Console.WriteLine("Opening Port Failed.");
                return false;
            }

            if (rcvThread == null) 
            {
                rcvThread = new Thread(new ThreadStart(ReceiveData));
                rcvThread.IsBackground = true;
                rcvThread.Start();
            }
            if ( sndThread == null)
            {
                sndThread = new Thread(new ThreadStart(ProcessWorkQueue));
                sndThread.IsBackground = true;
                sndThread.Start();
            }
            isConnected = true;
            return true;

        }
        public static void OnProcessExit(object sender, EventArgs e)
        {
            // fermer les ports quand nous en avons plus besoin 
            if (sp != null && sp.IsOpen)
                sp.Close();
        }
        public static void TryClosePort() 
        {
            try
            {
                if (sp != null && sp.IsOpen)
                    sp.Close();
                isConnected = false;

            }
            catch(System.Exception e)
            {
                Console.WriteLine(e.Message);

            }
        }

        public static void ReceiveData()
        {
            while (true)
            {
                if (sp != null && sp.IsOpen)
                {
                    try
                    {
                        string r_data;
                        r_data = sp.ReadLine(); //< j'obient la valeur ... 
                        if (r_data.Contains("OK"))
                        {
                            _Busy = false;
                            Console.WriteLine("System is not busy anymore");
                        }
                           
                        Console.WriteLine(r_data);
                    }
                    catch ( System.TimeoutException e)
                    {

                    }
                    catch (System.SystemException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                else
                    Thread.Sleep(500);
            }
        }
        public static void ProcessWorkQueue() // multi threaded message send
        {
            while (true)
            {
                if (_Busy || sp == null )
                    Thread.Sleep(10); 
                else
                {
                    if (sp.IsOpen 
                        && msgQueue.Count > 0 
                        && !executionPaused)
                    {
                        _Busy = true;
                        Console.WriteLine("Sending:" + msgQueue[0] + " Msg count# : " + msgQueue.Count);
                        sp.WriteLine(msgQueue[0]);
                        msgQueue.RemoveAt(0);
                    }
                }

            }
        }
        // Control stuff
        public static int penUpValue = 30;
        public static int penDownValue = 120;

        public static void SetPenDistance(int downValue, int upValue) 
        {
            penUpValue = upValue;
            penDownValue = downValue;
        }
        public static void PauseExecution() 
        {
            executionPaused = true;
        }
        public static void StopExecution() 
        {
            executionPaused = true;
            msgQueue.Clear();
        }
        public static void ContinueExecution() 
        {
            executionPaused = false;
        }
        public static void StartExecution(XYPath path = null) 
        {
            msgQueue.Clear();
            if ( path != null) 
            {
                LoadPointsToWorkQueue(path);
            }
            executionPaused = false;
        }
        public static void LoadPointsToWorkQueue(XYPath path)
        {
            List<List<System.Numerics.Vector2>> points = path.shapesParts;
            foreach (List<System.Numerics.Vector2> lp in points)
            {
                // 0 Lever le crayon
                msgQueue.Add("M1 " + penUpValue.ToString());
                // 1 aller a la position 0 de l'ensemble de points
                msgQueue.Add("G0 X" + lp[0].X + " Y" + lp[0].Y);
                // 2 baisser le crayon
                msgQueue.Add("M1 " + penDownValue.ToString());
                for (int i = 1; i < lp.Count; i++)
                {
                    // 4 se deplacer jusquau dernier point 
                    msgQueue.Add("G0 X" + lp[i].X + " Y" + lp[i].Y);
                }
                // 4 lever le crayon  crayon
                msgQueue.Add("M1 " + penUpValue.ToString());
            }
        }
        public static void LoadPointsToWorkQueue(List<List<System.Numerics.Vector2>> points)
        {

            foreach (List<System.Numerics.Vector2> lp in points)
            {
                // 0 Lever le crayon
                msgQueue.Add("M1 " + penUpValue.ToString());
                // 1 aller a la position 0 de l'ensemble de points
                msgQueue.Add("G0 X" + lp[0].X + " Y" + lp[0].Y);
                // 2 baisser le crayon
                msgQueue.Add("M1 " + penDownValue.ToString());
                for (int i = 1; i < lp.Count; i++)
                {
                    // 4 se deplacer jusquau dernier point 
                    msgQueue.Add("G0 X" + lp[i].X + " Y" + lp[i].Y);
                }
                // 4 lever le crayon  crayon
                msgQueue.Add("M1 " + penUpValue.ToString());
            }
        }
    }
}
