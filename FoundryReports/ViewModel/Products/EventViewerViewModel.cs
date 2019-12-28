using System.Collections.Generic;
using System.Collections.ObjectModel;
using FoundryReports.Core.Source.Prediction;

namespace FoundryReports.ViewModel.Products
{
    public class EventViewerViewModel : BaseViewModel
    {
        public ObservableCollection<ProductEventViewModel> Events { get; } = new ObservableCollection<ProductEventViewModel>();

        private bool _noEvents;

        public bool NoEvents
        {
            get => _noEvents;
            set
            {
                _noEvents = value;
                OnPropertyChanged();
            }
        }

        public void ShowEvents(IEnumerable<IProductEvent> events)
        {
            Events.Clear();
            NoEvents = true;
            foreach (var productEvent in events)
            {
                NoEvents = false;
                Events.Add(new ProductEventViewModel(productEvent));
            }
        }
    }
}
