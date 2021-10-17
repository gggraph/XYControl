using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;

namespace XYController
{
   
    class COM
    {
        public static List<string> MsgQueue = new List<string>();

        public static int PEN_DOWN = 120;
        public static int PEN_UP = 30;

        public static SerialPort sp_XY;
        public static Thread RCV_PLOTTER;
        public static Thread SND_PLOTTER;

        public static bool _Busy = false;
        public static bool _pause = false;

        public static void LoadPointsToMsgQueue(List<List<Point>> points)
        {

            foreach (List<Point> lp in points)
            {

                // 1 aller a la position 0 de l'ensemble de points
                MsgQueue.Add("GO X" + lp[0].X + "Y" + lp[0].Y);
                // 2 baisser le crayon
                MsgQueue.Add("M1 " + PEN_DOWN.ToString());
                for (int i = 1; i < lp.Count; i++)
                {
                    // 4 se deplacer jusquau dernier point 
                    MsgQueue.Add("GO X" + lp[0].X + "Y" + lp[0].Y);
                }
                // 4 lever le crayon  crayon
                MsgQueue.Add("M1 " + PEN_UP.ToString());
            }
        }

        public static void ConfigurePort(int baudRate, int PDOWN, int PUP)
        {
            string[] ports = SerialPort.GetPortNames();
            Console.WriteLine("List of usable ports : ");
            foreach (string s in ports)
            {
                Console.Write(s + " / ");
            }
            Console.WriteLine("");
            Console.WriteLine("Please type port name connected to plotter : ");

            while (true)
            {

                string portName = Console.ReadLine();
                try
                {
                    sp_XY = new SerialPort(portName, baudRate);
                    sp_XY.Open();
                    sp_XY.ReadTimeout = 100;
                    Console.WriteLine("Port  " + portName + " opened.");
                    break;
                }
                catch
                {
                    if (!UI.ValidYesOrNo("")) { return; }
                    Console.WriteLine("Opening Port Failed.");
                }
            }
            PEN_DOWN = PDOWN;
            PEN_UP = PUP;

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            RCV_PLOTTER = new Thread(new ThreadStart(Receive_XY_Data));
            RCV_PLOTTER.IsBackground = true;
            RCV_PLOTTER.Start();
            SND_PLOTTER = new Thread(new ThreadStart(ProcessMessageQueue));
            SND_PLOTTER.IsBackground = true;
            SND_PLOTTER.Start();
           
        }

        public static void Receive_XY_Data()
        {
            while (true)
            {
                if (sp_XY != null)
                {
                    if (sp_XY.IsOpen == true)
                    {
                        try
                        {
                            string r_data;

                            r_data = sp_XY.ReadLine(); //< j'obient la valeur ... 
                            if (r_data == "OK")
                            {
                                _Busy = false;
                            }
                            Console.WriteLine(r_data);


                        }
                        catch (System.TimeoutException e)
                        {
                        }
                    }

                }
            }
        }

        public static void ProcessMessageQueue() // multi threaded message send
        {
            while (true)
            {
                if (_Busy)
                    Thread.Sleep(10); // just a security. probably not necessary
                else
                {
                    if (MsgQueue.Count > 0 && !_pause)
                    {
                        _Busy = true;
                        sp_XY.WriteLine(MsgQueue[0]);
                        MsgQueue.RemoveAt(0);
                    }

                }

            }

        }

        public static void DisconnectPort()
        {
            if (RCV_PLOTTER.IsAlive)
            {
                RCV_PLOTTER.Abort();

            }

            if (SND_PLOTTER.IsAlive)
            {
                SND_PLOTTER.Abort();

            }
            if (sp_XY != null)
            {
                if (sp_XY.IsOpen == true)
                {

                    sp_XY.Close();
                }
            }
        }
        public static void OnProcessExit(object sender, EventArgs e)
        {

            // fermer les ports quand nous en avons plus besoin 
            if (sp_XY != null)
            {
                if (sp_XY.IsOpen == true)
                {

                    sp_XY.Close();
                }
            }

        }
    }
}
