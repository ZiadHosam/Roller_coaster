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
    public partial class Form1 : Form
    {
        float CurrPntX = 0;
        float CurrPntY = 0;
        int flag = 0, scroll_flag = 0;
        int Xshow, Yshow, Xold, Yold;
        BezierCurve obj = new BezierCurve();
        float my_t_inForm = 0.5f;
        PointF carPoint;
        int indexCurrDragNode = -1;
        Bitmap off;
        Bitmap background;

        List<DDA> Lines = new List<DDA>();
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
        }
        void Form1_Load(object sender, EventArgs e)
        {
            off = new Bitmap(this.ClientSize.Width * 2, this.ClientSize.Height * 2);
            background = new Bitmap("bg3.jpg");
            Xshow = 0;
            Yshow = -ClientSize.Height;

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

            }
            if (e.KeyCode == Keys.Right)
            {
                DDA ptrav = new DDA();
                if (Lines.Count != 0)
                {
                    ptrav.Xst = Lines[Lines.Count - 1].Xend;
                    ptrav.Xend = Lines[Lines.Count - 1].Xend + 100;
                    ptrav.Yst = Lines[Lines.Count - 1].Yend;
                    ptrav.Yend = Lines[Lines.Count - 1].Yend;
                }
                else
                {
                    ptrav.Xst = 0;
                    ptrav.Xend = 100;
                    ptrav.Yst = ClientSize.Height / 2;
                    ptrav.Yend = ClientSize.Height / 2;
                }

                Lines.Add(ptrav);
                CurrPntX = Lines[Lines.Count - 1].Xend;
                CurrPntY = Lines[Lines.Count - 1].Yend;
            }
            if (e.KeyCode == Keys.Left)
            {
                if (Lines.Count != 0)
                {
                    Lines.RemoveAt(Lines.Count - 1);

                    if (Lines.Count != 0)
                    {
                        CurrPntX = Lines[Lines.Count - 1].Xend;
                        CurrPntY = Lines[Lines.Count - 1].Yend;
                    }
                    else
                    {
                        CurrPntX = 0;
                        CurrPntY = 0;
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
                if (Lines.Count > 0)
                {
                    DDA L = Lines[Lines.Count - 1];
                    L.Rotate(L, L.Xst, L.Yst, -0.35f);
                }
            }
            if (e.KeyCode == Keys.E)
            {
                if (Lines.Count > 0)
                {
                    DDA L = Lines[Lines.Count - 1];
                    L.Rotate(L, L.Xst, L.Yst, +0.35f);
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
            g.Clear(Color.White);

            g.DrawImage(background, 0, ClientSize.Height, this.ClientSize.Width, this.ClientSize.Height);
            g.DrawImage(background, ClientSize.Width, ClientSize.Height, this.ClientSize.Width, this.ClientSize.Height);
            obj.DrawCurve(g);




            for (int i = 0; i < Lines.Count; i++)
            {
                Pen p = new Pen(Color.White, 7);
                g.DrawLine(p, Lines[i].Xst, Lines[i].Yst, Lines[i].Xend, Lines[i].Yend);
            }
            for (int i = 0; i < Circles.Count; i++)
            {
                Circles[i].Drawcircle(g);
            }

            carPoint = obj.CalcCurvePointAtTime(my_t_inForm);
            g.FillEllipse(Brushes.SkyBlue, carPoint.X - 15, carPoint.Y - 15, 30, 30);
            g.DrawString("right:newl,left:deletel//c:newc,x:shrinkc,v:enlargec//e:rotatel down,r:rotatel up " + CurrPntX, new Font("System", 20), Brushes.White, 10, 10);
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
            g.DrawImage(off, 0, 0,200,200);
        }
    }
}


