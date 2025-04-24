using Maui.eCommerce.ViewModels;

namespace Maui.eCommerce.Views;

//receipt query
[QueryProperty(nameof(Receipt), "receipt")]
public partial class ReceiptView : ContentPage
{
    public ReceiptView()
    {
        InitializeComponent();          
    }

    //receipt string
    private string _receipt = string.Empty;

    public string Receipt
    {
        get
        {
            return _receipt;
        }
        set
        {
            //Store the text
            _receipt = Uri.UnescapeDataString(value ?? string.Empty);
            //Creating the view model with the text
            BindingContext = new ReceiptViewModel(_receipt);
        }
    }

    private async void CloseClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
}