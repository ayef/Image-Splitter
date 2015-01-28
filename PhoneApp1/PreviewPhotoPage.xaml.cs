/*
 * Created By:  Ayesha Ahmad
 * Date:        13-Feb-2013
 * Purpose:     Preview page for captured image  
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

namespace PhoneApp1
{
    /// <summary>
    /// Preview page for captured image  
    /// </summary>
    public partial class PreviewPhotoPage : PhoneApplicationPage
    {
        public PreviewPhotoPage()
        {
            InitializeComponent();
            Deployment.Current.Dispatcher.BeginInvoke(delegate()
            {
                DoPreviewImage.Source = App.gCapturedImage;
            });
        }

        // AACODE: If User accepts this image, navigate to the ChannelSplitter Page
        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            if (App.gBACK_KEY_NAVIGATION == 0)
            {
                App.gFrom = "PreviewPhotoPage";
                App.gAction = "ProcessImage";
                NavigationService.GoBack();
            }
            else 
            {
                NavigationService.Navigate(new Uri(App.GetDynamicUri("PreviewPhotoPage", "ProcessImage"), UriKind.Relative));
            }

        }

        // AACODE: If User wants to retake image, navigate back to the Main Page
        private void Retake_Click(object sender, RoutedEventArgs e)
        {
            if (App.gBACK_KEY_NAVIGATION == 0)
            {
                App.gFrom = "PreviewPhotoPage";
                App.gAction = "Retake";
                NavigationService.GoBack();
            }
            else
            {
                NavigationService.GoBack();
            }
        }


    }
}