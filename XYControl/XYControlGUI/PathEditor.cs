using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using XYControl;
using System.IO;
using System.Numerics;

namespace XYControlGUI
{
    public class PathEditor
    {

        public const int VIEWSIZE = 500;
        public const int IMPORTSIZE = 200;
        public int PlotterDimension = 100;
        // so the factor is like PlotterDimension/VIEWSIZE

        public class Bounds
        {
            public float x;
            public float y;
            public float w;
            public float h;
            public Bounds(float x, float y, float w, float h)
            {
                this.x = x;
                this.y = y;
                this.w = w;
                this.h = h;
            }
        }

        public class XYObjectPath
        {
            public XYPath shape;
            public Bounds bounds;
            // Dimension...
            public float scale = 1f;
            public float scaleX = 1f;
            public float scaleY = 1f;

            public Point mousePositionAtSelection;
            public Bounds boundsAtSelection;

            public bool isResizing = false;
            public bool isReScaling = false;
            public XYObjectPath(XYPath shape, Bounds bounds)
            {
                this.shape = shape;
                this.bounds = bounds;
            }

            public bool IsPointInsideBounds(float x, float y) 
            {
                // Point X/Y can be multiplied
                float scaleBoundX = bounds.x +  bounds.w * scale * scaleX;
                float scaleBoundY = bounds.y +  bounds.h * scale * scaleY;

                float leftx = Math.Min(bounds.x, scaleBoundX);
                float rightx = Math.Max(bounds.x, scaleBoundX);
                float lefty = Math.Min(bounds.y, scaleBoundY);
                float righty = Math.Max(bounds.y, scaleBoundY);

                return (x >= leftx && x <= rightx && y >= lefty && y <= righty);
                /*
                return (x >= bounds.x && x <= (bounds.x + bounds.w) &&
                        y >= bounds.y && y <= (bounds.y + bounds.h) );
                */
            }
            public void Scale(float x, float y)
            {
                scale = 1f;
                scaleX *= x;
                scaleY *= y;
            }
            public void Multiply(float multiplier)
            {
                scale = 1f;
                scaleX *= multiplier;
                scaleY *= multiplier;
            }

            public void FlipX()
            {
                shape.FlipX();
               
            }
            public void FlipY()
            {
                shape.FlipY();
            }


        }
        public Form1 mForm;
        public Panel mPanel;
        public PictureBox viewBox;
        public List<XYObjectPath> objects = new List<XYObjectPath>();

        public XYObjectPath selectedObject = null;

        public bool mouseDown = false;
        public bool shiftPressed = false;
        public bool ctrlPressed = false;
        public PathEditor(Form1 form, Panel editorPanel )
        {
            mForm = form;
            mPanel = editorPanel;
            InitView();
        }
        public void InitView()
        {
            viewBox = new PictureBox();
            mPanel.Controls.Add(viewBox);
            mPanel.AllowDrop = true;
            mPanel.DragEnter += mPanel_DragEnter;
            mPanel.DragDrop += mPanel_DragDrop;

            mForm.KeyDown += MForm_KeyDown;
            mForm.KeyUp += MForm_KeyUp;

            viewBox.MouseDown += ViewBox_MouseDown;
            viewBox.MouseMove += ViewBox_MouseMove;
            viewBox.MouseUp += ViewBox_MouseUp;

            

            Bitmap bmp = new Bitmap(500, 500);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                Rectangle ImageSize = new Rectangle(0, 0, bmp.Width, bmp.Height);
                g.FillRectangle(Brushes.Black, ImageSize);
            }
            viewBox.Size = new Size(bmp.Width, bmp.Height);
            viewBox.Image = bmp;
        }



