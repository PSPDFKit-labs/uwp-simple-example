using PSPDFKit.Document;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Popups;

namespace UWPSimpleExample
{
    public static class FileUtils
    {
        public static async Task<IEnumerable<StorageFile>> PickMultipleFiles(string[] extensions)
        {
            try
            {
                // Open a Picker so the user can choose multiple Pdf
                FileOpenPicker picker = new FileOpenPicker()
                {
                    ViewMode = PickerViewMode.Thumbnail,
                    SuggestedStartLocation = PickerLocationId.DocumentsLibrary
                };

                if (extensions.Length == 0)
                {
                    throw new ArgumentException("At least one file extension must be supplied to the file picker.");
                }

                foreach (string extension in extensions)
                {
                    picker.FileTypeFilter.Add(extension);
                }

                // Multiple single files need to be added to either the MostRecentUsed or FutureAccess lists for further usage.
                // For demonstration purposes we're using the MRU managed by UWP, which has a limit of 25 files.
                List<StorageFile> files = (await picker.PickMultipleFilesAsync()).Take(25).ToList();

                foreach (StorageFile file in files)
                {
                    StorageApplicationPermissions.MostRecentlyUsedList.Add(file);
                }

                return files;
            }
            catch (Exception ex)
            {
                MessageDialog messageDialog = new MessageDialog(ex.Message);
                await messageDialog.ShowAsync();
                return null;
            }
        }

        public static async Task<IEnumerable<StorageFile>> PickMultipleFiles(string extension)
        {
            return await PickMultipleFiles(new[] { extension });
        }

        public static async Task<StorageFile> PickFileToOpenAsync(string[] extensions)
        {
            try
            {
                // Open a Picker so the user can choose a Pdf
                FileOpenPicker picker = new FileOpenPicker
                {
                    ViewMode = PickerViewMode.Thumbnail,
                    SuggestedStartLocation = PickerLocationId.DocumentsLibrary
                };

                if (extensions.Length == 0)
                {
                    throw new ArgumentException("At least one file extension must be supplied to the file picker.");
                }

                foreach (string extension in extensions)
                {
                    picker.FileTypeFilter.Add(extension);
                }

                return await picker.PickSingleFileAsync();
            }
            catch (Exception ex)
            {
                MessageDialog messageDialog = new MessageDialog(ex.Message);
                await messageDialog.ShowAsync();
                return null;
            }
        }

        public static async Task<StorageFile> PickFileToOpenAsync(string extension)
        {
            return await PickFileToOpenAsync(new[] { extension });
        }

        public static async Task<StorageFile> PickFileToSaveAsync(string extension)
        {
            try
            {
                FileSavePicker savePicker = new FileSavePicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
                savePicker.FileTypeChoices.Add("File", new List<string> { extension });
                savePicker.SuggestedFileName = "NewFile";

                return await savePicker.PickSaveFileAsync();
            }
            catch (Exception ex)
            {
                MessageDialog messageDialog = new MessageDialog(ex.Message);
                await messageDialog.ShowAsync();
                return null;
            }
        }

        public static async Task<StorageFile> CopyFileToTempFolderAsync(Uri fileToCopy)
        {
            StorageFile pdf = await StorageFile.GetFileFromApplicationUriAsync(fileToCopy);
            return await pdf.CopyAsync(ApplicationData.Current.TemporaryFolder, pdf.Name, NameCollisionOption.ReplaceExisting);
        }
    }
}
