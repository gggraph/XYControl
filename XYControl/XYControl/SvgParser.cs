using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows;
using System.Numerics;
namespace XYControl
{
    public class SvgParser
    {
        public static List<float[]> buildBezierSegment(float[] p0, float[] p1, float[] p2, float[] p3)
        {
            List<float[]> segList = new List<float[]>();
            float px = p0[0];
            float py = p0[1];
            for (int i = 1; i < 100; i++)
            {
                float ratio = (float)(i) / 100;
                float x00 = p0[0] + (p1[0] - p0[0]) * ratio; float y00 = p0[1] + (p1[1] - p0[1]) * ratio;
                float x01 = p1[0] + (p2[0] - p1[0]) * ratio; float y01 = p1[1] + (p2[1] - p1[1]) * ratio;
                float x02 = p2[0] + (p3[0] - p2[0]) * ratio; float y02 = p2[1] + (p3[1] - p2[1]) * ratio;
                float x10 = (x01 - x00) * ratio + x00;
                float y10 = (y01 - y00) * ratio + y00;
                float x11 = (x02 - x01) * ratio + x01;
                float y11 = (y02 - y01) * ratio + y01;
                float x20 = (x11 - x10) * ratio + x10;
                float y20 = (y11 - y10) * ratio + y10;
                float dx = x20 - px;
                float dy = y20 - py;
                float dis = (float)Math.Sqrt(dx * dx + dy * dy);
                if (dis > 1)
                {
                    segList.Add(new float[2] { x20, y20 });
                    px = x20; py = y20;
                }
            }
            if (segList.Count == 0)
            {
                segList.Add(new float[2] { p3[0], p3[1] });
            }
            return segList;
        }
        public static List<float[]> buildQuadraticBezierSegment(float[] p0, float[] p1, float[] p2)
        {
            List<float[]> segList = new List<float[]>();
            float currentSegmentX = p0[0];
            float currentSegmentY = p0[1];
            for (int i = 1; i < 100; i++)
            {
                float t = (float)(i) / 100;
                float curvePointX = (1 - t) * (1 - t) * p0[0] + 2 * (1 - t) * t * p1[0] + t * t * p2[0];
                float curvePointY = (1 - t) * (1 - t) * p0[1] + 2 * (1 - t) * t * p1[1] + t * t * p2[1];
                float dis = (float)Math.Sqrt(Math.Pow((curvePointX - currentSegmentX), 2) + Math.Pow((curvePointY - currentSegmentY), 2));
                if (dis > 1)
                {
                    segList.Add(new float[2] { curvePointX, curvePointY });
                    currentSegmentX = curvePointX;
                    currentSegmentY = curvePointY;
                }

            }
            if (segList.Count == 0)
            {
                segList.Add(new float[2] { p2[0], p2[1] });
            }
            return segList;
        }
        public static List<float[]> buildArcSegment(float rx, float ry, float phi, int fA, int fS, float x1, float y1, float x2, float y2)
        {
            List<float[]> segList = new List<float[]>();
            phi = phi / 180 * (float)Math.PI;
            float x1p = (float)Math.Cos(phi) * (x1 - x2) / 2 + (float)Math.Sin(phi) * (y1 - y2) / 2;
            float y1p = (float)-Math.Sin(phi) * (x1 - x2) / 2 + (float)Math.Cos(phi) * (y1 - y2) / 2;
            float lam = x1p * x1p / (rx * rx) + y1p * y1p / (ry * ry);
            if (lam > 1)
            {
                rx = (float)Math.Sqrt(lam) * rx;
                ry = (float)Math.Sqrt(lam) * ry;
            }
            float tmp = (rx * rx * ry * ry - rx * rx * y1p * y1p - ry * ry * x1p * x1p) / (rx * rx * y1p * y1p + ry * ry * x1p * x1p);
            float st = (float)Math.Sqrt(Math.Round(tmp, 5));
            float cp_sign;
            if (fA == fS)
            {
                cp_sign = -1;
            }
            else
            {
                cp_sign = 1;
            }

            float cxp = cp_sign * (st * rx * y1p / ry);
            float cyp = cp_sign * (-st * ry * x1p / rx);
            float cx = (float)Math.Cos(phi) * cxp - (float)Math.Sin(phi) * cyp + (x1 + x2) / 2;
            float cy = (float)Math.Sin(phi) * cxp + (float)Math.Cos(phi) * cyp + (y1 + y2) / 2;

            float Vxc = (x1p - cxp) / rx;
            float Vyc = (y1p - cyp) / ry;
            Vxc = (x1p - cxp) / rx;
            Vyc = (y1p - cyp) / ry;
            float Vxcp = (-x1p - cxp) / rx;
            float Vycp = (-y1p - cyp) / ry;

            if (Vyc >= 0) cp_sign = 1;
            else cp_sign = -1;

            float th1 = cp_sign * (float)Math.Acos(Vxc / (float)Math.Sqrt(Vxc * Vxc + Vyc * Vyc)) / (float)Math.PI * 180;
            if ((Vxc * Vycp - Vyc * Vxcp) >= 0) cp_sign = 1;
            else cp_sign = -1;
            tmp = (Vxc * Vxcp + Vyc * Vycp) / ((float)Math.Sqrt(Vxc * Vxc + Vyc * Vyc) * (float)Math.Sqrt(Vxcp * Vxcp + Vycp * Vycp));
            float dth = cp_sign * (float)Math.Acos((float)Math.Round(tmp, 3)) / (float)Math.PI * 180;

            if (fS == 0 && dth > 0) dth -= 360;
            if (fS >= 1 && dth < 0) dth += 360;

            float theta = th1 / 180 * (float)Math.PI;
            float px = rx * (float)Math.Cos(theta) + cx;
            float py = ry * (float)Math.Sin(theta) + cy;
            for (int i = 1; i < 101; i++)
            {
                float ratio = (float)i / 100;
                theta = (th1 + dth * ratio) / 180 * (float)Math.PI;
                float x = (float)Math.Cos(phi) * rx * (float)Math.Cos(theta) - (float)Math.Sin(phi) * ry * (float)Math.Sin(theta) + cx;
                float y = (float)Math.Sin(phi) * rx * (float)Math.Cos(theta) + (float)Math.Cos(phi) * ry * (float)Math.Sin(theta) + cy;
                float dx = x - px; float dy = y - py;
                float dis = (float)Math.Sqrt(dx * dx + dy * dy);
                if (dis > 1)
                {
                    segList.Add(new float[2] { x, y });
                    px = x;
                    py = y;
                }
            }
            return segList;
        }

