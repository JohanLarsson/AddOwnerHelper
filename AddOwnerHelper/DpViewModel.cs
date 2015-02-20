namespace AddOwnerHelper
{
    using System.ComponentModel;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Windows;

    using AddOwnerHelper.Annotations;

    public class DpViewModel : INotifyPropertyChanged
    {
        private bool _isChecked;
        private bool _isSelected;

        public DpViewModel(FieldInfo fieldInfo)
        {
            FieldInfo = fieldInfo;
            DependencyProperty = (DependencyProperty)FieldInfo.GetValue(null);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                if (value.Equals(_isChecked))
                {
                    return;
                }
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (value.Equals(_isSelected))
                {
                    return;
                }
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public FieldInfo FieldInfo { get; private set; }
        
        public DependencyProperty DependencyProperty { get; private set; }

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