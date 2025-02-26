using Library.eCommerce.Models;
using Spring2025_Ecommerce.Models;
using System;
using System.Collections.Generic;
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
                new Item{ Product = new Product{Id = 1, Name = "Product 1"}, Id = 1, Quantity = 1 },
                new Item{ Product = new Product{Id = 2, Name = "Product 2"}, Id = 2, Quantity = 2 },
                new Item{ Product = new Product{Id = 3, Name = "Product 3"}, Id = 3, Quantity = 3 },
            };
        }

        //here
        private int LastKey
        {
            get
            {
                if (!Products.Any())
                {
                    return 0;
                }

                return Products.Select(p => p?.Id ?? 0).Max();
            }
        }

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

            if (item.Id == 0)
            {
                item.Id = LastKey + 1;
                item.Product.Id = item.Id;
                Products.Add(item);
            }

            return item;
        }

        public Item? Delete(int id)
        {
            if (id == 0)
            {
                return null;
            }

            Item? product = Products.FirstOrDefault(p => p.Id == id);
            Products.Remove(product);

            return product;
        }

        public Item? GetById(int id)
        {
            return Products.FirstOrDefault(p => p.Id == id);
        }

    }


}
