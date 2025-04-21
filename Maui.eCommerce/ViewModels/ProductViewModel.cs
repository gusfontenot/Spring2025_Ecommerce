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
        private Item? cacheModel { get; set; }
        public string? Name {
            get
            {
                return Model?.Product.Name ?? string.Empty;
            }

            set
            {
                if(Model?.Product?.Name != value && Model != null)
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

        public double? Price
        {
            get
            {
                return Model?.Product.Price;
            }
            set
            {
                if (Model?.Product != null && Model.Product.Price != value)
                {
                    Model.Product.Price = value ?? 0;
                }
            }
        }

        public Item? Model { get; set; }

        public void AddOrUpdate()
        {
            ProductServiceProxy.Current.AddOrUpdate(Model);
        }

        public void Undo()
        {
            ProductServiceProxy.Current.AddOrUpdate(cacheModel);
        }

        public ProductViewModel()
        {
            Model = new Item();
            cacheModel = null;
        }

        public ProductViewModel(Item? model)
        {
            Model = model;
            if (model != null)
            {
                cacheModel = new Item(model);
            }
        }
    }
}
