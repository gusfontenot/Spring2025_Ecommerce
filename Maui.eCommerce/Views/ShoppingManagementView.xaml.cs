using Maui.eCommerce.ViewModels;

namespace Maui.eCommerce.Views;

public partial class ShoppingManagementView : ContentPage
{
	public ShoppingManagementView()
	{
		InitializeComponent();
		BindingContext = new ShoppingManagementViewModel();
	}

    private void AddToCartClicked(object sender, EventArgs e)
    {
		(BindingContext as ShoppingManagementViewModel).PurchaseItem();
    }

    private void RemoveFromCartClicked(object sender, EventArgs e)
    {
        (BindingContext as ShoppingManagementViewModel).ReturnItem();
    }

    private void InLineAddClicked(object sender, EventArgs e)
    {
        (BindingContext as ShoppingManagementViewModel).RefreshUX();
    }

    private void ReturnAllClicked(object sender, EventArgs e)
    {
        (BindingContext as ShoppingManagementViewModel)?.ReturnAll();
    }

    private void CheckoutClicked(object sender, EventArgs e)
    {
        var vm = BindingContext as ShoppingManagementViewModel;
        if (vm == null)
        {
            return;
        }
        string receipt = vm.Checkout();

        Shell.Current.GoToAsync($"//Receipt?receipt={(receipt)}");
    }

    private void GoBackClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//MainPage");
    }
}