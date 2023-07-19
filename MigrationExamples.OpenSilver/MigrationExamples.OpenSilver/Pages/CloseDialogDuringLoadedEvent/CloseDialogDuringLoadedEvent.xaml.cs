using System.Windows;
using System.Windows.Controls;

namespace MigrationExamples.OpenSilver.Pages.CloseDialogDuringLoadedEvent
{
    public partial class CloseDialogDuringLoadedEvent : Page
    {
        public CloseDialogDuringLoadedEvent()
        {
            this.InitializeComponent();
        }

        private void TestWrongImplementation_Click(object sender, RoutedEventArgs e)
        {
            WrongWayCloseChildWindow childWindow = new WrongWayCloseChildWindow();
            childWindow.Show();
        }

        private void TestRightImplementation_Click(object sender, RoutedEventArgs e)
        {
            RightWayCloseChildWindow childWindow = new RightWayCloseChildWindow();
            childWindow.Show();
        }
    }
}
