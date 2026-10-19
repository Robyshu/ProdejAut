using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdejAutUkol.Models
{
    public class AutoModel
    {
        public string Model { get; set; } = string.Empty;
        public DateTime DatumProdeje { get; set; }
        public double Cena { get; set; }
        public double DPH { get; set; }
    }
}
