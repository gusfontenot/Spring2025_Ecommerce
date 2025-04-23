using System.Threading.Tasks;
using Maui.eCommerce.ViewModels;

namespace Maui.eCommerce.Views;

public partial class TaxRateView : ContentPage
{
	public TaxRateView()
	{
		InitializeComponent();
		BindingContext = new TaxRateViewModel();
	}

    private void SaveClicked(object sender, EventArgs e)
    {
		(BindingContext as TaxRateViewModel)!.SaveRate();
		Shell.Current.GoToAsync("//MainPage");
    }
}