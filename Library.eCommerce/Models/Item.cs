using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spring2025_Ecommerce.Models;

namespace Library.eCommerce.Models
{
    public class Item
    {
        public int Id { get; set; }

        public Product Product { get; set; }

        public int Quantity { get; set; }

        public string Display
        {
            get
            {
                return Product?.Display ?? string.Empty;
            }
        }

        public Item()
        {
            Product = new Product();
        }

    }
}
