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

    public class PNGSplit {
 
        private Bitmap image;

        public PNGSplit(String filename) {
            var ms = new MemoryStream(File.ReadAllBytes(filename)); // Don't use using!!
            this.image = new Bitmap(ms);
        }

        public PNGSplit(Bitmap image) {
            this.image = image;
        }

        private Bitmap getAlphaGrid(Bitmap image) {
            Bitmap alphaImg = new Bitmap(image.Width, image.Height);

            for (int col = 0; col < image.Width; col++) {
                for (int row = 0; row < image.Height; row++) {
                    byte alpha = image.GetPixel(col, row).A;
                    Color color = Color.FromArgb(255, alpha, alpha, alpha);
                    alphaImg.SetPixel(col, row, color);
                }
            }

            return alphaImg;
        }

        private Bitmap getAlphalessImage(Bitmap image) {
            Bitmap alphalessImg = new Bitmap(image.Width, image.Height);

            for (int col = 0; col < image.Width; col++) {
                for (int row = 0; row < image.Height; row++) {
                    Color color = image.GetPixel(col, row);
                    Color alphaless = Color.FromArgb(255, color.R, color.G, color.B);
                    alphalessImg.SetPixel(col, row, alphaless);
                }
            }

            return alphalessImg;
        }

        public void exportAlphaImage(String exportFilename) {
            getAlphaGrid(this.image).Save(exportFilename);
        }

        public void exportAlphalessImage(String exportFilename) {
            getAlphalessImage(this.image).Save(exportFilename);
        }
    }
}