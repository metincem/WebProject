using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KelimeOyunu.WebUI.Models
{
    public class TahminSonuc
    {
        public bool Dogru { get; set; }
        public string Kelime { get; set; }
        public int TotalPuan { get; set; }
        public int SoruNo { get; set; }
    }
}
