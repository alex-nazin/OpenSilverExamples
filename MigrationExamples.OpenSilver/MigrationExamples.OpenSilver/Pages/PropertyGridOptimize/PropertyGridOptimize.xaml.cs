using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace MigrationExamples.OpenSilver.Pages.PropertyGridOptimize
{
    public partial class PropertyGridOptimize : UserControl
    {
        public PropertyGridOptimize()
        {
            this.InitializeComponent();
            this.Loaded += PropertyGridOptimize_Loaded;
        }

        private void PropertyGridOptimize_Loaded(object sender, RoutedEventArgs e)
        {
            PropertyGridWithOptimization.Group = OptGrouping.IsChecked.GetValueOrDefault();
            PropertyGridWithoutOptimization.Group = NonOptGrouping.IsChecked.GetValueOrDefault();

            PropertyGridWithoutOptimization.ItemSource = new ObservableCollection<PropertyItem>(
                PropertyItemsGenerator.Generate());
            PropertyGridWithOptimization.ItemSource = new ObservableCollection<PropertyItem>(
                PropertyItemsGenerator.Generate());
        }

        private void OptGrouping_Checked(object sender, RoutedEventArgs e)
        {
            PropertyGridWithOptimization.Group = true;
        }

        private void OptGrouping_Unchecked(object sender, RoutedEventArgs e)
        {
            PropertyGridWithOptimization.Group = false;
        }

        private void NonOptGrouping_Checked(object sender, RoutedEventArgs e)
        {
            PropertyGridWithoutOptimization.Group = true;
        }

        private void NonOptGrouping_Unchecked(object sender, RoutedEventArgs e)
        {
            PropertyGridWithoutOptimization.Group = false;
        }

        private void NonOptSetNewItems_Click(object sender, RoutedEventArgs e)
        {
            PropertyGridWithoutOptimization.ItemSource = new ObservableCollection<PropertyItem>(
                PropertyItemsGenerator.Generate());
        }

        private void OptSetNewItems_Click(object sender, RoutedEventArgs e)
        {
            PropertyGridWithOptimization.ItemSource = new ObservableCollection<PropertyItem>(
                PropertyItemsGenerator.Generate());
        }
    }
}
