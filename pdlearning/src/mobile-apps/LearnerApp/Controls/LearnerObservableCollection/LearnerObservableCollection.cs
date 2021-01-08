using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using LearnerApp.ViewModels;

namespace LearnerApp.Controls.LearnerObservableCollection
{
    public class LearnerObservableCollection<T> : ObservableCollection<T>
    {
        public LearnerObservableCollection()
        {
        }

        public LearnerObservableCollection(IEnumerable<T> collection) : base(collection ?? new List<T>())
        {
        }

        public void AddRange(IEnumerable<T> items, bool notifyChange = true)
        {
            if (items == null || items.Any() == false)
            {
                return;
            }

            CheckReentrancy();
            int lastIndex = Items.Count;

            foreach (var item in items)
            {
                Items.Add(item);
            }

            if (notifyChange)
            {
                OnPropertyChanged("Count");
                OnPropertyChanged("Item[]");
                OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Add,
                        (IList)items,
                        lastIndex));
            }
        }

        public void Replace(int index, T loadingItemSkeletonViewModel)
        {
            SetItem(index, loadingItemSkeletonViewModel);
        }

        private void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
    }
}
