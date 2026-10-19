using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdejAutUkol.Models
{
    public class SouhrnModel
    {
        public string NazevModelu { get; set; } = string.Empty;
        public double CenaBezDPH { get; set; }
        public double CenaSDPH { get; set; }
    }
}
