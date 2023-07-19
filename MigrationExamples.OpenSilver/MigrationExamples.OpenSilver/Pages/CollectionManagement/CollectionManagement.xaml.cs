using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MigrationExamples.OpenSilver.Pages.CollectionManagement
{
    public partial class CollectionManagement : Page
    {
        private readonly ObservableCollection<UserModel> _nonOptimalModels = new ObservableCollection<UserModel>();
        private readonly ObservableCollection<UserModel> _optimalModels = new ObservableCollection<UserModel>();

        private readonly ObservableCollection<UserViewModel> _nonOptimalViewModels = new ObservableCollection<UserViewModel>();
        private readonly ObservableCollection<UserViewModel> _optimalViewModels = new ObservableCollection<UserViewModel>();

        private readonly Random random = new Random();

        public CollectionManagement()
        {
            this.InitializeComponent();
            this.Loaded += CollectionManagement_Loaded;
        }

        private void CollectionManagement_Loaded(object sender, RoutedEventArgs e)
        {
            _nonOptimalModels.CollectionChanged += NonOptimalModels_CollectionChanged;
            _optimalModels.CollectionChanged += OptimalModels_CollectionChanged;

            NonOptimalItemsControl.ItemsSource = _nonOptimalViewModels;
            OptimalItemsControl.ItemsSource = _optimalViewModels;

            AddUsers(20);
        }

        private void AddUsers(int count)
        {
            UserModel[] users = new UserModel[count];

            for (int i = 0; i < count; i++)
            {
                string name = Names.Values[random.Next(0, Names.Values.Length)];
                users[i] = new UserModel() { Name = name };
            }

            Stopwatch swNonOpt = Stopwatch.StartNew();
            foreach (UserModel user in users)
            {
                _nonOptimalModels.Add(user);
            }
            swNonOpt.Stop();

            Stopwatch swOpt = Stopwatch.StartNew();
            foreach (UserModel user in users)
            {
                _optimalModels.Add(user);
            }
            swOpt.Stop();

            NonOptimalResults.Text = $"Added {count} users in {swNonOpt.ElapsedMilliseconds} ms";
            OptimalResults.Text = $"Added {count} users in {swOpt.ElapsedMilliseconds} ms";
        }

        private void DeleteUsers(int count)
        {
            Debug.Assert(_nonOptimalModels.Count == _optimalModels.Count);

            int localCount1 = count;
            Stopwatch swNonOpt = Stopwatch.StartNew();
            while (_nonOptimalModels.Count > 0 && localCount1 > 0)
            {
                _nonOptimalModels.RemoveAt(0);
                --localCount1;
            }
            swNonOpt.Stop();

            int localCount2 = count;
            Stopwatch swOpt = Stopwatch.StartNew();
            while (_optimalModels.Count > 0 && localCount2 > 0)
            {
                _optimalModels.RemoveAt(0);
                --localCount2;
            }
            swOpt.Stop();

            NonOptimalResults.Text = $"Removed {count} users in {swNonOpt.ElapsedMilliseconds} ms";
            OptimalResults.Text = $"Removed {count} users in {swOpt.ElapsedMilliseconds} ms";
        }

        private void OptimalModels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                    {
                        foreach (UserModel model in e.NewItems.OfType<UserModel>())
                        {
                            UserViewModel userViewModel = new UserViewModel(model);

                            int index = 0;
                            while (index < _optimalViewModels.Count && _optimalViewModels[index].Order <= userViewModel.Order)
                            {
                                ++index;
                            }

                            _optimalViewModels.Insert(index, userViewModel);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        foreach (UserModel model in e.OldItems.OfType<UserModel>())
                        {
                            UserViewModel userViewModel = _optimalViewModels.FirstOrDefault(_ => _.Model == model);
                            if (userViewModel != null)
                            {
                                _optimalViewModels.Remove(userViewModel);
                            }
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                    OptimalItemsControl.ItemsSource = new ObservableCollection<UserViewModel>(_optimalModels.OrderBy(x => x.Order).Select(x => new UserViewModel(x)));
                    break;
            }

        }

        private void NonOptimalModels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NonOptimalItemsControl.ItemsSource = new ObservableCollection<UserViewModel>(_nonOptimalModels.OrderBy(x => x.Order).Select(x => new UserViewModel(x)));
        }

        private void AddNewUsers_Click(object sender, RoutedEventArgs e)
        {
            AddUsers(5);
        }

        private void DeleteUsers_Click(object sender, RoutedEventArgs e)
        {
            DeleteUsers(5);
        }
    }
}
