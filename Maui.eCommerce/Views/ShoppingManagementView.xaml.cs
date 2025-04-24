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
        var viewModel = BindingContext as ShoppingManagementViewModel;
        if (viewModel == null)
        {
            return;
        }
        //go through with checkout
        string receipt = viewModel.Checkout();
        //goto receipt page
        Shell.Current.GoToAsync($"//Receipt?receipt={(receipt)}");
    }

    private void GoBackClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//MainPage");
    }
}