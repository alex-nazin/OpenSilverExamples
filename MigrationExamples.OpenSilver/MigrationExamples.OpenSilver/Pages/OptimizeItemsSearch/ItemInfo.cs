using System.ComponentModel;
using System.Windows;

namespace MigrationExamples.OpenSilver.Pages.HideSomeItems
{
    public class ItemInfo : INotifyPropertyChanged
    {
        private Visibility _visibility = Visibility.Visible;

        public string MainText { get; set; }
        public string AdditionalText { get; set; }
        public string DetailText { get; set; }
        public Visibility Visibility
        {
            get { return _visibility; }
            set { _visibility = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Visibility))); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
