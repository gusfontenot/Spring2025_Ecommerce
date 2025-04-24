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
        //Event handlers for UI updates for inventory and cart
        public event EventHandler? InvChange;
        public event EventHandler? CartChange;

        //change carts implementation
        private Dictionary<string, List<Item>> carts = new();
        private string currentCart = "Default";

        private ShoppingCartService()
        {
            carts = new Dictionary<string, List<Item>>();
            currentCart = "Default"; //current cart on startup is default cart
            carts[currentCart] = new List<Item>();
            
            //UI updates for product removal/details updated
            ProductServiceProxy.Current.ProductRemoved += productRemoved;
            ProductServiceProxy.Current.ProductUpdated += productUpdated;
        }

        //function for removing item from inventory and updating the UI in real tiem
        private void productRemoved(object? sender, int productId)
        {
            foreach(var cart in carts.Values)
            {
                var toRemove = cart.FirstOrDefault(i => i.Id == productId);

                if(toRemove != null)
                {
                    cart.Remove(toRemove);
                }
            }
            //notify of change in cart
            CartChange?.Invoke(this, EventArgs.Empty);
        }

        //function for when a product is updated while in the cart so the UI reflects the changes
        private void productUpdated(object? sender, int productId)
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
                    item.Product = updatedId.Product;
                }
            }
            //notify of changes in the cart
            CartChange?.Invoke(this, EventArgs.Empty);
        }

        public string CurrentCart
        {
            get
            {
                return currentCart;
            }
            set
            {
                currentCart = value;
                
                if(!carts.ContainsKey(currentCart))
                {
                    carts[currentCart] = new List<Item>();
                }
            }
        }

        public List<Item> CartItems
        {
            get
            {
                return carts[CurrentCart]; //list of items currently in cart
            }
        }
        public List<string> AllNames
        {
            get
            {
                return carts.Keys.ToList(); //list of all cart names
            }
        }

        //function for switching to a different shopping cart
        public void SwitchCart(string name)
        {
            CurrentCart = name;
            CartChange?.Invoke(this, EventArgs.Empty);
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

        //Function for the inline text box functionality for specifying different quantities of a product to add to the cart
        public int numAdd(Item item, int quantity)
        {
            if(quantity < 1 || item == null)
            {
                return 0;
            }

            var invItem = _prodSvc.GetById(item.Id);

            if(invItem == null)
            {
                return 0;
            }

            //min number to be able to add
            int ableToAdd = Math.Min(quantity, invItem.Quantity ?? 0);

            if(ableToAdd == 0) //return if none
            {
                return 0;
            }
            else
            {
                invItem.Quantity -= ableToAdd;
            }

            var cartItem = CartItems.FirstOrDefault(i => i.Id == item.Id);

            if(cartItem != null)
            {
                cartItem.Quantity += ableToAdd;
            }
            else
            {
                cartItem = new Item(item)
                {
                    Quantity = ableToAdd
                };

                CartItems.Add(cartItem);
            }

            //notify of changes in the inventory and cart
            InvChange?.Invoke(this, EventArgs.Empty);
            CartChange?.Invoke(this, EventArgs.Empty);

            return ableToAdd; //return num to add to cart
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
                var checkInv = _prodSvc.GetById(i.Id);
                if (checkInv == null)
                {
                    continue;
                }

                var quantity = i.Quantity ?? 0;
                if (quantity < 1)
                {
                    continue;
                }

                var price = i.Product!.Price;
                subtotal += (price * quantity);

                //printing the products being purchased along with their details
                stringBuild.AppendLine($"{i.Product.Name,-18} x{quantity,-3}  {price,6:C2}");
            }

            //When subtotal is zero we will assume the cart is empty since nothing is free
            if (subtotal == 0)
            {
                return "Your shopping cart is empty - no products to purchase.";
            }

            double _taxRate = TaxRateService.Current.TaxRate; //specified tax rate by user
            double TaxAmount = Math.Round(subtotal * _taxRate, 2); //sales tax value
            double total = Math.Round(subtotal + TaxAmount, 2);

            //Final details of the receipt along with the 30 "-" characters for stylization
            stringBuild.AppendLine($"Tax ({_taxRate:P0}):".PadRight(22) + $"{TaxAmount,10:C2}");
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
