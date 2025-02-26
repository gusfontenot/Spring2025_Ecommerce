using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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
            if(propertyName is null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RefreshProductList()
        {
            NotifyPropertyChanged(nameof(Products));
        }

        public ObservableCollection<Item?> Products
        {
            get
            {
                var filteredList = _svc.Products.Where(p => p?.Product?.Name?.ToLower().Contains(Query?.ToLower() ?? string.Empty) ?? false);
                return new ObservableCollection<Item?>(filteredList);
            }
        }

        public Item? Delete()
        {
            var item = _svc.Delete(SelectedProduct?.Id ?? 0); //goto the service and delete the item that is selected then go to front end and tell it I changed it
            NotifyPropertyChanged("Products"); //this tells the front end I believe (hands the functionality off)
            return item;
        }
    }
}
