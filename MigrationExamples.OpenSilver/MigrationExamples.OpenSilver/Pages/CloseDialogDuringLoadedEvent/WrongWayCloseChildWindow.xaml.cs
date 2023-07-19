using System.Windows;
using System.Windows.Controls;

namespace MigrationExamples.OpenSilver.Pages.CloseDialogDuringLoadedEvent
{
    public partial class WrongWayCloseChildWindow : ChildWindow
    {
        public WrongWayCloseChildWindow()
        {
            InitializeComponent();
            this.Loaded += WrongWayCloseChildWindow_Loaded;
        }

        private void WrongWayCloseChildWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

