using System;
using Windows.UI;

namespace coloris.Harmony
{
    public class HsvColor
    {
        private HsvColor() { }

        public double H { get; private set; }
        public double S { get; private set; }
        public double V { get; private set; }

        public static HsvColor FromHsv(double h, double s, double v)
        {
            return new HsvColor { H = h % 360, S = s, V = v };
        }
    }

    public class RybColor
    {
        private RybColor() { }

        public byte R { get; private set; }
        public byte Y { get; private set; }
        public byte B { get; private set; }

        public static RybColor FromRyb(byte r, byte y, byte b)
        {
            return new RybColor { R = r, Y = y, B = b };
        }
    }

    public static class Convert
    {
        private static double Cubic(double t, double a, double b)
        {
            var weight = t * t * (3 - 2 * t);
            return a + weight * (b - a);
        }

        private static double GetRedFromRyb(double iR, double iY, double iB)
        {
            //red
            var x0 = Cubic(iB / 255.0, 1.0, 0.163);
            var x1 = Cubic(iB / 255.0, 1.0, 0.0);
            var x2 = Cubic(iB / 255.0, 1.0, 0.5);
            var x3 = Cubic(iB / 255.0, 1.0, 0.2);
            var y0 = Cubic(iY / 255.0, x0, x1);
            var y1 = Cubic(iY / 255.0, x2, x3);
            return Math.Ceiling(255 * Cubic(iR / 255.0, y0, y1));
        }

        private static double GetGreenFromRyb(double iR, double iY, double iB)
        {
            //green
            var x0 = Cubic(iB / 255.0, 1.0, 0.373);
            var x1 = Cubic(iB / 255.0, 1.0, 0.66);
            var x2 = Cubic(iB / 255.0, 0.0, 0.0);
            var x3 = Cubic(iB / 255.0, 0.5, 0.094);
            var y0 = Cubic(iY / 255.0, x0, x1);
            var y1 = Cubic(iY / 255.0, x2, x3);
            return Math.Ceiling(255 * Cubic(iR / 255.0, y0, y1));
        }

        private static double GetBlueFromRyb(double iR, double iY, double iB)
        {
            //blue
            var x0 = Cubic(iB / 255.0, 1.0, 0.6);
            var x1 = Cubic(iB / 255.0, 0.0, 0.2);
            var x2 = Cubic(iB / 255.0, 0.0, 0.5);
            var x3 = Cubic(iB / 255.0, 0.0, 0.0);
            var y0 = Cubic(iY / 255.0, x0, x1);
            var y1 = Cubic(iY / 255.0, x2, x3);
            return Math.Ceiling(255 * Cubic(iR / 255.0, y0, y1));
        }


        public static HsvColor RgbToHsv(Color c)
        {
            var max = Math.Max(Math.Max(c.R, c.G), c.B);
            var min = Math.Min(Math.Min(c.R, c.G), c.B);
            double h, s, v;
            if (max == min)
            {
                h = 0;
            }
            else if (max == c.R)
            {
                h = (60 * (c.G - c.B) / ((double)max - min) + 360) % 360;
            }
            else if (max == c.G)
            {
                h = (60 * (c.B - c.R) / ((double)max - min)) + 120;
            }
            else if (max == c.B)
            {
                h = (60 * (c.R - c.G) / ((double)max - min)) + 240;
            }
            else
            {
                throw new ArgumentException();
            }

            if (max == 0)
            {
                s = 0;
            }
            else
            {
                s = ((max - min) / (double)max);
            }
            v = max / 255.0;
            return HsvColor.FromHsv(h, s, v);
        }

