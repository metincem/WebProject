using KelimeOyunu.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KelimeOyunu.WebUI.Models
{
    public class SurecHarfModel
    {
        public Surec SurecModel { get; set; }
        public int HarfIndex { get; set; }
        public string Harf { get; set; }
        public bool HarflerBitti { get; set; }
        public int SoruPuan { get; set; }
    }
}
