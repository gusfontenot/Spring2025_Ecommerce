using Library.eCommerce.Services;
using Maui.eCommerce.ViewModels;

namespace Maui.eCommerce.Views;

public partial class ProductDetails : ContentPage
{
	public ProductDetails()
	{
        InitializeComponent();
		BindingContext = new ProductViewModel();
	}

    private void GoBackClicked(object sender, EventArgs e)
    {
		Shell.Current.GoToAsync("//InventoryManagement");
    }

    private void OkClicked(object sender, EventArgs e)
    {
        var name = (BindingContext as ProductViewModel).Name;
        ProductServiceProxy.Current.AddOrUpdate(new Spring2025_Ecommerce.Models.Product { Name = name });
        Shell.Current.GoToAsync("//InventoryManagement");
    }
}