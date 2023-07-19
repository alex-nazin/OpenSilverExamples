using System;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;

namespace MigrationExamples.OpenSilver
{
    public partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!HtmlPage.Document.DocumentUri.OriginalString.Contains("#"))
            {
                NavigateToPage("/Home");
            }

            this.Loaded -= MainPage_Loaded;
        }

        private void ButtonHome_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage("/Home");
        }

        private void WriteableBitmapWorkaround_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage("/WriteableBitmapWorkaround");
        }

        private void PropertyGridOptimization_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage("/PropertyGridOptimize");
        }

        private void FixRectangleFill_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage("/ShapesGradientFill");
        }

        private void RemoveCustomCursor_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage("/CustomCursor");
        }

        private void HttpClientUsage_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage("/HttpClientUsage");
        }

        private void RightWayToCloseChildWindow_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage("/CloseDialogDuringLoadedEvent");
        }

        private void OptimizeItemsSearch_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage("/OptimizeItemsSearch");
        }

        private void ShapesRemoving_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage("/ShapesRemoving");
        }

        private void SolutionForUploadFiles_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage("/UploadFiles");
        }

        private void ReplaceAnimationWithGif_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage("/ReplaceAnimationWithGif");
        }

        private void ObservableCollectionManagement_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage("/CollectionManagement");
        }

        private void ItemsInstantiation_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage("/ObjectInstantiation");
        }

        private void NavigateToPage(string targetUri)
        {
            // Navigate to the target page:
            Uri uri = new Uri(targetUri, UriKind.Relative);
            PageContainer.Source = uri;

            // Scroll to top:
            ContentScrollViewer.ScrollToVerticalOffset(0d);
        }
    }
}
