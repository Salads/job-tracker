using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace MainApp.Models
{
    
    public class ObservableCollectionEx<T> : ObservableCollection<T>
        where T : INotifyPropertyChanged
    {
        public ObservableCollectionEx()
        {
            CollectionChanged += HandleCollectionChanged;
        }

        public event PropertyChangedEventHandler? PropertyChangedEx;

        private void HandleCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // Subscribe/Unsubscribe from property changed events as they are added/removed.

            if(e.OldItems != null)
            {
                foreach(T item in e.OldItems)
                {
                    item.PropertyChanged -= HandlePropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (T item in e.NewItems)
                {
                    item.PropertyChanged += HandlePropertyChanged;
                }
            }
        }

        protected override void ClearItems()
        {
            // A gotcha - Make sure we unsubscribe when all items are removed via this method.
            foreach (T item in this)
            {
                item.PropertyChanged -= HandlePropertyChanged;
            }

            base.ClearItems();
        }

        private void HandlePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            //OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            PropertyChangedEx?.Invoke(sender, e);
        }
    }
}
