using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.eCommerce.Services;
using Spring2025_Ecommerce.Models;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Library.eCommerce.Models;

namespace Maui.eCommerce.ViewModels
{
    public class InventoryManagementViewModel : INotifyPropertyChanged
    {
        public Item? SelectedProduct { get; set; }
        public string? Query { get; set; }
        private ProductServiceProxy _svc = ProductServiceProxy.Current;

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (propertyName is null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RefreshProductList()
        {
            NotifyPropertyChanged(nameof(Products));
        }

        public enum SortOption
        {
            Name,
            Price
        }

        public SortOption SelectedSortOption { get; set; } = SortOption.Name;

        public ObservableCollection<Item?> Products
        {
            get
            {
                var filtered = _svc.Products
                    .Where(p => p?.Product?.Name?.ToLower().Contains(Query?.ToLower() ?? string.Empty) ?? false);

                IEnumerable<Item?> sorted = SelectedSortOption switch
                {
                    SortOption.Price => filtered.OrderBy(p => p?.Product?.Price),
                    _ => filtered.OrderBy(p => p?.Product?.Name)
                };

                return new ObservableCollection<Item?>(sorted);
            }
        }

        public void SetSort(SortOption option)
        {
            SelectedSortOption = option;
            RefreshProductList();
        }

        public Item? Delete()
        {
            var item = _svc.Delete(SelectedProduct?.Id ?? 0);
            NotifyPropertyChanged(nameof(Products));
            return item;
        }
    }
}
