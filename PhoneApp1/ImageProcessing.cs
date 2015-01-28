/*
 * Created By:  Ayesha Ahmad
 * Date:        13-Feb-2013
 * Purpose:     This class contains image processing functions used in the app
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PhoneApp1
{
    /// <summary>
    /// Purpose:    This class contains image processing functions used in the app
    /// </summary>
    public class ImageProcessing
    {
        // Returns gray value of a pixel
        private static int GetGRAY(int inputPixel)
        {
            // AACODE:  Following code for converting to grayscale taken from 
            // MSDN "How to: Work with Grayscale in a Camera Application for Windows Phone"
            // http://msdn.microsoft.com/en-us/library/hh202982%28v=vs.92%29.aspx
            int a, r, g, b;
            a = inputPixel >> 24;
            r = (inputPixel & 0x00ff0000) >> 16;
            g = (inputPixel & 0x0000ff00) >> 8;
            b = (inputPixel & 0x000000ff);

            // Luminosity =(int)(0.109375*R + 0.59375*G + 0.296875*B + 0.5)
            int L = (7 * r + 38 * g + 19 * b + 32) >> 6;

            return ((a & 0xFF) << 24) | ((L & 0xFF) << 16) | ((L & 0xFF) << 8) | (L & 0xFF);
        }

        // Return pixel with only RED channel value intact
        private static int GetRED(int inputPixel)
        {
            return ((-1 << 24) | (inputPixel & 0x00ff0000));
        }

        // Return pixel with only GREEN channel value intact
        private static int GetGREEN(int inputPixel)
        {
            return ((-1 << 24) | (inputPixel & 0x0000ff00));
        }

        // Return pixel with only BLUE channel value intact
        private static int GetBLUE(int inputPixel)
        {
            return ((-1 << 24) | (inputPixel & 0x000000ff));
        }


        // Splits input image into R, G, B, and Gray tiles and returns this new image as a WriteableBitmap
        public static WriteableBitmap ChannelSplit(WriteableBitmap input, int inputWidth, int inputHeight)
        {
            WriteableBitmap output = new WriteableBitmap(inputWidth, inputHeight);

            int pixel;
            // BUG Alert: will only work for images with even height and width
            for (int i = 0; i < inputWidth; i += 2)
            {
                for (int j = 0; j < inputHeight; j += 2)
                {
                    //get the pixel from the original image
                    pixel = input.Pixels[inputWidth * j + i];

                    // RED tile
                    output.Pixels[inputWidth * (j / 2) + (i / 2)] = GetRED(pixel);

                    // GREEN tile
                    output.Pixels[inputWidth * (j / 2) + (i / 2) + (inputWidth / 2)] = GetGREEN(pixel);

                    // BLUE tile
                    output.Pixels[inputWidth * (j / 2) + (i / 2) + (inputWidth * (inputHeight / 2))] = GetBLUE(pixel);

                    // GRAYSCALE tile
                    output.Pixels[inputWidth * (j / 2) + (i / 2) + (inputWidth * (inputHeight / 2)) + (inputWidth / 2)] = GetGRAY(pixel);
                }
            }

            return output;
        }
    }
}
