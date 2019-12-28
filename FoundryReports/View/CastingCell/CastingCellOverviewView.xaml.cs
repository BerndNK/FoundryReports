using System.Windows;

namespace FoundryReports.View.CastingCell
{
    public partial class CastingCellOverviewView
    {
        public CastingCellOverviewView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // reselect item to force a refresh, if no item was selected previously, select the first item
            var lastSelectedItem = CustomerSelection.SelectedItem;
            if (lastSelectedItem == null)
            {
                CustomerSelection.SelectedIndex = 0;
            }
            else
            {
                CustomerSelection.SelectedItem = null;
                CustomerSelection.SelectedItem = lastSelectedItem;
            }
        }
    }
}