        public float xbias { get; set; }
        public float ybias { get; set; }
        public List<float[]> tf { get; set; }
        public List<List<float[]>> originPathList { get; set; }
        public List<List<float[]>> rawPathTest { get; set; }
        
        public static XYPath GetXYPath(string filePath)
        {
           
            List<string> svgPaths = new List<string>();
            using (XmlReader reader = XmlReader.Create(filePath)) 
            {
                while (reader.Read()) 
                {
                   
                    if (reader.Name.Equals("path")) 
                        svgPaths.Add(reader.GetAttribute("d"));
                }
                //reader.ReadToFollowing("path");
                //svgPath = reader.GetAttribute("d");
            }
            XYPath result = new XYPath(new List<List<Vector2>>());
            foreach ( string s in svgPaths) 
            {
                SvgParser parser = new SvgParser();
                parser.parsePath(s);
                int pointcount = 0;
                List<List<Vector2>> pathes = new List<List<Vector2>>();
                foreach (List<float[]> f in parser.originPathList)
                {
                    List<Vector2> segments = new List<Vector2>();

                    for (int i = 0; i < f.Count; i++)
                    {
                        Vector2 e = new Vector2(f[i][0], f[i][1]);
                        segments.Add(e);
                        pointcount++;

                    }
                    pathes.Add(segments);
                }
                result.Concat(pathes);
            }



            return result; 

        }
        public SvgParser()
        {
            this.xbias = 0;
            this.ybias = 0;
            this.tf = new List<float[]>();
            this.originPathList = new List<List<float[]>>();
            this.rawPathTest = new List<List<float[]>>();
        }

