using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace VTSCore.Layers.Radar
{
    internal class ColorLinear
    {
        struct ColorPos
        {
            public double Pos;
            public Color Color;
        }
        List<ColorPos> _colors = new List<ColorPos>();

        public ColorLinear(Color startColor, Color endColor)
        {
            AddColor(0, startColor);
            AddColor(1, endColor);
        }

        public void AddColor(double pos, Color color)
        {
            ColorPos cp = new ColorPos() { Pos = pos, Color = color };
            int index = getIndex(pos);
            _colors.Insert(index, cp);
        }

        private int getIndex(double pos)
        {
            int index = 0;
            for (; index < _colors.Count; index++)
                if (_colors[index].Pos >= pos)
                    break;
            return index;
        }

        public Color GetColor(double pos)
        {
            int index = getIndex(pos);
            if (index < _colors.Count)
            {
                ColorPos right = _colors[index];
                if (index == 0 || pos == right.Pos)
                    return right.Color;
                else
                {
                    ColorPos left = _colors[index - 1];
                    double rightRatio = (pos - left.Pos) / (right.Pos - left.Pos);
                    double leftRatio = 1 - rightRatio;
                    byte r = (byte)(leftRatio * left.Color.R + rightRatio * right.Color.R);
                    byte g = (byte)(leftRatio * left.Color.G + rightRatio * right.Color.G);
                    byte b = (byte)(leftRatio * left.Color.B + rightRatio * right.Color.B);
                    byte a = (byte)(leftRatio * left.Color.A + rightRatio * right.Color.A);
                    return Color.FromArgb(a, r, g, b);
                }
            }
            else
                return Colors.Transparent;
        }
    }
}
