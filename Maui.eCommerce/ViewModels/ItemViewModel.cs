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

        public ICommand? AddCommand { get; set; }

        private void DoAdd()
        {
            var updatedItem = ShoppingCartService.Current.AddOrUpdate(Model);
        }

        void SetupCommands()
        {
            AddCommand = new Command(DoAdd);
        }

        public ItemViewModel()
        {
            Model = new Item();
        }

        public ItemViewModel(Item model)
        {
            Model = model;
            SetupCommands();
        }
    
    }
}
