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

        //enumeration for sorting by Name or by Price
        public enum SortOption
        {
            Name,
            Price
        }

        //setting sorting method, the default is to sort by name
        public SortOption SelectedSortOption { get; set; } = SortOption.Name;

        //returns the sorted list
        public ObservableCollection<Item?> Products
        {
            get
            {   //sorting the list of products based on search query, and it is not case sensitive
                var filtered = _svc.Products
                    .Where(p => p?.Product?.Name?.ToLower().Contains(Query?.ToLower() ?? string.Empty) ?? false);

                //Here it is sorting based on the sort option, either name or price
                IEnumerable<Item?> sorted;

                if(SelectedSortOption == SortOption.Price)
                {
                    sorted = filtered.OrderBy(p => p?.Product?.Price);
                }
                else
                {
                    sorted = filtered.OrderBy(p => p?.Product?.Name);
                }

                    //returning the sorted items
                    return new ObservableCollection<Item?>(sorted);
            }
        }

        //function to allow specification of sorting method
        public void SetSort(SortOption option)
        {
            SelectedSortOption = option;
            RefreshProductList(); //notify UI
        }

        public Item? Delete()
        {
            var item = _svc.Delete(SelectedProduct?.Id ?? 0);
            NotifyPropertyChanged(nameof(Products));
            return item;
        }
    }
}
