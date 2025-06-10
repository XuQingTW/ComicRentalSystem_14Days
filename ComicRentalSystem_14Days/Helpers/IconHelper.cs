using System;
using System.Drawing;

namespace ComicRentalSystem_14Days.Helpers
{
    public static class IconHelper
    {
        private static Icon? _appIcon;

        public static Icon GetAppIcon()
        {
            if (_appIcon != null) return _appIcon;

            Bitmap bmp = new Bitmap(32, 32);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                using var brush = new SolidBrush(ModernBaseForm.PrimaryColor);
                g.FillRectangle(brush, 2, 4, 12, 24);
                g.FillRectangle(brush, 18, 4, 12, 24);
                using var font = new Font("Segoe UI", 8F, FontStyle.Bold);
                g.DrawString("CR", font, Brushes.White, new PointF(4, 8));
            }

            _appIcon = Icon.FromHandle(bmp.GetHicon());
            return _appIcon;
        }
    }
}
