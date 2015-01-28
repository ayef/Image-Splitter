/*
 * Created By:  Ayesha Ahmad
 * Date:        14-Feb-2013
 * Purpose:     Take a Photo
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

namespace PhoneApp1
{
    /// <summary>
    /// Displays camera viewfinder and allows user to capture an image
    /// </summary>
    public partial class TakePhotoPage : PhoneApplicationPage
    {
        // The camera chooser used to capture a picture
        PhotoCamera cam;

        //The application bar buttons that are used.
        ApplicationBarIconButton appBarCamera;

        // Constructor
        public TakePhotoPage()
        {
            InitializeComponent();

            //Creates an application bar and then sets visibility and menu properties
            BuildLocalizedApplicationBar();

        }

        // AACODE: Change the viewFinder's width according to the orientation so that the viewfinder always fills screen
        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            if ((e.Orientation == PageOrientation.LandscapeRight) || (e.Orientation == PageOrientation.LandscapeLeft))
            {
                viewfinderCanvas.Width = (cam.Resolution.Width > cam.Resolution.Height ? cam.Resolution.Width : cam.Resolution.Height);
            }
            else
            {
                viewfinderCanvas.Width = (cam.Resolution.Width > cam.Resolution.Height ? cam.Resolution.Height : cam.Resolution.Width);
            }
            base.OnOrientationChanged(e);
        }

        //Code for initialization, capture completed, image availability events; also setting the source for the viewfinder.
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

            // Check to see if the camera is available on the phone.
            if ((PhotoCamera.IsCameraTypeSupported(CameraType.Primary) == true) ||
                 (PhotoCamera.IsCameraTypeSupported(CameraType.FrontFacing) == true))
            {
                // Initialize the camera, when available.
                if (PhotoCamera.IsCameraTypeSupported(CameraType.FrontFacing))
                {
                    // Use front-facing camera if available.
                    cam = new Microsoft.Devices.PhotoCamera(CameraType.FrontFacing);
                }
                else
                {
                    // Otherwise, use standard camera on back of phone.
                    cam = new Microsoft.Devices.PhotoCamera(CameraType.Primary);
                }

                // Event is fired when the capture sequence is complete and an image is available.
                cam.CaptureImageAvailable += new EventHandler<Microsoft.Devices.ContentReadyEventArgs>(cam_CaptureImageAvailable);

                //Set the VideoBrush source to the camera.
                viewfinderBrush.SetSource(cam);

            }
            else
            {
                // The camera is not supported on the phone.
                this.Dispatcher.BeginInvoke(delegate()
                {
                    // Write message.
                    //txtDebug.Text = "A Camera is not available on this phone.";
                });

                // Disable UI.
                appBarCamera.IsEnabled = false;
            }
        }
        
        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (cam != null)
            {
                // Dispose camera to minimize power consumption and to expedite shutdown.
                cam.Dispose();

                // Release memory, ensure garbage collection.
                cam.CaptureImageAvailable -= cam_CaptureImageAvailable;
            }
        }


        // AACODE: Informs when full resolution photo has been taken, saves to variable App.gCapturedImage
        void cam_CaptureImageAvailable(object sender, Microsoft.Devices.ContentReadyEventArgs e)
        {
            string fileName = App.gSavedCounter + ".jpg";

            try
            {
                // Write message to the UI thread.
                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    //txtDebug.Text = "Captured image available";
                });

                // AACODE: Passed this code to UI thread because it was causing invalid cross-thread exception
                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    // Load the captured image stream to the global gCapturedImage variable
                    App.gCapturedImage = new WriteableBitmap((int)cam.Resolution.Width, (int)cam.Resolution.Height);
                    e.ImageStream.Position = 0;
                    App.gCapturedImage.LoadJpeg(e.ImageStream);
                    e.ImageStream.Close();

                    // If photo has been taken, navigates to PreviewPhoto Page.
                    if (App.gBACK_KEY_NAVIGATION == 0)
                    {
                        App.gFrom = "TakePhotoPage";
                        App.gAction = "PreviewPhoto";
                        NavigationService.GoBack();
                    }
                    else
                    {
                        NavigationService.Navigate(new Uri(App.GetDynamicUri("TakePhotoPage", "PreviewPhoto"), UriKind.Relative));
                    }

                });
            }
            catch (Exception ex)
            {
                // TODO: Add error handling
            }

        }

        // AACODE: Takes a picture when camera button is clicked
        // TODO: enable taking pic on viewfinder tap
        private void appBarCamera_Click(object sender, EventArgs e)
        {
            if (cam != null)
            {
                try
                {
                    // Start image capture.
                    cam.CaptureImage();
                }
                catch (Exception ex)
                {
                    // TODO: Add error handling
                }
            }

        }

        // AACODE: Modified sample code for building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsVisible = true;

            // Initialize buttons, set events, set text values to the localized string from AppResources, & add to the application bar
            // 'Take picture' button
            appBarCamera = new ApplicationBarIconButton(new Uri("/Assets/AppBar/feature.camera.png", UriKind.Relative));
            appBarCamera.Text = AppResources.AppBarCameraText;
            appBarCamera.Click += new EventHandler(appBarCamera_Click);
            ApplicationBar.Buttons.Add(appBarCamera);


        }
    }
}
