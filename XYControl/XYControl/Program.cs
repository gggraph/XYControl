using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;


namespace XYControl
{
    class Program
    {
        static void Main(string[] args)
        {
            //var path = SvgParser.GetXYPath("mon fichier.svg");
            COM.ConnectToPort("COM11", 115200);

            var path = SvgParser.GetXYPath("a.svg");

            path.ApplyOffset(10, 10);
            path.ForceToOrigin();

            foreach ( List<Vector2> segments in path.shapesParts) 
            {
                for(int i = 0; i < segments.Count; i++) 
                {
                    float x = segments[i].X;
                    float y = segments[i].Y;

                    x = x * 5;
                    y = y * 2.5f;

                    segments[i] = new Vector2(x, y);

                }
            }

            path.SaveAsBmp();

            //COM.LoadPointsToWorkQueue(path);



            /*
            List<System.Numerics.Vector2> points = new List<System.Numerics.Vector2>();
            points.Add(new System.Numerics.Vector2(10, 10));
            points.Add(new System.Numerics.Vector2(10, 80));
            points.Add(new System.Numerics.Vector2(80, 80));
            points.Add(new System.Numerics.Vector2(80, 10));

            var shapes = new List<List<System.Numerics.Vector2>>();
            shapes.Add(points);
            XYPath path = new XYPath(shapes);
            */
            COM.LoadPointsToWorkQueue(path);
            while (true) { }
        }
    }
}
