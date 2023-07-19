using System.ComponentModel;
using System.Windows;

namespace MigrationExamples.OpenSilver.Pages.CustomCursor
{
    public class MyCustomCursorViewModel
        : INotifyPropertyChanged
    {
        private Visibility visibility;
        private double left;
        private double top;

        public event PropertyChangedEventHandler PropertyChanged;

        public void MoveTo(Point pt)
        {
            Left = pt.X;
            Top = pt.Y;
        }

        public Visibility Visibility
        {
            get { return visibility; }
            set { visibility = value; NotifyOfPropertyChange(nameof(Visibility)); }
        }

        public string ImageUrl
        {
            get { return "DDCursor.png"; }
        }

        public double Left
        {
            get { return left; }
            set { left = value; NotifyOfPropertyChange(nameof(Left)); }
        }

        public double Top
        {
            get { return top; }
            set { top = value; NotifyOfPropertyChange(nameof(Top)); }
        }

        private void NotifyOfPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