        private void lineTo(float x, float y)
        {
            this.rawPathTest[rawPathTest.Count - 1].Add(new float[2] { x, y });
            for (int i = 0; i < this.tf.Count; i++)
            {

                // tf weird shit here 
                float[] tf = this.tf[-1 - i]; // wtf ??
                float x1 = tf[0] * x + tf[2] * y + tf[4];
                float y1 = tf[1] * x + tf[3] * y + tf[5];
                x = x1;
                y = y1;
            }
            float[] point = new float[2] { x, y };
            this.originPathList[originPathList.Count - 1].Add(point);

        }
        private void moveTo(float x, float y)
        {
            this.rawPathTest.Add(new List<float[]>());
            this.rawPathTest[rawPathTest.Count - 1].Add(new float[2] { x, y });
            for (int i = 0; i < this.tf.Count; i++)
            {

                // tf weird shit here 
                float[] tf = this.tf[-1 - i]; // wtf ??
                float x1 = tf[0] * x + tf[2] * y + tf[4];
                float y1 = tf[1] * x + tf[3] * y + tf[5];
                x = x1;
                y = y1;
            }
            float[] initpoint = new float[2] { x, y };
            this.originPathList.Add(new List<float[]>());
            this.originPathList[originPathList.Count - 1].Add(initpoint);
        }
        private void parsePath(string path)
        {
            List<float[]> pbuff = new List<float[]>();
            path = path
                .Replace("e-", "ee")
                .Replace("-", " -")
                .Replace("s", " s ")
                .Replace("S", " S ")
                .Replace("c", " c ")
                .Replace("C", " C ")
                .Replace("v", " v ")
                .Replace("V", " V ")
                .Replace("l", " l ")
                .Replace("L", " L ")
                .Replace("A", " A ")
                .Replace("a", " a ")
                .Replace(",", " ")
                .Replace("M", " M ")
                .Replace("h", " h ")
                .Replace("H", " H ")
                .Replace("m", " m ")
                .Replace("z", " z ")
                .Replace("q", " q ")
                .Replace("Q", " Q ");

            string[] ss = path.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < ss.Length; i++)
                ss[i] = ss[i].Replace("ee", "e-").Replace(".",",");
            int ptr = 0;
            string state = "";
            string prevstate = "";
            float curvecnt = 0;
            float[] lastControl = new float[2];
            float x = this.xbias;
            float y = this.ybias;
            float x0 = x; // some initial variable to backup?
            float y0 = y; // some initial variable to backup?
           
