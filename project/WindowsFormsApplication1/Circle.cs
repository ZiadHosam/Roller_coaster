using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
namespace WindowsFormsApplication1
{
    public class Circle
    {
        public int Rad;
        public float XC;
        public float YC;
        public float thRadian;
        public float st, end;
        public int dir;

        //public void Drawcircle(Graphics g)
        //{
        //    for (float i = 0; i <= (2 * Math.PI); i += (float)((2 * Math.PI) / 360))
        //    {
        //        //(float)(i * Math.PI / 180);

        //        float x = (float)(Rad * Math.Cos(360 - i));
        //        float y = (float)(Rad * Math.Sin(360 - i));

        //        x += XC;
        //        y += YC;

        //        g.FillEllipse(Brushes.White, x, y, 5, 5);
        //    }
        //}
        //public PointF Getnextpoint(int theta)
        //{

        //    PointF p = new PointF();

        //    thRadian = (float)(theta * Math.PI / 180);

        //    p.X = (float)(Rad * Math.Cos(thRadian)) + XC;
        //    p.Y = (float)(Rad * Math.Sin(thRadian)) + YC;
        //    return p;
        //}
        public void Drawcircle(Graphics g)
        {
            for (float i = end; i >= st; i -= 1.0f)
            {
                thRadian = (float)((i * Math.PI) / 180);
                float x = (float)(Rad * Math.Cos(thRadian));
                float y = (float)(Rad * Math.Sin(thRadian));

                x += XC;
                y += YC;

                g.FillEllipse(Brushes.White, x, y, 5, 5);
            }
            PointF tempst = Getnextpoint((int)st);
            PointF tempend = Getnextpoint((int)end);
            //g.DrawLine(Pens.Black, XC, YC, tempst.X, tempst.Y);
            //g.DrawLine(Pens.Black, XC, YC, tempend.X, tempend.Y);
        }
        public PointF Getnextpoint(int theta)
        {

            PointF p = new PointF();

            thRadian = (float)((360 - theta) * Math.PI / 180);

            p.X = (float)(Rad * Math.Cos(thRadian)) + XC;
            p.Y = (float)(Rad * Math.Sin(thRadian)) + YC;
            return p;
        }

        public void Rotate(float xRef, float yRef, float ang)
        {
            ///////////////////
            //// translate
            //////////////////
            XC -= xRef;
            YC -= yRef;

            ///////////////////
            //// Rotate around origin
            //////////////////
            double xn = XC * Math.Cos(ang) - YC * Math.Sin(ang);
            double Yn = XC * Math.Sin(ang) + YC * Math.Cos(ang);

            XC = (float)xn;
            YC = (float)Yn;


            ///////////////////
            //// undo the translation
            //////////////////
            XC += xRef;
            YC += yRef;

            //return L;
        }

    }
}
