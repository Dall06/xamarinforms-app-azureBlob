using AppAzureBlob.Services;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppAzureBlob.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImageFileUploaderPage : ContentPage
    {
        byte[] byteData;
        public ImageFileUploaderPage()
        {
            InitializeComponent();
        }

        private async void btnTakePicture_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (Device.RuntimePlatform == Device.UWP)
                {
                    await CrossMedia.Current.Initialize();
                }

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await Application.Current.MainPage.DisplayAlert("No Camera", ":( No camera available.", "OK");
                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Sample",
                    Name = "test.jpg"
                });

                if (file == null)
                    return;

                byteData = await Helper.Converters.FileToByteArray(file.Path);
                ImageFile.Source = ImageSource.FromStream(() => new MemoryStream(byteData));
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
                await Task.Delay(5000);
                lblMessage.Text = "";
            }
        }

        private async void btnSelectPicture_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (Device.RuntimePlatform == Device.UWP)
                {
                    await CrossMedia.Current.Initialize();
                }

                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Not supported", "OK");
                    return;
                }

                var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
                });

                if (file == null)
                    return;

                byteData = await Helper.Converters.FileToByteArray(file.Path);
                ImageFile.Source = ImageSource.FromStream(() => new MemoryStream(byteData));
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
                await Task.Delay(5000);
                lblMessage.Text = "";
            }
        }

        private async void btnUploadPicture_Clicked(object sender, EventArgs e)
        {
            if (byteData != null && byteData.Length > 0)
            {
                try
                {
                    btnUploadPicture.IsEnabled = false;
                    actIndicator.IsRunning = true;

                    await AzureService.UploadFileAsync(AzureContainer.Images, new MemoryStream(byteData));

                    btnUploadPicture.IsEnabled = true;
                    actIndicator.IsRunning = false;

                    byteData = null;
                    ImageFile.Source = null;
                }
                catch (Exception ex)
                {
                    lblMessage.Text = ex.Message;
                    await Task.Delay(5000);
                    lblMessage.Text = "";
                }
            }
        }

    }
}