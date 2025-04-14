using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spring2025_Ecommerce.Models;

namespace Library.eCommerce.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public double Price { get; set; }

        public int Quantity { get; set; }

        //here
        public string? Display
        {
            get
            {
                return $"{Id}. {Name}, Price: ${Price}, ";
            }
        }

        public ProductDTO()
        {
            Name = string.Empty;
            Quantity = 0;
            Price = 0;
        }

        public ProductDTO(Product p)
        {
            Name = p.Name;
            Id = p.Id;
        }

        public ProductDTO(ProductDTO p)
        {
            Name = p.Name;
            Id = p.Id;
        }

        public override string ToString()
        {
            return Display ?? string.Empty;
        }
    }
}
