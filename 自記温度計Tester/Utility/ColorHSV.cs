using System;
using System.Drawing;

namespace 自記温度計Tester
{
    public class ColorHSV
    {
        public float H;
        public float S;
        public float V;

        public ColorHSV()
        {
            H = 0;
            S = 0;
            V = 0;
        }

         //h,s,vは0.0~1.0の値である必要があります
        public ColorHSV(float h, float s, float v)
        {
            H = Math.Max(0f, Math.Min(1f, h));
            S = Math.Max(0f, Math.Min(1f, s));
            V = Math.Max(0f, Math.Min(1f, v));
        }


    }

    public class ColorConv
    {
        // RGB から HSV へ変換する
        public static ColorHSV RGB2HSV(Color color)
        {
            float r = (float)color.R / 255f;
            float g = (float)color.G / 255f;
            float b = (float)color.B / 255f;

            float max = Math.Max(Math.Max(r, g), b);
            float min = Math.Min(Math.Min(r, g), b);

            float h = max - min;

            if (h > 0f)
            {
                if (max == r)
                {
                    h = (g - b) / h;
                    if (h < 0f)
                    {
                        h += 6f;
                    }
                }
                else if (max == g)
                {
                    h = 2f + (b - r) / h;
                }
                else
                {
                    h = 4f + (r - g) / h;
                }
            }
            h /= 6f;

            float s = max - min;
            if (max != 0)
            {
                s /= max;
            }
            float v = max;

            return new ColorHSV(h, s, v);
        }

        // HSV から RGB へ変換する
        public static Color HSV2RGB(ColorHSV hsv)
        {
            Color result = Color.FromArgb(0, 0, 0);

            if (hsv == null)
            {
                return result;
            }

            float h = hsv.H;
            float s = hsv.S;
            float v = hsv.V;

            float r = v;
            float g = v;
            float b = v;

            if (s > 0f)
            {
                h *= 6f;
                int i = (int)h;
                float f = h - (float)i;
                switch (i)
                {
                    default:
                    case 0:
                        g *= 1f - s * (1f - f);
                        b *= 1f - s;
                        break;
                    case 1:
                        r *= 1f - s * f;
                        b *= 1f - s;
                        break;
                    case 2:
                        r *= 1f - s;
                        b *= 1f - s * (1f - f);
                        break;
                    case 3:
                        r *= 1f - s;
                        g *= 1f - s * f;
                        break;
                    case 4:
                        r *= 1f - s * (1f - f);
                        g *= 1f - s;
                        break;
                    case 5:
                        g *= 1f - s;
                        b *= 1f - s * f;
                        break;
                }
            }

            r *= 255f;
            g *= 255f;
            b *= 255f;

            result = Color.FromArgb((int)r, (int)g, (int)b);
            return result;
        }
    }
}
