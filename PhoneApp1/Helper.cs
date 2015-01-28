/*
 * Created By:  Ayesha Ahmad
 * Date:        15-Feb-2013
 * Purpose:     This class contains image processing functions used in the app
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Media;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.IO;

namespace PhoneApp1
{
    /// <summary>
    /// Purpose:    This class contains helper functions used in the app
    /// </summary>
    public class Helper
    {
        // AACODE: checks whether a file exists in Isolated Storage, returns true if file exists otherwise false
        public static bool IsolatedStorageFileExists(string fileName)
        {
            var myStore = IsolatedStorageFile.GetUserStoreForApplication();
            if (myStore.FileExists(fileName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // AACODE: Converts bitmap image to jpeg and saves it in the media library
        // Returns true if save successful otherwise false
        public static bool SavePictureToMediaLibrary(string fileName, WriteableBitmap bmp)
        {
            // Reference to the Media Library
            MediaLibrary library = new MediaLibrary();

            try 
            {
                // If the file does not exist then save the file
                if (library.Pictures[library.Pictures.Count - 1].Name != fileName)
                {
                    // Convert the bitmap file to jpeg format
                    MemoryStream memStream = new MemoryStream();
                    bmp.SaveJpeg(memStream, bmp.PixelWidth, bmp.PixelHeight, 0, 100);
                    memStream.Seek(0, SeekOrigin.Begin);

                    // Save photo to the media library camera roll.
                    library.SavePictureToCameraRoll(fileName, memStream);
                    memStream.Close();
                }
                else 
                {
                    // This file has already been saved
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                // TODO: Log error message
                if (library.Pictures.Count == 0)
                {
                    // Convert the bitmap file to jpeg format
                    MemoryStream memStream = new MemoryStream();
                    bmp.SaveJpeg(memStream, bmp.PixelWidth, bmp.PixelHeight, 0, 100);
                    memStream.Seek(0, SeekOrigin.Begin);

                    // Save photo to the media library camera roll.
                    library.SavePictureToCameraRoll(fileName, memStream);
                    memStream.Close();
                    return true;
                }
                return false;
            }
        }


    }
}
