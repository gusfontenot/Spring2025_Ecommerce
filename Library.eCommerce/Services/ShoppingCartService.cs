using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.eCommerce.Models;
using Spring2025_Ecommerce.Models;

namespace Library.eCommerce.Services
{
    public class ShoppingCartService
    {
        private ProductServiceProxy _prodSvc = ProductServiceProxy.Current;
        private List<Item> items;

        //Event handlers for UI updates for inventory and cart
        public event EventHandler? InvChange;
        public event EventHandler? CartChange;

        public List<Item> CartItems
        {
            get
            {
                return items;
            }
        }
        public static ShoppingCartService Current {
            get
            {
                if(instance == null)
                {
                    instance = new ShoppingCartService();
                }

                return instance;
            }
        }

        private static ShoppingCartService? instance;
        private ShoppingCartService() 
        {
            items = new List<Item>();
            ProductServiceProxy.Current.ProductRemoved += UIProductRemoved; //UI updated for the current
            ProductServiceProxy.Current.ProdUpdated += UIProductUpdated; //Ui updated for current
        }

        //added this function to update the UI when a product in the SC is then removed in IM
        private void UIProductRemoved(object? sender, int productId)
        {
            var toRemove = items.FirstOrDefault(i => i.Id == productId);
            if(toRemove != null)
            {
                items.Remove(toRemove);
                updateEvents();
            }
            
            CartChange?.Invoke(this, EventArgs.Empty);
        }

        //added this function to update the UI when a product in the SC is then edited in IM
        private void UIProductUpdated(object? sender, int productId)
        {
            var updatedId = _prodSvc.GetById(productId);

            if(updatedId == null)
            {
                return;
            }

            foreach(var item in CartItems)
            {
                if(item.Id == productId)
                {
                    item.Product = updatedId.Product; //updating the product details
                }
            }

            CartChange?.Invoke(this, EventArgs.Empty); //Refreshing the UI here
        }

        //Add or update function for the users shopping cart
        public Item? AddOrUpdate(Item item)
        {
            var existingInvItem = _prodSvc.GetById(item.Id);
            if(existingInvItem == null || existingInvItem.Quantity == 0) {
                return null;
            }

            if (existingInvItem != null)
            {
                existingInvItem.Quantity--;
            }

            var existingItem = CartItems.FirstOrDefault(i => i.Id == item.Id);
            if (existingItem == null) //does not exists in the list
            {
                var newItem = new Item(item);
                newItem.Quantity = 1;
                CartItems.Add(newItem);
            }
            else //exists in the list
            {
                existingItem.Quantity++;
            }

            return existingInvItem;
        }

        public Item? ReturnItem(Item item)
        {
            if (item?.Id <= 0 || item == null)
            {
                return null;
            }

            var itemToReturn = CartItems.FirstOrDefault(c => c.Id == item.Id);
            if (itemToReturn != null)
            {
                itemToReturn.Quantity--;
                var inventoryItem = _prodSvc.Products.FirstOrDefault(p => p.Id == itemToReturn.Id);
                if(inventoryItem == null)
                {
                    _prodSvc.AddOrUpdate(new Item(itemToReturn));
                }
                else
                {
                    inventoryItem.Quantity++;
                }
            }

            return itemToReturn;
        }

        //function added for if the return all of a product button is clicked
        public bool ReturnAll(Item cartItem)
        {
            if(cartItem == null)
            {
                return false;
            }

            var existingItem = CartItems.FirstOrDefault(i => i.Id == cartItem.Id);
            if(existingItem == null)
            {
                return false;
            }

            var invItem = _prodSvc.GetById(existingItem.Id);
            if(invItem != null)
            {
                invItem.Quantity += existingItem.Quantity ?? 0;
            }

            CartItems.Remove(existingItem); //remove the item completely from the cart
            updateEvents(); //update UI
            return true;
        }

        //receipt function using stringbuild for constructing an itemized receipt when user checks out
        public string MakeReceipt()
        {
            //need stringbuild
            var stringBuild = new System.Text.StringBuilder();
            double subtotal = 0;

            foreach(var i in CartItems)
            {
                //add to final
                var checkInv = _prodSvc.GetById(i.Id);
                if(checkInv == null)
                {
                    continue;
                }

                //add to final
                var quantity = i.Quantity ?? 0;
                if(quantity < 1)
                {
                    continue;
                }

                var price = i.Product!.Price;
                subtotal += (price * quantity);

                //printing the products being purchased along with their details
                stringBuild.AppendLine($"{i.Product.Name,-18} x{quantity,-3}  {price,6:C2}");
            }

            //When subtotal is zero we will assume the cart is empty since nothing is free
            if(subtotal == 0)
            {
                return "Your shopping cart is empty - no products to purchase.";
            }

            var total = Math.Round(subtotal * 1.07, 2);

            //Final details of the receipt along with the 30 "-" characters for stylization
            stringBuild.AppendLine(new string('-', 30));
            stringBuild.AppendLine($"{"Subtotal:",-22}{subtotal,10:C2}");
            stringBuild.AppendLine($"{"Total:",-22}{total,10:C2}");
            return stringBuild.ToString();
        }

        //function to help update the UI when changes are made in the Inv or the cart views
        private void updateEvents()
        {
            InvChange?.Invoke(this, EventArgs.Empty);
            CartChange?.Invoke(this, EventArgs.Empty);
        }

    }
}
