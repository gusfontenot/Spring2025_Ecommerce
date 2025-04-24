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

       
        public ShoppingManagementViewModel()
        {
            ProductServiceProxy.Current.InventoryChanged += invUpdate; //refresh ui
            ShoppingCartService.Current.CartChange += cartUpdate; //refresh ui
        }

        //function to update UI for inv changes
        private void invUpdate(object sender, EventArgs e)
        {
            RefreshUX();
        }
        //function to update UI for cart changes
        private void cartUpdate(object sender, EventArgs e)
        {
            RefreshUX();
        }

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

        public event PropertyChangedEventHandler? PropertyChanged;

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

        //function for when return all button is clicked for a product in the cart
        public void ReturnAll()
        {
            if(SelectedCartItem == null)
            {
                return;
            }

            bool success = _cartSvc.ReturnAll(SelectedCartItem.Model);
        
            if(success == true) //update UI if successful
            {
                NotifyPropertyChanged(nameof(Inventory));
                NotifyPropertyChanged(nameof(ShoppingCart));
            }
        }

        //checkout function that calls the makereceipt function from the SCS file
        public string Checkout()
        {
            string receipt = _cartSvc.MakeReceipt();
            _cartSvc.CartItems.Clear();
            NotifyPropertyChanged(nameof(Inventory)); //update ui
            NotifyPropertyChanged(nameof(ShoppingCart)); //update ui
            return receipt;
        }
    }
}
