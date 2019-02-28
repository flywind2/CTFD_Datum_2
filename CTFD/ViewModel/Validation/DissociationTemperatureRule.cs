using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CTFD.ViewModel.Validation
{
    public class DissociationTemperatureRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
           
            var result = new ValidationResult(true, null);
            if (string.IsNullOrEmpty(value.ToString()) == false)
            {
                var data = Convert.ToInt32(value);
                if (data <= 0) result = new ValidationResult(false, "不能小于0！");
                else if (data > 200) result = new ValidationResult(false, "不能大于200！");
            }
            return result;
        }
    }
}
