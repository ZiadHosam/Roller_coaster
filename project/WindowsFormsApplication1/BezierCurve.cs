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
	/// <summary>
	/// 
	/// </summary>
	public class BezierCurve
	{
		
		public List<PointF> ControlPoints;

        public float t_inc = 0.001f;
        
        public Color cl = Color.White;
        public Color clr1 = Color.Blue;
        public Color ftColor = Color.Black;

		public BezierCurve()
		{
			ControlPoints = new List<PointF>();
		}


		private float Factorial(int n)
		{
			float res = 1.0f;

			for (int i=2; i<=n; i++)
				res *= i;

			return res;
		}

		private float C(int n,int i)
		{
			float res = Factorial(n) / (Factorial(i) * Factorial(n-i));
			return res;
		}

		private double Calc_B(float t,int i)
		{
			int n = ControlPoints.Count-1;            
			double res =    C(n,i) * 
                            Math.Pow((1-t),(n-i)) * 
                            Math.Pow(t, i);
			return res;
		}

		public PointF GetPoint(int i)
		{
			 return ControlPoints[i];
		}

		public PointF CalcCurvePointAtTime(float t)
		{
			PointF pt = new PointF();
			for (int i=0; i<ControlPoints.Count; i++)
			{
				float B = (float)Calc_B(t,i);
				pt.X += B * ControlPoints[i].X;
				pt.Y += B * ControlPoints[i].Y;
			}
			
			return pt;
		}

		private void DrawControlPoints(Graphics g)
		{
            Font Ft = new Font("System" , 10);
			for (int i=0; i<ControlPoints.Count; i++)
			{
				g.FillEllipse(	new SolidBrush(clr1), 
								ControlPoints[i].X-5, 
								ControlPoints[i].Y-5, 10,10);

                g.DrawString("P# " + i, Ft, new SolidBrush( ftColor), ControlPoints[i].X - 15, ControlPoints[i].Y - 15);
			}
		}

		public int isCtrlPoint(int XMouse, int YMouse)
		{
			Rectangle rc;
			for (int i=0; i<ControlPoints.Count; i++)
			{
				rc = new Rectangle((int)ControlPoints[i].X-5, (int)ControlPoints[i].Y-5, 10,10);
				if (XMouse >= rc.Left && XMouse <= rc.Right && YMouse >= rc.Top && YMouse <= rc.Bottom)
				{
					return i;
				}
			}
			return -1;
		}

		public void ModifyCtrlPoint(int i , int XMouse, int YMouse)
		{
			PointF p = ControlPoints[i];			
            
			p.X =  XMouse;
			p.Y =  YMouse;
			ControlPoints[i] = p;	
		}

        public void SetControlPoint(Point pt)
        {
            ControlPoints.Add(pt);            
        }

		private void DrawCurvePoints(Graphics g)
		{
			if (ControlPoints.Count <= 0)
				return;

			    PointF	curvePoint;
				for (float t=0.0f; t<=1.0; t+=t_inc)
				{
					    curvePoint = CalcCurvePointAtTime(t);
                        g.FillEllipse(  new SolidBrush(cl), 
                                        curvePoint.X-2, curvePoint.Y-2, 
                                        4, 4); 
				}
		}

		public void DrawCurve(Graphics g)
		{
			DrawControlPoints(g);
			DrawCurvePoints(g);
		}

        public BezierCurve Rotate(BezierCurve curve, float xRef, float yRef, float speed)
        {
            ///////////////////
            //// translate
            //////////////////
            for (int i = 0; i < curve.ControlPoints.Count; i++)
            {
                PointF L = curve.ControlPoints[i];
                L.X -= xRef;
                L.Y -= yRef;

                double xn = L.X * Math.Cos(speed) - L.Y * Math.Sin(speed);
                double Yn = L.X * Math.Sin(speed) + L.Y * Math.Cos(speed);

                L.X = (float)xn;
                L.Y = (float)Yn;

                L.X += xRef;
                L.Y += yRef;

                curve.ControlPoints[i] = L;
            }

            ///////////////////
            //// Rotate around origin
            //////////////////


            ///////////////////
            //// undo the translation
            //////////////////

            return curve;
        }

    }
}
