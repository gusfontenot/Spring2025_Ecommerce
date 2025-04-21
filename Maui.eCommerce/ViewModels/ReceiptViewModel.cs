using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maui.eCommerce.ViewModels
{
    public class ReceiptViewModel
    {
        public string ReceiptText { get; }

        public ReceiptViewModel(string text)
        {
            ReceiptText = text ?? string.Empty;
        }
    }
}
