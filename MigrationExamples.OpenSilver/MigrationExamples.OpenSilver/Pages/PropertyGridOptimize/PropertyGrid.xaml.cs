using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MigrationExamples.OpenSilver.Pages.PropertyGridOptimize
{
    public partial class PropertyGrid : UserControl
    {
        private readonly List<string> closedGroups = new List<string>();
        private readonly LinearGradientBrush defaultTitleBrush;
        private readonly LinearGradientBrush focusedTitleBrush;
        private bool isGridSet;

        private List<PropertyItem> localItemSource = new List<PropertyItem>();
        private FrameworkElement firstElement;
        private bool addGroup;

        public static readonly DependencyProperty ItemSourceProperty = DependencyProperty.Register(
                        "ItemSource", typeof(ObservableCollection<PropertyItem>), typeof(PropertyGrid),
                        new PropertyMetadata(ItemSourcePropertyChanged));
        public ObservableCollection<PropertyItem> ItemSource
        {
            get { return (ObservableCollection<PropertyItem>)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        private static void ItemSourcePropertyChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            ((PropertyGrid)sender).SortItems();
            ((PropertyGrid)sender).SetGrid();
        }

        public static readonly DependencyProperty GroupProperty = DependencyProperty.Register(
                                "Group", typeof(bool), typeof(PropertyGrid),
                                new PropertyMetadata(GroupPropertyChanged));
        public bool Group
        {
            get { return (bool)GetValue(GroupProperty); }
            set { SetValue(GroupProperty, value); }
        }

        private static void GroupPropertyChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            ((PropertyGrid)sender).SortItems();
            ((PropertyGrid)sender).SetGrid();
        }

        public PropertyGrid()
        {
            this.InitializeComponent();

            defaultTitleBrush = new LinearGradientBrush { StartPoint = new Point(0, 0.1), EndPoint = new Point(0, 0.9) };
            defaultTitleBrush.GradientStops.Add(new GradientStop { Color = Color.FromArgb(255, 167, 179, 189), Offset = 0 });
            defaultTitleBrush.GradientStops.Add(new GradientStop { Color = Color.FromArgb(255, 125, 142, 157), Offset = 1 });

            focusedTitleBrush = new LinearGradientBrush { StartPoint = new Point(0, 0.1), EndPoint = new Point(0, 0.9) };
            focusedTitleBrush.GradientStops.Add(new GradientStop { Color = Color.FromArgb(255, 154, 168, 183), Offset = 1 });
            focusedTitleBrush.GradientStops.Add(new GradientStop { Color = Color.FromArgb(255, 208, 215, 220), Offset = 0 });
        }

        private void SortItems()
        {
            localItemSource = ItemSource?.ToList() ?? new List<PropertyItem>();
            if (Group)
            {
                localItemSource.Sort((a, b) => (1000 * a.GroupName.CompareTo(b.GroupName) + a.Name.CompareTo(b.Name)));
            }
            else
            {
                localItemSource.Sort((a, b) => (a.Name.CompareTo(b.Name)));
            }
        }

        private void SetGrid()
        {
            LayoutRoot.Children.Clear();

            addGroup = true;
            int j = 0;
            string group = "";

            Grid groupGridwTitle = new Grid();
            Grid groupGrid = new Grid();
            foreach (var item in localItemSource)
            {
                if (item.GroupName != group && Group || addGroup)
                {
                    addGroup = false;

                    group = item.GroupName;

                    groupGridwTitle = new Grid();
                    if (Group)
                        groupGridwTitle.RowDefinitions.Add(new RowDefinition { Height = new GridLength(18) });

                    groupGridwTitle.RowDefinitions.Add(new RowDefinition());

                    groupGrid = new Grid();

                    if (Group)
                    {
                        if (closedGroups.Contains(item.GroupName))
                            groupGrid.Visibility = System.Windows.Visibility.Collapsed;

                        if (!isGridSet && item.GroupName != "Common")
                        {
                            groupGrid.Visibility = System.Windows.Visibility.Collapsed;
                            closedGroups.Add(item.GroupName);
                        }
                    }

                    groupGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    groupGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3) });
                    groupGrid.ColumnDefinitions.Add(new ColumnDefinition { MinWidth = 100 });

                    Grid groupTitle = new Grid();

                    Polygon arrowClosed = new Polygon
                    {
                        Fill = new SolidColorBrush(Colors.White),
                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        Visibility = System.Windows.Visibility.Collapsed,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                        Margin = new Thickness(0, 0, 10, 0)
                    };
                    arrowClosed.Points.Add(new Point(0, 0));
                    arrowClosed.Points.Add(new Point(5, 2.5));
                    arrowClosed.Points.Add(new Point(0, 5));

                    Polygon arrowOpen = new Polygon
                    {
                        Fill = new SolidColorBrush(Colors.White),
                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                        Margin = new Thickness(0, 0, 10, 0)
                    };
                    arrowOpen.Points.Add(new Point(0, 0));
                    arrowOpen.Points.Add(new Point(5, 0));
                    arrowOpen.Points.Add(new Point(2.5, 5));

                    groupTitle.Children.Add(arrowClosed);
                    groupTitle.Children.Add(arrowOpen);

                    groupTitle.Children.Add(new TextBlock
                    {
                        Text = group,
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
                    if (Group)
                    {
                        Grid.SetRow(groupTitle, 0);
                        Grid.SetRow(groupGrid, 1);
                    }
                    else
                        Grid.SetRow(groupGrid, 0);

                    Border titleBorder = new Border
                    {
                        BorderBrush = new SolidColorBrush(Color.FromArgb(255, 167, 179, 189)),
                        BorderThickness = new Thickness(0, 0, 0, 1),
                        Background = defaultTitleBrush
                    };
                    titleBorder.Child = groupTitle;

                    if (Group)
                        groupGridwTitle.Children.Add(titleBorder);

                    groupGridwTitle.Children.Add(groupGrid);

                    LayoutRoot.Children.Add(groupGridwTitle);
                    j = 0;
                }

                groupGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20) });

                Binding binding = new Binding("Name");
                binding.Mode = BindingMode.TwoWay;
                TextBlock textBlock = new TextBlock { FontSize = 11, VerticalAlignment = System.Windows.VerticalAlignment.Center };
                textBlock.Margin = new Thickness(5, 1, 1, 1);
                textBlock.SetBinding(TextBlock.TextProperty, binding);
                textBlock.DataContext = item;

                Border border = new Border();
                border.BorderThickness = new Thickness(0, 0, 0, 1);
                border.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 167, 179, 189));
                Grid.SetColumn(border, 0);
                Grid.SetRow(border, j);

                border.Child = textBlock;

                groupGrid.Children.Add(border);


                Binding binding2 = new Binding("Value");
                binding2.Mode = BindingMode.TwoWay;

                FrameworkElement valueElement;
                if (item.HasOptions && !item.IsReadOnly)
                {
                    Setter setter = new Setter { Property = ComboBoxItem.MinHeightProperty, Value = 16d };
                    Style style = new System.Windows.Style { TargetType = typeof(ComboBoxItem) };
                    style.Setters.Add(setter);

                    valueElement = new ComboBox { FontSize = 11, BorderThickness = new Thickness(), ItemContainerStyle = style };
                    Binding comboSourceBinding = new Binding("Options");

                    valueElement.SetBinding(ComboBox.ItemsSourceProperty, comboSourceBinding);
                    valueElement.SetBinding(ComboBox.SelectedItemProperty, binding2);

                    ComboBoxSelectBehavior comboBoxSelectBehavior = new ComboBoxSelectBehavior();
                    System.Windows.Interactivity.Interaction.GetBehaviors(valueElement).Add(comboBoxSelectBehavior);

                    ((ComboBox)valueElement).IsEnabled = !item.IsReadOnly;
                    ((ComboBox)valueElement).Background = new SolidColorBrush(Colors.White);

                    if (firstElement == null)
                        firstElement = valueElement;
                }
                else if (item.IsBool && !item.IsReadOnly)
                {
                    valueElement = new ComboBox { FontSize = 11, BorderThickness = new Thickness() };
                    Binding boolSourceBinding = new Binding("BoolOptions");
                    binding2.Converter = new String2BoolConverter();

                    valueElement.SetBinding(ComboBox.ItemsSourceProperty, boolSourceBinding);
                    valueElement.SetBinding(ComboBox.SelectedItemProperty, binding2);

                    ((ComboBox)valueElement).IsEnabled = !item.IsReadOnly;
                    ((ComboBox)valueElement).Background = new SolidColorBrush(Colors.White);

                    if (firstElement == null)
                        firstElement = valueElement;
                }
                else
                {
                    binding2.ValidatesOnExceptions = true;
                    binding2.NotifyOnValidationError = true;

                    if (item.IsBool)
                        binding2.Converter = new String2BoolConverter();

                    valueElement = new TextBox { FontSize = 11 };
                    ((TextBox)valueElement).Background = new SolidColorBrush(Colors.Transparent);
                    ((TextBox)valueElement).BorderThickness = new Thickness();

                    if (item.IsReadOnly)
                        ((TextBox)valueElement).Foreground = new SolidColorBrush(Color.FromArgb(255, 110, 110, 110));

                    valueElement.SetBinding(TextBox.TextProperty, binding2);

                    SingleTextBoxFocusBehavior singleTextBoxFocusBehavior = new SingleTextBoxFocusBehavior();
                    System.Windows.Interactivity.Interaction.GetBehaviors(valueElement).Add(singleTextBoxFocusBehavior);

                    ((TextBox)valueElement).IsReadOnly = item.IsReadOnly;

                    if (firstElement == null)
                        firstElement = valueElement;
                }
                valueElement.GotFocus += new RoutedEventHandler(valueElement_GotFocus);
                valueElement.LostFocus += new RoutedEventHandler(valueElement_LostFocus);

                valueElement.DataContext = item;
                Border border2 = new Border();
                border2.BorderThickness = new Thickness(0, 0, 0, 1);
                border2.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 167, 179, 189));
                Grid.SetColumn(border2, 2);
                Grid.SetRow(border2, j);

                border2.Child = valueElement;

                groupGrid.Children.Add(border2);

                if (item.IsDialog)
                {
                    if (!item.IsReadOnly)
                        System.Windows.Interactivity.Interaction.GetBehaviors(valueElement).Add(new TextBoxUpdateBehavior());

                    Button openDialogButton = new Button
                    {
                        Margin = new Thickness(1, 1, 1, 2),
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                        IsTabStop = false,
                        Padding = new Thickness(3)
                    };

                    openDialogButton.Content = new TextBlock { Text = "...", Margin = new Thickness(0, -8, 0, 0) };
                    Grid.SetColumn(openDialogButton, 2);
                    Grid.SetRow(openDialogButton, j);

                    openDialogButton.Click += new RoutedEventHandler(openDialogButton_Click);

                    groupGrid.Children.Add(openDialogButton);

                    valueElement.KeyDown += new KeyEventHandler(valueElement_KeyDown);
                }

                if (j % 2 == 1)
                {
                    //border.Background = new SolidColorBrush(Color.FromArgb(255, 248, 250, 251));
                    //border2.Background = new SolidColorBrush(Color.FromArgb(255, 248, 250, 251));
                }

                Grid middle = new Grid
                {
                    Width = 3,
                    VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                    Background = new SolidColorBrush(Colors.LightGray)
                };
                Rectangle rectangle = new Rectangle
                {
                    Width = 1,
                    VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                    Fill = new SolidColorBrush(Color.FromArgb(255, 167, 179, 189))
                };
                Grid.SetColumn(middle, 1);
                Grid.SetRow(middle, j);
                middle.Children.Add(rectangle);

                groupGrid.Children.Add(middle);

                j++;
            }

            isGridSet = true;
        }

        void button_Click(object sender, RoutedEventArgs e)
        {
            Grid groupGridwTitle = FindParent<Grid>(FindParent<Grid>((UIElement)sender));
            Grid title = FindParent<Grid>((UIElement)sender);
            if (groupGridwTitle.Children[1].Visibility == System.Windows.Visibility.Collapsed)
            {
                groupGridwTitle.Children[1].Visibility = System.Windows.Visibility.Visible;
                title.Children[0].Visibility = System.Windows.Visibility.Collapsed;
                title.Children[1].Visibility = System.Windows.Visibility.Visible;
                closedGroups.Remove(((TextBlock)title.Children[2]).Text);
            }
            else
            {
                groupGridwTitle.Children[1].Visibility = System.Windows.Visibility.Collapsed;
                title.Children[0].Visibility = System.Windows.Visibility.Visible;
                title.Children[1].Visibility = System.Windows.Visibility.Collapsed;
                closedGroups.Add(((TextBlock)title.Children[2]).Text);
            }
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
                PropertyItem p = (PropertyItem)((FrameworkElement)sender).DataContext;

                MessageBox.Show($"In real application this action opens dialog to edit [{p.Name}] properties",
                    "Information", MessageBoxButton.OK);
            }
        }

        void openDialogButton_Click(object sender, RoutedEventArgs e)
        {
            Grid group = FindParent<Grid>((UIElement)sender);
            int index = group.Children.IndexOf((UIElement)sender);

            PropertyItem p = (PropertyItem)((FrameworkElement)((Border)group.Children[index - 1]).Child).DataContext;

            MessageBox.Show($"In real application this button opens dialog to edit [{p.Name}] properties",
                "Information", MessageBoxButton.OK);
        }

        void valueElement_LostFocus(object sender, RoutedEventArgs e)
        {
            Grid group = FindParent<Grid>((UIElement)sender);
            int index = group.Children.IndexOf(FindParent<Border>((UIElement)sender));

            ((Border)group.Children[index - 1]).Background = new SolidColorBrush(Colors.White);

            if (sender is ComboBox)
                if (!((ComboBox)sender).IsDropDownOpen)
                    ((ComboBox)sender).BorderThickness = new Thickness();
        }

        void valueElement_GotFocus(object sender, RoutedEventArgs e)
        {
            Grid group = FindParent<Grid>((UIElement)sender);
            int index = group.Children.IndexOf(FindParent<Border>((UIElement)sender));

            ((Border)group.Children[index - 1]).Background = new SolidColorBrush(Color.FromArgb(255, 212, 221, 229));

            if (sender is ComboBox)
                ((ComboBox)sender).BorderThickness = new Thickness(1);
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
