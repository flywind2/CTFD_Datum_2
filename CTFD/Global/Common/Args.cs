using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTFD.Global.Common
{
    public class ThermalChangeArgs:EventArgs
    {
        public ThermalChange ThermalChange { get; set; }

        public ThermalChangeArgs(ThermalChange thermalChange)
        {
            this.ThermalChange = thermalChange;
        }
    }
}
