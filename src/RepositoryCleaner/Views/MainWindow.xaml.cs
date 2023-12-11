namespace RepositoryCleaner.Views
{
    using System.Windows;
    using System.Windows.Controls;
    using Models;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private bool _isUpdatingCheckboxes;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnCheckBoxCheckedChanged(object sender, RoutedEventArgs e)
        {
            if (_isUpdatingCheckboxes)
            {
                return;
            }

            _isUpdatingCheckboxes = true;

            var initiatingCheckBox = (CheckBox)sender;
            bool isChecked = initiatingCheckBox.IsChecked ?? false;

            var selectedCells = dataGrid.SelectedCells;
            foreach (var selectedCell in selectedCells)
            {
                var item = (Repository)selectedCell.Item;
                item.IsIncluded = isChecked;
            }

            _isUpdatingCheckboxes = false;
        }
    }
}
