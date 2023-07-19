using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MigrationExamples.OpenSilver.Pages.PropertyGridOptimize
{
    public partial class PropertyGridNew : UserControl
    {
        public static readonly DependencyProperty ItemSourceProperty = DependencyProperty.Register(
                        "ItemSource", typeof(ObservableCollection<PropertyItem>), typeof(PropertyGridNew),
                        new PropertyMetadata(ItemSourcePropertyChanged));
        public ObservableCollection<PropertyItem> ItemSource
        {
            get { return (ObservableCollection<PropertyItem>)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        private static void ItemSourcePropertyChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            ((PropertyGridNew)sender).SetGrid();
        }

        public static readonly DependencyProperty GroupProperty = DependencyProperty.Register(
                                "Group", typeof(bool), typeof(PropertyGridNew),
                                new PropertyMetadata(GroupPropertyChanged));
        public bool Group
        {
            get { return (bool)GetValue(GroupProperty); }
            set { SetValue(GroupProperty, value); }
        }

        private static void GroupPropertyChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            ((PropertyGridNew)sender).SetGrid();
        }

        private class CachedRow
        {
            public string Name { get; set; }
            public Border CaptionPart { get; set; }
            public Border ValuePart { get; set; }
        }

        private FrameworkElement firstElement;
        private readonly LinearGradientBrush defaultTitleBrush;
        private readonly LinearGradientBrush focusedTitleBrush;
        private bool isGridSet;
        private List<string> closedGroups = new List<string>();
        private List<CachedRow> nonGroupedCachedRows = new List<CachedRow>();
        private Dictionary<string, List<CachedRow>> groupedCachedRows = new Dictionary<string, List<CachedRow>>();

        public PropertyGridNew()
        {
            this.InitializeComponent();

            defaultTitleBrush = new LinearGradientBrush { StartPoint = new Point(0, 0.1), EndPoint = new Point(0, 0.9) };
            defaultTitleBrush.GradientStops.Add(new GradientStop { Color = Color.FromArgb(255, 167, 179, 189), Offset = 0 });
            defaultTitleBrush.GradientStops.Add(new GradientStop { Color = Color.FromArgb(255, 125, 142, 157), Offset = 1 });

            focusedTitleBrush = new LinearGradientBrush { StartPoint = new Point(0, 0.1), EndPoint = new Point(0, 0.9) };
            focusedTitleBrush.GradientStops.Add(new GradientStop { Color = Color.FromArgb(255, 154, 168, 183), Offset = 1 });
            focusedTitleBrush.GradientStops.Add(new GradientStop { Color = Color.FromArgb(255, 208, 215, 220), Offset = 0 });
        }

        private void SetGrid()
        {
            if (Group)
            {
                LayoutRootNonGrouped.Visibility = Visibility.Collapsed;
                ClearDataContexts();
                SetGridGrouped();
                LayoutRoot.Visibility = Visibility.Visible;
            }
            else
            {
                LayoutRoot.Visibility = Visibility.Collapsed;
                ClearDataContexts();
                SetGridSimple(ItemSource ?? Enumerable.Empty<PropertyItem>(), CaptionsContainer, ValuesContainer, nonGroupedCachedRows);
                LayoutRootNonGrouped.Visibility = Visibility.Visible;
            }

            isGridSet = true;
        }

        private void SetGridSimple(IEnumerable<PropertyItem> items, StackPanel captionsContainer, StackPanel valuesContainer, List<CachedRow> cache)
        {
            foreach (PropertyItem item in items)
            {
                CachedRow row = cache.FirstOrDefault(_ => _.Name == item.Name);

                if (row == null)
                {
                    row = new CachedRow
                    {
                        Name = item.Name,
                        CaptionPart = CreateCaptionPart(),
                        ValuePart = CreateValuePart(item),
                    };

                    int index = 0;
                    for (; index < cache.Count; ++index)
                    {
                        int compareResult = string.Compare(row.Name, cache[index].Name);
                        if (compareResult < 0)
                        {
                            break;
                        }
                    }

                    cache.Insert(index, row);

                    captionsContainer.Children.Insert(index, row.CaptionPart);
                    valuesContainer.Children.Insert(index, row.ValuePart);
                }

                row.CaptionPart.DataContext = item;
                row.ValuePart.DataContext = item;
            }

            foreach (FrameworkElement captionElement in captionsContainer.Children.Cast<FrameworkElement>())
            {
                captionElement.Visibility = captionElement.DataContext != null
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }

            foreach (FrameworkElement valueElement in valuesContainer.Children.Cast<FrameworkElement>())
            {
                valueElement.Visibility = valueElement.DataContext != null
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        private void SetGridGrouped()
        {
            foreach (IGrouping<string, PropertyItem> group in (ItemSource ?? Enumerable.Empty<PropertyItem>()).GroupBy(_ => _.GroupName))
            {
                string groupName = group.Key;

                // Try to find existing group
                Grid groupGridWithTitle = LayoutRoot.Children.OfType<Grid>().FirstOrDefault(_ => (_.Tag as string) == groupName);
                if (groupGridWithTitle == null)
                {
                    groupGridWithTitle = CreateGroup(groupName);

                    int index = 0;
                    foreach (Grid child in LayoutRoot.Children.OfType<Grid>())
                    {
                        int compareResult = string.Compare(groupName, child.Tag as string);
                        if (compareResult < 0)
                        {
                            break;
                        }

                        ++index;
                    }

                    LayoutRoot.Children.Insert(index, groupGridWithTitle);
                }

                if (!groupedCachedRows.TryGetValue(groupName, out List<CachedRow> cache))
                {
                    cache = new List<CachedRow>();
                    groupedCachedRows.Add(groupName, cache);
                }

                Grid innerGrid = groupGridWithTitle.Children.OfType<Grid>().Single();

                if (closedGroups.Contains(groupName))
                {
                    innerGrid.Visibility = Visibility.Collapsed;
                }

                if (!isGridSet && groupName != "Common")
                {
                    innerGrid.Visibility = Visibility.Collapsed;
                    closedGroups.Add(groupName);
                }

                StackPanel captionsContainer = innerGrid.Children.OfType<StackPanel>().Single(_ => _.Name == groupName + "_CaptionsContainer");
                StackPanel valuesContainer = innerGrid.Children.OfType<StackPanel>().Single(_ => _.Name == groupName + "_ValuesContainer");

                SetGridSimple(group, captionsContainer, valuesContainer, cache);
            }

            foreach (Grid grid in LayoutRoot.Children.OfType<Grid>())
            {
                foreach (Grid innerGrid in grid.Children.OfType<Grid>())
                {
                    StackPanel anyContainer = innerGrid.Children.OfType<StackPanel>().First();
                    grid.Visibility = anyContainer.Children.OfType<FrameworkElement>().Any(_ => _.DataContext != null)
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
            }
        }

        private void ClearDataContexts()
        {
            // clear contexts for non-grouped values
            {
                foreach (StackPanel stackPanel in LayoutRootNonGrouped.Children.OfType<StackPanel>())
                {
                    foreach (FrameworkElement child in stackPanel.Children.Cast<FrameworkElement>())
                    {
                        child.DataContext = null;
                    }
                }
            }

            // clear context or grouped values
            {
                foreach (Grid grid in LayoutRoot.Children.OfType<Grid>())
                {
                    foreach (Grid innerGrid in grid.Children.OfType<Grid>())
                    {
                        foreach (StackPanel stackPanel in innerGrid.Children.OfType<StackPanel>())
                        {
                            foreach (FrameworkElement child in stackPanel.Children.OfType<FrameworkElement>())
                            {
                                try
                                {
                                    child.DataContext = null;
                                }
                                catch (Exception)
                                {
                                    // In some cases OpenSilver doesn't like null data context, but it is not important in our case
                                }
                            }
                        }
                    }
                }
            }
        }

        private Grid CreateGroup(string groupName)
        {
            Grid groupGridWithTitle = new Grid()
            {
                Tag = groupName,
            };

            groupGridWithTitle.RowDefinitions.Add(new RowDefinition { Height = new GridLength(18) });
            groupGridWithTitle.RowDefinitions.Add(new RowDefinition());

            Grid groupGrid = new Grid();

            if (closedGroups.Contains(groupName))
            {
                groupGrid.Visibility = Visibility.Collapsed;
            }

            if (!isGridSet && groupName != "Common")
            {
                groupGrid.Visibility = Visibility.Collapsed;
                closedGroups.Add(groupName);
            }

            groupGrid.ColumnDefinitions.Add(new ColumnDefinition());
            groupGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            groupGrid.ColumnDefinitions.Add(new ColumnDefinition { MinWidth = 100 });

            StackPanel captionsContainer = new StackPanel()
            {
                Name = groupName + "_CaptionsContainer"
            };
            Grid.SetColumn(captionsContainer, 0);
            groupGrid.Children.Add(captionsContainer);

            Border gridSplitter = new Border
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xA7, 0xB3, 0xBD)),
                Width = 3,
            };
            Grid.SetColumn(gridSplitter, 1);
            groupGrid.Children.Add(gridSplitter);

            StackPanel valuesContainer = new StackPanel()
            {
                Name = groupName + "_ValuesContainer"
            };
            Grid.SetColumn(valuesContainer, 2);
            groupGrid.Children.Add(valuesContainer);

            Grid groupTitle = new Grid();

            Polygon arrowClosed = new Polygon
            {
                Fill = new SolidColorBrush(Colors.White),
                VerticalAlignment = VerticalAlignment.Center,
                Visibility = Visibility.Collapsed,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 10, 0)
            };
            arrowClosed.Points.Add(new Point(0, 0));
            arrowClosed.Points.Add(new Point(5, 2.5));
            arrowClosed.Points.Add(new Point(0, 5));

            Polygon arrowOpen = new Polygon
            {
                Fill = new SolidColorBrush(Colors.White),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 10, 0)
            };
            arrowOpen.Points.Add(new Point(0, 0));
            arrowOpen.Points.Add(new Point(5, 0));
            arrowOpen.Points.Add(new Point(2.5, 5));

            groupTitle.Children.Add(arrowClosed);
            groupTitle.Children.Add(arrowOpen);

            groupTitle.Children.Add(new TextBlock
            {
                Text = groupName,
                Margin = new Thickness(5, 0, 0, 0),
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.White)
            });

            Button button = new Button { Opacity = 0.1 };
            button.Click += new RoutedEventHandler(button_Click);
            button.GotFocus += new RoutedEventHandler(button_GotFocus);
            button.LostFocus += new RoutedEventHandler(button_LostFocus);
            groupTitle.Children.Add(button);

            Grid.SetRow(groupTitle, 0);
            Grid.SetRow(groupGrid, 1);

            Border titleBorder = new Border
            {
                BorderBrush = new SolidColorBrush(Color.FromArgb(255, 167, 179, 189)),
                BorderThickness = new Thickness(0, 0, 0, 1),
                Background = defaultTitleBrush
            };
            titleBorder.Child = groupTitle;

            groupGridWithTitle.Children.Add(titleBorder);
            groupGridWithTitle.Children.Add(groupGrid);

            return groupGridWithTitle;
        }

        private Border CreateCaptionPart()
        {
            TextBlock textBlock = new TextBlock
            {
                FontSize = 11,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5, 1, 1, 1),
                ClipToBounds = true,
            };

            Binding binding = new Binding("Name")
            {
                Mode = BindingMode.TwoWay,
            };

            textBlock.SetBinding(TextBlock.TextProperty, binding);

            Border border = new Border()
            {
                BorderThickness = new Thickness(0, 0, 0, 1),
                BorderBrush = new SolidColorBrush(Color.FromArgb(255, 167, 179, 189)),
                Child = textBlock,
                Height = 20,
            };

            return border;
        }

        private Border CreateValuePart(PropertyItem item)
        {
            FrameworkElement valueElement;
            if (item.HasOptions && !item.IsReadOnly)
            {
                valueElement = new ComboBox
                {
                    FontSize = 11,
                    Background = new SolidColorBrush(Colors.White),
                    BorderThickness = new Thickness(0),
                    DataContext = item,
                };

                Binding comboSourceBinding = new Binding(nameof(PropertyItem.Options));
                valueElement.SetBinding(ComboBox.ItemsSourceProperty, comboSourceBinding);

                Binding valueBinding = new Binding(nameof(PropertyItem.Value))
                {
                    Mode = BindingMode.TwoWay,
                };
                valueElement.SetBinding(ComboBox.SelectedItemProperty, valueBinding);

                Binding isEnabledBinding = new Binding(nameof(PropertyItem.IsReadOnly))
                {
                    Converter = new BooleanInverterConverter()
                };
                valueElement.SetBinding(ComboBox.IsEnabledProperty, isEnabledBinding);

                ComboBoxSelectBehavior comboBoxSelectBehavior = new ComboBoxSelectBehavior();
                Interaction.GetBehaviors(valueElement).Add(comboBoxSelectBehavior);
            }
            else if (item.IsBool && !item.IsReadOnly)
            {
                valueElement = new ComboBox
                {
                    FontSize = 11,
                    Background = new SolidColorBrush(Colors.White),
                    BorderThickness = new Thickness(0),
                };

                Binding comboSourceBinding = new Binding(nameof(PropertyItem.BoolOptions));
                valueElement.SetBinding(ComboBox.ItemsSourceProperty, comboSourceBinding);

                Binding valueBinding = new Binding(nameof(PropertyItem.Value))
                {
                    Mode = BindingMode.TwoWay,
                    Converter = new String2BoolConverter(),
                };
                valueElement.SetBinding(ComboBox.SelectedItemProperty, valueBinding);

                Binding isEnabledBinding = new Binding(nameof(PropertyItem.IsReadOnly))
                {
                    Converter = new BooleanInverterConverter()
                };
                valueElement.SetBinding(ComboBox.IsEnabledProperty, isEnabledBinding);

                ComboBoxSelectBehavior comboBoxSelectBehavior = new ComboBoxSelectBehavior();
                Interaction.GetBehaviors(valueElement).Add(comboBoxSelectBehavior);
            }
            else
            {
                valueElement = new TextBox
                {
                    FontSize = 11,
                    Background = new SolidColorBrush(Colors.Transparent),
                    Foreground = item.IsReadOnly ? new SolidColorBrush(Color.FromArgb(255, 110, 110, 110)) : new SolidColorBrush(Colors.Black),
                    VerticalAlignment = VerticalAlignment.Stretch,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    BorderThickness = new Thickness(0),
                };

                Binding valueBinding = new Binding(nameof(PropertyItem.Value))
                {
                    Mode = BindingMode.TwoWay,
                    ValidatesOnExceptions = true,
                    NotifyOnValidationError = true,
                    Converter = item.IsBool ? new String2BoolConverter() : null,
                };
                valueElement.SetBinding(TextBox.TextProperty, valueBinding);

                Binding isReadOnlyBinding = new Binding(nameof(PropertyItem.IsReadOnly));
                valueElement.SetBinding(TextBox.IsReadOnlyProperty, isReadOnlyBinding);

                SingleTextBoxFocusBehavior singleTextBoxFocusBehavior = new SingleTextBoxFocusBehavior();
                Interaction.GetBehaviors(valueElement).Add(singleTextBoxFocusBehavior);
            }

            if (firstElement == null)
            {
                firstElement = valueElement;
            }

            valueElement.GotFocus += new RoutedEventHandler(valueElement_GotFocus);
            valueElement.LostFocus += new RoutedEventHandler(valueElement_LostFocus);

            Border border = new Border()
            {
                BorderThickness = new Thickness(0, 0, 0, 1),
                BorderBrush = new SolidColorBrush(Color.FromArgb(255, 167, 179, 189)),
                Height = 20,
            };

            if (item.IsDialog)
            {
                if (!item.IsReadOnly)
                {
                    Interaction.GetBehaviors(valueElement).Add(new TextBoxUpdateBehavior());
                }

                Button openDialogButton = new Button
                {
                    Margin = new Thickness(1, 1, 1, 2),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    IsTabStop = false,
                    Padding = new Thickness(3),
                    Content = new TextBlock
                    {
                        Text = "...",
                        Margin = new Thickness(0, -8, 0, 0)
                    },
                };

                openDialogButton.Click += new RoutedEventHandler(openDialogButton_Click);
                valueElement.KeyDown += new KeyEventHandler(valueElement_KeyDown);

                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

                Grid.SetColumn(valueElement, 0);
                Grid.SetColumnSpan(valueElement, 2);
                Grid.SetColumn(openDialogButton, 1);

                grid.Children.Add(valueElement);
                grid.Children.Add(openDialogButton);

                border.Child = grid;
            }
            else
            {
                border.Child = valueElement;
            }

            return border;
        }

        void button_LostFocus(object sender, RoutedEventArgs e)
        {
            Border titleBorder = FindParent<Border>((UIElement)sender);
            titleBorder.Background = defaultTitleBrush;
        }

        void button_GotFocus(object sender, RoutedEventArgs e)
        {
            Border titleBorder = FindParent<Border>((UIElement)sender);
            titleBorder.Background = focusedTitleBrush;
        }

        void valueElement_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PropertyItem p = (PropertyItem)((FrameworkElement)sender)?.DataContext;
                if (p != null)
                {
                    MessageBox.Show($"In real application this action opens dialog to edit [{p.Name}] properties",
                        "Information", MessageBoxButton.OK);
                }

            }
        }

        void openDialogButton_Click(object sender, RoutedEventArgs e)
        {
            PropertyItem p = (PropertyItem)(sender as FrameworkElement)?.DataContext;
            if (p != null)
            {
                MessageBox.Show($"In real application this button opens dialog to edit [{p.Name}] properties",
                   "Information", MessageBoxButton.OK);
            }

        }

        void valueElement_LostFocus(object sender, RoutedEventArgs e)
        {
            Border parent = FindParent<Border>((UIElement)sender);
            if (parent != null)
            {
                parent.Background = new SolidColorBrush(Colors.White);
            }

            if (sender is ComboBox comboBox && !comboBox.IsDropDownOpen)
            {
                comboBox.BorderThickness = new Thickness();
            }
        }

        void valueElement_GotFocus(object sender, RoutedEventArgs e)
        {
            Border parent = FindParent<Border>((UIElement)sender);
            if (parent != null)
            {
                parent.Background = new SolidColorBrush(Color.FromArgb(255, 212, 221, 229));
            }

            if (sender is ComboBox comboBox)
            {
                comboBox.BorderThickness = new Thickness(1);
            }
        }

        void button_Click(object sender, RoutedEventArgs e)
        {
            Grid groupGridwTitle = FindParent<Grid>(FindParent<Grid>((UIElement)sender));
            Grid title = FindParent<Grid>((UIElement)sender);
            if (groupGridwTitle.Children[1].Visibility == Visibility.Collapsed)
            {
                groupGridwTitle.Children[1].Visibility = Visibility.Visible;
                title.Children[0].Visibility = Visibility.Collapsed;
                title.Children[1].Visibility = Visibility.Visible;
                closedGroups.Remove(((TextBlock)title.Children[2]).Text);
            }
            else
            {
                groupGridwTitle.Children[1].Visibility = Visibility.Collapsed;
                title.Children[0].Visibility = Visibility.Visible;
                title.Children[1].Visibility = Visibility.Collapsed;
                closedGroups.Add(((TextBlock)title.Children[2]).Text);
            }
        }

        public static T FindParent<T>(UIElement control) where T : UIElement
        {
            UIElement p = VisualTreeHelper.GetParent(control) as UIElement;
            if (p != null)
            {
                if (p is T)
                    return p as T;
                else
                    return FindParent<T>(p);
            }
            return null;
        }


    }
}
