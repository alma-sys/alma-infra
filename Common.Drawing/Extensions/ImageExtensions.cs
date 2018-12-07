using System;
using System.Drawing;

namespace Alma.Common.Drawing
{
    public static class ImageHelper
    {
        public static Image RedimensionarImagem(this Image image, int larguraMax, int alturaMax)
        {
            if (image == null)
                return null;
            if (larguraMax > image.Width && alturaMax > image.Height)
                return image;

            var ratioX = (double)larguraMax / image.Width;
            var ratioY = (double)alturaMax / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var largura = (int)(image.Width * ratio);
            var altura = (int)(image.Height * ratio);

            var novaimg = new Bitmap(largura, altura);

            using (var graphics = Graphics.FromImage(novaimg))
                graphics.DrawImage(image, 0, 0, largura, altura);

            return novaimg;
        }

    }
}
