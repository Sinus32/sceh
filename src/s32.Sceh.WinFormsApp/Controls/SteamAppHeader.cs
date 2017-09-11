using s32.Sceh.WinApp.WinApi;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace s32.Sceh.WinApp.Controls
{
    public class SteamAppHeader : Panel
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams parms = base.CreateParams;
                parms.ExStyle |= ExtendedWindowStyle.WS_EX_TRANSPARENT;
                return parms;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawLine(Pens.Gray, 0, this.Height, this.Width, this.Height);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            using (var brush = new LinearGradientBrush(this.ClientRectangle, Color.Black, Color.Black, 300, true))
            {
                ColorBlend cb = new ColorBlend();
                cb.Positions = new float[] { 0f, 0.42f, 0.58f, 1f };
                cb.Colors = new Color[] { GetMyColor(), Color.Transparent, Color.Transparent, GetOtherColor() };
                brush.InterpolationColors = cb;
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }

        private Color GetMyColor()
        {
            return Color.Red;
        }

        private Color GetOtherColor()
        {
            return Color.Red;
        }
    }
}