using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// https://stackoverflow.com/a/10054729

namespace PCATApp {

    public class PNGCombine {
 
        private Bitmap rgbImage;
        private Bitmap alphaImage;

        public PNGCombine(String rgbFilename, String alphaFilename) {
            using (FileStream stream = File.Open(rgbFilename, FileMode.Open)) {
                this.rgbImage = new Bitmap(stream);
            }

            using (FileStream stream = File.Open(alphaFilename, FileMode.Open)) {
                this.alphaImage = new Bitmap(stream);

                if (this.alphaImage.Size != this.rgbImage.Size) {
                    this.alphaImage = new Bitmap(this.alphaImage, this.rgbImage.Size);
                }
            }
        }

        public PNGCombine(Bitmap rgbImage, Bitmap alphaImage) {
            this.rgbImage = rgbImage;
            this.alphaImage = alphaImage;
        }

        private Bitmap combineRGBA(Bitmap rgbImage, Bitmap alphaImage) {
            if (rgbImage.Height != alphaImage.Height || rgbImage.Width != alphaImage.Width) {
                throw new Exception("These two images have different dimensions!");
            }

            Bitmap finalImage = new Bitmap(rgbImage.Width, rgbImage.Height);

            for (int col = 0; col < rgbImage.Width; col++) {
                for (int row = 0; row < rgbImage.Height; row++) {
                    Color color = rgbImage.GetPixel(col, row);
                    Color combined = Color.FromArgb(alphaImage.GetPixel(col, row).R, color.R, color.G, color.B);
                    finalImage.SetPixel(col, row, combined);
                }
            }

            return finalImage;
        }

        public void exportImage(String exportFilename) {
            combineRGBA(this.rgbImage, this.alphaImage).Save(exportFilename);
        }
    }
}