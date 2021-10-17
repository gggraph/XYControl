using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYController
{
    class UI
    {
        public static bool ValidYesOrNo(string warning)
        {
            //bool confirmed = false;
            //string Key;
            Console.WriteLine(warning);
            ConsoleKey response;
            do
            {
                Console.Write("Do you want to procceed ? [y/n] ");
                response = Console.ReadKey(false).Key;   // true is intercept key (dont show), false is show
                if (response != ConsoleKey.Enter)
                    Console.WriteLine();

            } while (response != ConsoleKey.Y && response != ConsoleKey.N);

            if (response == ConsoleKey.Y)
            {
                return true;
            }
            else
            {
                return false;
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
                {
                    break;

                }
                else
                {
                    if (specificdelimiter == '\"' && searchstring[i] == '\"')
                    {
                        delimcounter++;
                        if (delimcounter == 2)
                        {
                            break;
                        }
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
