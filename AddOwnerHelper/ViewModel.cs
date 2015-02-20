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
        private readonly ObservableCollection<FieldInfo> _dependencyProperties = new ObservableCollection<FieldInfo>();

        private FieldInfo _selectedFieldInfo;

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
                        DependencyProperties.Add(fieldInfo);
                    }
                }
            }
            NewOwner = "NewOwner";
            _collectionView = CollectionViewSource.GetDefaultView(DependencyProperties);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<FieldInfo> DependencyProperties
        {
            get
            {
                return _dependencyProperties;
            }
        }

        public FieldInfo SelectedFieldInfo
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
                        var fieldInfo = o as FieldInfo;
                        if (fieldInfo == null)
                        {
                            return false;
                        }
                        if (fieldInfo.Name.StartsWith(_filter, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                        if (fieldInfo.DeclaringType.Name.StartsWith(_filter, StringComparison.OrdinalIgnoreCase))
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
                        writer.WriteAddOwnerField(SelectedFieldInfo, NewOwner);
                        writer.WriteLine();
                        writer.WriteAddOwnerProperty(SelectedFieldInfo, NewOwner);
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
