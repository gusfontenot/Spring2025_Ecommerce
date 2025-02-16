namespace Maui.eCommerce.Views;

public partial class InventoryManagementView : ContentPage
{
	public InventoryManagementView()
	{
		InitializeComponent();
	}

    private void CancelClicked(object sender, EventArgs e)
    {
		Shell.Current.GoToAsync("//MainPage");
    }
}