            while (ptr < ss.Length)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(ss[ptr], @"^[a-zA-Z]+$"))
                {
                    prevstate = state;
                    state = ss[ptr];
                    ptr += 1;
                    curvecnt = 0;
                    if (state == "C" || state == "c" || state == "Q" || state == "q")
                    {
                        pbuff = new List<float[]>() { new float[2] { x, y } };
                    }
                    if (state == "z" || state == "Z")
                    {
                        x = x0;
                        y = y0;
                        this.lineTo(x0, y0);
                    }
                    if (state == "s" || state == "S")
                    {
                        pbuff = new List<float[]>() { new float[2] { x, y } };
                    }
                }
                else
                {
                    //Console.WriteLine("parsing : " + ss[ptr]);
                    bool _uknw = true;
                    switch (state)
                    {
                        case "h":
                            _uknw = false;
                            float dis = float.Parse(ss[ptr]);
                            this.lineTo(x + dis, y);
                            x = x + dis; y = y;
                            ptr++;
                            break;

                        case "H":
                            _uknw = false;
                            dis = float.Parse(ss[ptr]);
                            this.lineTo(dis, y);
                            x = dis; y = y;
                            ptr++;
                            break;
                        case "v":
                            _uknw = false;
                            dis = float.Parse(ss[ptr]);
                            this.lineTo(x, y + dis);
                            x = x; y = y + dis;
                            ptr++;
                            break;
                        case "V":
                            _uknw = false;
                            dis = float.Parse(ss[ptr]);
                            this.lineTo(x, dis);
                            x = x; y = dis;
                            ptr++;
                            break;
                        case "M":
                            _uknw = false;
                            float ax = float.Parse(ss[ptr]) + this.xbias;
                            float ay = float.Parse(ss[ptr + 1]) + this.ybias;
                            ptr += 2;
                            curvecnt++;
                            x = ax; y = ay;
                            if (curvecnt > 1) { this.lineTo(x, y); }
                            else { this.moveTo(x, y); x0 = x; y0 = y; }
                            break;
                        case "m":

                            _uknw = false;
                            float dx = float.Parse(ss[ptr]);
                            float dy = float.Parse(ss[ptr + 1]);
                            ptr += 2;
                            x = x + dx; y = y + dy;
                            curvecnt++;
                            if (curvecnt > 1) { this.lineTo(x, y); }
                            else { this.moveTo(x, y); x0 = x; y0 = y; }
                            break;
                      
                        case "a":
                            _uknw = false;
                            float rx = float.Parse(ss[ptr]);
                            float ry = float.Parse(ss[ptr + 1]);
                            float phi = float.Parse(ss[ptr + 2]);
                            int fA = int.Parse(ss[ptr + 3]);
                            int fS = int.Parse(ss[ptr + 4]);
                            float px = float.Parse(ss[ptr + 5]) + x;
                            float py = float.Parse(ss[ptr + 6]) + y;
                            ptr += 7;
                            List<float[]> arcSeg = buildArcSegment(rx, ry, phi, fA, fS, x, y, px, py);
                            foreach (float[] s in arcSeg)
                            {
                                this.lineTo(s[0], s[1]);
                            }
                            x = px; y = py;
                            break;
                        case "A":
                            _uknw = false;
                            rx = float.Parse(ss[ptr]);
                            ry = float.Parse(ss[ptr + 1]);
                            phi = float.Parse(ss[ptr + 2]);
                            fA = int.Parse(ss[ptr + 3]);
                            fS = int.Parse(ss[ptr + 4]);
                            px = float.Parse(ss[ptr + 5]) + xbias;
                            py = float.Parse(ss[ptr + 6]) + ybias;
                            ptr += 7;
                            arcSeg = buildArcSegment(rx, ry, phi, fA, fS, x, y, px, py);
                            foreach (float[] s in arcSeg)
                            {
                                this.lineTo(s[0], s[1]);
                            }
                            x = px; y = py;
                            break;

                        case "c":
                            _uknw = false;
                            dx = float.Parse(ss[ptr]);
                            dy = float.Parse(ss[ptr + 1]);
                            pbuff.Add(new float[2] { x + dx, y + dy });
                            ptr += 2;
                            curvecnt++;
                            if (curvecnt == 3)
                            {
                                List<float[]> bzseg = buildBezierSegment(pbuff[0], pbuff[1], pbuff[2], pbuff[3]);
                                lastControl = pbuff[2];
                                foreach (float[] s in bzseg)
                                {
                                    this.lineTo(s[0], s[1]);
                                }
                                x = x + dx; y = y + dy;
                                pbuff = new List<float[]>(){new float[2] { x, y }};
                                //pbuff.Add();
                                curvecnt = 0;

                            }
                            break;

                        case "C":
                            _uknw = false;
                            ax = float.Parse(ss[ptr]) + this.xbias;
                            ay = float.Parse(ss[ptr + 1]) + this.ybias;
                            pbuff.Add(new float[2] { ax, ay });
                            ptr += 2;
                            curvecnt++;
                            if (curvecnt == 3)
                            {
                                List<float[]> bzseg = buildBezierSegment(pbuff[0], pbuff[1], pbuff[2], pbuff[3]);
                                lastControl = pbuff[2];
                                foreach (float[] s in bzseg)
                                {
                                    this.lineTo(s[0], s[1]);
                                }
                                x = ax; y = ay;
                                pbuff = new List<float[]>();
                                pbuff.Add(new float[2] { ax, ay });
                                curvecnt = 0;

                            }
                            break;
                        case "q":
                            _uknw = false;
                            dx = float.Parse(ss[ptr]);
                            dy = float.Parse(ss[ptr + 1]);
                            pbuff.Add(new float[2] { x + dx, y + dy });
                            ptr += 2;
                            curvecnt++;
                            if (curvecnt == 2)
                            {
                                List<float[]> bzseg = buildQuadraticBezierSegment(pbuff[0], pbuff[1], pbuff[2]);
                                lastControl = pbuff[1];
                                foreach (float[] s in bzseg)
                                {
                                    this.lineTo(s[0], s[1]);
                                }
                                x = x + dx; y = y + dy;
                                pbuff = new List<float[]>();
                                pbuff.Add(new float[2] { x, y });
                                curvecnt = 0;

                            }
                            break;
                        case "Q":
                            _uknw = false;
                            ax = float.Parse(ss[ptr]) + this.xbias;
                            ay = float.Parse(ss[ptr + 1]) + this.ybias;
                            pbuff.Add(new float[2] { ax, ay });
                            ptr += 2;
                            curvecnt++;
                            if (curvecnt == 2)
                            {
                                List<float[]> bzseg = buildQuadraticBezierSegment(pbuff[0], pbuff[1], pbuff[2]);
                                lastControl = pbuff[1];
                                foreach (float[] s in bzseg)
                                {
                                    this.lineTo(s[0], s[1]);
                                }
                                x = ax; y = ay;
                                pbuff = new List<float[]>();
                                pbuff.Add(new float[2] { ax, ay });
                                curvecnt = 0;

                            }
                            break;
                        case "s":
                            _uknw = false;
                            dx = float.Parse(ss[ptr]);
                            dy = float.Parse(ss[ptr + 1]);
                            ptr += 2;
                            curvecnt++;
                            if (curvecnt == 1)
                            {
                                if (prevstate == "S" || prevstate == "s" || prevstate == "C" || prevstate == "c")
                                {
                                    float[] controlPoint = new float[2] { 2 * pbuff[0][0] - lastControl[0], 2 * pbuff[0][1] - lastControl[1] };
                                    pbuff.Add(controlPoint);
                                    pbuff.Add(new float[2] { x + dx, y + dy });

                                }
                                else
                                {
                                    pbuff.Add(pbuff[0]);
                                    pbuff.Add(new float[2] { x + dx, y + dy });

                                }

                            }
                            if (curvecnt == 2)
                            {
                                pbuff.Add(new float[2] { x + dx, y + dy });
                                List<float[]> bzseg = buildBezierSegment(pbuff[0], pbuff[1], pbuff[2], pbuff[3]);
                                lastControl = pbuff[2];
                                foreach (float[] s in bzseg)
                                {
                                    this.lineTo(s[0], s[1]);
                                }
                                x = x + dx;
                                y = y + dy;
                                pbuff = new List<float[]>();
                                pbuff.Add(new float[2] { x, y });
                                curvecnt = 0;
                            }

                            break;
                        case "S":
                            _uknw = false;
                            ax = float.Parse(ss[ptr]) + this.xbias;
                            ay = float.Parse(ss[ptr + 1]) + this.ybias;
                            ptr += 2;
                            curvecnt++;
                            if (curvecnt == 1)
                            {
                                if (prevstate == "S" || prevstate == "s" || prevstate == "C" || prevstate == "c")
                                {
                                    float[] controlPoint = new float[2] { 2 * pbuff[0][0] - lastControl[0], 2 * pbuff[0][1] - lastControl[1] };
                                    pbuff.Add(controlPoint);
                                    pbuff.Add(new float[2] { ax, ay });

                                }
                                else
                                {
                                    pbuff.Add(pbuff[0]);
                                    pbuff.Add(new float[2] { ax, ay });

                                }

                            }
                            if (curvecnt == 2)
                            {
                                pbuff.Add(new float[2] { ax, ay });
                                List<float[]> bzseg = buildBezierSegment(pbuff[0], pbuff[1], pbuff[2], pbuff[3]);
                                lastControl = pbuff[2];
                                foreach (float[] s in bzseg)
                                {
                                    this.lineTo(s[0], s[1]);
                                }
                                x = ax;
                                y = ay;
                                pbuff = new List<float[]>();
                                pbuff.Add(new float[2] { x, y });
                                curvecnt = 0;
                            }

                            break;
                        case "l":
                            _uknw = false;
                            dx = float.Parse(ss[ptr]);
                            dy = float.Parse(ss[ptr + 1]);
                            ptr += 2;
                            curvecnt++;
                            x = x + dx;
                            y = y + dy;
                            this.lineTo(x, y);
                            break;
                        case "L":
                            _uknw = false;
                            ax = float.Parse(ss[ptr]) + this.xbias;
                            ay = float.Parse(ss[ptr + 1]) + this.ybias;
                            ptr += 2;
                            curvecnt++;
                            x = ax;
                            y = ay;
                            this.lineTo(x, y);
                            break;



                    }
                    if (_uknw)
                    {
                        ptr++;
                        Console.WriteLine("unknown state : " + state);

                    }


                }

            }
        }

    }
}
