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
            this._collectionView = CollectionViewSource.GetDefaultView(this.DependencyProperties);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<FieldInfo> DependencyProperties
        {
            get
            {
                return this._dependencyProperties;
            }
        }

        public FieldInfo SelectedFieldInfo
        {
            get
            {
                return this._selectedFieldInfo;
            }
            set
            {
                if (Equals(value, this._selectedFieldInfo))
                {
                    return;
                }
                this._selectedFieldInfo = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged("Code");
            }
        }

        public string Filter
        {
            get
            {
                return this._filter;
            }
            set
            {
                if (value == this._filter)
                {
                    return;
                }
                this._filter = value;
                this.OnPropertyChanged();
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
                return this._newOwner;
            }
            set
            {
                if (value == this._newOwner)
                {
                    return;
                }
                this._newOwner = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged("Code");
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
                return CodeWriter.Write(SelectedFieldInfo, NewOwner);
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
