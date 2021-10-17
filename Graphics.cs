using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
namespace XYController
{
    class Graphics
    {
        public static void ApplyOffsetToPointsList(ref List<List<Point>> points, Point offset)
        {
            foreach (List<Point> lp in points)
            {
                for (int i = 0; i < lp.Count; i++)
                {

                    lp[i] = new Point(lp[i].X + offset.X, lp[i].Y + offset.Y);
                   
                }

            }
        }
        public  static void ForceListOfPointsToOrigin(ref List<List<Point>> points)
        {
            float lowestX = float.MaxValue;
            float lowestY = float.MaxValue;
            float highestX = 0;
            float highestY = 0;
            foreach (List<Point> lp in points)
            {
                foreach (Point p in lp)
                {
                    if (p.X < lowestX)
                        lowestX = p.X;
                    if (p.Y < lowestY)
                        lowestY = p.Y;
                    if (p.X > highestX)
                        highestX = p.X;
                    if (p.Y > highestY)
                        highestY = p.Y;
                }

            }

            foreach (List<Point> lp in points)
            {
                for (int i = 0; i < lp.Count; i++)
                {

                    lp[i] = new Point(lp[i].X - lowestX, lp[i].Y - lowestY);

                }

            }

        }

        public static void RawScaleListOfPoints(ref List<List<Point>> points, float dividerX, float dividerY)
        {

            foreach (List<Point> lp in points)
            {
                for (int i = 0; i < lp.Count; i++)
                {

                    lp[i] = new Point(lp[i].X * dividerX, lp[i].Y * dividerY);
                    
                }

            }

        }

        public static void NormalizeListForPlotterDimension(ref List<List<Point>> points, float squaresize)
        {
            // [0] Get the lowest x y 
            float lowestX = float.MaxValue;
            float lowestY = float.MaxValue;
            float highestX = 0;
            float highestY = 0;
            foreach (List<Point> lp in points)
            {
                foreach (Point p in lp)
                {
                    if (p.X < lowestX)
                        lowestX = p.X;
                    if (p.Y < lowestY)
                        lowestY = p.Y;
                    if (p.X > highestX)
                        highestX = p.X;
                    if (p.Y > highestY)
                        highestY = p.Y;
                }

            }
            float diffX = highestX - lowestX;
            float diffY = highestY - lowestY;
            float divider = 1f;
            if (diffX > diffY && diffX > squaresize)
            {
                divider = squaresize / diffX;
            }
            if (diffX < diffY && diffY > squaresize)
            {
                divider = squaresize / diffY;
            }
            foreach (List<Point> lp in points)
            {
                for (int i = 0; i < lp.Count; i++)
                {

                    lp[i] = new Point(lp[i].X - lowestX, lp[i].Y - lowestY);

                }

            }
            RawScaleListOfPoints(ref points, divider, divider);
         
        }

        public static void InvertXListOfPoints(ref List<List<Point>> points, float widthsize)
        {
            foreach (List<Point> lp in points)
            {
                for (int i = 0; i < lp.Count; i++)
                    lp[i] = new Point(widthsize-lp[i].X, lp[i].Y);
            }
        }

        public static void InvertYListOfPoints(ref List<List<Point>> points, float heightsize)
        {
            foreach (List<Point> lp in points)
            {
                for (int i = 0; i < lp.Count; i++)
                    lp[i] = new Point(lp[i].X, heightsize - lp[i].Y);
            }
        }

        public static string GetSVGPathFromFile(string filePath)
        {
            string result = "";
            
            string[] lines = File.ReadAllLines(filePath);
            string[] r = GetLinesBetweenTokens(ref lines, "<path", "/>");
            r = GetLinesBetweenTokens(ref r, "d=\"","\"");
            result = StringArrayToString(ref r);

            return result;
        }
        public static string StringArrayToString(ref string[] lines)
        {
            string result = "";
            for (int i = 0; i < lines.Length; i++)
            {
                result += lines[i];
            }
            return result;
        }

        public static string[] GetLinesBetweenTokens(ref string[] lines,  string tokenA, string tokenB)
        {
            List<string> rList = new List<string>();
            bool ftoken = false;
            for (int i = 0; i < lines.Length; i++)
            {
                if ( lines[i].Contains(tokenA))
                {
                    ftoken = true;
                    lines[i]= lines[i].Replace(tokenA, "");
                }
                if ( ftoken)
                {
                   
                    if (lines[i].Contains(tokenB))
                    {
                        lines[i] = lines[i].Replace(tokenB, "");
                        rList.Add(lines[i]);
                        break;
                    }
                    rList.Add(lines[i]);
                }
            }
            return rList.ToArray();
        }

        public static List<List<Point>> GetPointListsFromSVGPath(string svgpath)
        {
            List<List<Point>> result = new List<List<Point>>();
            SVG.SvgParser svgp = new SVG.SvgParser();
            int pointcount = 0;
            svgp.parsePath(svgpath);
            foreach (List<float[]> f in svgp.originPathList)
            {
                List<Point> segments = new List<Point>();
                
                for (int i = 0; i < f.Count; i++)
                {

                    Point e = new Point(f[i][0], f[i][1]);
                    segments.Add(e);
                   
                    pointcount++;
                   
                }
                result.Add(segments);

            }
            return result;
        }

        public static void SaveBMPfromPointsList(List<List<Point>> points, int width , int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            Pen blackPen = new Pen(Color.Black, 1);
            using (var graphics = System.Drawing.Graphics.FromImage(bmp))
            {
                foreach (List<Point> lp in points)
                {
                    for (int i = 1; i < lp.Count; i++)
                    {
                        Point a = lp[i - 1];
                        Point b = lp[i];
                        graphics.DrawLine(blackPen, a.X,a.Y,b.X, b.Y);
                    }

                }
                
            }
            bmp.Save(DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".png");
        }

    }
}
