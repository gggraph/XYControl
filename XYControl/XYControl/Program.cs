using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYControl
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = SvgParser.GetXYPath("mon fichier.svg");
            path.ForceToOrigin();
            path.Multiply(3);
            path.ApplyOffset(20, 2);
            COM.LoadPointsToWorkQueue(path);
            while (true) { }
        }
    }
}
