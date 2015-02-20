namespace AddOwnerHelper
{
    using System.Windows;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new ViewModel();
            DataContext = _viewModel;
        }

        private void OnClearClick(object sender, RoutedEventArgs e)
        {
            foreach (var dp in _viewModel.DependencyProperties)
            {
                dp.IsChecked = false;
            }
        }
    }
}