        public void ClearView()
        {
            using (Graphics g = Graphics.FromImage(viewBox.Image))
            {
                Rectangle ImageSize = new Rectangle(0, 0, viewBox.Width, viewBox.Height);
                g.FillRectangle(Brushes.Black, ImageSize);
            }
        }
        public void RefreshView()
        {
            ClearView();
            DrawXYObject(objects);
            HighLightSelectedObject();
            viewBox.Invalidate();
        }
        public void HighLightSelectedObject()
        {
            if (selectedObject == null)
                return;
            using (Graphics g = Graphics.FromImage(viewBox.Image))
            {

                float scaleBoundX = selectedObject.bounds.x 
                    + selectedObject.bounds.w * selectedObject.scale * selectedObject.scaleX;
                float scaleBoundY = selectedObject.bounds.y + selectedObject.bounds.h * selectedObject.scale * selectedObject.scaleY;

                float leftx = Math.Min(selectedObject.bounds.x, scaleBoundX);
                float rightx = Math.Max(selectedObject.bounds.x, scaleBoundX);
                float lefty = Math.Min(selectedObject.bounds.y, scaleBoundY);
                float righty = Math.Max(selectedObject.bounds.y, scaleBoundY);

                Pen p = new Pen(Brushes.Red);
                g.DrawRectangle(p,
                   new Rectangle(
                       (int)leftx,
                       (int)lefty,
                       (int)(rightx-leftx),
                       (int)(righty-lefty)));
                /*
                g.DrawRectangle(p,
                    new Rectangle(
                        (int)selectedObject.bounds.x,
                        (int)selectedObject.bounds.y,
                        (int)(selectedObject.bounds.w * selectedObject.scale * selectedObject.scaleX),
                        (int)(selectedObject.bounds.h * selectedObject.scale * selectedObject.scaleY)));*/
            }
        }

