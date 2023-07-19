using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using static CSharpXamlForHtml5.DomManagement;

namespace MigrationExamples.OpenSilver.Pages.UploadFiles
{
    public partial class UploadFiles : Page
    {
        public UploadFiles()
        {
            this.InitializeComponent();
        }

        private void UploadViaOpenFileDialog_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialogResult.Text = string.Empty;

            try
            {
                OpenFileDialog fileDialog = new OpenFileDialog
                {
                    Filter = "Any Files |*.*"
                };
                bool? ok = fileDialog.ShowDialog();

                switch (ok)
                {
                    case true:
                        var size = fileDialog.File.Length;
                        OpenFileDialogResult.Text = $"Successfully read {size} bytes";
                        break;

                    case false:
                        OpenFileDialogResult.Text = $"Try to display dialog was failed";
                        break;

                    case null:
                        OpenFileDialogResult.Text = $"Nothing happens after ShowDialog() call";
                        break;
                }
            }
            catch (Exception ex)
            {
                OpenFileDialogResult.Text = ex.GetType().Name;
            }
        }

        private async void UploadViaJsBasedFunction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var results = await FileUploaderDialog.UploadFiles("Any Files (*) |*.*", false, ResultKind.DataURL);

                if (results.Count > 0)
                {
                    FileReadResult fileReadResult = results[0];
                    if (fileReadResult.Text.StartsWith("data"))
                    {
                        fileReadResult.Text = fileReadResult.Text.Substring(fileReadResult.Text.LastIndexOf(',') + 1);
                    }

                    byte[] bytes = Convert.FromBase64String(fileReadResult.Text.Trim());
                    JsBasedFunctionResult.Text = $"Successfully read {bytes.Length} bytes";
                }
            }
            catch (Exception ex)
            {
                JsBasedFunctionResult.Text = ex.GetType().Name;
            }
        }
    }
}
