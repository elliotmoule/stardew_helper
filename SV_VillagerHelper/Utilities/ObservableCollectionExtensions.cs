using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace SV_VillagerHelper.Utilities
{
    public static class ObservableCollectionExtensions
    {
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (items == null)
                throw new ArgumentNullException(nameof(items));

            // Disable collection change notification
            var notifyCollectionChanged = collection as INotifyCollectionChanged;
            if (notifyCollectionChanged != null)
            {
                collection.CollectionChanged -= NotifyCollectionChangedEventHandler;
            }

            try
            {
                foreach (var item in items.Order())
                {
                    collection.Add(item);
                }
            }
            finally
            {
                // Enable collection change notification
                if (notifyCollectionChanged != null)
                {
                    collection.CollectionChanged += NotifyCollectionChangedEventHandler;
                    // Trigger a reset notification to indicate the entire collection has changed
                    collection.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }

        private static void NotifyCollectionChangedEventHandler(object sender, NotifyCollectionChangedEventArgs e)
        {
            // This handler is temporarily removed during the AddRange operation
        }

        private static void OnCollectionChanged<T>(this ObservableCollection<T> collection, NotifyCollectionChangedEventArgs e)
        {
            var eventHandler = collection.GetType().GetField("CollectionChanged", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                              ?.GetValue(collection) as NotifyCollectionChangedEventHandler;

            eventHandler?.Invoke(collection, e);
        }
    }
}