        public XYPath DumpFullPath() 
        {
            // Manipulating other list will conflit with 
            XYPath path = new XYPath(new List<List<Vector2>>());
            foreach(XYObjectPath ob in objects) 
            {
                List<List<Vector2>> shapesParts = new List<List<Vector2>>();
                foreach(List<Vector2> segments in ob.shape.shapesParts) 
                {
                    Vector2[] cp = new Vector2[segments.Count];
                    segments.CopyTo(cp);
                    shapesParts.Add(cp.ToList());
                }
                XYPath currentPath = new XYPath(shapesParts);
              
                currentPath.ApplyOffset(ob.bounds.x, ob.bounds.y);
                currentPath.Scale(1f / ob.scaleX, 1f / ob.scaleY);
                currentPath.Multiply(1f / ob.scale);
                path.Concat(currentPath);
            }
            // We can also 
            //path.FlipY();
            float upDim = (float)PlotterDimension / (float)VIEWSIZE;
            Console.WriteLine("set size:" + upDim);
            path.Multiply(upDim);
            return path;
        }
        public void DrawXYObject(List<XYObjectPath> objects)
        {
            using (Graphics g = Graphics.FromImage(viewBox.Image))
            {
                Pen p = new Pen(Brushes.White);
                foreach (XYObjectPath ob in objects)
                {
                    foreach (List<Vector2> segment in ob.shape.shapesParts)
                    {
                        for (int i = 0; i < segment.Count() - 1; i++)
                        {
                            // Console.WriteLine($"X{segment[i].X} Y{segment[i].Y}");
                            g.DrawLine(p,
                            new Point(
                            (int)(segment[i].X * ob.scale * ob.scaleX + ob.bounds.x), 
                            (int)(segment[i].Y * ob.scale * ob.scaleY + ob.bounds.y)
                            ),
                            new Point(
                            (int)(segment[i + 1].X * ob.scale * ob.scaleX + ob.bounds.x), 
                            (int)(segment[i + 1].Y * ob.scale * ob.scaleY + ob.bounds.y))
                            );
                        }
                    }
                }
            }
        }
        public void DrawXYObject(XYObjectPath xyObject)
        {
            using (Graphics g = Graphics.FromImage(viewBox.Image))
            {
                Pen p = new Pen(Brushes.White);
                foreach (List<Vector2> segment in xyObject.shape.shapesParts)
                {
                    for (int i = 0; i < segment.Count() - 1; i++)
                    {
                        // Console.WriteLine($"X{segment[i].X} Y{segment[i].Y}");

                        g.DrawLine(p,
                           new Point(
                           (int)(segment[i].X * xyObject.scale * xyObject.scaleX + xyObject.bounds.x),
                           (int)(segment[i].Y * xyObject.scale * xyObject.scaleY + xyObject.bounds.y)
                           ),
                           new Point(
                           (int)(segment[i + 1].X * xyObject.scale * xyObject.scaleX + xyObject.bounds.x),
                           (int)(segment[i + 1].Y * xyObject.scale * xyObject.scaleY + xyObject.bounds.y))
                           );
                    }
                }
            }
        }
        public XYObjectPath CreateXYObjectFromImage(string imagePath, Point origin)
        {
           string filePath = PoTrace.BuildSVGFromImage(
                            Environment.CurrentDirectory,
                            imagePath,
                            Path.Combine(Environment.CurrentDirectory, "temp"));

            XYControl.XYPath path = XYControl.SvgParser.GetXYPath(filePath);
            path.ForceToOrigin();
            path.FitToDimension(200, 200);
            //path.ApplyOffset(origin.X, origin.Y); // To Apply After Building Path
            XYObjectPath ob = new XYObjectPath(path, new Bounds(origin.X, origin.Y, 200, 200));
            //DrawXYObject(ob);
            objects.Add(ob);
            selectedObject = ob;
            RefreshView();
            return ob;
        }
        public XYObjectPath CreateXYObjectFromSVG(string filePath, Point origin)
        {
            XYControl.XYPath path = XYControl.SvgParser.GetXYPath(filePath);
            path.ForceToOrigin();
            path.FitToDimension(200, 200);
            //path.ApplyOffset(origin.X, origin.Y); // To Apply After Building Path
            XYObjectPath ob = new XYObjectPath(path, new Bounds(origin.X, origin.Y, 200, 200));
            //DrawXYObject(ob);
            objects.Add(ob);
            selectedObject = ob;
            RefreshView();
            return ob;
        }
        public XYObjectPath FindObjectAtPosition(int x, int y)
        {
            foreach (XYObjectPath ob in objects)
            {
                if (ob.IsPointInsideBounds(x,y))
                    return ob;
            }
            return null;
        }
        private void mPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void mPanel_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string filePath in files)
                {
                    if (filePath.EndsWith(".svg"))
                    {
                        Point dropPoint = mPanel.PointToClient(new Point(e.X, e.Y));
                        CreateXYObjectFromSVG(filePath, dropPoint);
                        return;
                    }
                    if (File.Exists("potrace.exe"))
                    {
                        
                        Point dropPoint = mPanel.PointToClient(new Point(e.X, e.Y));
                        CreateXYObjectFromImage(filePath, dropPoint);
                    }
                }
            }
        }

        private void ViewBox_MouseDown(object sender, MouseEventArgs e)
        {
            mPanel.Focus();
            XYObjectPath ob = FindObjectAtPosition(e.X, e.Y);
            if (ob != selectedObject)
            {
                selectedObject = ob;
                RefreshView();
            }
            if (selectedObject != null)
            {
                selectedObject.mousePositionAtSelection = new Point(e.X, e.Y);
                selectedObject.boundsAtSelection = new Bounds(
                    selectedObject.bounds.x,
                    selectedObject.bounds.y,
                    selectedObject.bounds.w,
                    selectedObject.bounds.h);

            }


            mouseDown = true;
        }
        private void ViewBox_MouseUp(object sender, MouseEventArgs e)
        {
            mPanel.Focus();
            mouseDown = false;
        }
        private void ViewBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseDown || selectedObject == null)
                return;

            if (ctrlPressed) 
            {
                if (shiftPressed)
                {
                    selectedObject.scaleX = 1f;
                    selectedObject.scaleY = 1f;
                    //base default is 200 and it is always like that...
                    // we want the image to exactly fit square distance of mouse and bounds.atselection
                    float dist = (e.X - selectedObject.bounds.x + e.Y - selectedObject.bounds.y) / 2;
                    selectedObject.scale = dist / 200f;
                    RefreshView();
                    return;

                }
                float distX = e.X - selectedObject.bounds.x;
                float distY = e.Y - selectedObject.bounds.y;
                selectedObject.scaleX = distX / 200f;
                selectedObject.scaleY = distY / 200f;
                RefreshView();
                return;
            }
          
            selectedObject.bounds = new Bounds(
                selectedObject.boundsAtSelection.x + e.X - selectedObject.mousePositionAtSelection.X,
                selectedObject.boundsAtSelection.y + e.Y - selectedObject.mousePositionAtSelection.Y,
                selectedObject.bounds.w,
                selectedObject.bounds.h);
            RefreshView();


        }
        private void MForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
                shiftPressed = false;
            if (e.KeyCode == Keys.ControlKey)
                ctrlPressed = false;
        }

        private void MForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Delete) && selectedObject != null)
            {
                objects.Remove(selectedObject);
                selectedObject = null;
                RefreshView();
                return;
            }
            if (e.KeyCode == Keys.ShiftKey)
                shiftPressed = true;
            if (e.KeyCode == Keys.ControlKey)
                ctrlPressed = true;
        }
    }
}
    
