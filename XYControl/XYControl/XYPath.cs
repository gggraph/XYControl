using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace XYControl
{
    public class XYPath
    {
   
        public List<List<Vector2>> shapesParts;
        public XYPath(List<List<Vector2>> pathes) 
        {
            this.shapesParts = pathes;
        }

        public void ApplyOffset(float x, float y) 
        {
            foreach (List<Vector2> lp in shapesParts)
            {
                for (int i = 0; i < lp.Count; i++)
                    lp[i] = new Vector2(lp[i].X + x, lp[i].Y + y);
            }
        }

        public int lineCount() 
        {
            int r = 0;
            foreach (List<Vector2> lp in shapesParts)
                r += lp.Count;
            return r;
        }
        public void ForceToOrigin()
        {
            float lowestX = float.MaxValue;
            float lowestY = float.MaxValue;
            float highestX = 0;
            float highestY = 0;
            foreach (List<Vector2> lp in shapesParts)
            {
                foreach (Vector2 p in lp)
                {
                    lowestX = Math.Min(lowestX, p.X);
                    lowestY = Math.Min(lowestY, p.Y);
                    highestX = Math.Max(highestX, p.X);
                    highestY = Math.Max(highestY, p.Y);
                }
            }
            foreach (List<Vector2> lp in shapesParts)
            {
                for (int i = 0; i < lp.Count; i++)
                    lp[i] = new Vector2(lp[i].X - lowestX, lp[i].Y - lowestY);
            }
        }

        public void Scale(float x, float y) 
        {
            foreach (List<Vector2> lp in shapesParts)
            {
                for (int i = 0; i < lp.Count; i++)
                    lp[i] = new Vector2(lp[i].X * x, lp[i].Y * y);
            }
        }
        public void Multiply(float multiplier)
        {
            foreach (List<Vector2> segments in shapesParts)
            {
                for (int i = 0; i < segments.Count; i++)
                    segments[i] = new Vector2(segments[i].X * multiplier, segments[i].Y * multiplier);

            }
        }

        public void FitToDimension(int w, int h) 
        {
            // [0] Get the lowest x y 
            float lowestX = float.MaxValue;
            float lowestY = float.MaxValue;
            float highestX = float.MinValue;
            float highestY = float.MinValue;
            foreach (List<Vector2> segments in shapesParts)
            {
                foreach (Vector2 p in segments)
                {
                    lowestX = Math.Min(lowestX, p.X);
                    lowestY = Math.Min(lowestY, p.Y);
                    highestX = Math.Max(highestX, p.X);
                    highestY = Math.Max(highestY, p.Y);
                }
            }
            float xScale = (float)w / (highestX - lowestX);
            float yScale = (float)h / (highestY - lowestY);
            foreach (List<Vector2> segments in shapesParts)
            {
                for (int i = 0; i < segments.Count; i++)
                {
                    segments[i] = new Vector2((segments[i].X - lowestX) *xScale, 
                        (segments[i].Y - lowestY)*yScale);
                }
            }
        }
        public void FlipX() 
        {
            float lowestX = float.MaxValue;
            float highestX = float.MinValue;
            foreach (List<Vector2> segments in shapesParts)
            {
                foreach (Vector2 p in segments)
                {
                    lowestX = Math.Min(lowestX, p.X);
                    highestX = Math.Max(highestX, p.X);
                }
            }
            float width = highestX - lowestX;
            foreach (List<Vector2> segments in shapesParts)
            {
                for (int i = 0; i < segments.Count; i++)
                    segments[i] = new Vector2(width-segments[i].X,segments[i].Y);
            }
        }
        public void FlipY()
        {
            float lowestY = float.MaxValue;
            float highestY = float.MinValue;
            foreach (List<Vector2> segments in shapesParts)
            {
                foreach (Vector2 p in segments)
                {
                    lowestY = Math.Min(lowestY, p.Y);
                    highestY = Math.Max(highestY, p.Y);
                }
            }
            float height = highestY - lowestY;
            foreach (List<Vector2> segments in shapesParts)
            {
                for (int i = 0; i < segments.Count; i++)
                    segments[i] = new Vector2(segments[i].X, height-segments[i].Y);
            }
        }
        public void Concat(XYPath otherPath) 
        {
            foreach (List<Vector2> segments in otherPath.shapesParts)
                shapesParts.Add(segments);
        }
        public void Concat(List<List<Vector2>> shapesParts)
        {
            foreach (List<Vector2> segments in shapesParts)
                this.shapesParts.Add(segments);
        }

        public void SaveAsBmp()
        {
            float highestX = float.MinValue;
            float highestY = float.MinValue;
            foreach (List<Vector2> segments in shapesParts)
            {
                foreach (Vector2 p in segments)
                {
                    highestX = Math.Max(highestX, p.X);
                    highestY = Math.Max(highestY, p.Y);
                }
            }
            int width = (int)highestX;
            int height = (int)highestY;
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(width, height);
            System.Drawing.Pen blackPen = new System.Drawing.Pen(System.Drawing.Color.Black, 1);
            using (var graphics = System.Drawing.Graphics.FromImage(bmp))
            {
                foreach (List<Vector2> segments in shapesParts)
                {
                    for (int i = 1; i < segments.Count; i++)
                        graphics.DrawLine(blackPen, segments[i - 1].X, segments[i - 1].Y, segments[i].X, segments[i].Y);
                }

            }
            bmp.Save(DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".png");
        }

    }
}
