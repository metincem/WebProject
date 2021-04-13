using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KelimeOyunu.WebUI.Models
{
    public class SiradakiModel
    {
        public string Aciklama { get; set; }
        public int TotalPuan { get; set; }
        public int SoruNo { get; set; }
        public int KalanSure { get; set; }
        public int KelimeUzunluk { get; set; }
    }
}
