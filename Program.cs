using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace XYController
{
    class Program
    {
        static void Main(string[] args)
        {
            Command();
            while ( true) { }
        }

        static void Command()
        {
            Console.WriteLine("°°°°°°°°°°°°°°°°°°°°°°° WELCOME TO XY Controller °°°°°°°°°°°°°°°°°°°°°°°");
            Console.WriteLine("");
            Console.WriteLine("_-_-_-_-_-_-_-_-_-_-_-_- LIST OF AVALAIBLE COMMAND -_-_-_-_-_-_-_-_-_-_-");
            Console.WriteLine("connect                        > Connect to machine. [baud:] [pup:] [pdw:] ");
            Console.WriteLine("disconnect                     > Disconnect ");
            Console.WriteLine("svg                            > Print SVG File   [file:] ");
            Console.WriteLine("img                            > Print image File [file:] ");
            Console.WriteLine("write                          > Write chars to serial port ");
            Console.WriteLine("stop                           > stop execution ");
            Console.WriteLine("print                          > start execution ");
            Console.WriteLine("clearqueue                     > Clear all pending messages ");
            Console.WriteLine("       _____________________ print parameters __________________");
            Console.WriteLine("x:                             > X offset ");
            Console.WriteLine("y:                             > Y offset ");
            Console.WriteLine("w:                             > multiply width ");
            Console.WriteLine("h:                             > multiply height ");
            Console.WriteLine("size:                          > Force points to fit plate dimensions");
            Console.WriteLine("-fx                            > flip X-Axis ");
            Console.WriteLine("-fy                            > flip Y-Axis ");
            Console.WriteLine("-o                             > Force points to Origin point. Set Top-Left corner to 0,0 ");
            Console.WriteLine("-s                             > save points as bitmap file in root directory");
            Console.WriteLine("_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_--_-_-_-_-_-_-_-_-_-_-_-_-_-_-_");
            List<List<Point>> pts = new List<List<Point>>();
            while ( true)
            {
                bool _printfile = false;
                string command = Console.ReadLine();

                if (command.Contains("stop"))
                {
                    COM._pause = true; continue;
                }
                if (command.Contains("print"))
                {
                    COM._pause = false; continue;
                }
                if (command.Contains("clearqueue"))
                {
                    COM._pause = true;
                    COM.MsgQueue = new List<string>();
                    COM._pause = false;
                    Console.WriteLine("[cleared]");
                    continue;
                }
                if (command.Contains("disconnect"))
                {
                    COM.DisconnectPort();
                    Console.WriteLine("[disconnected]");
                    continue;
                }
                if ( command.Contains("connect"))
                {
                    string s_baud = UI.GetStringAfterArgs(command, "baud:");
                    string s_pup = UI.GetStringAfterArgs(command, "pup:");
                    string s_pdw = UI.GetStringAfterArgs(command, "pdw:");

                    int baud = 115200;
                    int pup = 30;
                    int pdw = 120;


                    int.TryParse(s_baud, out baud); int.TryParse(s_pup, out pup); int.TryParse(s_pdw, out pdw);
                    COM.ConfigurePort(baud, pdw, pup);
                    continue;
                }
                if (command.Contains("write "))
                {
                    command = command.Replace("write ", "");
                    COM.MsgQueue.Add(command);
                    Console.WriteLine("[done]");
                    continue;

                }
                if (command.Contains("svg "))
                {
                    string filePath = UI.getfilePath(UI.GetStringAfterArgs(command, "file:", '\"').ToCharArray());
                    if (!File.Exists(filePath))
                    {
                        Console.WriteLine("File not exist.");
                        continue;
                    }
                    string s = Graphics.GetSVGPathFromFile(filePath);
                    pts = Graphics.GetPointListsFromSVGPath(s);
                    _printfile = true;

                }

                if (command.Contains("img "))
                {
                    string filePath = UI.getfilePath(UI.GetStringAfterArgs(command, "file:", '\"').ToCharArray());
                    if (!File.Exists(filePath))
                    {
                        Console.WriteLine("File not exist.");
                        continue;
                    }
                    pts = BMP.FastProcessFile(filePath);
                     _printfile = true;

                }

                if (_printfile)
                {
                    _printfile = false;
                    string s_size = UI.GetStringAfterArgs(command, "size:");
                    string s_offx = UI.GetStringAfterArgs(command, "x:");
                    string s_offy = UI.GetStringAfterArgs(command, "y:");
                    string s_scaleX = UI.GetStringAfterArgs(command, "w:");
                    string s_scaleY = UI.GetStringAfterArgs(command, "h:");

                    int size = 0;
                    int offx = 0;
                    int offy = 0;
                    int w = 0;
                    int h = 0;


                    int.TryParse(s_size, out size); int.TryParse(s_offx, out offx); int.TryParse(s_offy, out offy);
                    int.TryParse(s_scaleX, out w); int.TryParse(s_scaleY, out h);

                    bool invX = false, invY = false, save = false, origin = false;

                    if (command.Contains("-fx"))
                        invX = true;
                    if (command.Contains("-fy"))
                        invY = true;
                    if (command.Contains("-s"))
                        save = true;
                    if (command.Contains("-o"))
                        origin = true;


                    if (size != 0)
                    {
                        Graphics.NormalizeListForPlotterDimension(ref pts, size);
                    }
                    if (origin)
                    {
                        Graphics.ForceListOfPointsToOrigin(ref pts);
                    }
                    if (w != 0 || h != 0)
                    {
                        Graphics.RawScaleListOfPoints(ref pts, w, h);
                    }
                    if (invX)
                    {
                        Graphics.InvertXListOfPoints(ref pts, size);
                    }
                    if (invY)
                    {
                        Graphics.InvertYListOfPoints(ref pts, size);
                    }
                    if (offx != 0 || offy != 0)
                    {
                        Graphics.ApplyOffsetToPointsList(ref pts, new Point(offx, offy));
                    }
                    if (save)
                    {
                        Graphics.SaveBMPfromPointsList(pts, 500, 500);
                    }
                    COM.LoadPointsToMsgQueue(pts);

                    Console.WriteLine("[done]");
                    continue;
                }
            }

        }
    }
}
