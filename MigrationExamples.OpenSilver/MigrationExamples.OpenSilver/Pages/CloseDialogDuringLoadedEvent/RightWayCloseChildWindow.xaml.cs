using System.Windows;
using System.Windows.Controls;

namespace MigrationExamples.OpenSilver.Pages.CloseDialogDuringLoadedEvent
{
    public partial class RightWayCloseChildWindow : ChildWindow
    {
        public RightWayCloseChildWindow()
        {
            InitializeComponent();
            this.Loaded += RightWayCloseChildWindowChildWindow_Loaded;
        }

        private void RightWayCloseChildWindowChildWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => Close());
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

