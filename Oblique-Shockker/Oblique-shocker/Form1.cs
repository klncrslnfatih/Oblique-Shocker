using NumSharp;
using Oblique_shocker.Properties;
using System;
using System.Numerics;
using System.Windows.Forms;

namespace Oblique_shocker
{
    public partial class Form1 : Form
    {
        // OBLIQUE SHOCKER

        // Merhaba! Ben Fatih KILINÇARSLAN. Compressible Aerodynamic dersimi alýrken bu projeyi gerçekleþtirme fikrim de ortaya çýktý.
        // Projeyi gerçekleþtirirken; NASA, Fundamentals of Aerodynamic | John Anderson, Chatgpt gibi sitelerden - kaynaklardan faydalandým.
        // Sizlerin geliþtirebilmesi için de notlar býraktým. Cp, Cv, S2-S1 gibi birkaç tane daha ek deðiþken býraktým belki iþinize yarar :)

        // Hello! My name is Fatih KILINÇARSLAN. While taking the Compressible Aerodynamics course, the idea of implementing this project came to mind.
        // During the implementation of the project, I benefited from sources such as NASA, Fundamentals of Aerodynamics by John Anderson, and ChatGPT.
        // I have also left some notes for you to further develop the project.I have included additional variables such as Cp, Cv, S2-S1,
        // etc.which might be useful to you :)

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void buttonCalculate_Click_1(object sender, EventArgs e)
        {
            try
            {
                // All inputs
                double mach1 = Convert.ToDouble(textBoxMach.Text);
                double gamma = Convert.ToDouble(textBoxGamma.Text);
                double deflection_angle_degree = Convert.ToDouble(textBoxTheta.Text);

                // Converting to rad
                double deflection_angle_rad = deflection_angle_degree * Math.PI / 180;

                // Calculating coeffecients
                double A = Math.Pow(mach1, 2) - 1;
                double B = 0.5 * (gamma + 1) * Math.Pow(mach1, 4) * Math.Tan(deflection_angle_rad);
                double C = (1 + 0.5 * (gamma + 1) * Math.Pow(mach1, 2)) * Math.Tan(deflection_angle_rad);
                double[] coeffs = { 1, C, -A, (B - A * C) };
                double[] roots = Solve(1, C, -A, (B - A * C));
                double[] positives_roots = new double[roots.Length];
                int positiveloop = 0;

                for (int i = 0; i < roots.Length; i++)
                {
                    if (roots[i] > 0)
                    {
                        positives_roots[positiveloop] = roots[i];
                        positiveloop++;
                    }
                }

                Array.Resize(ref positives_roots, positiveloop); // Only positives values for resize the array

                double[] thetas = new double[positives_roots.Length];
                for (int i = 0; i < positives_roots.Length; i++)
                {
                    thetas[i] = Math.Atan(1 / positives_roots[i]);
                }

                double theta_weak = thetas.Min();
                double theta_strong = thetas.Max();

                double wave_angle_week_rad = theta_weak;
                double wave_angle_strong_rad = theta_strong;
                double wave_angle_strong_degree = wave_angle_strong_rad * 180 / Math.PI;
                double wave_angle_week_degree = wave_angle_week_rad * 180 / Math.PI;

                double Mn1 = mach1 * Math.Sin(wave_angle_week_rad);
                double Mn2 = Math.Sqrt((1 + ((gamma - 1) / 2) * Math.Pow(Mn1, 2)) / (gamma * Math.Pow(Mn1, 2) - (gamma - 1) / 2));
                double mach2 = Mn2 / Math.Sin(wave_angle_week_rad - deflection_angle_rad);

                double p2p1 = 1 + 2 * gamma * (Math.Pow(Mn1, 2) - 1) / (gamma + 1);
                double rho2rho1 = ((gamma + 1) * Math.Pow(Mn1, 2)) / (2 + (gamma - 1) * Math.Pow(Mn1, 2));
                double T2T1 = p2p1 * (1 / rho2rho1);

                double R = 287;
                double cp = gamma * R / (gamma - 1);
                double cv = R / (gamma - 1);

                double s2_s1 = cp * Math.Log(Math.E, (1 + (2 * gamma * (Math.Pow(Mn1, 2) - 1) / (gamma + 1)))
                              * ((2 + (gamma - 1) * (Math.Pow(Mn1, 2))) / ((gamma + 1) * (Math.Pow(Mn1, 2)))))
                              - R * Math.Log(Math.E, 1 + 2 * gamma * (Math.Pow(Mn1, 2) - 1) / (gamma + 1));

                // This formulas from John Anderson, Fundamental of Aeoradynamics book
                //double P02P01 = Math.Pow(Math.E, -s2_s1 / R);

                // From NASA | Glenn Research Center | Normal Shocks Interactive 
                double P02P01 = Math.Pow(((gamma + 1) * Math.Pow(Mn1, 2)) / ((gamma - 1) * Math.Pow(Mn1, 2) + 2), gamma / (gamma - 1))
                               * Math.Pow((gamma + 1) / (2 * gamma * Math.Pow(Mn1, 2) - (gamma - 1)), 1 / (gamma - 1));


                double P02P2 = Math.Pow((1 + (gamma - 1) * Math.Pow(Mn2, 2) / 2), gamma / (gamma - 1));
                double P02P1 = P02P2 * p2p1;


                // Writing to the textboxes
                textBoxMn1.Text = Mn1.ToString("0.000");
                textBoxMn2.Text = Mn2.ToString("0.000");
                textBoxMach2.Text = mach2.ToString("0.000");
                textBoxWaveAngleWeak.Text = wave_angle_week_degree.ToString("0.000");
                textBoxWaveAngleStrong.Text = wave_angle_strong_degree.ToString("0.000");

                textBoxP2P1.Text = p2p1.ToString("0.000");
                textBoxT2T1.Text = T2T1.ToString("0.000");
                textBoxRHO2RHO1.Text = rho2rho1.ToString("0.000");
                textBoxP02P01.Text = P02P01.ToString("0.000");
                textBoxP02P1.Text = P02P1.ToString("0.000");


                // DRAWING THE SHOCKWAVE AND BODY
                // After Shock
                Graphics after_shock;
                after_shock = this.CreateGraphics();
                SolidBrush after_shock_brush = new SolidBrush(Color.LightGreen);
                Point[] after_shock_point = new Point[4];

                after_shock_point[0].X = 1200;
                after_shock_point[0].Y = 450;

                after_shock_point[1].X = 750;
                after_shock_point[1].Y = 450;

                after_shock_point[2].X = 750;
                after_shock_point[2].Y = 100;

                after_shock_point[3].X = 1200;
                after_shock_point[3].Y = 100;

                after_shock.FillPolygon(after_shock_brush, after_shock_point);

                // Body
                Graphics body;
                body = this.CreateGraphics();
                SolidBrush body_pen = new SolidBrush(Color.Black);
                Point[] body_point = new Point[3];

                body_point[0].X = 1200;
                body_point[0].Y = 450;

                body_point[1].X = 850;
                body_point[1].Y = 450;

                body_point[2].X = 1200;
                body_point[2].Y = Convert.ToInt32(450 - Math.Tan(deflection_angle_rad) * 350);

                body.FillPolygon(body_pen, body_point);

                // Before Shock
                Graphics air_polygon;
                air_polygon = this.CreateGraphics();
                SolidBrush air_polygon_brush = new SolidBrush(Color.Red);
                Point[] air_polygon_point = new Point[5];

                air_polygon_point[0].X = 850;
                air_polygon_point[0].Y = 450;

                air_polygon_point[1].X = 750;
                air_polygon_point[1].Y = 450;

                air_polygon_point[2].X = 750;
                air_polygon_point[2].Y = 100;

                air_polygon_point[3].X = 850;
                air_polygon_point[3].Y = 100;

                air_polygon_point[4].X = Convert.ToInt32(850 + Math.Tan((90 - wave_angle_week_degree) * Math.PI / 180) * 350);
                air_polygon_point[4].Y = 100;

                air_polygon.FillPolygon(air_polygon_brush, air_polygon_point);
            }
            catch (Exception)
            {
                MessageBox.Show("Check your values.", "Error Message");
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            textBoxGamma.Clear();
            textBoxMach.Clear();
            textBoxTheta.Clear();
            textBoxMn1.Clear();
            textBoxMn2.Clear();
            textBoxMach2.Clear();
            textBoxWaveAngleWeak.Clear();
            textBoxWaveAngleStrong.Clear();
            textBoxP2P1.Clear();
            textBoxT2T1.Clear();
            textBoxRHO2RHO1.Clear();
            textBoxP02P01.Clear();
            textBoxP02P1.Clear();
        }

        static double[] Solve(double a, double b, double c, double d)
        {
            if (a == 0 && b == 0) 
            {
                if (c == 0)
                {
                    if (d == 0)
                    {
                        return new double[] { double.MinValue };
                    }
                    else
                    {
                       
                        return new double[0];
                    }
                }
                else
                {
                    return new double[] { -d / c };
                }
            }
            else if (a == 0) 
            {
                double discriminant = c * c - 4 * b * d;
                if (discriminant < 0)
                {
                    return new double[0];
                }
                else if (discriminant == 0)
                {
                    double x = -c / (2 * b);
                    return new double[] { x, x };
                }
                else
                {
                   
                    double x1 = (-c + Math.Sqrt(discriminant)) / (2 * b);
                    double x2 = (-c - Math.Sqrt(discriminant)) / (2 * b);
                    return new double[] { x1, x2 };
                }
            }
            else 
            {
                double f = FindF(a, b, c);
                double g = FindG(a, b, c, d);
                double h = FindH(g, f);

                if (f == 0 && g == 0 && h == 0)
                {
                    double x = Math.Pow(d / a, 1 / 3.0) * -1;
                    return new double[] { x, x, x };
                }
                else if (h <= 0)
                {
                    double i = Math.Sqrt(((g * g) / 4) - h);
                    double j = Math.Pow(i, 1 / 3.0);
                    double k = Math.Acos(-(g / (2 * i)));
                    double L = -j;
                    double M = Math.Cos(k / 3.0);
                    double N = Math.Sqrt(3) * Math.Sin(k / 3.0);
                    double P = -b / (3 * a);

                    double x1 = 2 * j * Math.Cos(k / 3.0) + P;
                    double x2 = L * (M + N) + P;
                    double x3 = L * (M - N) + P;

                    return new double[] { x1, x2, x3 };
                }
                else
                {
                    double R = -(g / 2.0) + Math.Sqrt(h);
                    double S = R >= 0 ? Math.Pow(R, 1 / 3.0) : -Math.Pow(-R, 1 / 3.0);
                    double T = -(g / 2.0) - Math.Sqrt(h);
                    double U = T >= 0 ? Math.Pow(T, 1 / 3.0) : -Math.Pow(-T, 1 / 3.0);

                    double x1 = S + U - b / (3.0 * a);

                    return new double[] { x1 };
                }
            }
        }

        static double FindF(double a, double b, double c)
        {
            return ((3.0 * c / a) - ((b * b) / (a * a))) / 3.0;
        }

        static double FindG(double a, double b, double c, double d)
        {
            return (((2.0 * (b * b * b)) / (a * a * a)) - ((9.0 * b * c) / (a * a)) + (27.0 * d / a)) / 27.0;
        }

        static double FindH(double g, double f)
        {
            return ((g * g) / 4.0 + (f * f * f) / 27.0);
        }
    }
}