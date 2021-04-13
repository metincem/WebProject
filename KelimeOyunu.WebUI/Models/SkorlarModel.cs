using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KelimeOyunu.WebUI.Models
{
    public class SkorlarModel
    {
        public string AdSoyad { get; set; }
        public int Puan { get; set; }
        public int ArtanSure { get; set; }
        public DateTime Zaman { get; set; }
    }
}
