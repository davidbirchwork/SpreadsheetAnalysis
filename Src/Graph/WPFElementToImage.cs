using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Graph {
    /// <summary>
    /// Class handles operations for printing the graph to image.
    /// </summary>
    public static class WpfElementToImage {
        //Set resolution of image.
        private const double Dpi = 96d;

        //Set pixelformat of image.
        private static readonly PixelFormat PixelFormat = PixelFormats.Pbgra32;

        /// <summary>
        /// Method exports the graphlayout to an png image.
        /// </summary>
        /// <param name="path">destination of image</param>
        /// <param name="surface">graphlayout you want to print</param>
        public static void ExportToPng(FrameworkElement surface, Uri path) {
            if (surface == null) {
                return;                
            }
            //Save current canvas transform
            Transform transform = surface.LayoutTransform;

            //Reset current transform (in case it is scaled or rotated)
            surface.LayoutTransform = null;

            //Get the size of canvas
            Size size = new Size(surface.ActualWidth, surface.ActualHeight);

            //Measure and arrange the surface
            //VERY IMPORTANT
            surface.Measure(size);
            surface.Arrange(new Rect(size));

            //Create a render bitmap and push the surface to it
            RenderTargetBitmap renderBitmap =
                new RenderTargetBitmap(
                    (int)size.Width,
                    (int)size.Height,
                    Dpi,
                    Dpi,
                    PixelFormat);

            //Render the graphlayout onto the bitmap.
            renderBitmap.Render(surface);

            //Create a file stream for saving image
            using (FileStream outStream = new FileStream(path.LocalPath, FileMode.Create)) {
                //Use png encoder for our data
                string extension = Path.GetExtension(path.LocalPath);
                BitmapEncoder encoder;
                if (String.Compare("gif", extension, true) == 0)
                    encoder = new GifBitmapEncoder();
                else if (String.Compare("png", extension, true) == 0)
                    encoder = new PngBitmapEncoder();
                else if (String.Compare("jpg", extension, true) == 0 || String.Compare("jpeg", extension, true) == 0)
                    encoder = new JpegBitmapEncoder();
                else return;
                //Push the rendered bitmap to it
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                //Save the data to the stream
                encoder.Save(outStream);
            }

            //Restore previously saved layout
            surface.LayoutTransform = transform;
        }
    }
}