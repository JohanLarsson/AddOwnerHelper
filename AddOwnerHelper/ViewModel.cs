namespace AddOwnerHelper
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
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
                        DependencyProperties.Add(new DpViewModel(fieldInfo));
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

        public DpViewModel SelectedFieldInfo
        {
            get
            {
                return _selectedFieldInfo;
            }
            set
            {
                if (Equals(value, _selectedFieldInfo))
                {
                    return;
                }
                _selectedFieldInfo = value;
                OnPropertyChanged();
                OnPropertyChanged("Code");
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
                        if (dpViewModel.FieldInfo.Name.StartsWith(_filter, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                        if (dpViewModel.FieldInfo.DeclaringType.Name.StartsWith(_filter, StringComparison.OrdinalIgnoreCase))
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
                if (SelectedFieldInfo == null)
                {
                    return "SelectedFieldInfo == null";
                }
                if (NewOwner == null)
                {
                    return "NewOwner == null";
                }
                var stringBuilder = new StringBuilder();
                using (var stringWriter = new StringWriter(stringBuilder))
                {
                    using (var writer = new IndentedTextWriter(stringWriter))
                    {
                        writer.WriteAddOwnerField(SelectedFieldInfo.FieldInfo, NewOwner);
                        writer.WriteLine();
                        writer.WriteAddOwnerProperty(SelectedFieldInfo.FieldInfo, NewOwner);
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
