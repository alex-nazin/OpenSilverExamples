using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MigrationExamples.OpenSilver.Pages.HideSomeItems
{
    public partial class OptimizeItemsSearch : Page
    {
        private ItemInfo[] slowApproachItems;
        private ItemInfo[] fastApproachItems;

        public OptimizeItemsSearch()
        {
            this.InitializeComponent();
            this.Loaded += HideSomeItems_Loaded;
        }

        private void HideSomeItems_Loaded(object sender, RoutedEventArgs e)
        {
            fastApproachItems = ItemInfoGenerator.GenerateItems();
            slowApproachItems = ItemInfoGenerator.GenerateItems();

            FastApproachList.ItemsSource = new ObservableCollection<ItemInfo>(fastApproachItems);
            SlowApproachList.ItemsSource = new ObservableCollection<ItemInfo>(slowApproachItems);
        }

        private void SlowApproach_TextChanged(object sender, TextChangedEventArgs e)
        {
            Stopwatch sw = Stopwatch.StartNew();

            if (string.IsNullOrEmpty(SlowApproachTextBox.Text))
            {
                SlowApproachList.ItemsSource = new ObservableCollection<ItemInfo>(slowApproachItems);
            }
            else
            {
                SlowApproachList.ItemsSource = new ObservableCollection<ItemInfo>(slowApproachItems
                    .Where(i => i.MainText.IndexOf(SlowApproachTextBox.Text, StringComparison.InvariantCultureIgnoreCase) != -1));
            }

            sw.Stop();
            SlowApproachTime.Text = $"Filled in {sw.ElapsedMilliseconds} ms, count is {SlowApproachList.ItemsSource.OfType<ItemInfo>().Count()}";
        }

        private void FastApproach_TextChanged(object sender, TextChangedEventArgs e)
        {
            Stopwatch sw = Stopwatch.StartNew();

            ObservableCollection<ItemInfo> fastApproachItems = FastApproachList.ItemsSource as ObservableCollection<ItemInfo>;

            if (string.IsNullOrEmpty(FastApproachTextBox.Text))
            {
                foreach (var item in fastApproachItems)
                {
                    item.Visibility = Visibility.Visible;
                }
            }
            else
            {
                foreach (var item in fastApproachItems)
                {
                    item.Visibility = item.MainText.IndexOf(FastApproachTextBox.Text, StringComparison.InvariantCultureIgnoreCase) != -1
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
            }

            sw.Stop();
            FastApproachTime.Text = $"Filled in {sw.ElapsedMilliseconds} ms, count is {fastApproachItems.Count(i => i.Visibility == Visibility.Visible)}";
        }
    }
}
