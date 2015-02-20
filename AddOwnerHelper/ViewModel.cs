namespace AddOwnerHelper
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    using AddOwnerHelper.Annotations;

    public class ViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<DpViewModel> _dependencyProperties = new ObservableCollection<DpViewModel>();

        private DpViewModel _selectedFieldInfo;

        private string _newOwner;

        private string _filter;

        private ICollectionView _collectionView;

        public ViewModel()
        {
            var types = typeof(TextBox).Assembly.GetTypes();
            foreach (var type in types)
            {
                var fieldInfos = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly).Where(x => x.FieldType == typeof(DependencyProperty)).ToArray();
                if (fieldInfos.Any())
                {
                    foreach (var fieldInfo in fieldInfos)
                    {
                        var dependencyProperty =(DependencyProperty) fieldInfo.GetValue(null);
                        if (dependencyProperty.OwnerType == fieldInfo.DeclaringType)
                        {
                            var dpViewModel = new DpViewModel(fieldInfo, dependencyProperty);
                            dpViewModel.PropertyChanged += (_, __) => OnPropertyChanged("Code");
                            DependencyProperties.Add(dpViewModel);
                        }

                    }
                }
            }
            NewOwner = "NewOwner";
            _collectionView = CollectionViewSource.GetDefaultView(DependencyProperties);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<DpViewModel> DependencyProperties
        {
            get
            {
                return _dependencyProperties;
            }
        }

        public string Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                if (value == _filter)
                {
                    return;
                }
                _filter = value;
                OnPropertyChanged();
                _collectionView.Filter = o =>
                    {
                        var dpViewModel = o as DpViewModel;
                        if (dpViewModel == null)
                        {
                            return false;
                        }
                        if (Regex.IsMatch(dpViewModel.FieldInfo.Name, _filter, RegexOptions.IgnoreCase))
                        {
                            return true;
                        }
                        if (Regex.IsMatch(dpViewModel.FieldInfo.DeclaringType.Name, _filter, RegexOptions.IgnoreCase))
                        {
                            return true;
                        }
                        return false;
                    };

            }
        }

        public string NewOwner
        {
            get
            {
                return _newOwner;
            }
            set
            {
                if (value == _newOwner)
                {
                    return;
                }
                _newOwner = value;
                OnPropertyChanged();
                OnPropertyChanged("Code");
            }
        }

        public string Code
        {
            get
            {
                var dpViewModels = DependencyProperties.Where(x => x.IsChecked || x.IsSelected)
                                                                 .ToArray();
                if (!dpViewModels.Any())
                {
                    return "No property selected";
                }

                var stringBuilder = new StringBuilder();
                using (var stringWriter = new StringWriter(stringBuilder))
                {
                    using (var writer = new IndentedTextWriter(stringWriter))
                    {
                        foreach (var dpViewModel in dpViewModels)
                        {
                            writer.WriteAddOwnerField(dpViewModel.FieldInfo, NewOwner);
                            writer.WriteLine();
                        }

                        foreach (var dpViewModel in dpViewModels)
                        {
                            writer.WriteAddOwnerProperty(dpViewModel.FieldInfo, NewOwner);
                            writer.WriteLine();
                        }
                    }
                }
                return stringBuilder.ToString();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
