using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace FoundryReports.ViewModel.Graph
{
    public class ObservableCollectionWrapper<TWrappedItem, TExposedItem> : ObservableCollection<TExposedItem> where TWrappedItem:TExposedItem
    {
        private readonly ObservableCollection<TWrappedItem> _wrappedCollection;
        
        public ObservableCollectionWrapper(ObservableCollection<TWrappedItem> wrappedCollection)
        {
            _wrappedCollection = wrappedCollection;
            wrappedCollection.CollectionChanged += WrappedCollectionOnCollectionChanged;
        }

        private void WrappedCollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var newItem in e.NewItems.OfType<TWrappedItem>())
                    {
                        Add(newItem);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var oldItem in e.OldItems.OfType<TWrappedItem>())
                    {
                        Add(oldItem);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException();
                case NotifyCollectionChangedAction.Move:
                    Move(e.OldStartingIndex, e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Clear();
                    foreach (var item in _wrappedCollection)
                    {
                        Add(item);
                    }
                    break;
            }
        }
    }
}