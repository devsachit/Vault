using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vault
{
    public static class ImageLoad
    {
        public static Image LoadBitmapImage(string fileName)
        {
            try
            {
                Image image;
                if (!File.Exists(fileName))
                {
                    return null;
                }
                using (Stream stream = File.OpenRead(fileName))
                {
                    image = System.Drawing.Image.FromStream(stream);
                    Graphics g = Graphics.FromImage(image);
                }
                return image;
            }
            catch
            {

                return null;
            }
        }


        public static Image GetThumbnailImage(string filename, int MaximumPixelofThumbnail)
        {
            Image img = LoadBitmapImage(filename);
            Size thumbnailSize = GetThumbnailSize(img, MaximumPixelofThumbnail);
            Bitmap bmp = new Bitmap(MaximumPixelofThumbnail, MaximumPixelofThumbnail);
            Graphics g = Graphics.FromImage(bmp);
            float factor;
            if (img.Width > img.Height)
            {
                factor = img.Height / (float)MaximumPixelofThumbnail;
                g.DrawImage(img, new RectangleF(MaximumPixelofThumbnail / 2 - img.Width / factor / 2, 0, img.Width / factor, MaximumPixelofThumbnail), new RectangleF(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);

            }
            else
            {
                factor = img.Width / (float)MaximumPixelofThumbnail;
                g.DrawImage(img, new RectangleF(0, MaximumPixelofThumbnail / 2 - img.Height / factor / 2, MaximumPixelofThumbnail, img.Height / factor), new RectangleF(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);

            }
            return bmp;
        }

        internal static string GetStringThumbnailof(Image fullImage)
        {
            // if(fullImage.RawFormat==ImageFormat.Png)
            return SD.StringImage.Conversion.ImageToString(fullImage, ImageFormat.Png);
            // else

            //   return SD.StringImage.Conversion.ImageToString(fullImage, ImageFormat.Jpeg);
        }

        private static string GetThumbnailImage(string v, object maximumPixelofThumbnail)
        {
            throw new NotImplementedException();
        }

        internal static string GetStringofImage(Bitmap bitmap)
        {
            if (bitmap.RawFormat == ImageFormat.Png)
                return SD.StringImage.Conversion.ImageToString(bitmap, ImageFormat.Png);
            else

                return SD.StringImage.Conversion.ImageToString(bitmap, ImageFormat.Jpeg);
        }

        public static Image GetThumbnailImage(Image img, int MaximumPixelofThumbnail)
        {
            if (img == null)
                return new Bitmap(1, 1);

            Size thumbnailSize = GetThumbnailSize(img, MaximumPixelofThumbnail);
            Bitmap bmp = new Bitmap(MaximumPixelofThumbnail, MaximumPixelofThumbnail);
            Graphics g = Graphics.FromImage(bmp);
            float factor;
            if (img.Width > img.Height)
            {
                factor = img.Height / (float)MaximumPixelofThumbnail;
                g.DrawImage(img, new RectangleF(MaximumPixelofThumbnail / 2 - img.Width / factor / 2, 0, img.Width / factor, MaximumPixelofThumbnail), new RectangleF(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);

            }
            else
            {
                factor = img.Width / (float)MaximumPixelofThumbnail;
                g.DrawImage(img, new RectangleF(0, MaximumPixelofThumbnail / 2 - img.Height / factor / 2, MaximumPixelofThumbnail, img.Height / factor), new RectangleF(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);

            }
            return bmp;
        }

        public static Size GetThumbnailSize(Image originalimage, int MaximumPixelOfThumbnail)
        {
            if (originalimage == null)
                return new Size(1, 1);
            // Width and height.
            int originalWidth = originalimage.Width;
            int originalHeight = originalimage.Height;

            // Compute best factor to scale entire image based on larger dimension.
            double factor;
            if (originalWidth > originalHeight)
            {
                factor = (double)MaximumPixelOfThumbnail / originalWidth;
            }
            else
            {
                factor = (double)MaximumPixelOfThumbnail / originalHeight;
            }

            // Return thumbnail size.
            return new Size((int)(originalWidth * factor), (int)(originalHeight * factor));
        }





        public static Image GetThumbnailImage_No_Crop(string filename, int MaximumPixelofThumbnail)
        {

            Image img = LoadBitmapImage(filename);

            Size thumbnailSize = GetThumbnailSize(img, MaximumPixelofThumbnail);
            return img.GetThumbnailImage(thumbnailSize.Width, thumbnailSize.Height, null, IntPtr.Zero);


        }

    }
}
