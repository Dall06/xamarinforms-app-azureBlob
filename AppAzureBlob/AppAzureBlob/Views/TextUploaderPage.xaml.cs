using AppAzureBlob.Services;
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
    public partial class TextUploaderPage : ContentPage
    {
        public TextUploaderPage()
        {
            InitializeComponent();
        }

        private async void btnUppload_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtInfo.Text))
            {
                try
                {
                    btnUppload.IsEnabled = false;
                    actIndicator.IsRunning = true;

                    var byteData = Encoding.UTF8.GetBytes(txtInfo.Text);

                    await AzureService.UploadFileAsync(AzureContainer.Text, new MemoryStream(byteData));

                    btnUppload.IsEnabled = true;
                    actIndicator.IsRunning = false;
                    txtInfo.Text = "";
                }
                catch (Exception ex)
                {
                    lblMessage.Text = ex.Message;
                    await Task.Delay(5000);
                    lblMessage.Text = "";
                }
            }
            else
            {
                lblMessage.Text = "Capture text to upload it to Azure";
                await Task.Delay(5000);
                lblMessage.Text = "";
            }
        }
    }
}