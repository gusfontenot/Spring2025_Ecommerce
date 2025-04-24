using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.eCommerce.Services;

namespace Maui.eCommerce.ViewModels
{
    public class TaxRateViewModel : INotifyPropertyChanged
    {
        private double _ratePercent = TaxRateService.Current.TaxRate * 100;

        public double RatePercent
        {
            get
            {
                return _ratePercent;
            }
            set
            {
                if(Math.Abs(_ratePercent - value) <= 0) //check if it is valid
                {
                    return;
                }

                _ratePercent = value;
                propertyChanged(nameof(RatePercent));
            }
        }

        public void SaveRate()
        {
            TaxRateService.Current.TaxRate = _ratePercent / 100.0;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void propertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

    }
}
