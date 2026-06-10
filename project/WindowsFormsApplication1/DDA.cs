using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
namespace WindowsFormsApplication1
{
    public class DDA : part
    {
        public float Xst, Yst;
        public float Xend, Yend;
        float dy, dx, m;
        public float cx, cy;
        int speed = 10;
        public bool travel;
        public void calc()
        {
            dy = Yend - this.Yst;
            dx = Xend - Xst;
            m = dy / dx;
            cx = Xst;
            cy = Yst;
            travel = true;
        }
        public bool CalcNextPoint()
        {
            
            if (Math.Abs(dx) > Math.Abs(dy))
            {
                if (Xst < Xend)
                {
                    cx += speed;
                    cy += m * speed;
                    if (cx >= Xend)
                    {
                        travel= false;
                    }

                }
                else
                {
                    cx -= speed;
                    cy -= m * speed;
                    if (cx <= Xend)
                    {
                        travel = false;
                    }
                }
            }
            else
            {
                if (Yst < Yend)
                {
                    cy += speed;
                    cx += 1 / m * speed;
                    if (cy >= Yend)
                    {
                        travel = false;
                    }
                }
                else
                {
                    cy -= speed;
                    cx -= 1 / m * speed;
                    if (cy <= Yend)
                    {
                        travel = false;
                    }
                }

            }
            return true;
        }
        public void Rotate(DDA L, float xRef, float yRef, float ang)
        {
            ///////////////////
            //// translate
            //////////////////
            L.Xst -= xRef;
            L.Yst -= yRef;
            L.Xend -= xRef;
            L.Yend -= yRef;

            ///////////////////
            //// Rotate around origin
            //////////////////
            double xn = L.Xst * Math.Cos(ang) - L.Yst * Math.Sin(ang);
            double Yn = L.Xst * Math.Sin(ang) + L.Yst * Math.Cos(ang);

            L.Xst = (float)xn;
            L.Yst = (float)Yn;

            xn = L.Xend * Math.Cos(ang) - L.Yend * Math.Sin(ang);
            Yn = L.Xend * Math.Sin(ang) + L.Yend * Math.Cos(ang);

            L.Xend = (float)xn;
            L.Yend = (float)Yn;

            ///////////////////
            //// undo the translation
            //////////////////
            L.Xst += xRef;
            L.Yst += yRef;
            L.Xend += xRef;
            L.Yend += yRef;

            //return L;
        }

    }
}
