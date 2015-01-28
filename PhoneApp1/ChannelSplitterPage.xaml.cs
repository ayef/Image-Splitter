/*
 * Created By:  Ayesha Ahmad
 * Date:        13-Feb-2013
 * Purpose:     Allows user to apply image processing function on selected image.
 *              Displays processed image, gives options for edit, save & zoom.
 *
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneApp1.Resources;



using Microsoft.Phone;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;
using System.Windows.Media;
using Microsoft.Devices;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Media;
using System.Windows.Input;

namespace PhoneApp1
{
    /// <summary>
    /// Allows user to apply image processing function on selected image
    /// Displays processed image, gives options for edit, save & zoom
    /// </summary>
    public partial class ChannelSplitterPage : PhoneApplicationPage
    {

        // Variables for applying ManipulationDelta Transform. 
        // Copied from: http://msdn.microsoft.com/en-us/library/windowsphone/develop/ff426933%28v=vs.105%29.aspx
        private TranslateTransform move = new TranslateTransform();
        private ScaleTransform resize = new ScaleTransform();
        private TransformGroup imageTransforms = new TransformGroup();
        
        // The application bar buttons
        ApplicationBarIconButton appBarSave;
        ApplicationBarIconButton appBarZoomIn;
        ApplicationBarIconButton appBarZoomOut;
        ApplicationBarIconButton appBarSplitChannels;

        // Zoom factor for zooming in using app bar buttons
        double zoomFactor;

        // Variable that tells whether the image has been processed or not, 'true' means image has been processed
        bool imageProcessed;

        // Variable that tells whether the image has been saved or not, 'true' means image has been saved
        bool imageSaved;
        
        // Stores local copy of captured image for processing
        WriteableBitmap imagebmp;

        public ChannelSplitterPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
            
            // Display the selected image
            ImageResult.Source = App.gCapturedImage;

            // Combine the moving and resizing tranforms into one TransformGroup.
            // The rectangle's RenderTransform can only contain a single transform or TransformGroup.
            // Copied from: http://msdn.microsoft.com/en-us/library/windowsphone/develop/ff426933%28v=vs.105%29.aspx
            imageTransforms.Children.Add(move);
            imageTransforms.Children.Add(resize);
            ImageResult.RenderTransform = imageTransforms;

            // Handle manipulation events.
            ImageResult.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(Image_ManipulationDelta);
        
            // Zoom factor for zooming in using app bar buttons
            zoomFactor = 1.5;
        }

        // AACODE: Initialize the variables, copy captured image to local variable 'imagebmp'
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            imageProcessed = false;
            imageSaved = false;

            // Allocate space if not already allocated
            if (imagebmp == null)
                imagebmp = new WriteableBitmap(App.gCapturedImage.PixelWidth, App.gCapturedImage.PixelHeight);

            // Copy captured image to local variable 'imagebmp'
            CopyWriteableBMP(App.gCapturedImage, imagebmp);
            
            base.OnNavigatedTo(e);
        }

        // AACODE: Copies WriteableBitmaps pixels from source bitmap to destination bitmap
        private void CopyWriteableBMP(WriteableBitmap src, WriteableBitmap dest)
        {
            Buffer.BlockCopy(src.Pixels, 0, dest.Pixels, 0, src.Pixels.Length * sizeof(int));
        }


        // AACODE: Calls channel splitting function for an image
        private void appBarSplitChannels_Click(object sender, EventArgs e)
        {
            // If user presses 'split' again, they will be prompted on whether they want to perform the operation again
            if (imageProcessed == true)
            {
                var result = MessageBox.Show("You have already split the image. Do you want to split it again?", "Split again?", MessageBoxButton.OKCancel );
                if (result == MessageBoxResult.OK)
                {
                    // Copy the current split image  into the original image buffer
                    CopyWriteableBMP(App.gChannelsImage, imagebmp);
                }
                else 
                {
                    return;
                }
            }

            // Call channel splitting function
            App.gChannelsImage = ImageProcessing.ChannelSplit(imagebmp, imagebmp.PixelWidth, imagebmp.PixelHeight);
            
            // Update UI
            ImageResult.Source = App.gChannelsImage;
            
            // Record that image has been processed
            imageProcessed = true;
            
            // Enable the save button now and set the imageSaved variable to false so that the edited picture can be saved
            appBarSave.IsEnabled = true;
            imageSaved = false;
        }

        // AACODE: Save image to library
        private void appBarSave_Click(object sender, EventArgs e)
        {
            // If the image has been processed but not saved yet, then try to save it
            if (imageProcessed == true )
            {
                if (imageSaved == false)
                {
                    bool result = Helper.SavePictureToMediaLibrary(++App.gSavedCounter + ".jpg", App.gChannelsImage);

                    if (result == false)
                        MessageBox.Show("File could not be saved.");
                    else
                    {
                        MessageBox.Show("File saved.");
                        // Disable the save button now and set the imageSaved variable to true
                        appBarSave.IsEnabled = false;
                        imageSaved = true;
                    }
                }
                else
                {
                    MessageBox.Show("This image has already been saved.");
                }
            }
            else
            {
                MessageBox.Show("You must apply the edit before the image can be saved.");
            }

        }

        // AACODE: Zoom in using app bar button, zoom in factor hardcoded in code
        private void appBarZoomIn_Click(object sender, EventArgs e)
        {
            // Scale the image.
            resize.ScaleX *= zoomFactor;
            resize.ScaleY *= zoomFactor;
        }

        // AACODE: Zoom out in using app bar button, zoom out factor hardcoded in code
        private void appBarZoomOut_Click(object sender, EventArgs e)
        {
            // Scale the image.
            resize.ScaleX *= 1.0 / zoomFactor;
            resize.ScaleY *= 1.0 / zoomFactor;
        }

        // AACODE: Warn the user if they have not saved the edited image
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // If the image has been edited but not saved, then warn the user
            if(imageProcessed == true && imageSaved == false)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to leave without saving?", "Warning", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
                else
                {
                    base.OnNavigatingFrom(e);
                }
            }
        }

        // Responds to translation and pinch inputs
        // Copied from: http://msdn.microsoft.com/en-us/library/windowsphone/develop/ff426933%28v=vs.105%29.aspx
        private void Image_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            // Move the image
            move.X += e.DeltaManipulation.Translation.X;
            move.Y += e.DeltaManipulation.Translation.Y;

            // Resize the image.
            if (e.DeltaManipulation.Scale.X > 0 && e.DeltaManipulation.Scale.Y > 0)
            {
                // Scale the image.
                resize.ScaleX *= e.DeltaManipulation.Scale.X;
                resize.ScaleY *= e.DeltaManipulation.Scale.Y;
            }
        }

        
        // AACODE: Modified sample code for building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsVisible = true;

            // Initialize buttons, set events, set text values to the localized string from AppResources, & add to the application bar
            // 'Channel splitting' button, disable button until image is available
            appBarSplitChannels = new ApplicationBarIconButton(new Uri("/Assets/AppBar/edit.png", UriKind.Relative));
            appBarSplitChannels.Text = AppResources.AppBarSplitChannelsText;
            appBarSplitChannels.Click += new EventHandler(appBarSplitChannels_Click);
            ApplicationBar.Buttons.Add(appBarSplitChannels);

            // Save button
            appBarSave = new ApplicationBarIconButton(new Uri("/Assets/AppBar/save.png", UriKind.Relative));
            appBarSave.Text = AppResources.AppBarSaveText;
            appBarSave.Click += new EventHandler(appBarSave_Click);
            
            // Save button is disabled until the editing occurs
            appBarSave.IsEnabled = false;
            ApplicationBar.Buttons.Add(appBarSave);

            // Zoom in button 
            appBarZoomIn = new ApplicationBarIconButton(new Uri("/Assets/AppBar/add.png", UriKind.Relative));
            appBarZoomIn.Text = AppResources.AppBarZoomInText;
            appBarZoomIn.Click += new EventHandler(appBarZoomIn_Click);
            ApplicationBar.Buttons.Add(appBarZoomIn);

            // Zoom in button 
            appBarZoomOut = new ApplicationBarIconButton(new Uri("/Assets/AppBar/minus.png", UriKind.Relative));
            appBarZoomOut.Text = AppResources.AppBarZoomOutText;
            appBarZoomOut.Click += new EventHandler(appBarZoomOut_Click);
            ApplicationBar.Buttons.Add(appBarZoomOut);

        }
 
    }

}