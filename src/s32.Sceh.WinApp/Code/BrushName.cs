using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace s32.Sceh.WinApp.Code
{
    public class BrushName
    {
        private readonly string _name;
        private readonly int _orderPosition;
        private readonly SolidColorBrush _value;

        public BrushName(string name, SolidColorBrush value)
        {
            _name = name;
            _value = value;
            _orderPosition = CalculateOrderPosition(value.Color);
        }

        public string Name
        {
            get { return _name; }
        }

        public int OrderPosition
        {
            get { return _orderPosition; }
        }

        public SolidColorBrush Value
        {
            get { return _value; }
        }

        public static BrushName[] GetBrushArray()
        {
            var brushes = new BrushName[]
            {
                new BrushName("Aquamarine", Brushes.Aquamarine),
                new BrushName("Black", Brushes.Black),
                new BrushName("Blue", Brushes.Blue),
                new BrushName("Brown", Brushes.Brown),
                new BrushName("BurlyWood", Brushes.BurlyWood),
                new BrushName("CadetBlue", Brushes.CadetBlue),
                new BrushName("Chocolate", Brushes.Chocolate),
                new BrushName("Coral", Brushes.Coral),
                new BrushName("CornflowerBlue", Brushes.CornflowerBlue),
                new BrushName("Crimson", Brushes.Crimson),
                new BrushName("Cyan", Brushes.Cyan),
                new BrushName("DarkBlue", Brushes.DarkBlue),
                new BrushName("DarkGoldenrod", Brushes.DarkGoldenrod),
                new BrushName("DarkGray", Brushes.DarkGray),
                new BrushName("DarkGreen", Brushes.DarkGreen),
                new BrushName("DarkKhaki", Brushes.DarkKhaki),
                new BrushName("DarkOliveGreen", Brushes.DarkOliveGreen),
                new BrushName("DarkOrange", Brushes.DarkOrange),
                new BrushName("DarkSeaGreen", Brushes.DarkSeaGreen),
                new BrushName("DarkSlateBlue", Brushes.DarkSlateBlue),
                new BrushName("DarkViolet", Brushes.DarkViolet),
                new BrushName("DeepPink", Brushes.DeepPink),
                new BrushName("DeepSkyBlue", Brushes.DeepSkyBlue),
                new BrushName("DimGray", Brushes.DimGray),
                new BrushName("DodgerBlue", Brushes.DodgerBlue),
                new BrushName("Firebrick", Brushes.Firebrick),
                new BrushName("Gainsboro", Brushes.Gainsboro),
                new BrushName("Gold", Brushes.Gold),
                new BrushName("Goldenrod", Brushes.Goldenrod),
                new BrushName("Gray", Brushes.Gray),
                new BrushName("Green", Brushes.Green),
                new BrushName("GreenYellow", Brushes.GreenYellow),
                new BrushName("HotPink", Brushes.HotPink),
                new BrushName("IndianRed", Brushes.IndianRed),
                new BrushName("Indigo", Brushes.Indigo),
                new BrushName("Khaki", Brushes.Khaki),
                new BrushName("LawnGreen", Brushes.LawnGreen),
                new BrushName("LightBlue", Brushes.LightBlue),
                new BrushName("LightPink", Brushes.LightPink),
                new BrushName("LightSalmon", Brushes.LightSalmon),
                new BrushName("LightSeaGreen", Brushes.LightSeaGreen),
                new BrushName("LightSkyBlue", Brushes.LightSkyBlue),
                new BrushName("Lime", Brushes.Lime),
                new BrushName("LimeGreen", Brushes.LimeGreen),
                new BrushName("Magenta", Brushes.Magenta),
                new BrushName("Maroon", Brushes.Maroon),
                new BrushName("MediumAquamarine", Brushes.MediumAquamarine),
                new BrushName("MediumOrchid", Brushes.MediumOrchid),
                new BrushName("MediumPurple", Brushes.MediumPurple),
                new BrushName("MediumSeaGreen", Brushes.MediumSeaGreen),
                new BrushName("MediumSpringGreen", Brushes.MediumSpringGreen),
                new BrushName("MediumVioletRed", Brushes.MediumVioletRed),
                new BrushName("MidnightBlue", Brushes.MidnightBlue),
                new BrushName("Moccasin", Brushes.Moccasin),
                new BrushName("Olive", Brushes.Olive),
                new BrushName("OliveDrab", Brushes.OliveDrab),
                new BrushName("Orange", Brushes.Orange),
                new BrushName("OrangeRed", Brushes.OrangeRed),
                new BrushName("PaleGreen", Brushes.PaleGreen),
                new BrushName("PaleVioletRed", Brushes.PaleVioletRed),
                new BrushName("Peru", Brushes.Peru),
                new BrushName("Plum", Brushes.Plum),
                new BrushName("Purple", Brushes.Purple),
                new BrushName("Red", Brushes.Red),
                new BrushName("RosyBrown", Brushes.RosyBrown),
                new BrushName("RoyalBlue", Brushes.RoyalBlue),
                new BrushName("SaddleBrown", Brushes.SaddleBrown),
                new BrushName("Salmon", Brushes.Salmon),
                new BrushName("SandyBrown", Brushes.SandyBrown),
                new BrushName("SeaGreen", Brushes.SeaGreen),
                new BrushName("Sienna", Brushes.Sienna),
                new BrushName("Silver", Brushes.Silver),
                new BrushName("SlateBlue", Brushes.SlateBlue),
                new BrushName("SpringGreen", Brushes.SpringGreen),
                new BrushName("SteelBlue", Brushes.SteelBlue),
                new BrushName("Tan", Brushes.Tan),
                new BrushName("Teal", Brushes.Teal),
                new BrushName("Thistle", Brushes.Thistle),
                new BrushName("Tomato", Brushes.Tomato),
                new BrushName("Turquoise", Brushes.Turquoise),
                new BrushName("White", Brushes.White),
                new BrushName("WhiteSmoke", Brushes.WhiteSmoke),
                new BrushName("Yellow", Brushes.Yellow),
                new BrushName("YellowGreen", Brushes.YellowGreen),
            };

            Array.Sort(brushes, new ColorComparer());

            return brushes;
        }

        private static int CalculateOrderPosition(Color c)
        {
            int chroma, diff, hue, result;

            if (c.R >= c.G) // R>G
            {
                if (c.G >= c.B) // R>G>B
                {
                    chroma = c.R - c.B;
                    diff = c.G - c.B;

                    if (chroma == 0)
                    {
                        hue = 0;
                        Check(c.R, c.G, c.B, chroma, 0, hue);
                    }
                    else
                    {
                        hue = 0x1000 + (diff << 12) / chroma;
                        Check(c.R, c.G, c.B, chroma, 1, hue);
                    }

                    result = hue << 8 | c.R;
                }
                else if (c.R >= c.B) // R>B>G
                {
                    chroma = c.R - c.G;
                    diff = c.G - c.B;
                    hue = 0x7000 + (diff << 12) / chroma;

                    Check(c.R, c.B, c.G, chroma, 6, hue);
                    result = hue << 8 | c.R;
                }
                else // B>R>G
                {
                    chroma = c.B - c.G;
                    diff = c.R - c.G;
                    hue = 0x5000 + (diff << 12) / chroma;

                    Check(c.B, c.R, c.G, chroma, 5, hue);
                    result = hue << 8 | c.B;
                }
            }
            else // G>R
            {
                if (c.R >= c.B) // G>R>B
                {
                    chroma = c.G - c.B;
                    diff = c.B - c.R;
                    hue = 0x3000 + (diff << 12) / chroma;

                    Check(c.G, c.R, c.B, chroma, 2, hue);
                    result = hue << 8 | c.G;
                }
                else if (c.G >= c.B) // G>B>R
                {
                    chroma = c.G - c.R;
                    diff = c.B - c.R;
                    hue = 0x3000 + (diff << 12) / chroma;

                    Check(c.G, c.B, c.R, chroma, 3, hue);
                    result = hue << 8 | c.G;
                }
                else // B>G>R
                {
                    chroma = c.B - c.R;
                    diff = c.R - c.G;
                    hue = 0x5000 + (diff << 12) / chroma;

                    Check(c.B, c.G, c.R, chroma, 4, hue);
                    result = hue << 8 | c.B;
                }
            }

            return result;
        }

        private static void Check(byte max, byte mid, byte min, int chroma, int df, int hue)
        {
            int a = max, b = mid;
            if (b > a)
                throw new ArgumentException();
            a = min;
            if (b < a)
                throw new ArgumentException();

            if (chroma < 0 || chroma > 0xff)
                throw new ArgumentException();

            int minDiff = df << 12;
            int maxDiff = minDiff + 0x1000;

            if (hue < minDiff || hue > maxDiff)
                throw new ArgumentException();
        }

        private class ColorComparer : IComparer<BrushName>
        {
            public int Compare(BrushName x, BrushName y)
            {
                return x.OrderPosition - y.OrderPosition;
            }
        }
    }
}
