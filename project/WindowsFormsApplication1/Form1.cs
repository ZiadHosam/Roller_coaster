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
    public abstract class type
    {
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
            if (i == 0)
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
        public float w = 150;
        public float h = 50;
        public int currline = 0;
        public Bitmap img;
    }
    public partial class Form1 : Form
    {
        float CurrPntX, PntXreset;
        float CurrPntY, PntYreset;
        int DefaultLineLen;
        int flag = 0, scroll_flag = 0, flagstart = 0, ftest = 0, flagurgway = 0, flag_type = 0;
        int Xshow, Yshow, Xold, Yold, offW, offH, scrollSpeed;
        BezierCurve obj = new BezierCurve();
        float my_t_inForm = 0.5f;
        float cur_t = 0f, CurveSpeed, stretchval;
        PointF carPoint;
        int indexCurrDragNode = -1, indexCurrNode = -1;
        Bitmap off;
        Bitmap background;
        Bitmap background2;
        Bitmap background3;
        float count = 0;
        Timer tt = new Timer();


        int maxScrollX;
        Ccar car = new Ccar();
        int carspeed = 10;
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
                    l.CalcNextPoint(carspeed);

                    car.x = l.cx - car.w;
                    car.y = l.cy - car.h;

                    if (!l.travel)
                    {
                        car.currline++;

                        if (car.currline >= Parts.Count)
                        {
                            flagstart = 0;
                            flagurgway = 0;
                        }
                    }

                    //for (int i = 0; i < Circles.Count; i++)
                    //{
                    //    if (car.x >= Circles[i].XC)
                    //    {
                    //        // Circles[i].Getnextpoint();
                    //        car.x = Circles[i].XC;
                    //        car.y = Circles[i].YC;
                    //    }

                    //}
                }
                else if (Parts[car.currline].i == 2)
                {
                    BezierCurve c = Parts[car.currline].curve;
                    cur_t += CurveSpeed;
                    PointF pnt = c.CalcCurvePointAtTime(cur_t);
                    car.x = pnt.X - car.w;
                    car.y = pnt.Y - car.h;
                    if (cur_t >= 1)
                    {
                        cur_t = 0f;
                        car.currline++;
                        if (car.currline >= Parts.Count)
                        {
                            flagstart = 0;
                        }
                    }

                }

            }
            if (ftest == 1)
            {
                Yshow += scrollSpeed;
            }
            //count++;
            DrawDubb(this.CreateGraphics());
        }

        void Form1_Load(object sender, EventArgs e)
        {
            offW = ClientSize.Width * 3;
            offH = ClientSize.Height;
            off = new Bitmap(offW, offH);
            background = new Bitmap("bg3.jpg");
            background2 = new Bitmap("bg2.jpg");
            background3 = new Bitmap("bg4.jpg");
            car.img = new Bitmap("train.jpg");
            car.img.MakeTransparent(Color.White);

            Xshow = 0;
            Yshow = 0;
            maxScrollX = -2 * ClientSize.Width;
            scrollSpeed = 20;
            CurveSpeed = 0.1f;
            stretchval = 10;
            PntXreset = 0;
            PntYreset = (int)(ClientSize.Height * 0.5);
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
            if (flagurgway == 0)
            {
                switch (e.KeyCode)
                {
                    //case Keys.Up:
                    //    my_t_inForm += 0.01f;
                    //    break;
                    //case Keys.Down:
                    //    my_t_inForm -= 0.01f;
                    //    break;

                    //case Keys.Space:
                    //    if (flag == 0)
                    //        flag = 1;
                    //    else
                    //        flag = 0;
                    //    break;
                    case Keys.D3:
                        part p = new part();
                        p.i = 2;
                        p.curve = create_curve();
                        Parts.Add(p);
                        float x = p.curve.ControlPoints[2].X;
                        float y = p.curve.ControlPoints[2].Y;
                        update_line(x, y);
                        break;
                    case Keys.ControlKey:
                        flagurgway = 1;
                        flag_type = 1;
                        break;
                }
                if (e.KeyCode == Keys.D1)
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
                if (e.KeyCode == Keys.J)
                {
                    if (Parts.Count != 0)
                    {
                        Parts.RemoveAt(Parts.Count - 1);

                        if (Parts.Count != 0)
                        {
                            part p = Parts[Parts.Count - 1];
                            if (p.i == 0)
                            {
                                DDA l = p.line;
                                update_line(l.Xend, l.Yend);
                            }
                            else if (p.i == 2)
                            {
                                BezierCurve c = p.curve;
                                PointF pnt = p.curve.GetPoint(p.curve.ControlPoints.Count - 1);
                                update_line(pnt.X, pnt.Y);
                            }
                        }
                        else
                        {
                            update_line(PntXreset, PntYreset);
                        }
                    }
                }
                if (e.KeyCode == Keys.D2)
                {
                    Circle ptrav = new Circle();
                    ptrav.st = 0;
                    ptrav.end = 360;
                    ptrav.XC = (int)CurrPntX;
                    ptrav.YC = (int)CurrPntY - 120;
                    ptrav.Rad = 120;
                    Circles.Add(ptrav);
                }
                if (e.KeyCode == Keys.L)
                {
                    if(Parts.Count > 0)
                    {
                        part p = Parts[Parts.Count - 1];
                        if(p.i == 0)
                        {
                            DDA l = p.line;
                            l.Xend += stretchval;
                            //l.Yend += l.dx;
                            l.calc();
                            update_line(l.Xend, l.Yend);
                        }
                    }
                }
                if (e.KeyCode == Keys.J)
                {
                    //if (Parts.Count > 0)
                    //{
                    //    part p = Parts[Parts.Count - 1];
                    //    if (p.i == 0)
                    //    {
                    //        DDA l = p.line;
                    //        l.Xend = stretchval;
                    //        update_line(l.Xend, l.Yend);
                    //    }
                    //}
                }
                if (e.KeyCode == Keys.I)
                {
                    if (Parts.Count > 0)
                    {
                        part p = Parts[Parts.Count - 1];
                        if (p.i == 0)
                        {
                            DDA l = p.line;
                            l.Rotate(l, l.Xst, l.Yst, -0.35f);
                            update_line(l.Xend, l.Yend);
                        }
                        else if (p.i == 1)
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
                        else if (p.i == 2)
                        {
                            BezierCurve c = p.curve;
                            PointF cSt = c.GetPoint(0);
                            c = c.Rotate(c, cSt.X, cSt.Y, -0.35f);
                            PointF cEnd = c.GetPoint(c.ControlPoints.Count - 1);
                            update_line(cEnd.X, cEnd.Y);
                        }
                    }
                }
                if (e.KeyCode == Keys.K)
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
                        else if (p.i == 1)
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
                        else if (p.i == 2)
                        {
                            BezierCurve c = p.curve;
                            PointF cSt = c.GetPoint(0);
                            c = c.Rotate(c, cSt.X, cSt.Y, +0.35f);
                            PointF cEnd = c.GetPoint(c.ControlPoints.Count - 1);
                            update_line(cEnd.X, cEnd.Y);
                        }
                    }
                }
                if (e.KeyCode == Keys.F)
                {
                    if (Parts.Count != 0)
                    {
                        //car.x = PntXreset;
                        //car.y = PntYreset - car.h;
                        car.currline = 0;
                        flagstart = 1;
                        flagurgway = 1;
                        for (int i = 0; i < Parts.Count; i++)
                        {
                            if (Parts[i].i == 0)
                                Parts[i].line.calc();
                        }
                    }
                }
                if (e.KeyCode == Keys.Z)
                {
                    carspeed -= 5;
                    CurveSpeed -= 0.03f;
                }
                if (e.KeyCode == Keys.X)
                {
                    carspeed += 5;
                    CurveSpeed += 0.03f;
                }

                DrawDubb(this.CreateGraphics());
            }
            else
            {
                if (e.KeyCode == Keys.T)
                {
                    flagstart = 0;
                    flagurgway = 0;
                }
                if (flag_type == 1)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.ControlKey:
                            flag_type = 0;
                            flagurgway = 0;
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
                }
                if (e.KeyCode == Keys.Z)
                {
                    carspeed -= 5;
                    CurveSpeed -= 0.03f;

                }
                if (e.KeyCode == Keys.X)
                {
                    carspeed += 5;
                    CurveSpeed += 0.03f;
                }
                DrawDubb(this.CreateGraphics());
            }
        }


        private BezierCurve create_curve()
        {
            BezierCurve c = new BezierCurve();
            c.SetControlPoint(new Point((int)CurrPntX, (int)CurrPntY));
            c.SetControlPoint(new Point((int)CurrPntX + (DefaultLineLen / 2), (int)CurrPntY - (DefaultLineLen / 2)));
            c.SetControlPoint(new Point((int)CurrPntX + DefaultLineLen, (int)CurrPntY));
            return c;
        }


        private void Form1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (flag_type)
            {
                //case 0:
                //    obj.SetControlPoint(new Point(e.X, e.Y));
                //    break;

                case 1:
                    int tmpindex = -1;
                    for (int i = 0; i < Parts.Count; i++)
                    {
                        part p = Parts[i];
                        if (p.i == 2)
                        {
                            int x = e.X - Xshow;
                            int y = e.Y - Yshow;
                            BezierCurve c = p.curve;
                            tmpindex = c.isCtrlPoint(x, y);
                            if (tmpindex != -1)
                            {
                                indexCurrDragNode = tmpindex;
                                flag = 1;
                                indexCurrNode = i;
                                break;
                            }
                        }
                    }
                    break;
            }
            scroll_flag = 1;
            Xold = e.X;
            Yold = e.Y;
            DrawDubb(this.CreateGraphics());
        }

        private void Form1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (flag_type == 1 && flag == 1)
            {
                part p = Parts[indexCurrNode];
                if (p.i == 2)
                {
                    if (indexCurrNode == Parts.Count - 1 && indexCurrDragNode != 0)
                    {
                        int x = e.X - Xshow;
                        int y = e.Y - Yshow;
                        BezierCurve c = p.curve;
                        c.ModifyCtrlPoint(indexCurrDragNode, x, y);
                        if (indexCurrNode == Parts.Count - 1)
                            update_line(x, y);
                    }
                }
                DrawDubb(this.CreateGraphics());
            }
            if (scroll_flag == 1 && flag_type == 0)
            {
                int Xnew = e.X - Xold;
                int Ynew = e.Y - Yold;
                Xshow += Xnew;
                if (Xshow > 0)
                    Xshow = 0;
                if (Xshow < maxScrollX)
                    Xshow = maxScrollX;

                //Yshow += Ynew;
                //if (Yshow > 0)
                //    Yshow = 0;
                //if (Yshow < ClientSize.Height)
                //    Yshow = ClientSize.Height;
                Xold = e.X;
                Yold = e.Y;
                DrawDubb(this.CreateGraphics());

            }

        }

        private void Form1_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (flag_type == 1)
            {
                flag = 0;
                //indexCurrDragNode = -1;
                part p = Parts[Parts.Count - 1];

                if (p.i == 2 && p.curve != null)
                {
                    float x = p.curve.ControlPoints[2].X;
                    float y = p.curve.ControlPoints[2].Y;

                    update_line(x, y);
                }
                DrawDubb(this.CreateGraphics());
            }
            scroll_flag = 0;
        }



        private void DrawScene(Graphics g)
        {
            g.Clear(Color.LightCyan);
            //g.Clear(Color.White);
            g.DrawImage(background, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
            g.DrawImage(background2, ClientSize.Width, 0, this.ClientSize.Width, this.ClientSize.Height);
            g.DrawImage(background3, ClientSize.Width * 2, 0, this.ClientSize.Width, this.ClientSize.Height);


            //carPoint = obj.CalcCurvePointAtTime(my_t_inForm);
            //g.FillEllipse(Brushes.SkyBlue, carPoint.X - 15, carPoint.Y - 15, 30, 30);
            int legendX = ClientSize.Width - 220 - Xshow;
            int legendY = 10 - Yshow;
            int lineH = 28;
            g.FillRectangle(new SolidBrush(Color.FromArgb(170, 0, 30, 60)), legendX - 10, legendY - 5, 210, 316);
            g.DrawRectangle(new Pen(Color.DeepSkyBlue, 1), legendX - 10, legendY - 5, 210, 316);
            Font keyFont = new Font("Arial", 11, FontStyle.Bold);
            Font descFont = new Font("Arial", 11, FontStyle.Regular);
            string[][] items = new string[][] {
            new string[] { "    1", "New Line" },
            new string[] { "    2", "New Circle" },
            new string[] { "    3", "New Curve" },
            new string[] { "    F", "Start" },
            new string[] { "    T", "Stop" },
            new string[] { "    ← ", "Delete Last" },
            new string[] { "   J/L", "Rotate" },
            new string[] { "   Z/X", "Speed -/+" },
            new string[] { "   I/K", "Radius -/+" },
            new string[] { "CTRL", "Edit Mode" },
            new string[] { "   A/D", "Scroll" },
            };
            for (int i = 0; i < items.Length; i++)
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(255, 0, 60, 120)), legendX, legendY + i * lineH, 55, 22);
                g.DrawRectangle(new Pen(Color.DeepSkyBlue, 1), legendX, legendY + i * lineH, 55, 22);
                g.DrawString(items[i][0], keyFont, Brushes.Cyan, legendX + 3, legendY + i * lineH + 3);
                g.DrawString(items[i][1], descFont, Brushes.LightCyan, legendX + 63, legendY + i * lineH + 3);
            }


           
            //obj.DrawCurve(g);

            g.DrawImage(car.img, car.x, car.y, car.w, car.h);




            for (int i = 0; i < Parts.Count; i++)
            {
                Pen pen = new Pen(Color.White, 4);
                if (Parts[i].i == 0)
                {
                    DDA l = Parts[i].line;
                    g.DrawLine(pen, l.Xst, l.Yst, l.Xend, l.Yend);
                    if (flag_type == 1)
                    {
                        g.FillEllipse(new SolidBrush(Color.Black),
                                l.Xst - 5,
                                l.Yst - 5, 10, 10);
                        g.FillEllipse(new SolidBrush(Color.Black),
                                l.Xend - 5,
                                l.Yend - 5, 10, 10);
                    }
                }
                else if (Parts[i].i == 1)
                {

                }
                else if (Parts[i].i == 2)
                {
                    BezierCurve c = Parts[i].curve;
                    c.DrawCurve(g, flag_type);

                }
            }

            for (int i = 0; i < Circles.Count; i++)
            {
                Circles[i].Drawcircle(g);
            }
            //g.DrawImage(off, 0 - Xshow, 0 - Yshow, ClientSize.Width/4+100, 200);
            g.DrawImage(off, 0 - Xshow, 0 - Yshow, ClientSize.Width/5, ClientSize.Height / 5);
            g.DrawString("Mini Map", new Font("Arial", 8, FontStyle.Bold), Brushes.White, 0 - Xshow, 0 - Yshow);

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
