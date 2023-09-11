using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace XYControl
{
    public class XYCommandLine
    {
        public static void RunCommand(string command)
        {
            XYPath path = null;
            bool _printfile = false;

            if (command.Contains("stop"))
            {
                COM.executionPaused = true; 
                return; ;
            }
            if (command.Contains("print"))
            {
                COM.executionPaused = false;
                return;
            }
            if (command.Contains("clearqueue"))
            {
                COM.msgQueue.Clear();
                Console.WriteLine("[cleared]");
                return;
            }
            if (command.Contains("write "))
            {
                command = command.Replace("write ", "");
                COM.msgQueue.Add(command);
                Console.WriteLine("[done]");
                return;

            }
            if (command.Contains("svg "))
            {
                string filePath = getfilePath(GetStringAfterArgs(command, "file:", '\"').ToCharArray()).Trim();
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("File not exist.");
                    return;
                }
                path = SvgParser.GetXYPath(filePath);
                _printfile = true;

            }

            if (command.Contains("img ") && File.Exists("potrace.exe"))
            {
                string filePath = getfilePath(GetStringAfterArgs(command, "file:", '\"').ToCharArray()).Trim();
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("File not exist.");
                    return;
                }
                string poTraceOutputFile = PoTrace.BuildSVGFromImage(
                           Environment.CurrentDirectory,
                           filePath,
                           Environment.CurrentDirectory);

                path = SvgParser.GetXYPath(poTraceOutputFile);
                _printfile = true;

            }

            if (_printfile)
            {
                _printfile = false;
                string s_size = GetStringAfterArgs(command, "size:");
                string s_offx = GetStringAfterArgs(command, "x:");
                string s_offy = GetStringAfterArgs(command, "y:");
                string s_scaleX = GetStringAfterArgs(command, "w:");
                string s_scaleY = GetStringAfterArgs(command, "h:");

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

                if (origin)
                {
                    path.ForceToOrigin();
                }
                if (size != 0)
                {
                    path.FitToDimension(size, size);
                  
                }
                if (w != 0 || h != 0)
                {
                    path.Scale(w, h);
                }
                if (invX)
                {
                    path.FlipX();
                }
                if (invY)
                {
                    path.FlipY();
                }
                if (offx != 0 || offy != 0)
                {
                    path.ApplyOffset(offx, offy);
                }
                if (save)
                {
                    path.SaveAsBmp();
                }
                COM.LoadPointsToWorkQueue(path);
            }
        }
        public static string getfilePath(char[] searchstring)
        {
            string filepath = "";// search in the quote -- we absolutely need quote
            int index = -1;
            for (int i = 0; i < searchstring.Length; i++)
            {
                if (searchstring[i] == '\"')
                {
                    index = i + 1;
                    break;
                }
            }
            if (index > -1)
            {
                for (int i = index; i < searchstring.Length; i++)
                {
                    if (searchstring[i] == '\"')
                    {
                        break;
                    }
                    filepath += searchstring[i].ToString();
                }
            }

            return filepath;
        }

        public static string GetStringAfterArgs(string searchstring, string arg, char specificdelimiter = ' ')
        {
            int startIndex = searchstring.IndexOf(arg);
            int delimcounter = 0;
            if (startIndex == -1) { return ""; }
            List<char> result = new List<char>();
            for (int i = startIndex + arg.Length; i < searchstring.Length; i++)
            {
                if (searchstring[i] == specificdelimiter && specificdelimiter == ' ')
                    break;
                else
                {
                    if (specificdelimiter == '\"' && searchstring[i] == '\"')
                    {
                        delimcounter++;
                        if (delimcounter == 2)
                            break;
                    }
                }
                result.Add(searchstring[i]);
            }
            char[] chars = new char[result.Count];
            for (int i = 0; i < result.Count; i++)
            {
                chars[i] = result[i];
            }
            return new string(chars);
        }


    }
}
