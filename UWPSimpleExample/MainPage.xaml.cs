using PSPDFKit.Document;
using PSPDFKit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using PSPDFKit.Pdf;
using Windows.Storage;
using PSPDFKitFoundation;
using Windows.UI.Popups;
using Windows.Storage.Streams;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPSimpleExample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private IRandomAccessStream _fileStream;

        public MainPage()
        {
            this.InitializeComponent();
            PdfView.InitializationCompletedHandler += OnPdfViewInitializationCompleted;
        }

        private void OnPdfViewInitializationCompleted(PdfView sender, Document doc)
        {
            Button_OpenPDF.IsEnabled = true;
        }

        private async void Button_OpenPDF_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".pdf");


            //default way to open a pdf document

            //var file = await picker.PickSingleFileAsync();
            //if (file != null)
            //{
            //    var document = DocumentSource.CreateFromStorageFile(file);
            //    await PdfView.Controller.ShowDocumentAsync(document);
            //}

            //using ExampleDataProvider to open a pdf file
            try
            {
                StorageFile file = await FileUtils.PickFileToOpenAsync(".pdf");
                if (file == null)
                {
                    return;
                }

                _fileStream?.Dispose();
                _fileStream = await file.OpenAsync(FileAccessMode.ReadWrite);
                ExampleDataProvider dataProvider = new ExampleDataProvider(_fileStream);
                DocumentSource documentSource = DocumentSource.CreateFromDataProvider(dataProvider);
                await PdfView.Controller.ShowDocumentAsync(documentSource);
            }
            catch (Exception ex)
            {
                MessageDialog messageDialog = new MessageDialog(ex.Message);
                await messageDialog.ShowAsync();
            }
        }
    }
}
