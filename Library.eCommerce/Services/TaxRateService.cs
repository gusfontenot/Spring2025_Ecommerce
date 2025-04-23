using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.eCommerce.Services
{
    public class TaxRateService
    {
        private TaxRateService() { }

        public static TaxRateService Current { get; } = new TaxRateService();

        public double TaxRate { get; set; } = 0.07; //The default tax rate

    }
}
