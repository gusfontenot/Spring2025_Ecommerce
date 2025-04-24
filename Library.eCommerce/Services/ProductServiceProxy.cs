using Library.eCommerce.DTO;
using Library.eCommerce.Models;
using Library.eCommerce.Utilities;
using Newtonsoft.Json;
using Spring2025_Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.eCommerce.Services
{
    public class ProductServiceProxy
    {
        private ProductServiceProxy()
        {
            Products = new List<Item?>
                {
                  new Item{ Product = new ProductDTO{Id = 1, Name = "Product 1"}, Id = 1, Quantity = 1 },
                  new Item{ Product = new ProductDTO{Id = 2, Name = "Product 2"}, Id = 2, Quantity = 2 },
                  new Item{ Product = new ProductDTO{Id = 3, Name = "Product 3"}, Id = 3, Quantity = 3 },
            };
        }

        //Event handlers for when product details or inventory is changed
        public event EventHandler? InventoryChanged;
        public event EventHandler<int>? ProductRemoved;
        public event EventHandler<int>? ProductUpdated;

        private static ProductServiceProxy? instance;
        private static object instanceLock = new object();
        public static ProductServiceProxy Current
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new ProductServiceProxy();
                    }
                }

                return instance;
            }
        }

        public List<Item?> Products { get; private set; }


        public Item AddOrUpdate(Item item)
        {
            if(item == null)
            {
                return item;
            }

            if (item.Id == 0) //creating a new id for a new product that is added
            {
                int newId = Products.Any() ? Products.Max(p => p?.Id ?? 0) + 1 : 1;
                item.Id = newId;
                if(item.Product != null)
                {
                    item.Product.Id = newId;
                }

                Products.Add(new Item(item));
            }
            else
            {
                var existingItem = Products.FirstOrDefault(p => p?.Id == item.Id);
                if(existingItem == null)
                {
                    Products.Add(new Item(item));
                }
                else
                {
                    var index = Products.IndexOf(existingItem);
                    Products[index] = new Item(item);
                }
                    
            }

            //update UI
            ProductUpdated?.Invoke(this, item.Id);
            InventoryChanged?.Invoke(this, EventArgs.Empty);
            return item;
        }

        public Item? Delete(int id)
        {
            if (id == 0)
            {
                return null;
            }

            Item? product = Products.FirstOrDefault(p => p.Id == id);
            if(product != null)
            {
                Products.Remove(product);
                InventoryChanged?.Invoke(this, EventArgs.Empty); //notifying inv change to cart
                ProductRemoved?.Invoke(this, id); //notify the product has been deleted for the cart
            }

            return product;
        }

        public Item? GetById(int id)
        {
            return Products.FirstOrDefault(p => p.Id == id);
        }

    }


}
