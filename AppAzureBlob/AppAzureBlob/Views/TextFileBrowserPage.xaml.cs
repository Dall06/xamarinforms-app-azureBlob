using AppAzureBlob.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppAzureBlob.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TextFileBrowserPage : ContentPage
    {
        string FileName = "";
        public TextFileBrowserPage()
        {
            InitializeComponent();
        }

        private async void GetFile_Clicked(object sender, EventArgs e)
        {
            try
            {
                var filesLst = await AzureService.GetFilesListAsync(AzureContainer.Text);
                lstVFiles.ItemsSource = filesLst;

                txtPreview.Text = "";
                btnDelete.IsEnabled = false;

            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
                await Task.Delay(5000);
                lblMessage.Text = "";
            }
        }

        private async void btnDelete_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(FileName))
            {
                try
                {
                    bool isDeleted = await AzureService.DeleteFileAsync(AzureContainer.Text, FileName);
                    if (isDeleted)
                    {
                        GetFile_Clicked(sender, e);
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = ex.Message;
                    await Task.Delay(5000);
                    lblMessage.Text = "";
                }
            }
        }

        private async void lstVFiles_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                if (e.SelectedItem != null)
                {
                    FileName = e.SelectedItem.ToString();
                    var byteData = await AzureService.GetFileAsync(AzureContainer.Text, FileName);
                    var text = Encoding.UTF8.GetString(byteData);
                    txtPreview.Text = text;
                    btnDelete.IsEnabled = true;
                }
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