using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MigrationExamples.OpenSilver.Pages.WriteableBitmapWorkaround
{
    public partial class WriteableBitmapWorkaround : UserControl
    {
        public WriteableBitmapWorkaround()
        {
            this.InitializeComponent();
            this.Loaded += WriteableBitmapWorkaround_Loaded;
        }

        private void WriteableBitmapWorkaround_Loaded(object sender, RoutedEventArgs e)
        {
            Color[] colors =
            {
                Colors.Red,
                Colors.Green,
                Colors.Blue,
                Colors.Pink,
                Colors.Coral,
                Colors.Yellow,
                Colors.Silver,
                Colors.SkyBlue,
            };

            int size = 200;
            var pixels = BitmapPixelsGenerator.GeneratePixels(size, colors);
            var source = BitmapSourceGenerator.GetBitmapSource(size, size, pixels);
            MyImage.Source = source;
        }
    }
}
