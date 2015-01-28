/*
 * Created By:  Ayesha Ahmad
 * Date:        12-Feb-2013
 * Purpose:     Main application for PictureChannelSplitter project. 
 *              Takes an image as input and creates a new equal-sized image composed of a grid of 4 tiles, 
 *              each tile displaying one of the following: red, green and blue channels, and grayscale
 *
 * My own code commented with tag: // AACODE:
 * 
 * CODE REFERENCES FOR THIS PROJECT 
 * 
 * 1.   "How to choose photo or take a new one in Windows Phone 7"
 *      http://windowsphonegeek.com/articles/2-how-to-choose-photo-or-take-a-new-one-in-windows-phone-7
 * 2.   Sample code from Windows Phone Dev Center "Photos Sample" 
 *      http://code.msdn.microsoft.com/wpapps/Photos-Sample-a38a2c8e
 * 3.   Capturing photos for Windows Phone
 *      http://msdn.microsoft.com/en-us/library/windowsphone/develop/jj207050%28v=vs.105%29.aspx
 * 4.   How to create a base camera app for Windows Phone
 *      http://msdn.microsoft.com/en-us/library/windowsphone/develop/hh202956%28v=vs.105%29.aspx
 * 5.   How to handle manipulation events for Windows Phone
 *      http://msdn.microsoft.com/en-us/library/windowsphone/develop/ff426933%28v=vs.105%29.aspx
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
    /// This class contains the Start page for this app. Asks the user to take a photo or select a photo from file. 
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        // The photo chooser used to select a picture from the Media Library
        PhotoChooserTask photoChooserTask;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
 
            // AACODE: Initialize variables, register all relevant events
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // AACODE: Variable App.gBACK_KEY_NAVIGATION is ALWAYS equal to 1 so this code is never called.
            if (App.gBACK_KEY_NAVIGATION == 0)
            {
                string from = App.gFrom;
                string action = App.gAction;
                if (from != "" && action != "")
                {
                    App.gFrom = App.gAction = "";
                    NavigationService.Navigate(new Uri(App.GetDynamicUri(from, action), UriKind.Relative));
                }
                else
                {
                    LayoutRoot.Visibility = System.Windows.Visibility.Visible;
                }
            }
           base.OnNavigatedTo(e);
        }

        // AACODE: Open PhotoChooser
        private void OnSelectPhoto_Click(object sender, RoutedEventArgs e)
        {
            photoChooserTask.Show();
        }

        // AACODE: Get image from photo chooser
        private void photoChooserTask_Completed(Object sender, PhotoResult e)
        {
            try 
            {
                // If a photo has been selected then take action, otherwise do nothing.
                if (e.ChosenPhoto != null)
                {
                    App.gCapturedImage = PictureDecoder.DecodeJpeg(e.ChosenPhoto);

                    // If photo has been chosen, navigate to ChannelSplitter Page.
                    NavigationService.Navigate(new Uri(App.GetDynamicUri("MainPage", "ProcessImage"), UriKind.Relative));
                }
            }
            catch (Exception ex)
            {
                this.Dispatcher.BeginInvoke(delegate()
                {
                    // TODO: Add erro
                    // txtDebug.Text = ex.Message;
                });
            }
            
        }

        private void OnTakeAPhoto_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(App.GetDynamicUri("MainPage", "TakePhoto" ), UriKind.Relative));
        }

        private void OnSelectFromFile_Click(object sender, RoutedEventArgs e)
        {
            photoChooserTask.Show();

        }
    }
}