using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Library.eCommerce.Models;
using Library.eCommerce.Services;

namespace Maui.eCommerce.ViewModels
{
    public class ShoppingManagementViewModel : INotifyPropertyChanged
    {
        private ProductServiceProxy _invSvc = ProductServiceProxy.Current;
        private ShoppingCartService _cartSvc = ShoppingCartService.Current;
        public ItemViewModel? SelectedItem { get; set; }
        public ItemViewModel? SelectedCartItem { get; set; }

        //sort begin
        public string SortOption { get; set; } = "Name";

        public ObservableCollection<ItemViewModel?> Inventory =>
        new(_invSvc.Products
            .Where(i => i?.Quantity > 0)
            .OrderBy(i => SortOption == "Price" ? i?.Product.Price : 0)
            .ThenBy(i => SortOption == "Name" ? i?.Product.Name : "")
            .Select(m => new ItemViewModel(m)));

        public ObservableCollection<ItemViewModel?> ShoppingCart =>
            new(_cartSvc.CartItems
                .Where(i => i?.Quantity > 0)
                .OrderBy(i => SortOption == "Price" ? i?.Product.Price : 0)
                .ThenBy(i => SortOption == "Name" ? i?.Product.Name : "")
                .Select(m => new ItemViewModel(m)));

        //sort end

        public static event Action? RefreshRequested;
        internal static void RefreshStatic()
        {
            RefreshRequested?.Invoke();
        }

        public ShoppingManagementViewModel()
        {
            _cartSvc.InvChange += (_, _) => RefreshUX();
            _cartSvc.CartChange += (_, _) => RefreshUX();
            RefreshRequested += RefreshUX;   
        }

        /*
        public ObservableCollection<ItemViewModel?> Inventory
        {
            get
            {
                return new ObservableCollection<ItemViewModel?>(_invSvc.Products
                    .Where(i => i?.Quantity > 0).Select(m => new ItemViewModel(m))
                    );
            }
        }

        public ObservableCollection<ItemViewModel?> ShoppingCart
        {
            get
            {
                return new ObservableCollection<ItemViewModel?>(_cartSvc.CartItems
                    .Where(i => i?.Quantity > 0).Select(m => new ItemViewModel(m))
                    );
            }
        }
        */

        public event PropertyChangedEventHandler? PropertyChanged;

        public List<string> CartNames => _cartSvc.AllNames;

        public string SelectedCartName
        {
            get
            {
                return _cartSvc.CurrentCart;
            }
            set
            {
                _cartSvc.SwitchCart(value);
                RefreshUX();
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (propertyName is null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RefreshUX()
        {
            NotifyPropertyChanged(nameof(Inventory));
            NotifyPropertyChanged(nameof(ShoppingCart));
        }

        public void PurchaseItem()
        {
            if(SelectedItem != null)
            {
                var shouldRefresh = SelectedItem.Model.Quantity >= 1;
                var updatedItem = _cartSvc.AddOrUpdate(SelectedItem.Model);

                if(updatedItem != null && shouldRefresh)
                {
                    NotifyPropertyChanged(nameof(Inventory));
                    NotifyPropertyChanged(nameof(ShoppingCart));
                }
            }
        }

        public void ReturnItem()
        {
            if(SelectedCartItem != null)
            {
                var shouldRefresh = SelectedCartItem.Model.Quantity >= 1;
                
                var updatedItem = _cartSvc.ReturnItem(SelectedCartItem.Model);

                if (updatedItem != null && shouldRefresh)
                {
                    NotifyPropertyChanged(nameof(Inventory));
                    NotifyPropertyChanged(nameof(ShoppingCart));
                }
            }
        }

        public void ReturnAll()
        {
            if(SelectedCartItem == null)
            {
                return;
            }

            bool success = _cartSvc.ReturnAll(SelectedCartItem.Model);
        
            if(success == true)
            {
                NotifyPropertyChanged(nameof(Inventory));
                NotifyPropertyChanged(nameof(ShoppingCart));
            }
        }

        public string Checkout()
        {
            string receipt = _cartSvc.MakeReceipt();
            _cartSvc.CartItems.Clear();
            NotifyPropertyChanged(nameof(Inventory));
            NotifyPropertyChanged(nameof(ShoppingCart));
            return receipt;
        }

        //part of the sort as well
        public void SetSort(string option)
        {
            SortOption = option;
            RefreshUX();
        }

        public void AddNewCart(string name)
        {
            if(!string.IsNullOrWhiteSpace(name) && !_cartSvc.AllNames.Contains(name))
            {
                _cartSvc.SwitchCart(name);  // this creates and switches
                NotifyPropertyChanged(nameof(CartNames));
                NotifyPropertyChanged(nameof(SelectedCartName));
                RefreshUX();
            }
        }

    }
}
