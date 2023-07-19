using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MigrationExamples.OpenSilver.Pages.CustomCursor
{
    public partial class CustomCursor : Page
    {
        private readonly MyCustomCursorViewModel myCustomCursorViewModel = new MyCustomCursorViewModel();

        public CustomCursor()
        {
            this.InitializeComponent();
            this.Loaded += CustomCursor_Loaded;
            this.Unloaded += CustomCursor_Unloaded;
        }

        private void CustomCursor_Loaded(object sender, RoutedEventArgs e)
        {
            SystemCursorCheck.IsChecked = true;
            MyCursor.DataContext = myCustomCursorViewModel;
        }

        private void CustomCursor_Unloaded(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Arrow;
            MouseMove -= CustomCursor_MouseMove;
        }

        private void SystemCursorCheck_Checked(object sender, RoutedEventArgs e)
        {
            MouseMove -= CustomCursor_MouseMove;
            myCustomCursorViewModel.Visibility = Visibility.Collapsed;
            Cursor = Cursors.Arrow;
        }

        private void CustomCursorCheck_Checked(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.None;
            myCustomCursorViewModel.Visibility = Visibility.Visible;
            MouseMove += CustomCursor_MouseMove;
        }

        private void CustomCursor_MouseMove(object sender, MouseEventArgs e)
        {
            var coords = e.GetPosition(this);
            myCustomCursorViewModel.MoveTo(coords);
        }
    }
}
