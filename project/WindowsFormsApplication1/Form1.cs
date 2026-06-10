using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public abstract class type {
        public int i;
    }

    public class part : type
    {
        public int i;
        public DDA line;
        public Circle circ;
        public BezierCurve curve;
        public void CalcNextPoint()
        {
            if(i == 0)
            {
                line.CalcNextPoint();
            }
        }
    }
    public class LineSegment
    {
        public PointF ptS, ptE;

        public void DrawYourSelf(Graphics g)
        {
            g.DrawLine(Pens.Black, ptS.X, ptS.Y, ptE.X, ptE.Y);
            g.FillEllipse(Brushes.Red, ptS.X - 5, ptS.Y - 5, 10, 10);
            g.FillEllipse(Brushes.Red, ptE.X - 5, ptE.Y - 5, 10, 10);
        }
    }
    public class Ccar
    {
        public float x;
        public float y = -100;
        public float w = 30;
        public float h = 30;
        public int currline = 0;
        public Bitmap img;
    }
    public partial class Form1 : Form
    {
        float CurrPntX, PntXreset;
        float CurrPntY, PntYreset;
        int DefaultLineLen;
        int flag = 0, scroll_flag = 0, flagstart = 0,ftest = 0;
        int Xshow, Yshow, Xold, Yold, offW, offH, scrollSpeed;
        BezierCurve obj = new BezierCurve();
        float my_t_inForm = 0.5f;
        PointF carPoint;
        int indexCurrDragNode = -1;
        Bitmap off;
        Bitmap background;
        float count = 0;
        Timer tt = new Timer();

        Ccar car = new Ccar();
        List<part> Parts = new List<part>();
        //List<part> Lines = new List<part>();
        List<Circle> Circles = new List<Circle>();
        public Form1()
        {
            InitializeComponent();
            this.Paint += new PaintEventHandler(Form1_Paint);
            this.WindowState = FormWindowState.Maximized;
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            this.Load += new EventHandler(Form1_Load);
            this.MouseDown += new MouseEventHandler(Form1_MouseDown);
            this.MouseMove += new MouseEventHandler(Form1_MouseMove);
            this.MouseUp += Form1_MouseUp;
            tt.Tick += Tt_Tick;
            tt.Start();
        }

        private void Tt_Tick(object sender, EventArgs e)
        {
            if (flagstart == 1)
            {

                if (Parts[car.currline].i == 0)
                {
                    DDA l = Parts[car.currline].line;
                    l.CalcNextPoint();

                    car.x = l.cx - car.w;
                    car.y = l.cy - car.h;

                    if (!l.travel)
                    {
                        car.currline++;

                        if (car.currline >= Parts.Count)
                        {
                            flagstart = 0;
                        }
                    }

                    for (int i = 0; i < Circles.Count; i++)
                    {
                        if (car.x >= Circles[i].XC)
                        {
                            // Circles[i].Getnextpoint();
                            car.x = Circles[i].XC;
                            car.y = Circles[i].YC;
                        }

                    }
                }

            }
            if(ftest == 1)
            {
                Yshow += scrollSpeed;
            }
            //count++;
            DrawDubb(this.CreateGraphics());
        }

        void Form1_Load(object sender, EventArgs e)
        {
            offW = ClientSize.Width * 2;
            offH = ClientSize.Height * 2;
            off = new Bitmap(offW, offH);
            background = new Bitmap("bg3.jpg");
            car.img = new Bitmap("bg4.jpg");

            Xshow = 0;
            Yshow = -ClientSize.Height;
            scrollSpeed = 20;
            PntXreset = 0;
            PntYreset = (int)(ClientSize.Height * 1.5);
            CurrPntX = PntXreset;
            CurrPntY = PntYreset;
            DefaultLineLen = 100;
        }

        public void update_line(float x, float y)
        {
            CurrPntX = x;
            CurrPntY = y;
        }
        void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    my_t_inForm += 0.01f;
                    break;
                case Keys.Down:
                    my_t_inForm -= 0.01f;
                    break;

                case Keys.Space:
                    if (flag == 0)
                        flag = 1;
                    else
                        flag = 0;
                    break;
                case Keys.W:
                    if (scroll_flag == 1)
                        return;
                    Yshow += scrollSpeed;
                    break;
                case Keys.S:
                    if (scroll_flag == 1)
                        return;

                    if (Yshow - scrollSpeed >= -ClientSize.Height)
                        Yshow -= scrollSpeed;
                    break;
                case Keys.A:
                    if (scroll_flag == 1)
                        return;
                    if (Xshow + scrollSpeed <= 0)
                        Xshow += scrollSpeed;
                    break;
                case Keys.D:
                    if (scroll_flag == 1)
                        return;
                    Xshow -= scrollSpeed;
                    break;

            }
            if (e.KeyCode == Keys.Right)
            {
                DDA ptrav = new DDA();
                
                ptrav.Xst = CurrPntX;
                ptrav.Xend = CurrPntX + DefaultLineLen;
                ptrav.Yst = CurrPntY;
                ptrav.Yend = CurrPntY;

                part p = new part();
                p.i = 0;
                p.line = ptrav;
                Parts.Add(p);

                update_line(ptrav.Xend, ptrav.Yend);
            }
            if (e.KeyCode == Keys.Left)
            {
                if (Parts.Count != 0)
                {
                    Parts.RemoveAt(Parts.Count - 1);

                    if (Parts.Count != 0)
                    {
                        part p = Parts[Parts.Count - 1];
                        if(p.i == 0)
                        {
                            DDA l = p.line;
                            update_line(l.Xend, l.Yend);
                        }
                    }
                    else
                    {
                        update_line(PntXreset, PntYreset);
                    }
                }
            }
            if (e.KeyCode == Keys.C)
            {
                Circle ptrav = new Circle();
                ptrav.st = 0;
                ptrav.end = 360;
                ptrav.XC = (int)CurrPntX;
                ptrav.YC = (int)CurrPntY - 120;
                ptrav.Rad = 120;
                Circles.Add(ptrav);
            }
            if (e.KeyCode == Keys.X)
            {
                if (Circles.Count > 0)
                {
                    if (Circles[Circles.Count - 1].Rad > 60)
                    {
                        Circles[Circles.Count - 1].YC += 20;
                        Circles[Circles.Count - 1].Rad -= 20;
                    }
                    else
                    {
                        Circles.RemoveAt(Circles.Count - 1);
                    }
                }
            }
            if (e.KeyCode == Keys.V)
            {
                if (Circles.Count > 0)
                {
                    if (Circles[Circles.Count - 1].Rad < 200)
                    {
                        Circles[Circles.Count - 1].YC -= 20;
                        Circles[Circles.Count - 1].Rad += 20;
                    }
                }
            }
            if (e.KeyCode == Keys.R)
            {
                if (Parts.Count > 0)
                {
                    part p = Parts[Parts.Count - 1];
                    if(p.i == 0)
                    {
                        DDA l = p.line;
                        l.Rotate(l, l.Xst, l.Yst, -0.35f);
                        update_line(l.Xend, l.Yend);
                    }
                }
            }
            if (e.KeyCode == Keys.E)
            {
                if (Parts.Count > 0)
                {
                    part p = Parts[Parts.Count - 1];
                    if (p.i == 0)
                    {
                        DDA l = p.line;
                        l.Rotate(l, l.Xst, l.Yst, +0.35f);
                        update_line(l.Xend, l.Yend);
                    }
                }
            }
            if (e.KeyCode == Keys.F)
            {
                if (Parts.Count != 0)
                {
                    //car.x = 0;
                    //car.y = ClientSize.Height/2-car.h;
                    flagstart = 1;
                    for (int i = 0; i < Parts.Count; i++)
                    {
                        if (Parts[i].i == 0)
                            Parts[i].line.calc();
                    }
                }
            }
            DrawDubb(this.CreateGraphics());

        }





        private void Form1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (flag)
            {
                case 0:
                    obj.SetControlPoint(new Point(e.X, e.Y));
                    break;

                case 1:
                    indexCurrDragNode = obj.isCtrlPoint(e.X, e.Y);
                    break;
            }
            scroll_flag = 1;
            Xold = e.X;
            Yold = e.Y;
            DrawDubb(this.CreateGraphics());
        }

        private void Form1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (flag == 1 && indexCurrDragNode != -1)
            {
                obj.ModifyCtrlPoint(indexCurrDragNode, e.X, e.Y);
                DrawDubb(this.CreateGraphics());
            }
            if (scroll_flag == 1)
            {
                int Xnew = e.X - Xold;
                int Ynew = e.Y - Yold;
                if (Xshow + Xnew <= 0)
                    Xshow += Xnew;
                else
                    Xshow = 0;
                if (Yshow + Ynew >= -ClientSize.Height)
                    Yshow += Ynew;
                else
                    Yshow = -ClientSize.Height;
                Xold = e.X;
                Yold = e.Y;
                DrawDubb(this.CreateGraphics());

            }

        }

        private void Form1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (flag == 1)
            {
                indexCurrDragNode = -1;
                DrawDubb(this.CreateGraphics());
            }
            scroll_flag = 0;
        }



        private void DrawScene(Graphics g)
        {
            g.Clear(Color.Black);
            //g.Clear(Color.White);

            g.DrawImage(background, 0, ClientSize.Height, this.ClientSize.Width, this.ClientSize.Height);
            g.DrawImage(background, ClientSize.Width, ClientSize.Height, this.ClientSize.Width, this.ClientSize.Height);
            obj.DrawCurve(g);

            g.DrawImage(car.img, car.x, car.y, car.w, car.h);




            for (int i = 0; i < Parts.Count; i++)
            {
                Pen pen = new Pen(Color.White, 7);
                if (Parts[i].i == 0)
                {
                    DDA l = Parts[i].line;
                    g.DrawLine(pen, l.Xst, l.Yst, l.Xend, l.Yend);
                }
            }
            for (int i = 0; i < Circles.Count; i++)
            {
                Circles[i].Drawcircle(g);
            }
            g.DrawImage(off, 0 - Xshow, 0 - Yshow, 200, 200);
            carPoint = obj.CalcCurvePointAtTime(my_t_inForm);
            g.FillEllipse(Brushes.SkyBlue, carPoint.X - 15, carPoint.Y - 15, 30, 30);
            g.DrawString("right:newl,left:deletel//c:newc,x:shrinkc,v:enlargec//e:rotatel down,r:rotatel up//// s:start " + CurrPntX, new Font("System", 20), Brushes.White, 10, 10);
        }
        private void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            DrawDubb(e.Graphics);
        }
        void DrawDubb(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, Xshow, Yshow);
        }
    }
}

//Some of you were asking how to rotate an image, here is some code similar to the technique we have learnt in the lectures, however it uses the ready mades provided by c#:

//    //The image you want to rotate

//    Bitmap bmp = new Bitmap(img.Width, img.Height);

//Graphics g = Graphics.FromImage(bmp);



////now we set the rotation point to the center of our image

//g.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);



////now rotate the image

//g.RotateTransform(rotationAngle);



////now we return the transformation we applied

//g.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);



////now draw our the new image

//g.DrawImage(img, new Point(0, 0));
