using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace XYController
{
    class BMP
    {
        public static int _greaseOffset = 5;
        public static int _minPathSize = 5;

        public static int _BlueTolerance = 50;
        public static int _RedTolerance = 50;
        public static int _GreenTolerance = 50;
        public static int _BlackTolerance = 50;

        public static  List<List<Point>> FastProcessFile(string filePath)
        {
   
            Bitmap bmp = new Bitmap(filePath, true);
            if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
            {
                Console.WriteLine("Cannot proccess indexed color image. Choose another pixel format on your file.");
                return null;
            }

            BitmapToGrayScale(ref bmp);
            FastBMPSave(ref bmp);

            return ProcessImage(ref bmp, Color.Black);
        }
        public static List<List<Point>> ProcessImage(ref Bitmap bmp, Color targetColor)
        {
           
            List<List<Point>> BMPPOINT = new List<List<Point>>();

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    if (CheckPixelColor(bmp.GetPixel(x, y), targetColor) && !isPixelinGlyphesFound(new Point(x, y), BMPPOINT)) //< we need to apply tolerance
                    {
                        BMPPOINT = FloodFill(ref bmp, new Point(x, y), targetColor, BMPPOINT);
                    }
                }
            }
            BMPPOINT = clearNeighboorPoints2(BMPPOINT);//BMPPOINT = clearNeighboorPoints(BMPPOINT);// we can v2 slower but more efficient  ...

            List<List<Point>> Paths = GetPaths(BMPPOINT);

            return Paths;
        }
        public static List<List<Point>> FloodFill(ref Bitmap bmp, Point pt, Color targetColor, List<List<Point>> gl)
        {

            List<Point> selectedPixels = new List<Point>();
            Stack<Point> pixels = new Stack<Point>();
            targetColor = bmp.GetPixel((int)pt.X, (int)pt.Y);
            pixels.Push(pt);

            while (pixels.Count > 0)
            {
                Point a = pixels.Pop();
                if (a.X < bmp.Width && a.X > 0 &&
                        a.Y < bmp.Height && a.Y > 0)//make sure we stay within bounds
                {

                    if (CheckPixelColor(bmp.GetPixel((int)a.X, (int) a.Y), targetColor))//bmp.GetPixel(a.X, a.Y) == targetColor) //< we need to apply tolerance here
                    {
                        bmp.SetPixel((int)a.X, (int)a.Y, Color.White); //< we have to let this ... 
                        selectedPixels.Add(a);
                        pixels.Push(new Point(a.X - 1, a.Y));
                        pixels.Push(new Point(a.X + 1, a.Y));
                        pixels.Push(new Point(a.X, a.Y - 1));
                        pixels.Push(new Point(a.X, a.Y + 1));
                        // check also diag...
                        pixels.Push(new Point(a.X - 1, a.Y + 1));
                        pixels.Push(new Point(a.X - 1, a.Y - 1));
                        pixels.Push(new Point(a.X + 1, a.Y + 1));
                        pixels.Push(new Point(a.X + 1, a.Y - 1));
                    }
                }
            }

            gl.Add(selectedPixels);
            // here it depends the case ... 
            return gl;
        }


        public static List<List<Point>> clearNeighboorPoints(List<List<Point>> gl)
        {
            int Offset = 5;
            for (int i = 0; i < gl.Count; i++)
            {
                List<Point> newOne = new List<Point>();
                List<Point> sorted = gl[i].OrderBy(x => x.X).ToList();
                foreach (Point p in sorted)
                { // maybe i could sort them before... 
                    bool _isneighboor = false;
                    foreach (Point pp in newOne)
                    {
                        // check if pp x is near p x ...
                        if (p.X >= pp.X - Offset && p.X <= pp.X + Offset && p.Y == pp.Y)
                        {
                            _isneighboor = true;
                        }
                    }
                    if (!_isneighboor)
                    {
                        newOne.Add(p);
                    }
                }
                gl[i] = newOne;
                // on supprime tous les x ...

            }
            return gl;
        }

        public static List<List<Point>> clearNeighboorPoints2(List<List<Point>> gl)
        {
            List<List<Point>> hLines = new List<List<Point>>(); // if i put hlines = gl ; this act as a pointer... so i have to create it from scratch 
            List<List<Point>> vLines = new List<List<Point>>();
            for (int i = 0; i < gl.Count; i++)
            {
                List<Point> pp = new List<Point>();
                for (int a = 0; a < gl[i].Count; a++)
                {
                    pp.Add(new Point(gl[i][a].X, gl[i][a].Y));
                }
                hLines.Add(pp);
            }
            for (int i = 0; i < gl.Count; i++)
            {
                List<Point> per = new List<Point>();
                for (int a = 0; a < gl[i].Count; a++)
                {
                    per.Add(new Point(gl[i][a].X, gl[i][a].Y));
                }
                vLines.Add(per);
            }

            int Offset = _greaseOffset;
            for (int i = 0; i < gl.Count; i++)
            {
                List<Point> newOne1 = new List<Point>();
                List<Point> newOne2 = new List<Point>();
                List<Point> sorted1 = gl[i].OrderBy(x => x.X).ToList();
                List<Point> sorted2 = gl[i].OrderBy(x => x.Y).ToList(); //< should be the same length as sorted 1
                for (int a = 0; a < sorted1.Count; a++)
                { // maybe i could sort them before... 
                    bool _isneighboor = false;
                    foreach (Point pp in newOne1)
                    {
                        if (sorted1[a].X >= pp.X - Offset && sorted1[a].X <= pp.X + Offset && sorted1[a].Y == pp.Y)
                        {
                            _isneighboor = true;
                        }
                    }
                    if (!_isneighboor)
                    {
                        newOne1.Add(sorted1[a]);
                    }

                    _isneighboor = false;
                    foreach (Point pp in newOne2)
                    {
                        if (sorted2[a].Y >= pp.Y - Offset && sorted2[a].Y <= pp.Y + Offset && sorted2[a].X == pp.X)
                        {
                            _isneighboor = true;
                        }
                    }
                    if (!_isneighboor)
                    {
                        newOne2.Add(sorted2[a]);
                    }

                }
                vLines[i] = newOne1;
                hLines[i] = newOne2;
                //on supprime tous les x ...

            }

            // ------------------------------------------------------------- REWORK HERE >

            int minPixelNeeded = _minPathSize;
            foreach (List<Point> lp in hLines)
            {
                for (int a = 0; a < lp.Count; a++)
                {
                    List<Point> pixFound = GetNearbyPoint(lp[a], lp); // specific flood fill 4 diag

                    if (pixFound.Count < minPixelNeeded)
                    {
                        for (int i = 0; i < pixFound.Count; i++)
                        {
                            lp.Remove(pixFound[i]); // could do bad things here
                        }
                        //< then delete all pixFound in lp ... 
                    }
                }
            }
            foreach (List<Point> lp in vLines)
            {
                for (int a = 0; a < lp.Count; a++)
                {
                    List<Point> pixFound = GetNearbyPoint(lp[a], lp);
                    if (pixFound.Count < minPixelNeeded)
                    {
                        for (int i = 0; i < pixFound.Count; i++)
                        {
                            lp.Remove(pixFound[i]);
                        }
                        //< then delete all pixFound in lp ... 
                    }
                }
            }
            // < ------------------------------------------------------------- REWORK HERE

            // we have cleaned vLines and hLines. Now we only have to merge both of those array [OK!!!]
            for (int i = 0; i < hLines.Count; i++)
            {
                for (int a = 0; a < vLines[i].Count; a++)
                {
                    if (!hLines[i].Contains(vLines[i][a]))
                    {
                        hLines[i].Add(vLines[i][a]);
                    }
                }
            }

            return hLines;
        }

        public static List<Point> GetNearbyPoint(Point pt, List<Point> mpool)
        {
            List<Point> pool = new List<Point>();
            for (int i = 0; i < mpool.Count; i++)
            {

                pool.Add(new Point(mpool[i].X, mpool[i].Y));
            }
            List<Point> selectedPixels = new List<Point>();
            Stack<Point> pixels = new Stack<Point>();
            pixels.Push(pt);
            while (pixels.Count > 0)
            {
                Point a = pixels.Pop();


                if (pool.Contains(a))
                {
                    pool.Remove(a); //< this actually remove my pool ...
                    selectedPixels.Add(a);
                    pixels.Push(new Point(a.X - 1, a.Y));
                    pixels.Push(new Point(a.X + 1, a.Y));
                    pixels.Push(new Point(a.X, a.Y - 1));
                    pixels.Push(new Point(a.X, a.Y + 1));

                    // check also diag...
                    pixels.Push(new Point(a.X - 1, a.Y + 1));
                    pixels.Push(new Point(a.X - 1, a.Y - 1));
                    pixels.Push(new Point(a.X + 1, a.Y + 1));
                    pixels.Push(new Point(a.X + 1, a.Y - 1));
                }

            }

            // here it depends the case ... 
            return selectedPixels;
        }

        public static List<List<Point>> GetPaths(List<List<Point>> gl) // ça ma l'air detre OK
        {
            // from reduced points . get from random point is nearest point ( not already proccessed ) and create array of points ... 
            // then we have vectors ... we can create GSCAN files ... 
            List<List<Point>> Paths = new List<List<Point>>();
            foreach (List<Point> lp in gl)
            {
                if (lp.Count > 0)
                {
                    List<Point> newPath = new List<Point>();
                    // we will try our best here
                    Point origin = lp[0]; // only works if lp have lines .. 
                    newPath.Add(origin);
                    while (true)
                    {
                        // get lowest distance point from origin. 
                        double lowestdistance = 1000;
                        Point nearestP = new Point(0, 0);
                        bool _found = false;
                        foreach (Point p in lp)
                        {
                            if (p != origin && !newPath.Contains(p))
                            {
                                double distance = GetDistance(p.X, p.Y, origin.X, origin.Y);
                                if (distance < lowestdistance) // DISTANCE MINIMUM REQUISE ! 
                                {
                                    lowestdistance = distance;
                                    nearestP = p;
                                    _found = true;
                                }
                            }

                        }
                        if (_found)
                        {
                            // set new origin 
                            newPath.Add(nearestP);
                            origin = nearestP;

                        }
                        else
                        {
                            break;
                        }

                    }
                    Paths.Add(newPath);
                }

            }

            return Paths;

        }
        public static double GetDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }

        public static bool isPixelinGlyphesFound(Point p, List<List<Point>> gl)
        {
            foreach (List<Point> lp in gl)
            {
                foreach (Point pp in lp)
                {
                    if (p.X == pp.X && p.Y == pp.Y)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool CheckPixelColor(Color pixelColor, Color targetColor)
        {
            int tolerance = 30;
            if (targetColor == Color.Black)
            {
                tolerance = _BlackTolerance;// tolerance = 50; // we are ok for such color // 5 
            }
            else if (targetColor == Color.Red)
            {
                tolerance = _RedTolerance;// tolerance = 50; // we are ok for such color // 5 
            }
            else if (targetColor == Color.Blue)
            {
                tolerance = _BlueTolerance;// tolerance = 50; // we are ok for such color // 5 
            }
            else if (targetColor == Color.Green)
            {
                tolerance = _GreenTolerance;// tolerance = 50; // we are ok for such color // 5 
            }

            if (pixelColor.R >= targetColor.R - tolerance && pixelColor.R <= targetColor.R + tolerance
                && pixelColor.G >= targetColor.G - tolerance && pixelColor.G <= targetColor.G + tolerance
                && pixelColor.B >= targetColor.B - tolerance && pixelColor.B <= targetColor.B + tolerance)
            {
                return true;
            }
            return false;
        }

        public static void BitmapToGrayScale(ref Bitmap bmp)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    Color c = bmp.GetPixel(x, y);
                    // get rgb moy... 
                    int m = c.R + c.G + c.B;
                    m /= 3;
                    Color newColor = Color.FromArgb(m, m, m);
                    bmp.SetPixel(x, y, newColor);
                }
            }
        }


        public static void ChangeBitmapColor(ref Bitmap bmp, Color oldColor, Color newColor, int _tolR, int _tolG, int _tolB)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y  < bmp.Height; y++)
                {
                    Color c = bmp.GetPixel(x,y);
                    if (c.R >= oldColor.R - _tolR && c.R <= oldColor.R + _tolR
                         && c.G >= oldColor.G - _tolG && c.G <= oldColor.G + _tolG
                         && c.B >= oldColor.B - _tolB && c.B <= oldColor.B + _tolB)
                    {
                        // set pixel
                        bmp.SetPixel(x, y, newColor);
                    }
                }
            }
        }

        public static void FastBMPSave(ref Bitmap bmp)
        {
            bmp.Save(DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".png");
        }


    }
}
