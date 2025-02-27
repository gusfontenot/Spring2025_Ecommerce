using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.eCommerce.Models;
using Library.eCommerce.Services;
using Spring2025_Ecommerce.Models;

namespace Maui.eCommerce.ViewModels
{
    public class ProductViewModel
    {
        public string? Name {
            get
            {
                return Model?.Product.Name ?? string.Empty;
            }

            set
            {
                if(Model.Product?.Name != value && Model != null)
                {
                    Model.Product.Name = value;
                }
            }
        }

        public int? Quantity
        {
            get
            {
                return Model?.Quantity;
            }
            set
            {
                if(Model != null && Model.Quantity != value)
                {
                    Model.Quantity = value;
                }
            }
        }

        public Item? Model { get; set; }

        public void AddOrUpdate()
        {
            ProductServiceProxy.Current.AddOrUpdate(Model);
        }

        public ProductViewModel()
        {
            Model = new Item();
        }

        public ProductViewModel(Item? model)
        {
            Model = model;
        }
    }
}
