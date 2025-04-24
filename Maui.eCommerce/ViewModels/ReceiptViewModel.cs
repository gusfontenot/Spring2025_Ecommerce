using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maui.eCommerce.ViewModels
{
    public class ReceiptViewModel
    {
        //view model for the receipt screen, with the string for the actual text
        public string ReceiptText { get; }

        public ReceiptViewModel(string text)
        {
            ReceiptText = text ?? string.Empty;
        }
    }
}
