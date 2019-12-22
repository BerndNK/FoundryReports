using System.Collections.ObjectModel;
using System.Windows.Input;
using FoundryReports.Utils;

namespace FoundryReports.ViewModel.DataManage
{
    public abstract class ListViewModel<T> : BaseViewModel where T : class
    {
        public ICommand AddItemCommand { get; }

        public ICommand RemoveItemCommand { get; }

        public ObservableCollection<T> Children { get; } = new ObservableCollection<T>();

        protected ListViewModel()
        {
            AddItemCommand = new DelegateCommand(() => AddItem());
            RemoveItemCommand = new DelegateCommand<T>(RemoveItem);
        }

        public T AddItem()
        {
            var productViewModel = NewViewModel();
            Children.Add(productViewModel);

            return productViewModel;
        }

        protected abstract T NewViewModel();

        protected abstract void RemoveFromModel(T viewModel);

        private void RemoveItem(T? item)
        {
            if (item == null)
                return;

            if (Children.Remove(item))
            {
                RemoveFromModel(item);
            }
        }
    }
}