using System;
using System.Drawing;

namespace Alma.Common.Drawing
{
    public static class ImageHelper
    {
        public static Image Resize(this Image image, int maxWidth, int maxHeight)
        {
            if (image == null)
                return null;
            if (maxWidth > image.Width && maxHeight > image.Height)
                return image;

            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var width = (int)(image.Width * ratio);
            var height = (int)(image.Height * ratio);

            var newImage = new Bitmap(width, height);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, width, height);

            return newImage;
        }

    }
}
