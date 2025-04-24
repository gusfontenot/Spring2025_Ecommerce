using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Library.eCommerce.Models;
using Library.eCommerce.Services;

namespace Maui.eCommerce.ViewModels
{
    public class ItemViewModel
    {
        public Item Model { get; set; }

        public string QuantityText { get; set; } = "1"; //making the default quantity 1 for text box

        public ICommand? AddCommand { get; set; }

        private void DoAdd()
        {
            var updatedItem = ShoppingCartService.Current.AddOrUpdate(Model);
        }

        public ItemViewModel()
        {
            Model = new Item();
        }

        public ItemViewModel(Item model)
        {
            Model = model;
            AddCommand = new Command(AddToCart);
        }
    
        private void AddToCart()
        {
            //making sure valid input for adding to cart
            if(!int.TryParse(QuantityText, out int quantity) || quantity < 1)
            {
                return;
            }

            //adding specified quantity to cart once verified as valid
            int add = ShoppingCartService.Current.numAdd(Model, quantity);

            if(add > 0)
            {
                ShoppingManagementViewModel.RefreshStatic();
            }
        }

    }
}