        public static RybColor RgbToRyb(Color c)
        {
#if false
            double w = Math.Min(Math.Min(c.R, c.G), c.B);
            var r = 255 - c.R;
            var y = 255 - c.G;
            var b = 255 - c.B;

            var mg = Math.Max(Math.Max(r, g), b);

            var y = Math.Min(r, g);
            r -= y;
            g -= y;

            if (g != 0 && b != 0)
            {
                g /= 2.0;
                b /= 2.0;
            }

            y += g;
            b += g;

            var my = Math.Max(Math.Max(r, y), b);
            if (my != 0)
            {
                var n = mg / (double)my;
                r *= n;
                y *= n;
                b *= n;
            }

            r += w;
            y += w;
            b += w;

            return RybColor.FromRyb((byte)r, (byte)y, (byte)b);
#endif
            throw new NotSupportedException();
        }

        public static Color HsvToRgb(HsvColor c)
        {
            var hi = Math.Floor(c.H / 60.0) % 6;
            var f = (c.H / 60.0) - Math.Floor(c.H / 60.0);
            var p = Math.Round(c.V * (1.0 - (c.S))) * 255;
            var q = Math.Round(c.V * (1.0 - (c.S) * f)) * 255;
            var t = Math.Round(c.V * (1.0 - (c.S) * (1.0 - f))) * 255;

            double r, g, b;
            switch ((byte)hi)
            {
                default:
                    throw new ArgumentException();
                case 0:
                    r = c.V; g = t; b = p;
                    break;
                case 1:
                    r = q; g = c.V; b = t;
                    break;
                case 2:
                    r = p; g = c.V; b = t;
                    break;
                case 3:
                    r = p; g = q; b = c.V;
                    break;
                case 4:
                    r = t; g = p; b = c.V;
                    break;
                case 5:
                    r = c.V; g = p; b = q;
                    break;
            }

            return Color.FromArgb(255, (byte)r, (byte)g, (byte)b);
        }

        public static RybColor HsvToRyb(HsvColor c)
        {
            return RgbToRyb(HsvToRgb(c));
        }

        public static Color RybToRgb(RybColor c)
        {
            var r = GetRedFromRyb(c.R, c.Y, c.B);
            var g = GetGreenFromRyb(c.R, c.Y, c.B);
            var b = GetBlueFromRyb(c.R, c.Y, c.B);
            return Color.FromArgb(255, (byte)r, (byte)g, (byte)b);
        }

        public static HsvColor RybToHsv(RybColor c)
        {
            return RgbToHsv(RybToRgb(c));
        }

        public static RybColor RybHsvToRyb(double h, double s, double v)
        {
            double r, y, b;
            switch ((byte)(h / 60))
            {
                default:
                    throw new ArgumentException("Hue must in 0 <= h < 360.");
                case 0:
                    r = 255;
                    y = (255 * (v)) * (h / 60.0) + (255 * (1 - v));
                    b = Math.Max(r, y) * (1 - v);
                    break;
                case 1:
                    r = (255 * (v)) * (1.0 - (h - 60) / 60.0) + (255 * (1 - v));
                    y = 255;
                    b = Math.Max(r, y) * (1 - v);
                    break;
                case 2:
                    y = 255;
                    b = (255 * (v)) * ((h - 120) / 60.0) + (255 * (1 - v));
                    r = Math.Max(y, b) * (1 - v);
                    break;
                case 3:
                    y = (255 * (v)) * (1.0 - (h - 180) / 60.0) + (255 * (1 - v));
                    b = 255;
                    r = Math.Max(y, b) * (1 - v);
                    break;
                case 4:
                    b = 255;
                    r = (255 * (v)) * ((h - 240) / 60.0) + (255 * (1 - v));
                    y = Math.Max(r, b) * (1 - v);
                    break;
                case 5:
                    b = (255 * (v)) * (1.0 - (h - 300) / 60.0) + (255 * (1 - v));
                    r = 255;
                    y = Math.Max(r, b) * (1 - v);
                    break;
            }
            r *= s;
            y *= s;
            b *= s;

            return RybColor.FromRyb((byte)r, (byte)y, (byte)b);
        }

        public static Color RybHsvToRgb(double h, double s, double v)
        {
            return RybToRgb(RybHsvToRyb(h, s, v));
        }
    }
}