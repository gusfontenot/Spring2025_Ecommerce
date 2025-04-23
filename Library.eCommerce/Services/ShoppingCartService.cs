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
        public event EventHandler? InvChange;
        public event EventHandler? CartChange;

        //change carts implementation
        private Dictionary<string, List<Item>> carts = new();
        private string currentCart = "Default";

        private ShoppingCartService()
        {
            carts = new Dictionary<string, List<Item>>();
            currentCart = "Default";
            carts[currentCart] = new List<Item>();
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

        public List<Item> CartItems => carts[CurrentCart];
        public List<string> AllNames => carts.Keys.ToList();

        public void SwitchCart(string name)
        {
            CurrentCart = name;
            CartChange?.Invoke(this, EventArgs.Empty);
        }
        //change carts implementation end

        /*
        public List<Item> CartItems
        {
            get
            {
                return items;
            }
        }
        */

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

        //public Action<object, object> InventoryChange { get; set; }

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

            int ableToAdd = Math.Min(quantity, invItem.Quantity ?? 0);

            if(ableToAdd == 0)
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

            InvChange?.Invoke(this, EventArgs.Empty);
            CartChange?.Invoke(this, EventArgs.Empty);

            return ableToAdd;
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

            CartItems.Remove(existingItem);
            updateEvents();
            return true;
        }

        public string MakeReceipt()
        {
            //need stringbuild
            var stringBuild = new System.Text.StringBuilder();
            double subtotal = 0;

            foreach(var i in CartItems)
            {
                var price = i.Product!.Price;
                var quantity = i.Quantity ?? 0;
                subtotal += (price * quantity);
                stringBuild.AppendLine($"{i.Product.Name,-18} x{quantity,-3}  {price,6:C2}");
            }

            double _taxRate = TaxRateService.Current.TaxRate;
            double TaxAmount = Math.Round(subtotal * _taxRate, 2);
            double total = Math.Round(subtotal + TaxAmount, 2);

            stringBuild.AppendLine($"Tax ({_taxRate:P0}):".PadRight(22) + $"{TaxAmount,10:C2}");
            stringBuild.AppendLine(new string('-', 30));
            stringBuild.AppendLine($"{"Subtotal:",-22}{subtotal,10:C2}");
            stringBuild.AppendLine($"{"Total:",-22}{total,10:C2}");
            return stringBuild.ToString();
        }

        private void updateEvents()
        {
            InvChange?.Invoke(this, EventArgs.Empty);
            CartChange?.Invoke(this, EventArgs.Empty);
        }

    }
}
